using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMovable : MonoBehaviour
{
    [SerializeField] private PlayerState state;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private GameObject collider;
    private bool freezeY = true;

    // Update is called once per frame
    void Update()
    {
        if (freezeY)
        {
            if (state.CodeTriggered("BOX"))
            {
                freezeY = false;
                rigidbody.constraints = rigidbody.constraints ^ RigidbodyConstraints.FreezePositionX;
            }
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == collider)
        {
            collider.SetActive(false);
        }
    }
}
