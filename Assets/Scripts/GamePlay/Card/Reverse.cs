using ExitGames.Client.Photon;
using Photon.Realtime;

public class Reverse : Card
{
    public Reverse()
    {
        name = "������";
        desc = "���� ������ �ݴ�� ��ȯ�մϴ�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_REVERSE, null, eventOptions, SendOptions.SendReliable);
    }
}
