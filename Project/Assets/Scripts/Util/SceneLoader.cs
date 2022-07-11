using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string targetScene;
    void Start()
    {
        
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
}
