using System.Collections.Generic;
using UnityEngine;

public class BattlePrepManager : MonoBehaviour
{
    [Header("References")]
    public GameObject gridManagerObject; // your GridManager in the scene

    private List<Vector2Int> startingTiles;
    private int partyLimit;

    private void Start()
    {
        // 1) Pull map data
        var initializer = FindFirstObjectByType<GridInitializer>();
        startingTiles = new List<Vector2Int>(initializer.AllySpawns);
        partyLimit = initializer.PartyLimit;

        // 2) Spawn party into battle
        SpawnParty();



        // 4) Begin turn loop
        TurnManager.Instance.StartBattle();
    }

    private void SpawnParty()
    {
        var gm = gridManagerObject.GetComponent<GridManager>();
        gm.SpawnSelectedParty(startingTiles, false);
    }

    //public void RefreshGridPreview()
         //{
         //    var gm = gridManagerObject.GetComponent<GridManager>();
         //    gm.SpawnSelectedParty(startingTiles, true);
         //}

    //private void HighlightStartTiles(bool on)
    //{
    //    var gm = gridManagerObject.GetComponent<GridManager>();
    //    foreach (var pos in startingTiles)
    //    {
    //        if (on) gm.HighlightTile(pos);
    //        else gm.ClearTileHighlight(pos);
    //    }
    //}


}    