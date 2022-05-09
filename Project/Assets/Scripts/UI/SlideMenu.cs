using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    public int targetIndex;
    public float animPosition;
    public float transitionDuration = 0.5f;
    private Vector3 startPosition;
    private float animTime = 0;
    private bool inTransition = false;


    void Start()
    {
        transform.localPosition = -transform.GetChild(targetIndex).localPosition;
    }

    private void Update()
    {
        if(inTransition)
        {
            animTime += Time.deltaTime;
            Vector3 targetPosition = -transform.GetChild(targetIndex).localPosition;
            float animRatio = animTime / transitionDuration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, 1 - (1 - animRatio) * (1 - animRatio));
            if(animTime > transitionDuration)
                inTransition = false;
        }
    }

    public void SelectNext()
    {
        targetIndex++;
        if(targetIndex >= transform.childCount)
            targetIndex = transform.childCount - 1;
        animTime = 0;
        inTransition = true;
        startPosition = transform.localPosition;
    }

    public void SelectPrevious()
    {
        targetIndex--;
        if(targetIndex < 0)
            targetIndex = 0;
        animTime = 0;
        inTransition = true;
        startPosition = transform.localPosition;
    }
}
