using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashManager : MonoBehaviour
{
    public CanvasGroup logo;
    public float fadeDuration = 1.5f;
    public float visibleTime = 2f;

    void Start()
    {
        StartCoroutine(SplashSequence());
    }

    IEnumerator SplashSequence()
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(visibleTime);
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator FadeIn()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            logo.alpha = time / fadeDuration;
            yield return null;
        }
        logo.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            logo.alpha = 1 - (time / fadeDuration);
            yield return null;
        }
        logo.alpha = 0;
    }
}
