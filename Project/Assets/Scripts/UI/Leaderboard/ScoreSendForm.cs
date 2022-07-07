using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

public class ScoreSendForm : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public TMPro.TextMeshProUGUI scoreText;
    public LeaderboardMenu leaderboardMenuPrefab;

    private void Start()
    {
        inputField.text = PlayerPrefs.GetString("username", "");
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
        int scoreId = PlayerPrefs.GetInt("scoreId", -1);
        WWWForm form = new WWWForm();
        string username = inputField.text;
        form.AddField("username", username);
        form.AddField("id", scoreId);
        EncryptionResult encryptedScore = EncryptionService.Encrypt(Encoding.UTF8.GetBytes(ScoreSystem.instance.score.ToString()));
        form.AddField("score", System.Convert.ToBase64String(encryptedScore.data));
        form.AddField("iv", System.Convert.ToBase64String(encryptedScore.iv));
        form.AddField("project", EncryptionService.projectId);
        

        string requestPath = "https://webservice.guilloteam.fr/score/add/";
        if(scoreId >= 0)
        {
            requestPath = "https://webservice.guilloteam.fr/score/update/";
        }
        UnityWebRequest webRequest = UnityWebRequest.Post(requestPath,  form);
        
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log(webRequest.downloadHandler.text);
                SimpleJSON.JSONNode rootNode = SimpleJSON.JSON.Parse(webRequest.downloadHandler.text);
                int rank = rootNode["data"]["rank"];
                int id = rootNode["data"]["id"];
                
                PlayerPrefs.SetInt("scoreId", id);
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.Save();
                
                LeaderboardMenu spawnedMenu = Instantiate(leaderboardMenuPrefab).gameObject.GetComponent<LeaderboardMenu>();
                spawnedMenu.pageIndex = rank / spawnedMenu.pageSize;
                spawnedMenu.tempScore = ScoreSystem.instance.score;
                spawnedMenu.tempUsername = username;
                Destroy(gameObject);
                break;
        }
    }
}
