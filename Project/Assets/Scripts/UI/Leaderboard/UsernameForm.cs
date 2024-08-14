using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UsernameForm : MonoBehaviour
{
    public TMPro.TMP_InputField textField;
    public Toggle disableLeaderboardToggle;
    public FocusButton confirmButton;

    void Start()
    {
        textField.text = ScoreSystem.instance.username;
        disableLeaderboardToggle.isOn = ScoreSystem.instance.offlineMode;
        textField.onValueChanged.AddListener(OnUsernameChanged);
        disableLeaderboardToggle.onValueChanged.AddListener(OnLeaderboardActivationChanged);
        UpdateButtonVisibility();
    }

    private void OnUsernameChanged(string username)
    {
        ScoreSystem.instance.username = username;
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.Save();
        UpdateButtonVisibility();

    }

    private void OnLeaderboardActivationChanged(bool value)
    {
        ScoreSystem.instance.offlineMode = value;
        PlayerPrefs.SetInt("offlineMode", value ? 1 : 0);
        PlayerPrefs.Save();
        UpdateButtonVisibility();
    }

    private void UpdateButtonVisibility()
    {
        confirmButton.interactable = ScoreSystem.instance.username.Length > 0 || ScoreSystem.instance.offlineMode;
    }
}
