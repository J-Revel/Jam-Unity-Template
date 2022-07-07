using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LeaderboardEntryType
{
    Disabled,
    Basic,
    BestPlayerScore,
    CurrentScore
}
public class LeaderboardLine : MonoBehaviour
{
    public int index;
    public System.Action dataChangedDelegate;
    private LeaderboardEntry _leaderboardEntry;
    public LeaderboardEntry leaderboardEntry { 
        set {
            _leaderboardEntry = value;
            dataChangedDelegate?.Invoke();
        }
        get {
            return _leaderboardEntry;
        }
    }

    public void Clear()
    {
        _leaderboardEntry.type = LeaderboardEntryType.Disabled;
        dataChangedDelegate?.Invoke();
    }
}
