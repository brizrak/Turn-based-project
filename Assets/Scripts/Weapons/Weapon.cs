using System.Collections;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    [SerializeField] protected float baseDamage;
    
    public int minRange = 1;
    public int maxRange;

    public virtual void Attack(Unit attacker, Unit target)
        => target.TakeDamage((int)(baseDamage * attacker.damageModifier));

    public virtual IEnumerator VisualizeAttack(Vector3 start, Vector3 end)
    {
        yield return null;
    }
}