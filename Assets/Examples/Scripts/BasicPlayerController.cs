using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicPlayerController : MonoBehaviour
{
    public float movementSpeed = 4f;
    public bool canMove = true;

    private Vector3 movementVelocity = Vector3.zero;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb.MovePosition(transform.position + movementVelocity * movementSpeed * Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        Vector2 movementInput = Vector2.zero;

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementVelocity = transform.right * movementInput.x + transform.forward * movementInput.y;
    }
}
