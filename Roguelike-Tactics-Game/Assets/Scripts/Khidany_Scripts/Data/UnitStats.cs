using UnityEngine;

[System.Serializable]
public class UnitStats
{
    public int maxHP;
    public int currentHP;
    public int atk;
    public int def;
    public int speed;
    public int luck;
    public int dex;
    public int moveRange;
    public string displayName;
    public int level;
    public Sprite portrait;


    // Constructor for players (based on CharacterData + ClassData)
    // Constructor for players (uses baseLevel from CharacterData)
    public UnitStats(CharacterData c, ClassData cl)
    {
        level = c.baseLevel;

        maxHP = c.baseHP + cl.hpMod;
        atk = c.baseAtk + cl.atkMod;
        def = c.baseDef + cl.defMod;
        speed = c.baseSpeed + cl.speedMod;
        luck = c.baseLuck + cl.luckMod;
        dex = c.baseDex + cl.dexMod;

        currentHP = maxHP;
        moveRange = cl.moveRange;

        displayName = c.characterName;
        portrait = c.portrait;
    }


    // Constructor for enemies (fully custom stats)
    public UnitStats(CharacterData c, ClassData cl, int level, int hp, int atk, int def, int spd, int luck, int dex)
    {
        this.level = level;

        maxHP = hp;
        currentHP = hp;
        this.atk = atk;
        this.def = def;
        this.speed = spd;
        this.luck = luck;
        this.dex = dex;
        moveRange = cl.moveRange;

        displayName = c.characterName;
        portrait = c.portrait;
    }

    public UnitStats() { }
}
