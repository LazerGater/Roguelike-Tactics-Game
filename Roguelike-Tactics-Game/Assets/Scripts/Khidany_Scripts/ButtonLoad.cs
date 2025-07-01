using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLoad : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
        
}
