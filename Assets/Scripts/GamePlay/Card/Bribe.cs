using Photon.Realtime;

public class Bribe : Card
{
    public Bribe()
    {
        name = "뇌물";
        desc = "즉시 상금 100$를 받습니다.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        if (GameManager.Instance.IsLocalTurn)
        {
            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.Receivers = ReceiverGroup.All;

            NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_BRIBE, GameManager.Instance.LocalPlayerIndex, eventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
    }
}
