using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodePath : MonoBehaviour
{
    public Node Node1;
    public Node Node2;

    private float angleTolerance = 0;

    private LineRenderer path;

    private void Start()
    {
        path = GetComponent<LineRenderer>();

        path.positionCount = 2;

        path.SetPosition(0, Node1.transform.position);
        path.SetPosition(1, Node2.transform.position);
    }

    public Node getNextNode(Node currentLocation)
    {
        if (currentLocation.Equals(Node1)) {
            return Node2;
        } else if (currentLocation.Equals(Node2))
        {
            return Node1;
        } else
        {
            Debug.LogError("Incorrect Node Binding for NodePath[" + Node1.ToString() + " " + Node2.ToString() + "]");
            return null;
        }
        
    }

    public bool ValidateMovement(Node currentLocation, float inputAngle)
    {
        if (currentLocation.Equals(Node1))
        {
            Vector2 vTo = (Vector2)Node2.transform.position - (Vector2)currentLocation.transform.position;

            float angleTo = Mathf.Atan2(vTo.y, vTo.x) * Mathf.Rad2Deg;

        
            if (Mathf.Sign(angleTo) == -1)
            {
                angleTo += 360;
            }

            if ( (angleTo - angleTolerance) <= inputAngle && (angleTo + angleTolerance) >= inputAngle)
            {
                return true;
            } else
            {
                return false;
            }
        }
        else if (currentLocation.Equals(Node2))
        {
            Vector2 vTo = (Vector2)Node1.transform.position - (Vector2)currentLocation.transform.position;

            float angleTo = Mathf.Atan2(vTo.y, vTo.x) * Mathf.Rad2Deg;


            if (Mathf.Sign(angleTo) == -1)
            {
                angleTo += 360;
            }


            if ((angleTo - angleTolerance) <= inputAngle && (angleTo + angleTolerance) >= inputAngle)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

  



}
