using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardLineDisplay : MonoBehaviour
{
    private LeaderboardLine leaderboardLine;
    public TMPro.TextMeshProUGUI usernameText;
    public TMPro.TextMeshProUGUI rankText;
    public TMPro.TextMeshProUGUI scoreText;
    public LeaderboardEntryType[] displayForEntryTypes;

    private void Start()
    {
        leaderboardLine = GetComponentInParent<LeaderboardLine>();
        leaderboardLine.dataChangedDelegate += OnDataChanged;
        OnDataChanged();

    }

    private void OnDataChanged()
    {
        usernameText.text = leaderboardLine.leaderboardEntry.username;
        rankText.text = leaderboardLine.leaderboardEntry.rank.ToString();
        scoreText.text = leaderboardLine.leaderboardEntry.score.ToString();
        bool display = false;
        for(int i=0; i<displayForEntryTypes.Length; i++)
        {
            if(displayForEntryTypes[i] == leaderboardLine.leaderboardEntry.type)
                display = true;
        }
        gameObject.SetActive(display);
    }
}
