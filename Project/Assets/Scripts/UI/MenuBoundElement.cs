using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuBoundDisplayType
{
    Show,
    Hide,
}
public class MenuBoundElement : MonoBehaviour
{
    public string menuId;
    public MenuBoundDisplayType displayType;
    private SlideMenu slideMenu;
    private bool wasMenuPresent = false;
    void Start()
    {
        slideMenu = GetComponentInParent<SlideMenu>();
        slideMenu.targetScreenChangedDelegate += OnTargetScreenChanged;
    }

    private void OnTargetScreenChanged()
    {
        bool menuPresent = false;
        foreach(SlideMenuScreen screen in slideMenu.screenStack)
        {
            if(screen.gameObject.name == menuId)
            {
                menuPresent = true;
                break;
            }
        }
        if(wasMenuPresent != menuPresent)
        {
            switch(displayType)
            {
                case MenuBoundDisplayType.Show:
                    gameObject.SetActive(menuPresent);
                    break;
                case MenuBoundDisplayType.Hide:
                    gameObject.SetActive(!menuPresent);
                    break;
            }
            wasMenuPresent = menuPresent;
        }
    }
    
    void Update()
    {
        
    }
}
