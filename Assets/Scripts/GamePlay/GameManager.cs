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

    //턴 넘기기 이벤트
    public void InvokeNextTurnEvent()
    {
        //승리자 탄생
        if (SurvivingPlayerCount == 1)
        {
            IsActiveGameTime = false; //유효 인게임 시간 해제

            //마지막 라운드 도달 시 최종 승자 판별
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
        float waitTime = 5f; //다음 라운드까지 5초 대기

        roundOverObject.SetActive(true);
        roundOverPrizeText.text = string.Format("Prize - ${0}", prize); //라운드 승자 상금 표기
        playerList[idx].ChangeBank(prize); //라운드 승자에게 상금 제공
        PrizeManager.Instance.ChangePrize(0, true);
        
        //5초간 대기
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        roundOverObject.SetActive(false);
        PrizeManager.Instance.ChangePrize(DefaultValues.BasePrize, true); //상금 풀 초기화
        yield return StartCoroutine(StartNextRound()); //다음 라운드로 이행 및 코루틴 종료까지 대기

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent
            ((byte)DefaultValues.EventCode.ROUND_OVER, null, eventOptions, SendOptions.SendReliable);

        IsActiveGameTime = true; //유효 인게임 시간 설정
    }

    private IEnumerator GameOverCoroutine(int idx, int prize)
    {
        float waitTime = 5f; //메인 화면 복귀까지 5초 대기

        yield return StartCoroutine(NextRoundCoroutine(idx, prize)); //라운드 종료 처리

        gameOverVictorText.text = string.Format("Victor - Player No. {0}", GetFinalVictorIndex() + 1);
        gameOverObject.SetActive(true);

        //5초간 대기
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        gameOverObject.SetActive(false);
        NetworkManager.Instance.GetClient.OpLeaveRoom(false); //로비 떠나기
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene"); //메인 화면 복귀
    }

    public IEnumerator StartNextRound()
    {
        CurrentRound++;
        SurvivingPlayerCount = 0;

        //죽은 플레이어들 복귀, 단 파산한 플레이어는 제외
        for (int i = 0; i < RemainingPlayers.Length; i++)
        {
            RemainingPlayers[i] = PlayersInGame[i];
            if (RemainingPlayers[i])
            {
                playerList[i].Return();
            }
        }

        //새 라운드 준비 완료까지 대기
        while (!IsReadyForNextRound())
        {
            yield return new WaitForEndOfFrame();
        }

        UpdateRoundText(CurrentRound);

        if (IsLocalTurn)
            TurnManager.NextTurn();
    }
}
