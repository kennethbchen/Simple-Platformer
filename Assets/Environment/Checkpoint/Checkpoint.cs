using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{


    public Vector2 RespawnPointOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetRespawnPoint()
    {
        return (Vector2)transform.position + RespawnPointOffset;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;


        Gizmos.DrawWireCube((Vector2)transform.position + RespawnPointOffset, new Vector2(1,1));

    }


}
