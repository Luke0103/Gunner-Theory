using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : Card
{
    public Thief()
    {
        name = "강도";
        desc = "격발 시 상대의 자금 80$를 가져온다.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        CardManager.Instance.Thief = true;
    }
}
