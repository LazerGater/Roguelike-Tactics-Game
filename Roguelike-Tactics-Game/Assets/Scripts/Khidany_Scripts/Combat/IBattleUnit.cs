using UnityEngine;

public interface IBattleUnit
{
    void SetGridPos(Vector2Int pos);
    Vector2Int GetGridPosition();
}
