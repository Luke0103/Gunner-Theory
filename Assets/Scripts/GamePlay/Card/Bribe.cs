using Photon.Realtime;

public class Bribe : Card
{
    public Bribe()
    {
        name = "����";
        desc = "��� ��� 100$�� �޽��ϴ�.";
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
