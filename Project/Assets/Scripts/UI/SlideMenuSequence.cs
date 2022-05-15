using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenuSequence : MonoBehaviour
{
    void Start()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            SlideMenuScreen screen = transform.GetChild(i).GetComponent<SlideMenuScreen>();
            screen.previousTarget = i > 0 ? transform.GetChild(i-1).name : "";
            screen.nextTarget = i + 1 < transform.childCount ? transform.GetChild(i+1).name : "";
        }
    }
    
    void Update()
    {
        
    }
}
