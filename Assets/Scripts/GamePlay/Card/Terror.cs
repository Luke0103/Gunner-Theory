using Photon.Realtime;

public class Terror : Card
{
    public Terror()
    {
        name = "테러";
        desc = "모든 플레이어의 카드를 파괴합니다.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.CARD_TERROR, null, RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
}
