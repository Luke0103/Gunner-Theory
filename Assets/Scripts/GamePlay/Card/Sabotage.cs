using Photon.Realtime;

public class Sabotage : Card
{
    public Sabotage()
    {
        name = "����";
        desc = "�ݹ� ���� �� ��ǥ �÷��̾��� �ݹ� Ȯ���� 15%�� �����մϴ�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        CardManager.Instance.Sabotage = true;
    }
}
