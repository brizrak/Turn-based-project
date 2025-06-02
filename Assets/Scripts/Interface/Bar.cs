using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private Image bar;

    private float _maxValue;

    public void Create(float maxValue)
    {
        _maxValue = maxValue;
    }

    public void UpdateBar(float value)
    {
        bar.fillAmount = value / _maxValue;
    }
}
