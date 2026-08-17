using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleResultController : MonoBehaviour
{
    public static BattleResultController Instance { get; private set; }

    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowEndScreen(bool playerWon)
    {
        winPanel.SetActive(playerWon);
        losePanel.SetActive(!playerWon);
        if(!playerWon)
            PlayerController.instance.ClearSavedPosition();
    }

    public void BackToMap()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void OnAutoBattleButtonPressed()
    {
        BattleController.instance.ToggleAutoBattle();
    }
}

