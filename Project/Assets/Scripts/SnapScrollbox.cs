using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnapScrollbox : MonoBehaviour, IEndDragHandler, IDragHandler
{
    public ScrollRect scrollRect;
    public float targetValue;
    private bool dragging = false;
    private bool wasDragging = false;
    public float autoScrollDuration = 0.5f;
    private float autoScrollTime = 0;
    public float gravity = 10;
    public float value = 0;
    public float dragOffsetScale = 2;
    public bool scrollToTarget;
    public int teleportValue = 0;
    private bool mustTeleport = true;

    private float dragStartValue;
    private int currentTargetIndex;
    public GameObject nextButton;
    public GameObject previousButton;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        UpdateButtonsVisibility();
    }

    public void OnDrag(PointerEventData data)
    {
        dragging = true;
    }

    public void OnEndDrag(PointerEventData data)
    {
        dragging = false;
    }

    

    private float currentViewCenter 
    { 
        get 
        {
            Rect contentRect = scrollRect.content.rect;
            float viewportSize = scrollRect.viewport.rect.width;
            float minViewCenter = viewportSize / 2;
            float maxViewCenter = contentRect.width - (viewportSize / 2);
            return Mathf.Lerp(minViewCenter, maxViewCenter, scrollRect.normalizedPosition.x); 
        }
    }

    private float GetChildSignedDistanceToViewCenter(int childIndex)
    {
        RectTransform childRect = scrollRect.content.GetChild(Mathf.Clamp(childIndex, 0, scrollRect.content.childCount-1)).GetComponent<RectTransform>();
        float childMinPos = childRect.anchoredPosition.x + childRect.rect.xMin;
        float childMaxPos = childRect.anchoredPosition.x + childRect.rect.xMax;
        float childCenterPos = (childMinPos + childMaxPos) / 2;
        return currentViewCenter - childCenterPos;
    }

    private float GetChildCenterPos(int childIndex)
    {
        RectTransform childRect = scrollRect.content.GetChild(Mathf.Clamp(childIndex, 0, scrollRect.content.childCount-1)).GetComponent<RectTransform>();
        float childMinPos = childRect.anchoredPosition.x + childRect.rect.xMin;
        float childMaxPos = childRect.anchoredPosition.x + childRect.rect.xMax;
        return (childMinPos + childMaxPos) / 2;
    }

    private float PositionToScrollValue(float position)
    {
        Rect contentRect = scrollRect.content.rect;
        float viewportSize = scrollRect.viewport.rect.width;
        float minViewCenter = viewportSize / 2;
        float maxViewCenter = contentRect.width - (viewportSize / 2);
        return (position - minViewCenter) / (maxViewCenter - minViewCenter);
    }

    private void Update()
    {
        if(mustTeleport)
        {
            RectTransform childRect = scrollRect.content.GetChild(teleportValue).GetComponent<RectTransform>();
            float childMinPos = childRect.anchoredPosition.x + childRect.rect.xMin;
            float childMaxPos = childRect.anchoredPosition.x + childRect.rect.xMax;
            float childCenterPos = (childMinPos + childMaxPos) / 2;
            scrollRect.normalizedPosition = new Vector2(PositionToScrollValue(childCenterPos), 0);
            targetValue = PositionToScrollValue(childCenterPos);
            mustTeleport = false;
        }
        else if(scrollToTarget)
        {
            float closestElementPos = Mathf.Infinity;
            float closestElementDistance = Mathf.Infinity;
            int closestElementIndex = -1;
            if(!dragging && wasDragging)
            {
                for(int i=0; i<scrollRect.content.childCount; i++)
                {
                    float childDistance = currentViewCenter - GetChildCenterPos(i) + (currentViewCenter - dragStartValue) * dragOffsetScale;
                    if(Mathf.Abs(childDistance) < closestElementDistance && i >= currentTargetIndex - 1 && i <= currentTargetIndex + 1)
                    {
                        closestElementDistance = Mathf.Abs(childDistance);
                        RectTransform childRect = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
                        float childMinPos = childRect.anchoredPosition.x + childRect.rect.xMin;
                        float childMaxPos = childRect.anchoredPosition.x + childRect.rect.xMax;
                        float childCenterPos = (childMinPos + childMaxPos) / 2;
                        closestElementPos = childCenterPos;
                        closestElementIndex = i;
                    }
                }
            }
            if(dragging && !wasDragging)
            {
                dragStartValue = currentViewCenter;
            }
            for(int i=0; i<scrollRect.content.childCount; i++)
            {
                float childDistance = currentViewCenter - GetChildCenterPos(i);
                if(Mathf.Abs(childDistance) < closestElementDistance)
                {
                    closestElementDistance = Mathf.Abs(childDistance);
                    RectTransform childRect = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
                    float childMinPos = childRect.anchoredPosition.x + childRect.rect.xMin;
                    float childMaxPos = childRect.anchoredPosition.x + childRect.rect.xMax;
                    float childCenterPos = (childMinPos + childMaxPos) / 2;
                    closestElementIndex = i;
                }
            }
            float closestChildDistance = GetChildSignedDistanceToViewCenter(closestElementIndex);
            int neighborChildIndex = closestElementIndex+1;
            if(closestChildDistance < 0)
            {
                neighborChildIndex = closestElementIndex - 1;
            }
            float neighborDistance = GetChildSignedDistanceToViewCenter(neighborChildIndex);
            value = Mathf.Clamp(closestElementIndex + (neighborChildIndex - closestElementIndex) * closestChildDistance / (closestChildDistance - neighborDistance), 0, scrollRect.content.childCount-1);
            if(!dragging && wasDragging)
            {
                targetValue = PositionToScrollValue(closestElementPos);
                currentTargetIndex = closestElementIndex;
                UpdateButtonsVisibility();
            }
            if(!dragging)
            {
                scrollRect.velocity -= new Vector2((targetValue - scrollRect.normalizedPosition.x) * gravity * Time.deltaTime, 0);
            }
        }
        wasDragging = dragging;
    }

    public void TeleportToValue(int value)
    {
        teleportValue = value;
        mustTeleport = true;
    }

    public void SelectNext()
    {
        currentTargetIndex++;
        if(currentTargetIndex >= scrollRect.content.childCount)
            currentTargetIndex = scrollRect.content.childCount - 1;
        UpdateButtonsVisibility();
        RectTransform childRect = scrollRect.content.GetChild(currentTargetIndex).GetComponent<RectTransform>();
        float childMinPos = childRect.anchoredPosition.x + childRect.rect.xMin;
        float childMaxPos = childRect.anchoredPosition.x + childRect.rect.xMax;
        float childCenterPos = (childMinPos + childMaxPos) / 2;
        targetValue = PositionToScrollValue(childCenterPos);
    }

    public void SelectPrevious()
    {
        currentTargetIndex--;
        if(currentTargetIndex < 0)
            currentTargetIndex = 0;
        UpdateButtonsVisibility();
        RectTransform childRect = scrollRect.content.GetChild(currentTargetIndex).GetComponent<RectTransform>();
        float childMinPos = childRect.anchoredPosition.x + childRect.rect.xMin;
        float childMaxPos = childRect.anchoredPosition.x + childRect.rect.xMax;
        float childCenterPos = (childMinPos + childMaxPos) / 2;
        targetValue = PositionToScrollValue(childCenterPos);
    }

    private void UpdateButtonsVisibility()
    {
        if(previousButton != null)
            previousButton.SetActive(currentTargetIndex > 0);
        if(nextButton != null)
            nextButton.SetActive(currentTargetIndex < scrollRect.content.childCount - 1);
    }
}
