using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenuTransition : MonoBehaviour
{
    public string target;
    private SlideMenu slideMenu;

    private void Start()
    {
        slideMenu = GetComponentInParent<SlideMenu>();
    }
    
    public void PlayTransition()
    {
        slideMenu.GoToScreen(target);
    }
}
