using UnityEngine;
using UnityEngine.UI;

public class HealthBarOverlay : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        health.OnPointsChanged += UpdateVisual;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        fillImage.fillAmount = health.Ratio;
    }
}