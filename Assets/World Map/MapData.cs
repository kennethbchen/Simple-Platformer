using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public static MapData Instance;

    public int CurrentLevel = 1;

    void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }    
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(int level)
    {
        CurrentLevel = level;        
    }

    public int GetLevel()
    {
        return CurrentLevel;
    }
}
