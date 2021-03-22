using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{
    public string SceneName;
    public Scene Scene;
    public NodePath[] Connections;


    public Scene getScene()
    {
        return Scene;
    }

    public string getSceneName()
    {
        return SceneName;
    }

    public override string ToString()
    {
        return SceneName;
    }


}
