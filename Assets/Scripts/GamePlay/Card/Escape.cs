using Photon.Realtime;
using ExitGames.Client.Photon;

public class Escape : Card
{
    public Escape()
    {
        name = "����";
        desc = "���� ���� ����� 20%�� ������ �̹� ���忡�� ��Ż�Ѵ�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        if (GameManager.Instance.IsLocalTurn)
        {
            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.Receivers = ReceiverGroup.All;

            NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_ESCAPE, GameManager.Instance.LocalPlayerIndex, eventOptions, SendOptions.SendReliable);
        }
    }
}
