using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NamedSpriteAnim
{
    public string name;
    public SpriteAnimConfig spriteAnim;
}

[System.Serializable]
public struct SpriteAnimRef
{
    public string animName;
    public SpriteAnimList animList;

    public Sprite GetSprite(float time)
    {
        foreach(var anim in animList.spriteAnims)
        {
            if(anim.name == animName)
            {
                return anim.spriteAnim.GetSpriteFromTime(time);
            }
        }
        return null;
    }
}

[CreateAssetMenu]
public class SpriteAnimList : ScriptableObject
{
    public NamedSpriteAnim[] spriteAnims;
    public string[] actionPointNames;

    public void AddActionPoint(string name)
    {
        string[] buffer = actionPointNames;
        actionPointNames = new string[actionPointNames.Length + 1];
        for(int i=0; i<buffer.Length; i++)
        {
            actionPointNames[i] = buffer[i];
        }
        actionPointNames[actionPointNames.Length - 1] = name;
        for(int i=0; i<spriteAnims.Length; i++)
        {
            spriteAnims[i].spriteAnim.AddActionPoint();
        }
    }

    public void RemoveActionPoint(int index)
    {
        string[] buffer = actionPointNames;
        actionPointNames = new string[actionPointNames.Length - 1];
        for(int i=0; i<index; i++)
        {
            actionPointNames[i] = buffer[i];
        }
        for(int i=index + 1; i<buffer.Length;i++)
        {
            actionPointNames[i-1] = buffer[i];
        }
        for(int i=0; i<spriteAnims.Length; i++)
        {
            spriteAnims[i].spriteAnim.RemoveActionPoint(index);
        }
    }
}
