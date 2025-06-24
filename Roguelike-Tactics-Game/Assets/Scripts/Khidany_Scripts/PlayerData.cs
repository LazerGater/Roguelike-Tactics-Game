using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string unitID;
    public string unitName;
    public Sprite portrait;

    public int maxHP;
    public int currentHP;
    public int atk;
    public int def;
    public int speed;
    public int luck;
    public int dex;

    public int maxMovePoints;
    public bool isSelectedForBattle = true;

    public PlayerData Clone()
    {
        return (PlayerData)this.MemberwiseClone();
    }
}
