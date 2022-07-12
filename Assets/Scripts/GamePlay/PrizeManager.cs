using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizeManager : MonoBehaviour
{
    public static PrizeManager Instance;

    [SerializeField] private TextMesh prizeText;

    public int GetPrize { get { return prize; } }

    private int prize;
    private float prizeInText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        prize = DefaultValues.BasePrize;
        prizeInText = prize;
        UpdatePrizeText(prize);
    }

    private void FixedUpdate()
    {
        prizeText.transform.eulerAngles = Camera.main.transform.eulerAngles;

        prizeInText = Mathf.Lerp(prizeInText, prize, 0.05f);
        UpdatePrizeText(Mathf.RoundToInt(prizeInText));
    }

    public void ChangePrize(int amount, bool isSubstitute = false)
    {
        if (isSubstitute)
        {
            prize = amount;
        }
        else
        {
            prize += amount;
        }
    }

    private void UpdatePrizeText(int prize)
    {
        prizeText.text = string.Format("{0}$", prize);
    }
}
