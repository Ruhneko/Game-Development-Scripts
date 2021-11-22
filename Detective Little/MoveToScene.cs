using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToScene : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    [SerializeField] private string code;
    [SerializeField] private string scene;
    private bool loaded = false;

    // Update is called once per frame
    void Update()
    {
        if (!loaded)
        {
            if (state.CodeTriggered(code))
            {
                loaded = true;
                SceneManager.LoadScene(scene);
            }
        }
    }
}
