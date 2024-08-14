using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;

    public int score;
    public int scoreId;
    public string username;
    public bool offlineMode = false;
    public bool resetId = false;

    public System.Action userDataChangedDelegate;

    public void Awake()
    {
        instance = this;
        
        scoreId = PlayerPrefs.GetInt("scoreId", -1);
        username = PlayerPrefs.GetString("username", "");
        offlineMode = PlayerPrefs.GetInt("offlineMode", 0) > 0;
        if(resetId)
        {
            scoreId = -1;
        }
    }

    public void SaveUserData(int id)
    {
        scoreId = id;
        PlayerPrefs.SetInt("scoreId", id);
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetInt("offlineMode", offlineMode ? 1 : 0);
        PlayerPrefs.Save();
        this.scoreId = id;
        userDataChangedDelegate?.Invoke();
    }

    public void SendScore(System.Action scoreSentEvent)
    {
        StartCoroutine(SendScoreCoroutine(scoreSentEvent));
    }

    public IEnumerator SendScoreCoroutine(System.Action scoreSentEvent)
    {
        WWWForm form = new WWWForm();
        string username = ScoreSystem.instance.username;// inputField.text;
        form.AddField("username", username);
        form.AddField("id", ScoreSystem.instance.scoreId);
        EncryptionResult encryptedScore = EncryptionService.instance.Encrypt(Encoding.UTF8.GetBytes(ScoreSystem.instance.score.ToString()));
        form.AddField("score", System.Convert.ToBase64String(encryptedScore.data));
        form.AddField("iv", System.Convert.ToBase64String(encryptedScore.iv));
        form.AddField("project", EncryptionService.instance.projectId);

        string requestPath = "https://webservice.guilloteam.fr/score/add/";
        if(ScoreSystem.instance.scoreId >= 0)
        {
            requestPath = "https://webservice.guilloteam.fr/score/update/";
        }
        UnityWebRequest webRequest = UnityWebRequest.Post(requestPath,  form);
        
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                SimpleJSON.JSONNode rootNode = SimpleJSON.JSON.Parse(webRequest.downloadHandler.text);
                int rank = rootNode["data"]["rank"];
                int id = rootNode["data"]["id"];

                Debug.Log(webRequest.downloadHandler.text);
                ScoreSystem.instance.SaveUserData(id);

                scoreSentEvent?.Invoke();
                // LeaderboardMenu spawnedMenu = Instantiate(leaderboardMenuPrefab, transform.parent).gameObject.GetComponent<LeaderboardMenu>();
                // spawnedMenu.pageIndex = rank / spawnedMenu.pageSize;
                // spawnedMenu.tempScore = ScoreSystem.instance.score;
                // spawnedMenu.tempUsername = username;
                // Destroy(gameObject);
                break;
        }
        webRequest.Dispose();
    }
}
