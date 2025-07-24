using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    public Sprite portrait;

    [Header("Base Stats")]
    public int baseLevel = 1;
    public int baseHP = 20;
    public int baseAtk = 5;
    public int baseDef = 2;
    public int baseSpeed = 3;
    public int baseLuck = 1;
    public int baseDex = 2;

    [Header("Growth Rates (%)")]
    public int hpGrowth = 50;
    public int atkGrowth = 50;
    public int defGrowth = 50;
    public int speedGrowth = 50;
    public int luckGrowth = 50;
    public int dexGrowth = 50;
}
