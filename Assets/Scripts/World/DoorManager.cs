using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public string sceneName;
    public string doorArea;

    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PlayerPrefs.SetString("Area", doorArea);
            GoToScene();
        }
    }

    public void GoToScene()
    {
        PlayerController.instance.ClearSavedPosition();

        PlayerPrefs.SetString("Scene", sceneName);
        SceneManager.LoadScene(sceneName);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}

