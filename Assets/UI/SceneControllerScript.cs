using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControllerScript : MonoBehaviour
{
    
    public static SceneControllerScript SharedInstance;
    public Animator FadeOverlay;

    // Start is called before the first frame update
    void Start()
    {
        SharedInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(int SceneID)
    {
        FadeOverlay.SetTrigger("Exit");
        StartCoroutine(WaitForAnimation(SceneID));
    }

    private IEnumerator WaitForAnimation(int SceneID)
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadSceneAsync(SceneID);
    }

}
