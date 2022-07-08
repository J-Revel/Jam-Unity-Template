using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class LeaderboardUtility
{
    public static string webServiceRoot = "https://webservice.guilloteam.fr/";
    public static UnityWebRequest GetLeaderboardRequest(string projectId, bool showTop, int pageSize, int scoreId = -1, int tempScore = 0, string tempUsername = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("pageIndex", 0);
        form.AddField("pageSize", pageSize);
        form.AddField("tempId", scoreId);
        form.AddField("tempScore", tempScore);
        form.AddField("tempUsername", tempUsername);
        form.AddField("project", projectId);
        UnityWebRequest webRequest = UnityWebRequest.Post(webServiceRoot + "score/" + ((showTop || scoreId < 0) ? "page/" : "around/"),  form);
        return webRequest;
    }

    public static UnityWebRequest RemoveLeaderboardScore(string projectId, string password, int scoreId)
    {
        WWWForm form = new WWWForm();
        form.AddField("pageIndex", 0);
        form.AddField("scoreId", scoreId);
        form.AddField("project", projectId);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(webServiceRoot + "admin/remove",  form);
        return webRequest;
    }

    public static UnityWebRequest EditLeaderboardScore(string projectId, string password, int scoreId, string username, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("pageIndex", 0);
        form.AddField("scoreId", scoreId);
        form.AddField("project", projectId);
        form.AddField("password", password);
        form.AddField("username", username);
        form.AddField("score", score);
        UnityWebRequest webRequest = UnityWebRequest.Post(webServiceRoot + "admin/edit", form);
        return webRequest;
    }
    
    public static LeaderboardEntry[] ParseLeaderboardQueryResult(JSONNode root, int scoreId = -1)
    {
        if(root["success"].AsBool)
        {
            JSONArray scoresArrayJson = root["data"]["scores"].AsArray;
            LeaderboardEntry[] result = new LeaderboardEntry[scoresArrayJson.Count];
            for(int i=0; i<result.Length; i++)
                result[i] = new LeaderboardEntry();
            for(int i=0; i<Mathf.Min(result.Length, scoresArrayJson.Count); i++)
            {
                LeaderboardEntry entry = new LeaderboardEntry();
                entry.id = scoresArrayJson[i]["id"].AsInt;
                entry.rank = scoresArrayJson[i]["rank"].AsInt;
                entry.score = scoresArrayJson[i]["score"].AsInt;
                entry.username = scoresArrayJson[i]["username"];
                if(scoresArrayJson[i]["is_new"])
                    entry.type = LeaderboardEntryType.CurrentScore;
                else
                {
                    if(entry.id == scoreId)
                        entry.type = LeaderboardEntryType.BestPlayerScore;
                    else
                        entry.type = LeaderboardEntryType.Basic;
                } 
                result[i] = entry;
            }

            for(int i=Mathf.Min(result.Length, scoresArrayJson.Count); i<result.Length; i++)
            {
                LeaderboardEntry entry = new LeaderboardEntry();
                entry.type = LeaderboardEntryType.Disabled;
                result[i] = entry;
            }
            return result;
        }
        else
        {
            Debug.LogError("Request Error : " + root["error"]);
        }
        return null;
    }

    public static UnityWebRequest CreateLeaderboardRequest(string leaderboardUid, string adminPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("project_uid", leaderboardUid);
        form.AddField("password", adminPassword);
        UnityWebRequest webRequest = UnityWebRequest.Post(webServiceRoot + "project/register",  form);
        return webRequest;
    }
}
