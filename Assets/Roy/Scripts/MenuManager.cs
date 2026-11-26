using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public SceneFade fader;

    public void Play()
    {
        fader.FadeToScene("Pruebas_r");
    }
    
    public void Exit()
    {
        Application.Quit();
        Debug.Log("SALIENDO DEL JUEGO");
    }

    public void Settings()
    {
        fader.FadeToScene("Settings");
    }

    public void Tutorial()
    {
        fader.FadeToScene("Tutorial");
    }
}

