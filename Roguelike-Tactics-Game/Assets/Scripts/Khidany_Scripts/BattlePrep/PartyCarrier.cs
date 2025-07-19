using System.Collections.Generic;
using UnityEngine;

public class PartyCarrier : MonoBehaviour
{
    public static PartyCarrier Instance { get; private set; }

    public List<PartyMember> playerParty = new List<PartyMember>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
