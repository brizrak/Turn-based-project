using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Magic")]
public class MagicWeapon : Weapon
{
    [SerializeField] private float aoeRadius = 1.5f;
    [SerializeField] private Material attackMaterial;
    
    public override void Attack(Unit attacker, Unit target)
    {
        target.TakeDamage(baseDamage * attacker.damageModifier);
        
        var hits = Physics.OverlapSphere(target.transform.position, aoeRadius);
        foreach (var hit in hits)
        {
            var unit = hit.GetComponent<Unit>();
            if (unit is not null && unit != target)
            {
                unit.TakeDamage(baseDamage / 2 * attacker.damageModifier);
            }
        }
    }
    
    public override IEnumerator VisualizeAttack(Vector3 start, Vector3 end)
    {
        var aoe = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(aoe.GetComponent<Collider>());
        aoe.transform.position = end;
        aoe.transform.localScale = Vector3.one * aoeRadius * 2;
        aoe.GetComponent<Renderer>().material = attackMaterial;
        
        yield return new WaitForSeconds(0.3f);
        Destroy(aoe);
    }
}