﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _speed = 500;
    [SerializeField]
    private float _jumpStrength = 500;
    [Header("Ground")]
    [SerializeField]
    private bool _grounded;
    [SerializeField]
    private float _groundOffset = 0;
    [SerializeField]
    private LayerMask _whatIsGround;


    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(_spriteRenderer.sprite.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO if !grounded zero friction so it doesn's get stuck on walls
        _animator.SetBool("Grounded", _grounded = CheckGround());
        Move();
    }

    private void LateUpdate()
    {
    }

    /// <summary>
    /// Uses Raycast to check if sprite is in contact with Ground
    /// </summary>
    /// <returns>True if there is contact with ground</returns>
    private bool CheckGround()
    {
        RaycastHit2D hitCenter = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y), 
            -Vector2.up,
            _spriteRenderer.bounds.size.y / 2 + 0.1f,
            _whatIsGround);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y), 
            new Vector3(0, -(_spriteRenderer.bounds.size.y / 2 + 0.1f)), 
            Color.green);

        RaycastHit2D hitLeft = Physics2D.Raycast(
            new Vector2(transform.position.x - (_spriteRenderer.bounds.size.x / 2) + _groundOffset, transform.position.y), 
            -Vector2.up,
             _spriteRenderer.bounds.size.y / 2 + 0.1f,
            _whatIsGround);
        Debug.DrawRay(new Vector3(transform.position.x - (_spriteRenderer.bounds.size.x / 2) + _groundOffset, transform.position.y), 
            new Vector3(0, -(_spriteRenderer.bounds.size.y / 2 + 0.1f)),
            Color.green);

        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + (_spriteRenderer.bounds.size.x / 2) - _groundOffset, transform.position.y), 
            -Vector2.up,
             _spriteRenderer.bounds.size.y / 2 + 0.1f,
            _whatIsGround);
        Debug.DrawRay(new Vector3(transform.position.x + (_spriteRenderer.bounds.size.x / 2) - _groundOffset, transform.position.y), 
            new Vector3(0, -(_spriteRenderer.bounds.size.y / 2 + 0.1f)), 
            Color.green);

        return hitCenter.collider != null ||
            hitLeft.collider != null ||
            hitRight.collider != null;
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput < 0.1f)
        {
            gameObject.transform.localScale = new Vector3(-1, 1);
        }
        else if (horizontalInput > 0.1f)
        {
            gameObject.transform.localScale = new Vector3(1, 1);
        }


        if (horizontalInput > 0.1f || horizontalInput < -0.1f)
        {
            _rigidbody.velocity = new Vector2(_speed * Time.deltaTime * horizontalInput, _rigidbody.velocity.y);
            _animator.SetBool("Walking", true);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }

        if (Input.GetButtonDown("Jump") && _grounded)
        {
            _rigidbody.AddForce(new Vector2(0, _jumpStrength));
            _animator.SetTrigger("Jumping");
        }
    }
}
