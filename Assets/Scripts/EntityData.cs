using UnityEngine;

[System.Serializable]
public class EntityData
{
    public string entityName;
    public int maxHealth;
    public int rolls;
    public int mainStat;
    public int minAttackDamage;
    public int maxAttackDamage;
    public int dodge;
    public int speed; // Détermine l'ordre de passage lors des combats
}
