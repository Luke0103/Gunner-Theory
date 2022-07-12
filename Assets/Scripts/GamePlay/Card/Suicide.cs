using Photon.Realtime;

public class Suicide : Card
{
    public Suicide()
    {
        name = "자폭";
        desc = "본인을 포함한 모든 플레이어의 격발 확률을 10% 감소시킨다.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_SUICIDE, null, eventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
}
