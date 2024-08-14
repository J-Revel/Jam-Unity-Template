using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteAnimConfig
{
    public Sprite[] sprites = {};
    public int[] durations = { };
    public Vector2[] actionPoints = {};
    public bool loop = true;

    public bool IsAnimationFinished(float time)
    {
        int duration_sum = 0;
        for (int i = 0; i < durations.Length; i++)
        {
            duration_sum += durations[i];
        }
        return time * 60 > duration_sum;
    }

    public int GetSpriteIndex(int index)
    {
        if (sprites.Length == 0)
            return 0;
        return loop ? index % sprites.Length : Mathf.Min(index, sprites.Length - 1);
    }

    public Sprite GetSpriteFromTime(float time)
    {
        if (sprites.Length == 0)
            return null;
        float cursor = 0;
        int i;
        for (i = 0; cursor <= time; i++)
        {
            int sprite_index = GetSpriteIndex(i);
            cursor += durations[sprite_index];
            if (durations[sprite_index] == 0)
                return sprites[sprite_index];
        }
        return sprites[GetSpriteIndex(i)];
    }

    public Sprite GetSpriteFromIndex(int index)
    {
        if (sprites.Length == 0)
            return null;
        return sprites[loop ? index % sprites.Length : Mathf.Min(index, sprites.Length - 1)];
    }

    public Vector2 GetSpritePoint(int pointIndex, int spriteIndex)
    {
        return actionPoints[pointIndex * sprites.Length + GetSpriteIndex(spriteIndex)];
    }

    public int GetSpriteIndex(float time)
    {
        if (sprites.Length == 0)
            return 0;
        float cursor = 0;
        int i;
        for (i = 0; cursor <= time; i++)
        {
            int sprite_index = GetSpriteIndex(i);
            cursor += durations[sprite_index] / 60.0f;
            if (durations[sprite_index] == 0)
                return sprite_index;
        }
        return GetSpriteIndex(i);
    }

    public void SetActionPoint(int pointIndex, int spriteIndex, Vector2 point)
    {
        actionPoints[pointIndex * sprites.Length + GetSpriteIndex(spriteIndex)] = point;
    }

    public void AddActionPoint()
    {
        Vector2[] backup = actionPoints;
        int actionPointCount = actionPoints.Length / sprites.Length;
        actionPoints = new Vector2[(actionPointCount + 1) * sprites.Length];
        for(int i=0; i<backup.Length; i++)
        {
            actionPoints[i] = backup[i];
        }
    }

    public void RemoveActionPoint(int index)
    {
        Vector2[] backup = actionPoints;
        int actionPointCount = actionPoints.Length / sprites.Length;
        actionPoints = new Vector2[(actionPointCount - 1) * sprites.Length];
        for(int i=0; i<sprites.Length * index; i++)
        {
            actionPoints[i] = backup[i];
        }
        for(int i=sprites.Length * (index + 1); i<backup.Length; i++)
        {
            actionPoints[i-sprites.Length] = backup[i];
        }
    }
}
