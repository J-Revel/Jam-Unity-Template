using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class ScoreDisplay : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    public string prefix = "Score : ";

    private void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }
    public void Update()
    {
        text.text = prefix + ScoreSystem.instance.score;
    }
}
