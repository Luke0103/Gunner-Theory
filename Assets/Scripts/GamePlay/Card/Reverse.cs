using ExitGames.Client.Photon;
using Photon.Realtime;

public class Reverse : Card
{
    public Reverse()
    {
        name = "역주행";
        desc = "진행 방향을 반대로 전환합니다.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_REVERSE, null, eventOptions, SendOptions.SendReliable);
    }
}
