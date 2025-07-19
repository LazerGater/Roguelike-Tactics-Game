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
}
