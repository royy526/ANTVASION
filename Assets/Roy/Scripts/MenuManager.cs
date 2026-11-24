using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public SceneFade fader;

    public void Play()
    {
        fader.FadeToScene("Patio");
    }
    
    public void Exit()
    {
        Application.Quit();
        Debug.Log("SALIENDO DEL JUEGO");
    }
}

