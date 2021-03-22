using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingHazardController : MonoBehaviour
{

    public Vector2 endPosition;
    public float speed;
    public float delta;

    private Vector2 startPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = (Vector2)transform.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector2.Lerp(startPosition, startPosition + endPosition, delta * Mathf.Sin(Time.time * speed) + .5f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;


        Gizmos.DrawWireCube((Vector2)transform.position + (Vector2)endPosition, new Vector2(1,1));
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + (Vector2)endPosition);
    }

}
