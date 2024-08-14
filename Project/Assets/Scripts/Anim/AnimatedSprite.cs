using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimatedSprite : MonoBehaviour
{
    [FormerlySerializedAs("spriteRenderer")] public SpriteRenderer sprite_renderer;
    [FormerlySerializedAs("animList")] public SpriteAnimList anim_list;
    [FormerlySerializedAs("animIndex")] public int anim_index;
    private float time;
    private int frameIndex = 0;

    void Update()
    {
        time += Time.deltaTime;
        int newFrameIndex = anim_list.spriteAnims[anim_index].spriteAnim.GetSpriteIndex(time);
        if(newFrameIndex != frameIndex)
        {
            sprite_renderer.sprite = anim_list.spriteAnims[anim_index].spriteAnim.GetSpriteFromIndex(newFrameIndex);
            frameIndex = newFrameIndex;
        }
    }

    public bool isAnimationFinished { get { return anim_list.spriteAnims[anim_index].spriteAnim.IsAnimationFinished(time); } }

    public void SelectAnim(string animName, bool loop = true)
    {
        for(int i=0; i<anim_list.spriteAnims.Length; i++)
        {
            if(anim_list.spriteAnims[i].name == animName && i != anim_index)
            {
                time = 0;
                anim_index = i;
                frameIndex = 0;
                sprite_renderer.sprite = anim_list.spriteAnims[anim_index].spriteAnim.GetSpriteFromIndex(frameIndex);
            }
        }
    }

    public Vector3 GetPointPosition(string pointName)
    {
        int pointIndex = 0;
        for(int i=0; i<anim_list.actionPointNames.Length; i++)
        {
            if(anim_list.actionPointNames[i] == pointName)
                pointIndex = i;
        }
        Sprite sprite = anim_list.spriteAnims[anim_index].spriteAnim.GetSpriteFromIndex(frameIndex);
        Vector2 posInSprite = anim_list.spriteAnims[anim_index].spriteAnim.GetSpritePoint(pointIndex, frameIndex);
        Vector2 textureSize = sprite.textureRect.size / sprite.pixelsPerUnit;
        return transform.position + transform.right * posInSprite.x * textureSize.x * (sprite_renderer.flipX ? -1 : 1) - transform.up * posInSprite.y * textureSize.y;
    }

    public Vector2 GetLocalPointPosition(string pointName)
    {
        int pointIndex = 0;
        for(int i=0; i<anim_list.actionPointNames.Length; i++)
        {
            if(anim_list.actionPointNames[i] == pointName)
                pointIndex = i;
        }
        Sprite sprite = anim_list.spriteAnims[anim_index].spriteAnim.GetSpriteFromIndex(frameIndex);
        Vector2 posInSprite = anim_list.spriteAnims[anim_index].spriteAnim.GetSpritePoint(pointIndex, frameIndex);
        Vector2 textureSize = sprite.textureRect.size / sprite.pixelsPerUnit;
        return Vector3.right * posInSprite.x * textureSize.x * (sprite_renderer.flipX ? -1 : 1) - Vector3.up * posInSprite.y * textureSize.y;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimatedSprite))]
[CanEditMultipleObjects]
public class AnimatedSpriteEditor : Editor
{
    private SerializedProperty sprite_renderer_prop,
        anim_index_prop,
        anim_list_prop;

    void OnEnable()
    {
        sprite_renderer_prop = serializedObject.FindProperty("sprite_renderer");
        anim_index_prop = serializedObject.FindProperty("anim_index");
        anim_list_prop = serializedObject.FindProperty("anim_list");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(sprite_renderer_prop);
        EditorGUILayout.PropertyField(anim_list_prop);
        AnimatedSprite sprite_target = target as AnimatedSprite;
        if (sprite_target.anim_list != null)
        {
            string[] anims = new string[sprite_target.anim_list.spriteAnims.Length];
            for (int i = 0; i < anims.Length; i++)
            {
                anims[i] = sprite_target.anim_list.spriteAnims[i].name;
            }
            anim_index_prop.intValue = EditorGUILayout.Popup("Anim", anim_index_prop.intValue, anims);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
