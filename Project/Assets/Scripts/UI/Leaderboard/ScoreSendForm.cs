using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Events;

public class ScoreSendForm : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public TMPro.TextMeshProUGUI scoreText;
    public LeaderboardMenu leaderboardMenuPrefab;
    public UnityEvent scoreSentEvent;

    private void Start()
    {
        inputField.text = ScoreSystem.instance.username;
    }

    private void Update()
    {
        if(scoreText != null)
            scoreText.text = "" + ScoreSystem.instance.score;
    }

    public void SendScore()
    {
        StartCoroutine(SendScoreCoroutine());
    }

    public IEnumerator SendScoreCoroutine()
    {
        WWWForm form = new WWWForm();
        string username = inputField.text;
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
                
                ScoreSystem.instance.SetUserData(id, username);
            
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
