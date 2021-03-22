using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapCursor : MonoBehaviour
{

    public bool CanMove;
    public Node Selected;
    public Text LevelText;

    private float x = 0;
    private float y = 0;

    private float LastMoveTime = 0;
    private float MoveCooldown = .3f;

    Vector2 dir = new Vector2(0, 0);
    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(MapData.Instance.GetLevel());
        Selected = GameObject.Find("Level " + MapData.Instance.GetLevel()).GetComponent<Node>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LevelText.transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
        LevelText.text = Selected.getSceneName();
        transform.position = Selected.transform.position;

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        dir = new Vector2(x, y);

        if (Input.GetButtonDown("Jump"))
        {
            MapData.Instance.SetLevel(Selected.getScene().handle);
            SceneControllerScript.SharedInstance.ChangeScene(Selected.getScene().handle);
        }

        if (dir.x != 0)
        {
            // Get angle of mouse position centered around current position
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (Mathf.Sign(angle) == -1)
            {
                angle += 360;
            }

            
            foreach (NodePath path in Selected.Connections)
            {
                if (Time.time - LastMoveTime > MoveCooldown && path.ValidateMovement(Selected, angle))
                {
                    LastMoveTime = Time.time;
                    Selected = path.getNextNode(Selected);
                    
                }
            }
        }
        
    }




    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + dir);
      
    }


}
