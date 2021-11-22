using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBrickDestroy : MonoBehaviour
{
    void OnCollisionEnter(Collision collidedWithThis)
    {

        if (collidedWithThis.gameObject.name == "Floor" || collidedWithThis.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }
    }
}
