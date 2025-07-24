[System.Serializable]
public class PartyMember
{
    public CharacterData character;
    public ClassData unitClass;
    public bool isSelectedForBattle = true;
    public int priorityID; // Used for ordering in battle spawn
}
