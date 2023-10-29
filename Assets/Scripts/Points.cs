using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Points : MonoBehaviour
{
    [SerializeField] private int points = 100;
    [SerializeField] private TextMeshProUGUI pointsText;

    public static Points Instance {get; private set;}

    public int PointsAmount => points;

    private void Start()
    {
        Instance = this;
        pointsText.text = points.ToString();
    }


    public bool TrySpend(int cost) 
    {
        if (points >= cost) 
        {
            points -= cost;
            pointsText.text = points.ToString();
            return true;
        }

        return false;
    }

    public void AddPoints(int amount)
    {
        points += amount;
        pointsText.text = points.ToString();
    }
}
