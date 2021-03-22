using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingHazardController : MonoBehaviour
{
    public float startAngle;
    public float endAngle;
    public float radius;
    public float delta;
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.transform.position = new Vector3(transform.position.x, transform.position.y - radius, transform.position.z);
        }

        transform.rotation = Quaternion.Euler(0, 0, startAngle + 90);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(0,0, startAngle), new Vector3(0,0, endAngle), delta * Mathf.Sin(Time.time * speed) + .5f));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, radius);
        
    }
}
