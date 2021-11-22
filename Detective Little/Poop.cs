using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poop : MonoBehaviour
{

    [SerializeField] float max_bumps = 0.0f;
    [SerializeField] float thrust = 0.0f;
    private float bumps = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if(bumps >= max_bumps)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collidedWithThis)
    {
        if (collidedWithThis.gameObject.name == "player")
        {
            Rigidbody rb;
            rb = collidedWithThis.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(0, 0, thrust, ForceMode.Impulse);
        }

        if (collidedWithThis.gameObject.name == "Floor")
        {
            Destroy(gameObject);
        }

        if (collidedWithThis.gameObject.layer == 6 || collidedWithThis.gameObject.layer == 7)
        {
            bumps++;
        }
    }
}
