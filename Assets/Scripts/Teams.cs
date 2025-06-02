using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Teams : MonoBehaviour
{
    [Serializable]
    public class UnitData
    {
        public GameObject prefab;
        public Weapon weapon;
        [Header("Data")]
        public float health;
        public float damageModifier;
        public int range;
        [Range(0f, 1f)]
        public float defence;
    }
    
    [SerializeField] private List<UnitData> playerTeam;
    [SerializeField] private List<UnitData> enemyTeam;
    [SerializeField] private int maxLenght;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material enemyMaterial;
    [SerializeField] private Weapon baseWeapon;

    private List<Unit> GetTeam(List<UnitData> newUnits, string name, Material material)
    {
        var team = new List<Unit>();
        int count = 0;
        var units = new GameObject(name);
        units.transform.SetParent(transform);
        foreach (var item in newUnits.Where(item => count < maxLenght))
        {
            var unit = Instantiate(item.prefab, Vector3.zero, Quaternion.identity, units.transform).GetComponent<Unit>();
            if (unit == null) continue;
            unit.GetComponent<Renderer>().material = material;
            unit.name = $"{name}_{count}";
            if (item.health > 0) unit.maxHealth = item.health;
            if (item.damageModifier > 0) unit.damageModifier = item.damageModifier;
            if (item.range > 0) unit.range = item.range;
            unit.defence = item.defence;
            unit.weapon = item.weapon ?? baseWeapon;
            team.Add(unit);
            count++;
        }
        return team;
    }
    
    public List<Unit> GetPlayerTeam() => GetTeam(playerTeam, "Player", playerMaterial);
    public List<Unit> GetEnemyTeam() => GetTeam(enemyTeam, "Enemy", enemyMaterial);
}
