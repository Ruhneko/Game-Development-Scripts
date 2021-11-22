using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBricksTrigger : MonoBehaviour
{
    [SerializeField] private GameObject brick;

    void OnCollisionEnter(Collision collidedWithThis)
    {
        if (collidedWithThis.gameObject.name == "player"){
            GetComponent<Rigidbody>().isKinematic = false;
            brick.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(gameObject);
        }
    }
}