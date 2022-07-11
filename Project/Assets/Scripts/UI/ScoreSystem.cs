using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;

    public int score;
    public int scoreId;
    public string username;

    public void Awake()
    {
        instance = this;
        
        scoreId = PlayerPrefs.GetInt("scoreId", -1);
        username = PlayerPrefs.GetString("username", "");
    }
    

    public void SetUserData(int id, string username)
    {
        scoreId = id;
        PlayerPrefs.SetInt("scoreId", id);
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.Save();
    }
}
