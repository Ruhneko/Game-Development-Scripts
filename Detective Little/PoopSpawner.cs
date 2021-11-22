using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSpawner : MonoBehaviour
{
    [SerializeField] private GameObject poop;
    [SerializeField] private float delay = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPoop(delay));
    }

    private IEnumerator SpawnPoop(float waitTime)
    {
        while (true)
        {
            GameObject myObj = GameObject.Instantiate<GameObject>(poop, this.transform);
            myObj.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
