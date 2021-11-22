using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    [SerializeField] private Transform camera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float perpendicular = -1 / camera.eulerAngles.y;
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            perpendicular,
            transform.eulerAngles.z
        );
    }
}
