using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlatformerCharacter2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 500;
    [SerializeField] private float jumpStrength = 500;
    [SerializeField] private float airControlModifier = 0.5f;
    [SerializeField] private bool airControl = true;

    [Header("Ground")]
    [SerializeField] private bool grounded = true;
    [SerializeField] private LayerMask whatIsGround = default;

    [Header("Sides")]
    [SerializeField] private bool leftContact;
    [SerializeField] private bool rightContact;
    [SerializeField] private bool upContact;

    [Header("Raycasting")]
    // TODO maybe Vector2 is not the clearest for offset, specially on horizontal
    [SerializeField] private Vector2 horizontalOffset = default;
    [SerializeField] private float horizontalExtension = default;
    [SerializeField] private Vector2 verticalOffset = default;
    [SerializeField] private float verticalExtension = default;

    private Animator animator;
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", grounded = CheckGround());
        leftContact = CheckLeft();
        rightContact = CheckRight();
        upContact = CheckUp();
    }


    /// <summary>
    /// Moves character
    /// </summary>
    public void Move(float horizontalInput, float verticalInput, bool jump)
    {
        float m = grounded ? speed : speed * airControlModifier;
        bool canMoveLateral = airControl || grounded;

        // Move left
        if (horizontalInput < -0.1f &&
            canMoveLateral &&
            !leftContact)
        {
            gameObject.transform.localScale = new Vector3(-1, 1);
            transform.position = new Vector3(transform.position.x + m * Time.deltaTime * horizontalInput, transform.position.y);
            animator.SetBool("Walking", true);
        }

        // Move right
        else if (horizontalInput > 0.1f &&
            canMoveLateral &&
            !rightContact)
        {
            gameObject.transform.localScale = new Vector3(1, 1);
            transform.position = new Vector3(transform.position.x + m * Time.deltaTime * horizontalInput, transform.position.y);
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        // Jump
        // TODO Add double jump
        if (jump && grounded)
        {
            rigidbody.AddForce(new Vector2(0, jumpStrength));
            animator.SetTrigger("Jumping");
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
            new Vector2(transform.position.x, transform.position.y + verticalOffset.y),
            Vector2.down,
            spriteRenderer.bounds.size.y / 2 + verticalExtension,
            whatIsGround);
        bool c = hitCenter.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + verticalOffset.y),
            new Vector3(0, -(spriteRenderer.bounds.size.y / 2 + verticalExtension)),
            c ? Color.red : Color.green);

        RaycastHit2D hitLeft = Physics2D.Raycast(
            new Vector2(transform.position.x - (spriteRenderer.bounds.size.x / 2) + verticalOffset.x, transform.position.y + verticalOffset.y),
            Vector2.down,
             spriteRenderer.bounds.size.y / 2 + verticalExtension,
            whatIsGround);
        bool l = hitLeft.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x - (spriteRenderer.bounds.size.x / 2) + verticalOffset.x, transform.position.y + verticalOffset.y),
            new Vector3(0, -(spriteRenderer.bounds.size.y / 2 + verticalExtension)),
            l ? Color.red : Color.green);

        RaycastHit2D hitRight = Physics2D.Raycast(
            new Vector2(transform.position.x + (spriteRenderer.bounds.size.x / 2) - verticalOffset.x, transform.position.y + verticalOffset.y),
            Vector2.down,
             spriteRenderer.bounds.size.y / 2 + verticalExtension,
            whatIsGround);
        bool r = hitRight.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x + (spriteRenderer.bounds.size.x / 2) - verticalOffset.x, transform.position.y + verticalOffset.y),
            new Vector3(0, -(spriteRenderer.bounds.size.y / 2 + verticalExtension)),
            r ? Color.red : Color.green);

        return c || l || r;
    }

    private bool CheckUp()
    {
        RaycastHit2D hitCenter = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y + verticalOffset.y),
            Vector2.up,
            spriteRenderer.bounds.size.y / 2 + verticalExtension,
            whatIsGround);
        bool c = hitCenter.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + +verticalOffset.y),
            new Vector3(0, spriteRenderer.bounds.size.y / 2 + verticalExtension),
            c ? Color.red : Color.green);

        RaycastHit2D hitLeft = Physics2D.Raycast(
            new Vector2(transform.position.x - (spriteRenderer.bounds.size.x / 2) + verticalOffset.x, transform.position.y + verticalOffset.y),
            Vector2.up,
             spriteRenderer.bounds.size.y / 2 + verticalExtension,
            whatIsGround);
        bool l = hitLeft.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x - (spriteRenderer.bounds.size.x / 2) + verticalOffset.x, transform.position.y + verticalOffset.y),
            new Vector3(0, spriteRenderer.bounds.size.y / 2 + verticalExtension),
            l ? Color.red : Color.green);

        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + (spriteRenderer.bounds.size.x / 2) - verticalOffset.x, transform.position.y + verticalOffset.y),
            Vector2.up,
             spriteRenderer.bounds.size.y / 2 + verticalExtension,
            whatIsGround);
        bool r = hitRight.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x + (spriteRenderer.bounds.size.x / 2) - verticalOffset.x, transform.position.y + verticalOffset.y),
            new Vector3(0, spriteRenderer.bounds.size.y / 2 + verticalExtension),
            r ? Color.red : Color.green);

        return c || l || r;
    }

    // TODO check left, right to know if there can be input to sides 
    //      seems to generate issues with ledges...
    private bool CheckLeft()
    {
        // TODO needs offset to adjust
        RaycastHit2D hitUpper = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y + spriteRenderer.bounds.size.y / 2 - horizontalOffset.y + horizontalOffset.x),
            Vector2.left,
            spriteRenderer.bounds.size.x / 2 + horizontalExtension,
            whatIsGround);
        bool u = hitUpper.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + spriteRenderer.bounds.size.y / 2 - horizontalOffset.y + horizontalOffset.x),
            new Vector3(-(spriteRenderer.bounds.size.x / 2 + horizontalExtension), 0),
            u ? Color.red : Color.green);

        RaycastHit2D hitMiddle = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y + horizontalOffset.x),
            Vector2.left,
            spriteRenderer.bounds.size.x / 2 + horizontalExtension,
            whatIsGround);
        bool m = hitMiddle.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + horizontalOffset.x),
            new Vector3(-(spriteRenderer.bounds.size.x / 2 + horizontalExtension), 0),
            m ? Color.red : Color.green);

        RaycastHit2D hitLower = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - spriteRenderer.bounds.size.y / 2 + horizontalOffset.y + horizontalOffset.x),
            Vector2.left,
            spriteRenderer.bounds.size.x / 2 + horizontalExtension,
            whatIsGround);
        bool l = hitLower.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - spriteRenderer.bounds.size.y / 2 + horizontalOffset.y + horizontalOffset.x),
            new Vector3(-(spriteRenderer.bounds.size.x / 2 + horizontalExtension), 0),
            l ? Color.red : Color.green);

        return u || m || l;
    }

    private bool CheckRight()
    {
        RaycastHit2D hitUpper = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y + spriteRenderer.bounds.size.y / 2 - horizontalOffset.y + horizontalOffset.x),
            Vector2.right,
            spriteRenderer.bounds.size.x / 2 + horizontalExtension,
            whatIsGround);
        bool u = hitUpper.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + spriteRenderer.bounds.size.y / 2 - horizontalOffset.y + horizontalOffset.x),
            new Vector3(spriteRenderer.bounds.size.x / 2 + horizontalExtension, 0),
            u ? Color.red : Color.green);

        RaycastHit2D hitMiddle = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y + horizontalOffset.x),
            Vector2.right,
            spriteRenderer.bounds.size.x / 2 + horizontalExtension,
            whatIsGround);
        bool m = hitMiddle.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + horizontalOffset.x),
            new Vector3(spriteRenderer.bounds.size.x / 2 + horizontalExtension, 0),
            m ? Color.red : Color.green);

        RaycastHit2D hitLower = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - spriteRenderer.bounds.size.y / 2 + horizontalOffset.y + horizontalOffset.x),
            Vector2.right,
            spriteRenderer.bounds.size.x / 2 + horizontalExtension,
            whatIsGround);
        bool l = hitLower.collider != null;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - spriteRenderer.bounds.size.y / 2 + horizontalOffset.y + horizontalOffset.x),
            new Vector3(spriteRenderer.bounds.size.x / 2 + horizontalExtension, 0),
            l ? Color.red : Color.green);

        return u || m || l;
    }
    #endregion
}
