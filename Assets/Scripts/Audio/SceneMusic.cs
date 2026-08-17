using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip musicScene;

    void Start()
    {
        if (musicScene != null)
        {
            AudioManager.instance.PlayMusic(musicScene);
        }

    }

}

