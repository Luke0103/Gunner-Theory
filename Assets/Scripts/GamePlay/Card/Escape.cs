using Photon.Realtime;
using ExitGames.Client.Photon;

public class Escape : Card
{
    public Escape()
    {
        name = "도주";
        desc = "현재 모인 상금의 20%를 가지고 이번 라운드에서 이탈한다.";
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
