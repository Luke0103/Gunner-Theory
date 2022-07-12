using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oiling : Card
{
    private int remainingTurn = 0;

    public Oiling()
    {
        name = "�⸧ĥ";
        desc = "�ڽ��� �ݹ� Ȯ�� ���� ��ġ�� 5�ϰ� 1.5��� �����Ѵ�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        GameManager.Instance.localPlayer.ChangeRateScale *= 1.5f;
        GameManager.Instance.NextTurnEvent += new GameManager.NextTurnEventHandler(UpdateRemainingTurn);
        remainingTurn = 5;
    }

    private void UpdateRemainingTurn()
    {
        remainingTurn--;

        if (remainingTurn <= 0)
        {
            GameManager.Instance.localPlayer.ChangeRateScale *= 2f / 3f;
            GameManager.Instance.NextTurnEvent -= UpdateRemainingTurn;
        }
    }
}
