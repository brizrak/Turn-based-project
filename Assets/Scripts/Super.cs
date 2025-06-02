using System.Collections.Generic;
using UnityEngine;

public class Super : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject superButton;
    
    public event Actions.SuperButton OnSuperButtonPressed;
    
    public void CastSuper(List<Unit> units)
    {
        foreach (var unit in units)
        {
            unit.TakeDamage(damage);
        }
    }

    public void PressSuperButton()
    {
        OnSuperButtonPressed?.Invoke();
    }
}
