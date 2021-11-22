using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform bottom;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float sideSpeed = 2f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float thrust = 200f;
    private Vector3 velocity = Vector3.zero;
    private Rigidbody rigidbody;
    private bool moving = false;

    private bool grounded = true;
    private bool onFallingBrick = false;
    private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        PlayerMove();
        PlayerAnimation();
    }

    private void PlayerInput()
    {
        velocity.x = Input.GetAxisRaw("Horizontal") * sideSpeed;
        velocity.z = Input.GetAxisRaw("Vertical");
    }


    void OnCollisionEnter(Collision collidedWithThis)
    {
        if (collidedWithThis.gameObject.layer == 7)
        {
            onFallingBrick = true;
        }
    }

    void OnCollisionExit(Collision collidedWithThis)
    {
        onFallingBrick = false;
    }

    private void PlayerMove()
    {
        transform.Translate(velocity * Time.deltaTime * speed);

        grounded = Physics.Raycast(bottom.position, Vector3.down, 0.5f);
        if (grounded && Input.GetKeyDown(KeyCode.Space) && !onFallingBrick)
        {
            grounded = false;
            rigidbody.AddForce(transform.up * thrust);
        }
    }

    private void PlayerAnimation()
    {
        if (!moving && velocity.magnitude > 0.1f)
        {
            moving = true;
            animator.SetBool("moving", true);
        }
        else if (moving && velocity.magnitude < 0.1f)
        {
            moving = false;
            animator.SetBool("moving", false);
        }

        if (velocity.x > 0)
        {
            sprite.flipX = false;
        }
        else if (velocity.x < 0)
        {
            sprite.flipX = true;
        }
    }
}