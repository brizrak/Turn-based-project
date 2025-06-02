using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Image healthFill;
    
    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        transform.position = target.position + offset;
        
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);
    }
    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthFill is null) return;
        healthFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }
}