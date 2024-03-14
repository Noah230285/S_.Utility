using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string PreviousActive { get; private set; }

    public static Scene LoadOrFindScene(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);
        if (!scene.IsValid())
        {
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
            scene = SceneManager.GetSceneByName(name);
        }
        return scene;
    }

    public static void LoadSingleScene(string name)
    {
        PreviousActive = SceneManager.GetActiveScene().name;
        ResetGameValues.Instace?.ResetValues();
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    public static void LoadPreviousActive()
    {
        LoadSingleScene(PreviousActive);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
