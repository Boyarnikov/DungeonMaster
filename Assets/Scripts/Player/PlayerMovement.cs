using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D body;
    public LayerMask layer;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;

    public float runSpeed = 10.0f;

    void Start()
    {
        layer = LayerMask.GetMask("Collider");
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        if (horizontal==0 && vertical==0) { body.velocity = Vector2.zero; return; }

        if (IsOnTilePointRelative(horizontal * 0.9f, vertical * 0.9f))
        {
            body.velocity = new Vector2(horizontal * runSpeed,
                vertical * 0.5f * runSpeed);
        }
        else
        {
            body.velocity = Vector2.zero;
        }

    }

    bool IsOnTilePointRelative(float x, float y)
    {
        Vector2 pos = transform.position +
            runSpeed * 0.01f *
            (Vector3.right * x + Vector3.up * y);
        RaycastHit2D hit = Physics2D.Raycast(pos + Vector2.up * 0.01f, Vector2.down, 0.02f, LayerMask.GetMask("Collider"));
        Debug.DrawRay(pos + Vector2.up * 0.01f, Vector2.down * 0.02f, UnityEngine.Color.red, 0.001f);
        return (hit.collider == null);
    }

}