using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheese : MonoBehaviour
{
    [SerializeField] private string sceneName;

//Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider collidedWithThis)
    {
        if (collidedWithThis.gameObject.name == "player")
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
