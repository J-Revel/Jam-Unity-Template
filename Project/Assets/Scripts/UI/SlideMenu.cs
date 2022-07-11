using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    public List<SlideMenuScreen> screenStack = new List<SlideMenuScreen>(){null};
    public SlideMenuScreen targetScreen { 
        get {
            return screenStack[screenStack.Count - 1];
        }
        set {
            screenStack[screenStack.Count - 1] = value;
            targetScreenChangedDelegate?.Invoke();
        }
    }
    public System.Action targetScreenChangedDelegate;
    public float animPosition;
    public float transitionDuration = 0.5f;
    private Vector3 startPosition;
    private float animTime = 0;
    private bool inTransition = false;
    public RectTransform screenSizeElement;
    public Transform screenContainer;
    public GameObject leftButton;
    public GameObject rightButton;

    public Dictionary<string, SlideMenuScreen> registeredScreens = new Dictionary<string, SlideMenuScreen>();
    

    void Start()
    {
        targetScreen = screenContainer.GetChild(0).GetComponent<SlideMenuScreen>();
        screenContainer.localPosition = - screenContainer.InverseTransformPoint(targetScreen.transform.position);
        UpdateButtonsDisplay();
    }

    private void Update()
    {
        targetScreen.availableSize = screenSizeElement.rect.size;
        if(inTransition)
        {
            animTime += Time.deltaTime;
            Vector3 targetPosition = - screenContainer.InverseTransformPoint(targetScreen.transform.position);
            float animRatio = animTime / transitionDuration;
            float oneMinusRatio = 1 - animRatio;
            screenContainer.localPosition = Vector3.Lerp(startPosition, targetPosition, (1 - oneMinusRatio * oneMinusRatio * oneMinusRatio) * (1 - oneMinusRatio * oneMinusRatio * oneMinusRatio));
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
        startPosition = screenContainer.localPosition;
        UpdateButtonsDisplay();
    }

    public void SelectPrevious()
    {
        targetScreen = registeredScreens[targetScreen.previousTarget];
        animTime = 0;
        inTransition = true;
        startPosition = screenContainer.localPosition;
        UpdateButtonsDisplay();
    }

    public void GoToScreen(string target)
    {
        targetScreen = registeredScreens[target];
        animTime = 0;
        inTransition = true;
        startPosition = screenContainer.localPosition;
        UpdateButtonsDisplay();
    }

    public void PushScreen(string target)
    {
        screenStack.Add(registeredScreens[target]);
        targetScreenChangedDelegate?.Invoke();
        animTime = 0;
        inTransition = true;
        startPosition = screenContainer.localPosition;
        UpdateButtonsDisplay();
    }

    public void PopScreen()
    {
        screenStack.RemoveAt(screenStack.Count - 1);
        targetScreenChangedDelegate?.Invoke();
        animTime = 0;
        inTransition = true;
        startPosition = screenContainer.localPosition;
        UpdateButtonsDisplay();
    }
}
