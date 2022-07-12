using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oiling : Card
{
    private int remainingTurn = 0;

    public Oiling()
    {
        name = "기름칠";
        desc = "자신의 격발 확률 증감 수치를 5턴간 1.5배로 조정한다.";
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
