using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SkillItem : MonoBehaviour
{
    [SerializeField, Space] private Points points;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private UnityEvent<Disease> onLevelUp;
    [SerializeField] private Disease disease;
    [SerializeField] private int cost = 10;

    public int Level => level;
    private int level;
    
    public void OnButtonDown()
    {
        if (points.TrySpend(cost))
        {
            level++;
            levelText.text = level.ToString();
            onLevelUp?.Invoke(disease);
        }
    }
}
