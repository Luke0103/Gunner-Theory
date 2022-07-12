using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CardManager : MonoBehaviour, IOnEventCallback
{
    public static CardManager Instance { get; private set; }

    private GameManager gm;
    [SerializeField] private Text cardNameText;
    [SerializeField] private Text cardDescText;

    [SerializeField] private GameObject cardUsedObject;
    [SerializeField] private Text cardUsedNameText;
    [SerializeField] private Text cardUsedDescText;

    private Card[] CardData =
    {
        new Provoke(),
        new Suicide(),
        new Oiling(),
        new Escape(),
        new Fund(),
        new Bribe(),
        new Reverse(),
        new Terror(),
        new Sabotage(),
        new Thief()
    };
    private Card currentCard;

    public bool Sabotage { get; set; }
    public bool Thief { get; set; }

    private void Start()
    {
        Instance = this;
        gm = GameManager.Instance;
        DrawNewCard();
        Sabotage = false;

        NetworkManager.Instance.GetClient.AddCallbackTarget(this);
    }

    public void DrawNewCard()
    {
        int rndIdx = Random.Range(0, CardData.Length);
        currentCard = CardData[rndIdx];

        cardNameText.text = CardData[rndIdx].GetName();
        cardDescText.text = CardData[rndIdx].GetDesc();
    }

    public void OnCardUse()
    {
        if (!gm.IsLocalTurn)
            return;

        currentCard.CardEvent();
        DisposeCard();
    }

    public void DisposeCard()
    {
        cardNameText.text = "";
        cardDescText.text = "";
        currentCard = null;
    }

    public void AttackCardActions(int attackerIndex, int targetIndex)
    {
        if (Sabotage)
        {
            GameManager.Instance.playerList[targetIndex].ChangeChance(15);
            Sabotage = false;
        }

        if (Thief)
        {
            ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
            table["AttackerIndex"] = attackerIndex;
            table["TargetIndex"] = targetIndex;

            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.Receivers = ReceiverGroup.All;

            NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_THIEF, table, eventOptions, SendOptions.SendReliable);
            Thief = false;
        }
    }

    private IEnumerator CardUsedCoroutine()
    {
        cardUsedObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        cardUsedObject.SetActive(false);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_USED)
        {
            string name = ((Dictionary<string, string>)photonEvent.CustomData)["CardName"];
            string desc = ((Dictionary<string, string>)photonEvent.CustomData)["CardDesc"];

            cardUsedNameText.text = name;
            cardUsedDescText.text = desc;

            StartCoroutine(CardUsedCoroutine());
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_SUICIDE)
        {
            GameManager.Instance.localPlayer.ChangeChance(-10, true);
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_ESCAPE)
        {
            int withdraw = (int)(PrizeManager.Instance.GetPrize * 0.2f);
            int idx = (int)photonEvent.CustomData;
            PlayerManager player = GameManager.Instance.playerList[idx];

            PrizeManager.Instance.ChangePrize(withdraw * -1);
            player.ChangeBank(withdraw);
            player.Escape();

            TurnManager.NextTurn();
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_REVERSE)
        {
            GameManager.Instance.IsClockwise = !GameManager.Instance.IsClockwise;
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_FUND)
        {
            int idx = (int)photonEvent.CustomData;
            GameManager.Instance.playerList[idx].ChangeBank(50);
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_BRIBE)
        {
            int idx = (int)photonEvent.CustomData;
            GameManager.Instance.playerList[idx].ChangeBank(100);
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_TERROR)
        {
            DisposeCard();
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_SABOTAGE)
        {
            int idx = (int)photonEvent.CustomData;

            GameManager.Instance.playerList[idx].ChangeChance(15);
        }

        if (photonEvent.Code == (byte)DefaultValues.EventCode.CARD_THIEF)
        {
            int tgtIdx = (int)((ExitGames.Client.Photon.Hashtable)photonEvent.CustomData)["TargetIndex"];
            int atkIdx = (int)((ExitGames.Client.Photon.Hashtable)photonEvent.CustomData)["AttackerIndex"];

            int value = DefaultValues.ThiefValue;

            if (GameManager.Instance.playerList[tgtIdx].GetBank < value)
            {
                value = GameManager.Instance.playerList[tgtIdx].GetBank;
            }

            GameManager.Instance.playerList[tgtIdx].ChangeBank(-value);
            GameManager.Instance.playerList[atkIdx].ChangeBank(value);
        }
    }
}
