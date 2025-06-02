using UnityEngine;
using UnityEngine.UI;

public class InterfaceBarManager : MonoBehaviour
{
    [SerializeField] private Bar playerHealthBar;
    [SerializeField] private Bar playerSuperBar;
    [SerializeField] private Bar enemyHealthBar;
    [SerializeField] private Bar enemySuperBar;

    public void CreateBar(float maxPHealth, float maxPSuper, float maxEHealth, float maxESuper)
    {
        playerHealthBar.Create(maxPHealth);
        playerSuperBar.Create(maxPSuper);
        enemyHealthBar.Create(maxEHealth);
        enemySuperBar.Create(maxESuper);
        UpdateHealth(maxPHealth, maxEHealth);
        UpdateSuper(0, 0);
    }

    public void UpdateHealth(float pHealth, float eHealth)
    {
        playerHealthBar.UpdateBar(pHealth);
        enemyHealthBar.UpdateBar(eHealth);
    }

    public void UpdateSuper(float pSuper, float eSuper)
    {
        playerSuperBar.UpdateBar(pSuper);
        enemySuperBar.UpdateBar(eSuper);
    }
}
