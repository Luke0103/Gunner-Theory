using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Provoke : Card
{
    public Provoke()
    {
        name = "����";
        desc = "�ڽ��� �ݹ� Ȯ���� ��� 90%�� �����Ѵ�.";
    }

    public override void CardEvent()
    {
        base.CardEvent();

        GameManager.Instance.localPlayer.ChangeChance(90);
    }
}
