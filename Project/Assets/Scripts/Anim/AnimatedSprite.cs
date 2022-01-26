using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SpriteAnimList animList;
    public int animIndex;
    private float time;
    private int frameIndex = 0;
    public bool loopAnim = true;

    void Start()
    {
        
    }

    void Update()
    {
        time += Time.deltaTime;
        int newFrameIndex = animList.spriteAnims[animIndex].spriteAnim.GetSpriteIndex(time);
        if(newFrameIndex != frameIndex)
        {
            spriteRenderer.sprite = animList.spriteAnims[animIndex].spriteAnim.GetSpriteFromIndex(newFrameIndex);
            frameIndex = newFrameIndex;
        }
    }

    public bool isAnimationFinished { get { return animList.spriteAnims[animIndex].spriteAnim.IsAnimationFinished(time); } }

    public void SelectAnim(string animName, bool loop = true)
    {
        for(int i=0; i<animList.spriteAnims.Length; i++)
        {
            if(animList.spriteAnims[i].name == animName && i != animIndex)
            {
                time = 0;
                animIndex = i;
                frameIndex = 0;
                spriteRenderer.sprite = animList.spriteAnims[animIndex].spriteAnim.GetSpriteFromIndex(frameIndex);
                loopAnim = loop;
            }
        }
    }

    public Vector3 GetPointPosition(string pointName)
    {
        int pointIndex = 0;
        for(int i=0; i<animList.actionPointNames.Length; i++)
        {
            if(animList.actionPointNames[i] == pointName)
                pointIndex = i;
        }
        Sprite sprite = animList.spriteAnims[animIndex].spriteAnim.GetSpriteFromIndex(frameIndex);
        Vector2 posInSprite = animList.spriteAnims[animIndex].spriteAnim.GetSpritePoint(pointIndex, frameIndex);
        Vector2 textureSize = sprite.textureRect.size / sprite.pixelsPerUnit;
        return transform.position + transform.right * posInSprite.x * textureSize.x * (spriteRenderer.flipX ? -1 : 1) - transform.up * posInSprite.y * textureSize.y;
    }

    public Vector2 GetLocalPointPosition(string pointName)
    {
        int pointIndex = 0;
        for(int i=0; i<animList.actionPointNames.Length; i++)
        {
            if(animList.actionPointNames[i] == pointName)
                pointIndex = i;
        }
        Sprite sprite = animList.spriteAnims[animIndex].spriteAnim.GetSpriteFromIndex(frameIndex);
        Vector2 posInSprite = animList.spriteAnims[animIndex].spriteAnim.GetSpritePoint(pointIndex, frameIndex);
        Vector2 textureSize = sprite.textureRect.size / sprite.pixelsPerUnit;
        return Vector3.right * posInSprite.x * textureSize.x * (spriteRenderer.flipX ? -1 : 1) - Vector3.up * posInSprite.y * textureSize.y;
    }
}
