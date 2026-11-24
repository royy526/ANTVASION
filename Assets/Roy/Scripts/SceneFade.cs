using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFade : MonoBehaviour
{
    public Image fadeImage;
    public float duration = 2f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float alpha = 1 - (time / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        float time = 0;

        while(time < duration)
        {
            time += Time.deltaTime;

            float alpha = time / duration;
            fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
