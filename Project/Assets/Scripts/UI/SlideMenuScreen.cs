using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenuScreen : MonoBehaviour
{
    public string nextTarget;
    public string previousTarget;

    private SlideMenu slideMenu;
    public Vector2Int size;
    public Vector2 availableSize;
    private RectTransform rectTransform;
    private RectTransform parentTransform;

    private void Start()
    {
        slideMenu = GetComponentInParent<SlideMenu>();
        slideMenu.registeredScreens[gameObject.name] = this;
        rectTransform = GetComponent<RectTransform>();
        parentTransform = transform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.sizeDelta = size;
        Vector2 scales = availableSize / size;
        rectTransform.localScale = Vector3.one * Mathf.Min(scales.x, scales.y);
    }
}
