using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float _raycastExtension = 0.1f;
    [Header("Movement")]
    [SerializeField] private float _speed = 500;
    [SerializeField] private float _jumpStrength = 500;
    [SerializeField] private float _airControlModifier = 0.5f;
    [SerializeField] private bool _airControl = false;

    [Header("Ground")]
    [SerializeField] private bool _grounded = true;
    [SerializeField] private float _groundOffset = 0;
    [SerializeField] private LayerMask _whatIsGround = default;

    [Header("Sides")]
    [SerializeField] private bool _leftContact;
    [SerializeField] private bool _rightContact;

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
        _leftContact = CheckLeft();
        _rightContact = CheckRight();
        Move();
    }

    // TODO maybe make animation depend on velocity and not input? (only works if controlling movement with rigidbody)
    //      probably not needed if working directly with transform, though
    // TODO using transform instead of rigidbody to control movement seems to give better results (ledges still weird)
    //      requires having air control on... though
    //      if against a wall, it will "bunce" against it.
    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float m = _grounded ? _speed : _speed * _airControlModifier;
        bool canMoveLateral = _airControl || _grounded; 

        // Move left
        if (horizontalInput < -0.1f &&
            canMoveLateral &&
            !_leftContact)
        {
            gameObject.transform.localScale = new Vector3(-1, 1);
            transform.position = new Vector3(transform.position.x + m * Time.deltaTime * horizontalInput, transform.position.y);
            _animator.SetBool("Walking", true);
        }
        // Move right
        else if (horizontalInput > 0.1f &&
            canMoveLateral &&
            !_rightContact)
        {
            gameObject.transform.localScale = new Vector3(1, 1);
            transform.position = new Vector3(transform.position.x + m * Time.deltaTime * horizontalInput, transform.position.y);
            _animator.SetBool("Walking", true);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && _grounded)
        {
            _rigidbody.AddForce(new Vector2(0, _jumpStrength));
            _animator.SetTrigger("Jumping");
        }
    }

    #region Raycasting
    /// <summary>
    /// Uses Raycast to check if sprite is in contact with Ground
    /// </summary>
    /// <returns>True if there is contact with ground</returns>
    private bool CheckGround()
    {
        RaycastHit2D hitCenter = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y), 
            Vector2.down,
            _spriteRenderer.bounds.size.y / 2 + _raycastExtension,
            _whatIsGround);
        bool c = hitCenter.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y), 
            new Vector3(0, -(_spriteRenderer.bounds.size.y / 2 + _raycastExtension)),
            c ? Color.red : Color.green);

        RaycastHit2D hitLeft = Physics2D.Raycast(
            new Vector2(transform.position.x - (_spriteRenderer.bounds.size.x / 2) + _groundOffset, transform.position.y), 
            Vector2.down,
             _spriteRenderer.bounds.size.y / 2 + _raycastExtension,
            _whatIsGround);
        bool l = hitLeft.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x - (_spriteRenderer.bounds.size.x / 2) + _groundOffset, transform.position.y), 
            new Vector3(0, -(_spriteRenderer.bounds.size.y / 2 + _raycastExtension)),
            l ? Color.red : Color.green);

        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + (_spriteRenderer.bounds.size.x / 2) - _groundOffset, transform.position.y), 
            Vector2.down,
             _spriteRenderer.bounds.size.y / 2 + _raycastExtension,
            _whatIsGround);
        bool r = hitRight.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x + (_spriteRenderer.bounds.size.x / 2) - _groundOffset, transform.position.y), 
            new Vector3(0, -(_spriteRenderer.bounds.size.y / 2 + _raycastExtension)),
            r ? Color.red : Color.green);

        return c || l || r;
    }


    // TODO check left, right to know if there can be input to sides 
    //      seems to generate issues with ledges...
    private bool CheckLeft()
    {
        // TODO needs offset to adjust
        RaycastHit2D hitUpper = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y + _spriteRenderer.bounds.size.y/2),
            Vector2.left,
            _spriteRenderer.bounds.size.x / 2 + _raycastExtension,
            _whatIsGround);
        bool u = hitUpper.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + _spriteRenderer.bounds.size.y / 2),
            new Vector3(-(_spriteRenderer.bounds.size.x / 2 + _raycastExtension), 0),
            u ? Color.red : Color.green);

        RaycastHit2D hitMiddle = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y),
            Vector2.left,
            _spriteRenderer.bounds.size.x / 2 + _raycastExtension,
            _whatIsGround);
        bool m = hitMiddle.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y),
            new Vector3(-(_spriteRenderer.bounds.size.x / 2 + _raycastExtension), 0),
            m ? Color.red : Color.green);

        RaycastHit2D hitLower = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - _spriteRenderer.bounds.size.y / 2),
            Vector2.left,
            _spriteRenderer.bounds.size.x / 2 + _raycastExtension,
            _whatIsGround);
        bool l = hitLower.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - _spriteRenderer.bounds.size.y / 2),
            new Vector3(-(_spriteRenderer.bounds.size.x / 2 + _raycastExtension), 0),
            l ? Color.red : Color.green);

        return u || m || l;
    }

    private bool CheckRight()
    {
        return false;
    }

    #endregion
}