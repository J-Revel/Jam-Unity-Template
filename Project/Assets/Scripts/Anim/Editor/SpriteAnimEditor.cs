using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI;
using UnityEditor;

public class AnimEditor : EditorWindow
{
    [MenuItem ("Window/Anim Editor")]

    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(AnimEditor));
    }

    private bool groupEnabled = false;
    private SpriteAnimList animList;
    private int selectedAnimIndex = 0;
    private int selectedAnimSprite = 0;
    private int selectedPointIndex = -1;
    private float displaySize = 1;
    private Vector2 scrollPos;
    private string newPointName;
    private Color[] pointColors;
    private float lastBlinkTime = 0;
    private bool blinkOn = false;

    private void Init()
    {
        lastBlinkTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if(Time.realtimeSinceStartup - lastBlinkTime > 0.5f)
        {
            blinkOn = !blinkOn;
            lastBlinkTime = Time.realtimeSinceStartup;
            Repaint();
        }
    }

    void OnGUI()
    {
        SpriteAnimList newAnimList = (SpriteAnimList)EditorGUILayout.ObjectField(animList, typeof(SpriteAnimList), false, null);
        if(newAnimList == null)
            return;
        if(newAnimList.actionPointNames == null)
            newAnimList.actionPointNames = new string[0];
        for(int i=0; i<newAnimList.actionPointNames.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
                newAnimList.actionPointNames[i] = EditorGUILayout.TextField(newAnimList.actionPointNames[i]);
                if(GUILayout.Button("Remove"))
                {
                    newAnimList.RemoveActionPoint(i);
                    EditorUtility.SetDirty(newAnimList);
                }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Add Point"))
            {
                newAnimList.AddActionPoint("new anim");
                EditorUtility.SetDirty(newAnimList);
            }
        EditorGUILayout.EndHorizontal();
        if((newAnimList == null) || newAnimList != animList)
        {
            animList = newAnimList;
            selectedAnimIndex = 0;
            selectedAnimSprite = 0;
        }
        if(newAnimList != null)
        {
            string[] animNames = new string[newAnimList.spriteAnims.Length];
            for(int i=0; i<newAnimList.spriteAnims.Length; i++)
            {
                NamedSpriteAnim animElement = newAnimList.spriteAnims[i];
                string animName = animElement.name;
                animNames[i] = animName;
            }
            int newSelectedAnimIndex = EditorGUILayout.Popup("animation", selectedAnimIndex, animNames);
            if(newSelectedAnimIndex != selectedAnimIndex)
            {
                selectedAnimIndex = newSelectedAnimIndex;
                Debug.Log(selectedAnimIndex);
            }
            NamedSpriteAnim selectedAnimElement = newAnimList.spriteAnims[selectedAnimIndex];
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Anim Name");
                string newName = EditorGUILayout.TextField(selectedAnimElement.name);
                if(newName != selectedAnimElement.name)
                {
                    newAnimList.spriteAnims[selectedAnimIndex].name = newName;
                    EditorUtility.SetDirty(newAnimList);
                }
            EditorGUILayout.EndHorizontal();
            float newFramePerSecond = EditorGUILayout.FloatField(selectedAnimElement.spriteAnim.framePerSecond);
            if(newFramePerSecond != selectedAnimElement.spriteAnim.framePerSecond)
            {
                selectedAnimElement.spriteAnim.framePerSecond = newFramePerSecond;
                EditorUtility.SetDirty(newAnimList);
            }
            SpriteAnimConfig spriteAnim = newAnimList.spriteAnims[selectedAnimIndex].spriteAnim;
            Sprite sprite = spriteAnim.GetSpriteFromIndex(selectedAnimSprite);
            Vector2 maxSpriteSize = Vector2.zero;
            for(int i=0; i<spriteAnim.sprites.Length; i++)
            {
                maxSpriteSize.x = Mathf.Max(maxSpriteSize.x, spriteAnim.sprites[i].rect.width);
                maxSpriteSize.y = Mathf.Max(maxSpriteSize.y, spriteAnim.sprites[i].rect.height);
            }
            
            displaySize = EditorGUILayout.Slider(displaySize, 0.01f, 10);
            selectedAnimSprite = EditorGUILayout.IntSlider(selectedAnimSprite, 0, spriteAnim.sprites.Length - 1);
            Rect rect = GUILayoutUtility.GetRect(100.0f, 100.0f, GUILayout.ExpandHeight(true));
            Vector2[] points = new Vector2[animList.actionPointNames.Length];
            for(int i=0; i<animList.actionPointNames.Length; i++)
            {
                points[i] = spriteAnim.GetSpritePoint(i, selectedAnimSprite);
            }
            string[] options = new string[newAnimList.actionPointNames.Length + 1];
            options[0] = "Move";
            for(int i=0; i<newAnimList.actionPointNames.Length; i++)
            {
                options[i+1] = newAnimList.actionPointNames[i];
            }
            selectedPointIndex = GUILayout.Toolbar(selectedPointIndex + 1, options) - 1;
            DrawSpriteScrollView(rect, sprite, maxSpriteSize, points, newAnimList);
        }
    }

    private bool drag = false;
    private Vector2 lastDragPos;
    
    private void DrawSpriteScrollView(Rect rect, Sprite sprite, Vector2 maxSpriteSize, Vector2[] points, SpriteAnimList newAnimList)
    {
        var e = Event.current;

        
        if (e.isMouse && e.type == EventType.MouseDown && (e.button != 0 || selectedPointIndex < 0))
        {
            lastDragPos = e.mousePosition;
            drag = true;
        }
        else if(e.isMouse && e.type == EventType.MouseUp && (e.button != 0 || selectedPointIndex < 0))
        {
            drag = false;
        }
        if(drag  && e.mousePosition != lastDragPos)
        {
            scrollPos += lastDragPos - e.mousePosition;
            lastDragPos = e.mousePosition;
            Repaint();
        }
        Vector2 center = new Vector2(maxSpriteSize.x / 2, maxSpriteSize.y / 2);
        Rect contentRect = new Rect(0, 0, rect.width + maxSpriteSize.x * displaySize, rect.height + maxSpriteSize.y * displaySize);
        Vector2 contentCenter = contentRect.position + contentRect.size / 2;
        Vector2 scrollCenter = contentCenter - rect.size / 2;
        scrollPos = GUI.BeginScrollView(
            rect,
            scrollCenter + scrollPos,
            contentRect
        ) - scrollCenter;
        DrawOnGUISprite(sprite, contentCenter, displaySize);

        if (selectedPointIndex >= 0 && e.isMouse && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
        {
            SpriteAnimConfig spriteAnim = newAnimList.spriteAnims[selectedAnimIndex].spriteAnim;
            spriteAnim.SetActionPoint(selectedPointIndex, selectedAnimSprite, WorldToRelative(sprite, contentCenter, displaySize, e.mousePosition));
            EditorUtility.SetDirty(newAnimList);
            Repaint();
        }

        EditorGUI.DrawRect(new Rect(contentCenter.x - 2, contentCenter.y - 2, 4, 4), Color.green);
            Color[] colors = SpriteAnimEditorSettings.GetOrCreateSettings().colors;
        for(int i=0; i<points.Length; i++)
        {
            Vector2 pointPos = RelativeToWorld(sprite, contentCenter, displaySize, points[i]);
            EditorGUI.DrawRect(new Rect(pointPos.x - 3, pointPos.y - 3, 6, 6), colors[i % colors.Length]);
            if(i == selectedPointIndex)
            {
                EditorGUI.DrawRect(new Rect(pointPos.x - 5, pointPos.y - 5, 10, 10), colors[i % colors.Length]);
                if(blinkOn)
                    EditorGUI.DrawRect(new Rect(pointPos.x - 4, pointPos.y - 4, 8, 8), Color.white);
            }
        }
        GUI.EndScrollView();
    }
    
    void DrawOnGUISprite(Sprite aSprite, Vector2 position, float scale)
    {
        Rect c = aSprite.rect;
        float spriteW = c.width;
        float spriteH = c.height;
        Rect rect = new Rect(position.x - aSprite.pivot.x * scale, position.y - (spriteH - aSprite.pivot.y) * scale, spriteW * scale, spriteH * scale);
        float ratio = rect.width / c.width;
        rect.width = rect.height * c.width / c.height;
        if (Event.current.type == EventType.Repaint)
        {
            var tex = aSprite.texture;
            c.xMin /= tex.width;
            c.xMax /= tex.width;
            c.yMin /= tex.height;
            c.yMax /= tex.height;
            GUI.DrawTextureWithTexCoords(rect, tex, c);
        }
    }

    Vector2 RelativeToWorld(Sprite aSprite, Vector2 position, float scale, Vector2 relativePoint)
    {
        Rect c = aSprite.rect;
        float spriteW = c.width;
        float spriteH = c.height;
        Rect rect = new Rect(position.x, position.y, spriteW * scale, spriteH * scale);
        return rect.position + rect.size * relativePoint;
    }

    Vector2 WorldToRelative(Sprite aSprite, Vector2 position, float scale, Vector2 screenPoint)
    {
        Rect c = aSprite.rect;
        float spriteW = c.width;
        float spriteH = c.height;
        Rect rect = new Rect(position.x, position.y, spriteW * scale, spriteH * scale);
        return (screenPoint - rect.position) / rect.size;
    }
}
