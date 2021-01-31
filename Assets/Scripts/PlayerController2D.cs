using System.Collections;
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
    [SerializeField]
    private float _airControlModifier = 0.5f;
    [SerializeField]
    private bool _airControl = false;
    [Header("Ground")]
    [SerializeField]
    private bool _grounded = true;
    [SerializeField]
    private float _groundOffset = 0;
    [SerializeField]
    private LayerMask _whatIsGround = default;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("Grounded", _grounded = CheckGround());
        Move();
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

    // TODO maybe make animation depend on velocity and not input?
    // TODO Flies off on ledges
    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float m = _grounded ? _speed : _speed * _airControlModifier;

        if (horizontalInput < 0.1f)
        {
            gameObject.transform.localScale = new Vector3(-1, 1);
        }
        else if (horizontalInput > 0.1f)
        {
            gameObject.transform.localScale = new Vector3(1, 1);
        }

        if ((horizontalInput > 0.1f || horizontalInput < -0.1f) &&
            (_airControl || _grounded))
        {
            _rigidbody.velocity = new Vector2(m * Time.deltaTime * horizontalInput, _rigidbody.velocity.y);
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
