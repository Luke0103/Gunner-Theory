using Photon.Realtime;

public class Suicide : Card
{
    public Suicide()
    {
        name = "����";
        desc = "������ ������ ��� �÷��̾��� �ݹ� Ȯ���� 10% ���ҽ�Ų��.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_SUICIDE, null, eventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
}
