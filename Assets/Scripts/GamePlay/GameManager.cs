using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public delegate void NextTurnEventHandler();
    public event NextTurnEventHandler NextTurnEvent;

    public delegate void StartGameEventHandler();
    public event StartGameEventHandler StartGameEvent;

    public List<PlayerManager> playerList = new List<PlayerManager>();
    public PlayerManager localPlayer { get; private set; }

    [SerializeField] private GameObject roundOverObject;
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private GameObject nextTurnButton;

    [SerializeField] private Text roundOverPrizeText;
    [SerializeField] private Text roundInfoText;
    [SerializeField] private Text gameOverVictorText;

    public bool[] RemainingPlayers { get; set; }
    public bool[] PlayersInGame { get; set; }

    public bool IsClockwise { get; set; }
    public bool IsLocalTurn { get { return CurrentPlayerIndex == LocalPlayerIndex; } }
    public bool IsActiveGameTime { get; private set; }

    public int CurrentPlayerIndex { get; set; }
    public int CurrentPlayerCount { get; set; }
    public int SurvivingPlayerCount { get; set; }
    public int LocalPlayerIndex { get; private set; }
    public int CurrentRound { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        IsActiveGameTime = true;
        IsClockwise = true;
        CurrentPlayerIndex = 0;
        CurrentPlayerCount = (int)NetworkManager.Instance.GetClient.CurrentRoom.MaxPlayers;
        CurrentRound = 1;
        SurvivingPlayerCount = CurrentPlayerCount;
        RemainingPlayers = new bool[CurrentPlayerCount];
        PlayersInGame = new bool[CurrentPlayerCount];
        for (int i = 0; i < RemainingPlayers.Length; i++)
        {
            RemainingPlayers[i] = true;
            PlayersInGame[i] = true;
        }

        UpdateRoundText(1);
    }

    public void SetLocalPlayer(PlayerManager pm)
    {
        if (pm.IsLocal)
        {
            localPlayer = pm;
            LocalPlayerIndex = pm.ActorIndex;
        }
    }

    private void UpdateRoundText(int round)
    {
        if (round > DefaultValues.MaxRound)
            return;

        roundInfoText.text = string.Format("Round {0} / {1}", round, DefaultValues.MaxRound);
    }

    private int GetRoundVictorIndex()
    {
        for (int i = 0; i < RemainingPlayers.Length; i++)
        {
            if (RemainingPlayers[i])
                return i;
        }

        return -1;
    }

    private int GetFinalVictorIndex()
    {
        int max = 0;
        int idx = 0;

        for (int i = 0; i < RemainingPlayers.Length; i++)
        {
            if (max < playerList[i].GetBank)
            {
                max = playerList[i].GetBank;
                idx = i;
            }
        }

        return idx;
    }

    public bool IsReadyForNextRound()
    {
        for (int i = 0; i < RemainingPlayers.Length; i++)
        {
            if (RemainingPlayers[i] != PlayersInGame[i])
            {
                return false;
            }
        }

        return true;
    }

    //�� �ѱ�� �̺�Ʈ
    public void InvokeNextTurnEvent()
    {
        //�¸��� ź��
        if (SurvivingPlayerCount == 1)
        {
            IsActiveGameTime = false; //��ȿ �ΰ��� �ð� ����

            //������ ���� ���� �� ���� ���� �Ǻ�
            if (CurrentRound >= DefaultValues.MaxRound)
            {
                nextTurnButton.SetActive(false);
                StartCoroutine(GameOverCoroutine(GetRoundVictorIndex(), PrizeManager.Instance.GetPrize));
                return;
            }

            nextTurnButton.SetActive(false);
            StartCoroutine(NextRoundCoroutine(GetRoundVictorIndex(), PrizeManager.Instance.GetPrize));
            return;
        }

        NextTurnEvent.Invoke();
    }

    public void InvokeStartGameEvent()
    {
        StartGameEvent.Invoke();
    }

    private IEnumerator NextRoundCoroutine(int idx, int prize)
    {
        float waitTime = 5f; //���� ������� 5�� ���

        roundOverObject.SetActive(true);
        roundOverPrizeText.text = string.Format("Prize - ${0}", prize); //���� ���� ��� ǥ��
        playerList[idx].ChangeBank(prize); //���� ���ڿ��� ��� ����
        PrizeManager.Instance.ChangePrize(0, true);
        
        //5�ʰ� ���
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        roundOverObject.SetActive(false);
        PrizeManager.Instance.ChangePrize(DefaultValues.BasePrize, true); //��� Ǯ �ʱ�ȭ
        yield return StartCoroutine(StartNextRound()); //���� ����� ���� �� �ڷ�ƾ ������� ���

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent
            ((byte)DefaultValues.EventCode.ROUND_OVER, null, eventOptions, SendOptions.SendReliable);

        IsActiveGameTime = true; //��ȿ �ΰ��� �ð� ����
    }

    private IEnumerator GameOverCoroutine(int idx, int prize)
    {
        float waitTime = 5f; //���� ȭ�� ���ͱ��� 5�� ���

        yield return StartCoroutine(NextRoundCoroutine(idx, prize)); //���� ���� ó��

        gameOverVictorText.text = string.Format("Victor - Player No. {0}", GetFinalVictorIndex() + 1);
        gameOverObject.SetActive(true);

        //5�ʰ� ���
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        gameOverObject.SetActive(false);
        NetworkManager.Instance.GetClient.OpLeaveRoom(false); //�κ� ������
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene"); //���� ȭ�� ����
    }

    public IEnumerator StartNextRound()
    {
        CurrentRound++;
        SurvivingPlayerCount = 0;

        //���� �÷��̾�� ����, �� �Ļ��� �÷��̾�� ����
        for (int i = 0; i < RemainingPlayers.Length; i++)
        {
            RemainingPlayers[i] = PlayersInGame[i];
            if (RemainingPlayers[i])
            {
                playerList[i].Return();
            }
        }

        //�� ���� �غ� �Ϸ���� ���
        while (!IsReadyForNextRound())
        {
            yield return new WaitForEndOfFrame();
        }

        UpdateRoundText(CurrentRound);

        if (IsLocalTurn)
            TurnManager.NextTurn();
    }
}
