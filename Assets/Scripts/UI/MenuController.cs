using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string sceneName;
    public AudioClip clickSfx;

    void Start()
    {
        if (PlayerPrefs.HasKey("Scene"))
        {
            sceneName = PlayerPrefs.GetString("Scene");
        }
        else
        {
            sceneName = "Level 1";
        }
    }

    void Update()
    {

    }

    public void OnNewGameButton()
    {

        PlayerPrefs.DeleteAll();
        CharacterSaveManager.DeleteAllSaves();
        SceneManager.LoadScene(sceneName);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
