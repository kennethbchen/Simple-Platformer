using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool CanMove = true;
    public bool WallSliding = false;
    public bool InSwingRange = false;
    public bool Swinging = false;

    public LayerMask swingLayer;
    public LayerMask deathLayer;

    [Space]
    [Header("Ground Properties")]
    public float runAcceleration;
    public float runDecelerationFactor;
    public float maxRunSpeed;

    [Space]
    [Header("Air Properties")]
    public float airAcceleration;
    public float airDecelerationFactor;
    public float maxAirSpeed;

    [Space]
    [Header("Wall Slide Properties")]
    public float slideFrictionFactor;
    public float slideTerminalVelocity;

    [Space]
    [Header("Swing Properties")]
    public float swingMaxDistance;
    public float maxSwingSpeed;

    [Space]
    public float jumpVelocity;
    public float fallMultiplier;
    public float lowJumpMultiplier;

    public float fastSpeedIncreaseFactor;




    private Rigidbody2D rb;
    private PlayerCollisionHandler col;
    private DistanceJoint2D joint;
    private GameObject swingTarget;

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

        /*
        if( InSwingRange && Input.GetKeyDown(KeyCode.J))
        {
            swing = true;
        }

        if(Swinging && Input.GetKeyDown(KeyCode.J))
        {
            sling = true;
        }
        */

       

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Set Swining Params
        if (swing)
        {
            Swinging = true;
            joint.connectedBody = swingTarget.GetComponent<Rigidbody2D>();
            joint.enabled = true;
            joint.distance = swingMaxDistance;
            swing = false;
        }

        /*
        //Change Movement Behavior
        if (!Swinging)
        {
            Walk();

            Jump();
            WallSlide();
            WallJump();
            
        } else
        {
            Swing();
            SwingJump();
            Sling();
        }
        */

        //Reset buffers
        jump = false;
        swing = false;
        sling = false;
    }

    void Walk()
    {
        float netVelocity = rb.velocity.x;

        if (col.onGround)
        {
            if (dir.x == 0)
            {
                //Decelerate if no input
                if (netVelocity > runAcceleration) netVelocity += netVelocity * runDecelerationFactor;
                if (netVelocity < runAcceleration) netVelocity -= netVelocity * runDecelerationFactor;
            }
            else
            {
                //Accelerate
                netVelocity += runAcceleration * dir.x;
            }

                //Clamp velocity
                netVelocity = Mathf.Clamp(netVelocity, -maxRunSpeed, maxRunSpeed);

        } else
        {
            if (dir.x == 0)
            {
                //Decelerate if no input
                if (netVelocity > airAcceleration) netVelocity += netVelocity * airDecelerationFactor;
                if (netVelocity < airAcceleration) netVelocity -= netVelocity * airDecelerationFactor;
            }
            else
            {
                //Accelerate
                netVelocity += airAcceleration * dir.x * Time.deltaTime;
            }

                //Clamp velocity
                netVelocity = Mathf.Clamp(netVelocity, -maxAirSpeed, maxAirSpeed);

        }


        

        //Update velocity
        rb.velocity = new Vector2(netVelocity, rb.velocity.y);
    }

    void Jump()
    {

        //Initial Jump Force
        if (jump && col.onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * jumpVelocity;
            jump = false;
        }

        //Modify falling rate
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        
    }

    void WallSlide()
    {


        if (col.onLeftWall && dir.x < 0 || col.onRightWall && dir.x > 0)
        {
            float newY = rb.velocity.y - (slideFrictionFactor * Time.deltaTime);

            if (newY <= slideTerminalVelocity) newY = slideTerminalVelocity;

            rb.velocity = new Vector2(rb.velocity.x, newY);

            WallSliding = true;
        } else
        {
            WallSliding = false;
        }


    }

    void WallJump()
    {
        if (jump && WallSliding)
        {
            rb.velocity = new Vector2(12f * -dir.x, 15);
            jump = false;
        } else if (jump && col.onWall)
        {
            float finalX = 0;
            if (col.onLeftWall)
            {
                finalX = 8f;
            } else if (col.onRightWall)
            {
                finalX = -8f;
            }

            rb.velocity = new Vector2(finalX, 15);

            jump = false;
        }
    }

    void Swing()
    {
        //Change Rotation based off of swing anchor
        transform.up = joint.connectedBody.transform.position - transform.position;


        //Add Swing force
        rb.AddForce(transform.right * dir.x * 20);


    }

    void SwingJump()
    {
        
        if (jump)
        {

            rb.AddForce(Vector2.up * 3);

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
            Vector2 slingDir = transform.up;
            Debug.Log(slingDir);
            //Reset Statuses
            Swinging = false;
            joint.enabled = false;
            jump = false;

            

            rb.AddForce(slingDir * 40, ForceMode2D.Impulse);

            //Reset Rotation
            transform.up = Vector2.up;
        }
        sling = false;
    }

    void kill()
    {
        Debug.Log("Kill");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            InSwingRange = true;
            swingTarget = collision.gameObject;

        }
        else if (collision.gameObject.layer == deathLayer)
        {
            kill();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        InSwingRange = false;
    }
}
