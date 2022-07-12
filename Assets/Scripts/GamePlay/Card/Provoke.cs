using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Provoke : Card
{
    public Provoke()
    {
        name = "도발";
        desc = "자신의 격발 확률을 즉시 90%로 조정한다.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        GameManager.Instance.localPlayer.ChangeChance(90);
    }
}
