using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviour, IOnEventCallback
{
    public enum ChangeType
    {
        DECREASE = -1,
        MAINTAIN = 0,
        INCREASE = 1
    }

    GameManager gm;

    ChangeType currentChangeType;

    [SerializeField] private GameObject textParent;

    [SerializeField] private TextMesh nameText;
    [SerializeField] private TextMesh prizeText;
    [SerializeField] private TextMesh chanceText;
    [SerializeField] private TextMesh changeTypeText;

    [SerializeField] private Renderer[] mesh;
    [SerializeField] private Animator anim;

    private int minChance;
    private int maxChance;
    private int minChangeRate;
    private int maxChangeRate;
    private int bank;
    private float chanceToHit;

    public int ActorIndex { get; set; }
    public int ActorNumber { get; set; }
    public int GetBank { get { return bank; } }
    public float GetChanceToHit { get { return chanceToHit; } }

    public float ChangeRateScale { get; set; }

    public bool IsLocal { get { return ActorNumber == NetworkManager.Instance.GetClient.LocalPlayer.ActorNumber; } }
    public bool CanAttack { get; set; }

    private void Start()
    {
        gm = GameManager.Instance;
        
        if (IsLocal)
        {
            minChance = DefaultValues.MinChance;
            maxChance = DefaultValues.MaxChance;
            minChangeRate = DefaultValues.MinChangeRate;
            maxChangeRate = DefaultValues.MaxChangeRate;

            CanAttack = true;

            currentChangeType = ChangeType.INCREASE;
            chanceToHit = Random.Range(minChance, maxChance + 1);
            ChangeChance();
            gm.NextTurnEvent += new GameManager.NextTurnEventHandler(ChangeChance);
            ChangeRateScale = 1f;
        }

        bank = DefaultValues.BaseBank;
        UpdateBankText();
        
        NetworkManager.Instance.GetClient.AddCallbackTarget(this);
    }

    private void FixedUpdate()
    {
        textParent.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }

    private void OnDisable()
    {
        NetworkManager.Instance.GetClient.RemoveCallbackTarget(this);
    }

    public void Init()
    {
        nameText.text = string.Format("No. {0}", ActorNumber);
        if (IsLocal)
        {
            GameManager.Instance.SetLocalPlayer(this);
            Debug.Log(ActorIndex);
            GameManager.Instance.InvokeStartGameEvent();
        }
    }

    public ChangeType ToggleChangeChanceType()
    {
        if (currentChangeType.Equals(ChangeType.INCREASE))
        {
            currentChangeType = ChangeType.MAINTAIN;
        }
        else if (currentChangeType.Equals(ChangeType.MAINTAIN))
        {
            currentChangeType = ChangeType.DECREASE;
        }
        else
        {
            currentChangeType = ChangeType.INCREASE;
        }

        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        table["ChangeType"] = currentChangeType;
        table["ActorNumber"] = ActorNumber;

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.PLAYER_CHANGED_STANCE, table, eventOptions, SendOptions.SendReliable);

        return currentChangeType;
    }

    public void UpdateChanceTypeText(ChangeType type)
    {
        if (type.Equals(ChangeType.INCREASE))
        {
            changeTypeText.text = "+";
        }
        else if (type.Equals(ChangeType.MAINTAIN))
        {
            changeTypeText.text = "●";
        }
        else
        {
            changeTypeText.text = "-";
        }
    }

    private void ChangeChanceText(float chance)
    {
        chanceText.text = string.Format("{0}%", Mathf.RoundToInt(chance));
    }

    private void ChangeChance()
    {
        if (!IsLocal)
            return;

        if (currentChangeType == ChangeType.DECREASE)
        {
            chanceToHit -= Random.Range(minChangeRate, maxChangeRate + 1) * ChangeRateScale;
            if (chanceToHit < minChance)
                chanceToHit = minChance;
        }
        else if (currentChangeType == ChangeType.INCREASE)
        {
            chanceToHit += Random.Range(minChangeRate, maxChangeRate + 1) * ChangeRateScale;
            if (chanceToHit > maxChance)
                chanceToHit = maxChance;
        }
        else
            return;

        RaiseChanceChangeEvent();
    }

    public void ChangeChance(int chance, bool isOffset = false)
    {
        if (isOffset)
        {
            chanceToHit += chance;
            if (chanceToHit > maxChance)
                chanceToHit = maxChance;
            else if (chanceToHit < minChance)
                chanceToHit = minChance;
        }
        else
            chanceToHit = chance;

        RaiseChanceChangeEvent();
    }

    private void RaiseChanceChangeEvent()
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("ActorNumber", ActorNumber);
        hashtable.Add("ChanceToHit", chanceToHit);

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.PLAYER_CHANCE_CHANGE, hashtable, eventOptions, SendOptions.SendReliable);
    }

    private void UpdateBankText()
    {
        prizeText.text = string.Format("{0}$", bank);
    }

    public void ChangeBank(int amount, bool isSubstitute = false)
    {
        if (isSubstitute)
            bank = amount;
        else
            bank += amount;

        if (bank <= 0)
        {
            bank = 0;
            Bankrupt();
        }

        UpdateBankText();
    }

    public void Killed()
    {
        anim.enabled = false;
        gm.RemainingPlayers[ActorIndex] = false;
        gm.SurvivingPlayerCount--;
    }

    public void Bankrupt()
    {
        anim.enabled = false;
        gm.RemainingPlayers[ActorIndex] = false;
        gm.PlayersInGame[ActorIndex] = false;
        gm.SurvivingPlayerCount--;
    }

    public void Escape()
    {
        foreach (Renderer r in mesh)
        {
            r.enabled = false;
        }
        gm.RemainingPlayers[ActorIndex] = false;
        gm.SurvivingPlayerCount--;
    }

    public void Return()
    {
        foreach (Renderer r in mesh)
        {
            r.enabled = true;
        }
        anim.enabled = true;
        gm.RemainingPlayers[ActorIndex] = true;
        gm.SurvivingPlayerCount++;
    }

    public IEnumerator Attack(int targetIndex, bool success)
    {
        transform.LookAt(gm.playerList[targetIndex].transform.position);
        anim.SetBool("Aiming", true);

        //애니메이션 완료까지 대기
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming"))
            yield return null;

        anim.SetTrigger("Attack");
        if (success)
        {
            gm.playerList[targetIndex].Killed(); //성공 시 탈락 처리
        }

        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Fire"))
            yield return null;

        anim.SetBool("Aiming", false);

        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming"))
            yield return null;

        //공격 이벤트 호출 및 턴 넘기기
        if (gm.IsLocalTurn && IsLocal)
        {
            CardManager.Instance.AttackCardActions(ActorIndex, targetIndex);
            TurnManager.NextTurn();
        }
    }
    
    private void OnMouseUpAsButton()
    {
        if (!gm.IsLocalTurn || IsLocal || !gm.localPlayer.CanAttack)
            return;

        gm.localPlayer.CanAttack = false;

        float dice = Random.Range(0f, 100f); //0~100 난수 생성

        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("Success", dice < gm.localPlayer.GetChanceToHit); //성공 여부
        hashtable.Add("AttackerIndex", gm.LocalPlayerIndex); //공격자 인덱스
        hashtable.Add("TargetIndex", ActorIndex); //목표 인덱스

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent
            ((byte)DefaultValues.EventCode.PLAYER_ATTACK, hashtable, eventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)DefaultValues.EventCode.PLAYER_CHANCE_CHANGE)
        {
            ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent.CustomData;

            if ((int)hashtable["ActorNumber"] != ActorNumber)
                return;

            chanceToHit = (float)hashtable["ChanceToHit"];
            ChangeChanceText(chanceToHit);
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.PLAYER_CHANGED_STANCE)
        {
            ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent.CustomData;

            if (ActorNumber == (int)hashtable["ActorNumber"])
                UpdateChanceTypeText((ChangeType)hashtable["ChangeType"]);
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.PLAYER_ATTACK)
        {
            ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent.CustomData;

            int attackerIndex = (int)hashtable["AttackerIndex"];
            int targetIndex = (int)hashtable["TargetIndex"];
            bool success = (bool)hashtable["Success"];

            if (ActorIndex == attackerIndex)
                StartCoroutine(Attack(targetIndex, success));
        }
    }
}
