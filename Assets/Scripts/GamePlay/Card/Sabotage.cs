using Photon.Realtime;

public class Sabotage : Card
{
    public Sabotage()
    {
        name = "공작";
        desc = "격발 실패 시 목표 플레이어의 격발 확률을 15%로 설정합니다.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        CardManager.Instance.Sabotage = true;
    }
}
