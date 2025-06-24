using System.Collections.Generic;
using UnityEngine;

public class PartyCarrier : MonoBehaviour
{
    public static PartyCarrier Instance { get; private set; }

    public List<PlayerData> playerParty = new List<PlayerData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
