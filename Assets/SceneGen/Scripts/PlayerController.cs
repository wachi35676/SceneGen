using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpHeight = 10.0f;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        _rigidbody2D.velocity = new Vector2(moveInput * speed, _rigidbody2D.velocity.y);

        // Flip character sprite based on movement direction
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // Set animation parameters
        if (Mathf.Abs(moveInput) > 0)
        {
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpHeight);
            _animator.SetTrigger("Jump");
        }
    }
}