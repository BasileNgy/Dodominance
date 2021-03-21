using UnityEngine;
using UnityEngine.UI;

public class HUDBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public void SetMyMaxValue(float myMaxValue)
    {
        slider.maxValue = myMaxValue;
    }

    public void SetMyCurrentValue(float myValue)
    {
        slider.value = myValue;
    }
}
