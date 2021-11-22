using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
