using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource audioSourceA;
    [SerializeField] private AudioSource audioSourceB;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float maxVolume = 0.5f;
    [SerializeField] private SceneMusic[] sceneMusicList;
    private AudioSource currentSource;
    private AudioSource nextSource;
    private AudioClip currentClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentSource = audioSourceA;
        nextSource = audioSourceB;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipForScene = GetClipForScene(scene.name);

        if (clipForScene == null)
        {
            StopMusic();
            return;
        }

        if (clipForScene == currentClip) return;

        StartCoroutine(FadeToNewCLip(clipForScene));
    }

    AudioClip GetClipForScene(string sceneName)
    {
        foreach (SceneMusic sm in sceneMusicList)
        {
            if (sm.sceneName == sceneName)
            {
                return sm.musicClip;
            }
        }
        return null;
    }

    IEnumerator FadeToNewCLip(AudioClip newClip)
    {
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            currentSource.volume = Mathf.Lerp(maxVolume, 0f, progress);
            nextSource.volume = Mathf.Lerp(0f, maxVolume, progress);

            yield return null;
        }

        currentSource.Stop();
        currentSource.clip = null;

        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;

        currentClip = newClip;
    }

    void StopMusic()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        float startVolume = currentSource.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            currentSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        currentSource.Stop();
        currentClip = null;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == currentClip) return;
        StartCoroutine(FadeToNewCLip(clip));
    }

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }
}