using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    public void SetHealth(float value)
    {
        if (fillImage == null)
        {
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(value);
    }
}
