using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    [Space]

    [Header("Collision Boxes")]
    public Vector2 botOffset;
    public Vector2 botSize;
    [Space]

    public Vector2 leftOffset;
    public Vector2 leftSize;
    [Space]

    public Vector2 rightOffset;
    public Vector2 rightSize;

    [Space]
    [Header("Status")]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    private Color debugCollisionColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        onGround = Physics2D.OverlapBox((Vector2) transform.position + botOffset, botSize, 0, groundLayer);

        
        onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, rightSize, 0, wallLayer)
            || Physics2D.OverlapBox((Vector2)transform.position + leftOffset, leftSize, 0, wallLayer);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, rightSize, 0, wallLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, leftSize, 0, wallLayer);
        
        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        
        Gizmos.DrawWireCube((Vector2)transform.position + (Vector2)botOffset, botSize);
        Gizmos.DrawWireCube((Vector2)transform.position + (Vector2)leftOffset, leftSize);
        Gizmos.DrawWireCube((Vector2)transform.position + (Vector2)rightOffset, rightSize);

    }
}
