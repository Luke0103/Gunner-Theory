using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : Card
{
    public Thief()
    {
        name = "����";
        desc = "�ݹ� �� ����� �ڱ� 80$�� �����´�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        CardManager.Instance.Thief = true;
    }
}
