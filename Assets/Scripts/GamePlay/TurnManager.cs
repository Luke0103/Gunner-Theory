using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class TurnManager : MonoBehaviour, IOnEventCallback
{
    private GameManager gm;
    [SerializeField] private Transform[] cameraPosList;
    [SerializeField] private GameObject[] objectsEnabledOnOwnTurn;
    [SerializeField] private Text chanceToggleButtonText;
    [SerializeField] private Text remainingTimeText;

    [SerializeField] private float turnSpeed;
    private float remainingTime;
    private bool isTurning;

    private void Start()
    {
        gm = GameManager.Instance;
        isTurning = false;
        remainingTime = DefaultValues.TurnDuration;
        NetworkManager.Instance.GetClient.AddCallbackTarget(this);
        gm.NextTurnEvent += new GameManager.NextTurnEventHandler(SetNextIndex);
        gm.StartGameEvent += new GameManager.StartGameEventHandler(ToggleObjectsByTurn);
    }

    private void OnDisable()
    {
        NetworkManager.Instance.GetClient.RemoveCallbackTarget(this);
    }

    private void Update()
    {
        if (gm.IsActiveGameTime)
        {
            remainingTime -= Time.deltaTime;
            remainingTimeText.text = string.Format("{0:00.00}", remainingTime);
        }
        
        if (remainingTime <= 0f && gm.IsLocalTurn)
        {
            remainingTime = DefaultValues.TurnDuration;
            NextTurn();
        }
    }

    private void LateUpdate()
    {
        if (isTurning)
        {
            RotateCamera();
        }
    }

    public static void NextTurn()
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent
            ((byte)DefaultValues.EventCode.NEXT_TURN, null, eventOptions, SendOptions.SendReliable);
    }

    public void ToggleChangeChanceType()
    {
        PlayerManager.ChangeType type = gm.localPlayer.ToggleChangeChanceType();

        switch(type)
        {
            case PlayerManager.ChangeType.INCREASE:
                chanceToggleButtonText.text = "È®·ü Áõ°¡";
                break;
            case PlayerManager.ChangeType.MAINTAIN:
                chanceToggleButtonText.text = "È®·ü À¯Áö";
                break;
            case PlayerManager.ChangeType.DECREASE:
                chanceToggleButtonText.text = "È®·ü °¨¼Ò";
                break;
        }
    }

    private void SetNextIndex()
    {
        if (gm.IsClockwise)
            Clockwise();
        else
            AntiClockwise();
        isTurning = true;

        ToggleObjectsByTurn();
    }

    public void RotateCamera()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPosList[gm.CurrentPlayerIndex].position, turnSpeed);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraPosList[gm.CurrentPlayerIndex].rotation, turnSpeed);
        if (Camera.main.transform.position == cameraPosList[gm.CurrentPlayerIndex].position &&
            Camera.main.transform.rotation == cameraPosList[gm.CurrentPlayerIndex].rotation)
        {
            isTurning = false;
        }
    }

    public void ToggleObjectsByTurn()
    {
        if (gm.IsLocalTurn)
        {
            gm.localPlayer.CanAttack = true;

            foreach (GameObject go in objectsEnabledOnOwnTurn)
            {
                go.SetActive(true);
            }
        }
        else
        {
            gm.localPlayer.CanAttack = false;
            foreach (GameObject go in objectsEnabledOnOwnTurn)
            {
                go.SetActive(false);
            }
        }
    }

    public void Clockwise()
    {
        do
        {
            gm.CurrentPlayerIndex++;
            if (gm.CurrentPlayerIndex >= gm.CurrentPlayerCount)
            {
                gm.CurrentPlayerIndex = 0;
            }
        } while (!gm.RemainingPlayers[gm.CurrentPlayerIndex]);
    }
    public void AntiClockwise()
    {
        do
        {
            gm.CurrentPlayerIndex--;
            if (gm.CurrentPlayerIndex < 0)
            {
                gm.CurrentPlayerIndex = gm.CurrentPlayerCount - 1;
            }
        } while (!gm.RemainingPlayers[gm.CurrentPlayerIndex]);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)DefaultValues.EventCode.NEXT_TURN)
        {
            gm.playerList[gm.CurrentPlayerIndex].ChangeBank(-DefaultValues.WithdrawPerTurn);
            PrizeManager.Instance.ChangePrize(DefaultValues.WithdrawPerTurn);
            remainingTime = DefaultValues.TurnDuration;
            gm.InvokeNextTurnEvent();
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.PLAYER_ATTACK)
        {
            if (gm.IsLocalTurn)
            {
                foreach (GameObject go in objectsEnabledOnOwnTurn)
                {
                    go.SetActive(false);
                }
            }
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.ROUND_OVER)
        {
            remainingTime = DefaultValues.TurnDuration;
            gm.InvokeNextTurnEvent();
        }
    }
}
