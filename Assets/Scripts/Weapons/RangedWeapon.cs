using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Ranged")]
public class RangedWeapon : Weapon
{
    [SerializeField] private Material attackMaterial;
    
    public override void Attack(Unit attacker, Unit target)
    {
        var distance = Mathf.Max(
            Mathf.Abs(attacker.transform.position.x - target.transform.position.x),
            Mathf.Abs(attacker.transform.position.y - target.transform.position.y)
        );
        
        var finalDamage = Mathf.Max(1, (baseDamage * attacker.damageModifier) - distance * 2);
        target.TakeDamage(finalDamage);
    }
    
    public override IEnumerator VisualizeAttack(Vector3 start, Vector3 end)
    {
        var line = new GameObject("AttackLine");
        var lr = line.AddComponent<LineRenderer>();
        lr.material = attackMaterial;
        lr.startColor = attackMaterial.color;
        lr.endColor = attackMaterial.color;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.SetPositions(new Vector3[] { start, end });
                
        yield return new WaitForSeconds(0.3f);
        Destroy(line);
    }
}