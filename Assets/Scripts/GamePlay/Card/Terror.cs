using Photon.Realtime;

public class Terror : Card
{
    public Terror()
    {
        name = "�׷�";
        desc = "��� �÷��̾��� ī�带 �ı��մϴ�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_TERROR, null, RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
}
