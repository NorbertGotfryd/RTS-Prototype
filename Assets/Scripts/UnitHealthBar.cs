using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    public GameObject healthCointainer;
    public RectTransform healthFill;

    private float maxSize;

    private void Awake()
    {
        maxSize = healthFill.sizeDelta.x;
        healthCointainer.SetActive(false);
    }

    public void UpdateHealthBar(int currentHP, int maxHP)
    {
        healthCointainer.SetActive(true);
        float healthPercentage = (float)currentHP / (float)maxHP;
        healthFill.sizeDelta = new Vector2(maxSize * healthPercentage, healthFill.sizeDelta.y);
    }
}
