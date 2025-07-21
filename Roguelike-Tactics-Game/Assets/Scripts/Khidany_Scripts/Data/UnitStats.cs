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

    // Constructor for players (based on CharacterData + ClassData)
    public UnitStats(CharacterData c, ClassData cl)
    {
        maxHP = c.baseHP + cl.hpMod;
        atk = c.baseAtk + cl.atkMod;
        def = c.baseDef + cl.defMod;
        speed = c.baseSpeed + cl.speedMod;
        luck = c.baseLuck + cl.luckMod;
        dex = c.baseDex + cl.dexMod;

        currentHP = maxHP;
        moveRange = cl.moveRange;
    }

    // Constructor for enemies (fully custom stats)
    public UnitStats(int hp, int atk, int def, int speed, int luck, int dex, int moveRange)
    {
        this.maxHP = hp;
        this.currentHP = hp;
        this.atk = atk;
        this.def = def;
        this.speed = speed;
        this.luck = luck;
        this.dex = dex;
        this.moveRange = moveRange;
    }
    public UnitStats() { }
}
