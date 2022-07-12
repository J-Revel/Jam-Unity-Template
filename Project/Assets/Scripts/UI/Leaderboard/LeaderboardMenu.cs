using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

[System.Serializable]
public struct RequestResult<T>
{
    public bool success;
    public T data;
}


[System.Serializable]
public struct LeaderboardEntry
{
    public int id;
    public int rank;
    public string username;
    public int score;
    public LeaderboardEntryType type;
}

[System.Serializable]
public struct LeaderboardRequestResult
{
    public LeaderboardEntry[] scores;
    public int pageCount;

}

public class LeaderboardMenu : MonoBehaviour
{
    public LeaderboardLine linePrefab;
    public int pageSize = 10;

    private LeaderboardLine[] lines;
    public int pageIndex;
    public GameObject loadingScreen;
    public RectTransform mainContainer;
    public int pageCount = 0;
    private int scoreId = -1;

    public int tempScore;
    public string tempUsername;

    public bool showTop = false;
    public bool showLocalScore = true;

    IEnumerator Start()
    {
        ScoreSystem.instance.userDataChangedDelegate += UpdateDisplay;
        lines = new LeaderboardLine[pageSize];
        for(int i=0; i<pageSize; i++)
        {
            lines[i] = Instantiate(linePrefab, mainContainer);
            RectTransform rectTransform = lines[i].GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1 - (float)(i+1) / pageSize);
            rectTransform.anchorMax = new Vector2(1, 1 - (float)i / pageSize);
            lines[i].index = i;
        }
        yield return UpdateDisplayCoroutine();
    }

    private void OnDestroy()
    {
        ScoreSystem.instance.userDataChangedDelegate -= UpdateDisplay;
    }

    private void UpdateDisplay()
    {
        StartCoroutine(UpdateDisplayCoroutine());
    }

    IEnumerator UpdateDisplayCoroutine()
    {
        loadingScreen.SetActive(true);
        for(int i=0; i<lines.Length; i++)
            lines[i].Clear();
        WWWForm form = new WWWForm();
        UnityWebRequest webRequest = null;
        if(showLocalScore)
           webRequest = LeaderboardUtility.GetLeaderboardRequest(EncryptionService.instance.projectId, showTop, pageSize, ScoreSystem.instance.scoreId, ScoreSystem.instance.score, ScoreSystem.instance.username);
        else
           webRequest = LeaderboardUtility.GetLeaderboardRequest(EncryptionService.instance.projectId, showTop, pageSize, ScoreSystem.instance.scoreId);
        yield return webRequest.SendWebRequest();
        loadingScreen.SetActive(false);
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                JSONNode root = JSON.Parse(webRequest.downloadHandler.text);
                LeaderboardEntry[] entries = LeaderboardUtility.ParseLeaderboardQueryResult(root, ScoreSystem.instance.scoreId);
                for(int i=0; i<Mathf.Min(lines.Length, entries.Length); i++)
                {
                    lines[i].leaderboardEntry = entries[i];
                }
                break;
            default:
                Debug.Log("Error: " + webRequest.error);
                break;
        }
        webRequest.Dispose();
    }

    public void ShowTop()
    {
        showTop = true;
        StartCoroutine(UpdateDisplayCoroutine());
    }

    public void ShowMyScore()
    {
        showTop = false;
        StartCoroutine(UpdateDisplayCoroutine());
    }
}
