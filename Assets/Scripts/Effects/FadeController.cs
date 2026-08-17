using System.Collections;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public GameObject fadePanelObject;
    public static FadeController instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator FadeOut(float duration = 1f)
    {
        if (fadePanelObject != null)
            fadePanelObject.SetActive(true);

        float fadeOutTimePassed = 0f;

        while (fadeOutTimePassed < duration)
        {
            fadePanel.alpha = Mathf.Lerp(0f, 1f, fadeOutTimePassed / duration);

            fadeOutTimePassed += Time.deltaTime;

            yield return null;
        }
        fadePanel.alpha = 1f;

    }

    public IEnumerator FadeIn(float duration = 1f)
    {
        if (fadePanelObject!=null )
            fadePanelObject.SetActive(true);

        fadePanel.alpha = 1f;

        float fadeInTimePassed= 0f;

        while (fadeInTimePassed < duration)
        {
            fadePanel.alpha = Mathf.Lerp(1f, 0f, fadeInTimePassed / duration);

            fadeInTimePassed+= Time.deltaTime;

            yield return null;
        }
        fadePanel.alpha = 0f;

        fadePanelObject.SetActive(false);
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }
}
