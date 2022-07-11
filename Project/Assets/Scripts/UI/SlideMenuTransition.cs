using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlideTransitionType
{
    Goto,
    Push,
    Pop,
}
public class SlideMenuTransition : MonoBehaviour
{
    public string target;
    private SlideMenu slideMenu;
    public SlideTransitionType transitionType;

    private void Start()
    {
        slideMenu = GetComponentInParent<SlideMenu>();
    }
    
    public void PlayTransition()
    {
        switch(transitionType)
        {
            case SlideTransitionType.Goto:
                slideMenu.GoToScreen(target);
                break;
            case SlideTransitionType.Push:
                slideMenu.PushScreen(target);
                break;
            case SlideTransitionType.Pop:
                slideMenu.PopScreen();
                break;
        }
    }
}
