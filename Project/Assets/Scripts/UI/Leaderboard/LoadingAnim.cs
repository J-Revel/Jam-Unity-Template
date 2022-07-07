using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnim : MonoBehaviour
{
    public CanvasGroup elementPrefab;
    public int elementCount;
    private CanvasGroup[] elements;
    public float revolutionDuration = 2;
    public float alphaAngleDistance = 30;
    private float time = 0;

    void Start()
    {
        elements = new CanvasGroup[elementCount];
        for(int i=0; i<elements.Length; i++)
        {
            elements[i] = Instantiate(elementPrefab, transform.position, Quaternion.AngleAxis(360 / elements.Length * i, Vector3.forward), transform);
        }
    }

    void Update()
    {
        time += Time.deltaTime;
        float currentAngle = (time * 360 / revolutionDuration) % 360;
        for(int i=0; i<elements.Length; i++)
        {
            float elementAngle = 360 / elements.Length * i;
            float deltaAngle = elementAngle - currentAngle;
            while(deltaAngle < -180)
                deltaAngle += 360;
            while(deltaAngle >= 180)
                deltaAngle -= 360;
            elements[i].alpha = Mathf.Clamp01(1 - Mathf.Abs(deltaAngle) / alphaAngleDistance);
        }
    }
}
