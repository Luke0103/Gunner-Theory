using Photon.Realtime;

public class Fund : Card
{
    public Fund()
    {
        name = "������";
        desc = "��� ��� 50$�� �޽��ϴ�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        if (GameManager.Instance.IsLocalTurn)
        {
            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.Receivers = ReceiverGroup.All;

            NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_FUND, GameManager.Instance.LocalPlayerIndex, eventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
    }
}
