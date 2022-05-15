using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    public SlideMenuScreen targetScreen;
    public float animPosition;
    public float transitionDuration = 0.5f;
    private Vector3 startPosition;
    private float animTime = 0;
    private bool inTransition = false;
    public RectTransform screenSizeElement;
    public GameObject leftButton;
    public GameObject rightButton;

    public Dictionary<string, SlideMenuScreen> registeredScreens = new Dictionary<string, SlideMenuScreen>();
    

    void Start()
    {
        targetScreen = transform.GetChild(0).GetComponent<SlideMenuScreen>();
        transform.localPosition = - transform.InverseTransformPoint(targetScreen.transform.position);
        UpdateButtonsDisplay();
    }

    private void Update()
    {
        targetScreen.availableSize = screenSizeElement.rect.size;
        if(inTransition)
        {
            animTime += Time.deltaTime;
            Vector3 targetPosition = - transform.InverseTransformPoint(targetScreen.transform.position);
            float animRatio = animTime / transitionDuration;
            float oneMinusRatio = 1 - animRatio;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (1 - oneMinusRatio * oneMinusRatio * oneMinusRatio) * (1 - oneMinusRatio * oneMinusRatio * oneMinusRatio));
            if(animTime > transitionDuration)
                inTransition = false;
        }
    }

    public void UpdateButtonsDisplay()
    {
        bool showLeftButton = false;
        bool showRightButton = false;
        if(targetScreen != null)
        {
            showLeftButton |= targetScreen.previousTarget != "";
            showRightButton |= targetScreen.nextTarget != "";
        }
        leftButton.SetActive(showLeftButton);
        rightButton.SetActive(showRightButton);
    }

    public void SelectNext()
    {
        targetScreen = registeredScreens[targetScreen.nextTarget];
        animTime = 0;
        inTransition = true;
        startPosition = transform.localPosition;
        UpdateButtonsDisplay();
    }

    public void SelectPrevious()
    {
        targetScreen = registeredScreens[targetScreen.previousTarget];
        animTime = 0;
        inTransition = true;
        startPosition = transform.localPosition;
        UpdateButtonsDisplay();
    }

    public void GoToScreen(string target)
    {
        targetScreen = registeredScreens[target];
        animTime = 0;
        inTransition = true;
        startPosition = transform.localPosition;
        UpdateButtonsDisplay();
    }
}
