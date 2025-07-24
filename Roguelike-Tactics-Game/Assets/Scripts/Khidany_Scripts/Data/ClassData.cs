using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "Game/Class Data")]
public class ClassData : ScriptableObject
{
    public string className;
    public int moveRange = 4;
    public string weaponType;

    [Header("Growth Modifiers (%)")]
    public int hpMod = 0;
    public int atkMod = 0;
    public int defMod = 0;
    public int speedMod = 0;
    public int luckMod = 0;
    public int dexMod = 0;

    [Header("Enemy Flat Stat Bonuses")]
    public int hpBonus = 0;
    public int atkBonus = 0;
    public int defBonus = 0;
    public int speedBonus = 0;
    public int luckBonus = 0;
    public int dexBonus = 0;
}
