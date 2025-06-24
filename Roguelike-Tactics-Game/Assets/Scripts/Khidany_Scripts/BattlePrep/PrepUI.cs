using UnityEngine;
using UnityEngine.UI;
public class BattlePrepUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject unitListPanel;
    [SerializeField] private GameObject PrepUI;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject mapViewPanel; 
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button unitListButton;
    [SerializeField] private Button mapViewButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button battleButton;
    [SerializeField] private Button forfeitButton;

    private bool battleStarted = false;
    public bool IsBattleStarted => battleStarted;


    public static BattlePrepUIManager Instance { get; private set; }
    public bool IsBlockingMapInput()
    {
        // Only blocks input when MapView is open (or later if other blockers exist)
        return mapViewPanel.activeSelf;
    }
    public static bool IsBattleActive { get; private set; } = false;



    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        unitListButton.onClick.AddListener(OpenUnitList);
        mapViewButton.onClick.AddListener(OpenMapView);
        settingsButton.onClick.AddListener(OpenSettings);
        battleButton.onClick.AddListener(OnBattleStart);
        forfeitButton.onClick.AddListener(OnForfeit);
    }

    public void OpenUnitList()
    {
        unitListPanel.SetActive(true);
        mapViewPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OpenMapView()
    {
        unitListPanel.SetActive(false);
        mapViewPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        unitListPanel.SetActive(false);
        mapViewPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBattleStart()
    {
        battleStarted = true;
        IsBattleActive = true;
        PrepUI.SetActive(false);
        // Trigger battle prep complete logic
    }

    public void OnForfeit()
    {
        // Load main menu or quit
    }
    private void HandleBack()
    {
        if (unitListPanel.activeSelf || mapViewPanel.activeSelf || settingsPanel.activeSelf)
        {
            // Close sub-panels and return to main prep menu
            unitListPanel.SetActive(false);
            mapViewPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
        else
        {
            // Already at base; maybe play error sound or flash UI
        }
    }

}
