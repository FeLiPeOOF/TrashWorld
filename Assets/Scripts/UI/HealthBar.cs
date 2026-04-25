using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    public void SetHealth(float value)
    {
        fillImage.fillAmount = value;
    }

    internal void SetMaxHealth(int maxHealth)
    {
        throw new NotImplementedException();
    }
}