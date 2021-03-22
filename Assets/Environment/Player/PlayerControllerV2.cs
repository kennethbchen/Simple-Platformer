using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerV2 : MonoBehaviour
{
    [Header("Layer Params")]
    public LayerMask swingLayer;
    public LayerMask deathLayer;
    public LayerMask respawnLayer;
    public LayerMask goalLayer;

    [Header("Checkpoint")]
    public Vector2 respawnPoint;

    [Header("States")]
    public bool CanMove = true;
    public bool CanFastFall = true;
    public bool WallSliding = false;
    public bool InSwingRange = false;
    public bool Swinging = false;
    public bool FacingRight = true;

    [Space]
    [Header("Global Properties")]
    public float horizontalSpeedCap;
    public float idleDecelerationFactor;


    [Space]
    [Header("Ground Movement Properties")]
    public float runForce;
    public float maxRunSpeed;


    [Space]
    [Header("Air Movement Properties")]
    public float airRunForce;
    public float maxAirSpeed;

    [Space]
    [Header("Jump Properties")]
    public float jumpForce;
    public float fastFallMultiplier;

    [Space]
    [Header("Wall Slide Properties")]
    public float slideFrictionFactor;
    public float slideTerminalVelocity;
    public float wallJumpHorizontalVelocity;
    public float wallJumpVerticalVelocity;

    [Space]
    [Header("Swing Properties")]
    public float swingForce;
    public float swingJumpForce;
    public float swingMaxDistance;
    public float maxSwingSpeed;

    [Space]
    [Header("Sling Properties")]
    public float slingForce;



    private Rigidbody2D rb;
    private PlayerCollisionHandler col;
    private DistanceJoint2D joint;
    private GameObject swingTarget;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    //Input Buffers
    private float x;
    private float y;
    private Vector2 dir;
    private bool jump;
    private bool swing;
    private bool sling;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<PlayerCollisionHandler>();
        joint = GetComponent<DistanceJoint2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        x = 0;
        y = 0;
        dir = Vector2.zero;
    }

    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        dir = new Vector2(x, y);

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (InSwingRange && Input.GetKeyDown(KeyCode.J))
        {
            swing = true;
        }

        if (Swinging && Input.GetKeyDown(KeyCode.J))
        {
            sling = true;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Kill();
        }



    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            SceneControllerScript.SharedInstance.ChangeScene(0);
        }

        if (swing)
        {
            Swinging = true;
            joint.connectedBody = swingTarget.GetComponent<Rigidbody2D>();
            joint.enabled = true;
            joint.distance = swingMaxDistance;
            swing = false;
        }


        //Change Movement Behavior
        if (!Swinging)
        {
            Walk();


            Jump();
            WallSlide();
            WallJump();

        } else
        {
            //CanFastFall = false;
            Swing();
            SwingJump();
            Sling();
            
        }

        spriteRenderer.flipX = !FacingRight;
        animator.SetBool("Facing Right", FacingRight);
        //Reset buffers
        jump = false;
        swing = false;
        sling = false;
    }

    void Walk()
    {
        if(dir.x < 0)
        {
            
            FacingRight = false;
        } else if(dir.x > 0)
        {
            FacingRight = true;
        }
        
        if (col.onGround)
        {
            if (dir.x != 0)
            {
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }

            if (dir.x > 0 && rb.velocity.x < maxRunSpeed || dir.x < 0 && rb.velocity.x > -maxRunSpeed)
            {
                rb.AddRelativeForce(new Vector2(dir.x * runForce, 0));
            } else if (dir.x == 0)
            {
                rb.AddRelativeForce(new Vector2(rb.velocity.x * idleDecelerationFactor, 0));
            }
        } else
        {
            animator.SetBool("Running", false);
            if (dir.x > 0 && rb.velocity.x < maxAirSpeed || dir.x < 0 && rb.velocity.x > -maxAirSpeed)
            {
                rb.AddRelativeForce(new Vector2(dir.x * airRunForce, 0));
            }
            else if (dir.x == 0)
            {
                rb.AddRelativeForce(new Vector2(rb.velocity.x * idleDecelerationFactor, 0));
            }
        }

    }

    void Jump()
    {
        

        if (jump && col.onGround)
        {
            rb.AddForce(rb.transform.up * jumpForce, ForceMode2D.Impulse);
        }

        
        if ( CanFastFall && rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fastFallMultiplier;
        }
        

        
    }

    void WallSlide()
    {


        if (col.onLeftWall && dir.x < 0 || col.onRightWall && dir.x > 0)
        {
            animator.SetBool("Wall Sliding", true);
            float newY = rb.velocity.y - (slideFrictionFactor * Time.deltaTime);

            if (newY <= slideTerminalVelocity) newY = slideTerminalVelocity;

            rb.velocity = new Vector2(0, newY);

            WallSliding = true;
        }
        else
        {
            animator.SetBool("Wall Sliding", false);
            WallSliding = false;
        }


    }

    void WallJump()
    {
        if (jump && (WallSliding || col.onLeftWall || col.onRightWall))
        {
           
            if (col.onLeftWall)
            {
                rb.velocity = new Vector2(wallJumpHorizontalVelocity * 1, wallJumpVerticalVelocity);
            }
            else if (col.onRightWall)
            {
                rb.velocity = new Vector2(wallJumpHorizontalVelocity * -1, wallJumpVerticalVelocity);
            }

            jump = false;
        }

    }

    void Swing()
    {
        //Change Rotation based off of swing anchor
        transform.up = joint.connectedBody.transform.position - transform.position;


        //Add Swing force
        rb.AddForce(transform.right * dir.x * swingForce);


    }

    void SwingJump()
    {

        if (jump)
        {

            rb.AddForce(-Vector2.up * swingJumpForce);

            //Reset Statuses
            Swinging = false;
            joint.enabled = false;
            jump = false;

            //Reset Rotation
            transform.up = Vector2.up;
        }
    }

    void Sling()
    {

        if (sling)
        {
            //Get angle to swing target
            Vector2 slingDir = transform.up;


            //Reset Statuses
            Swinging = false;
            joint.enabled = false;
            jump = false;



            rb.AddForce(slingDir * slingForce, ForceMode2D.Impulse);

            //Reset Rotation
            transform.up = Vector2.up;
        }
        sling = false;
    }

    private void Kill()
    {
        Respawn();
    }

    private void Respawn()
    {
        this.transform.position = respawnPoint;
    }

    private void SetRespawnPoint(Vector2 respawnPoint)
    {
        this.respawnPoint = respawnPoint;
    }

    private void Win()
    {
        rb.bodyType = RigidbodyType2D.Static;
        SceneControllerScript.SharedInstance.ChangeScene(0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & swingLayer.value) != 0)
        {
            InSwingRange = true;
            swingTarget = collision.gameObject;
            
        }
        else if ( (1<<collision.gameObject.layer & deathLayer.value) != 0)
        {
            
            Kill();
        } else if ( (1<<collision.gameObject.layer & respawnLayer.value) != 0)
        {
            SetRespawnPoint(collision.GetComponent<Checkpoint>().GetRespawnPoint());
        } else if ((1 << collision.gameObject.layer & goalLayer.value) != 0)
        {
            Win();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        InSwingRange = false;
    }
}
