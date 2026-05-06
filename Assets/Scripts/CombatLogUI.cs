using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CombatLogUI : MonoBehaviour
{
    public static CombatLogUI Instance { get; private set; }

    public TextMeshProUGUI logText;
    public int maxLines = 5;
    private List<string> logMessages = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Log(string message)
    {
        Debug.Log($"[CombatLog] {message}");
        
        if (logText == null) return;

        logMessages.Add(message);
        if (logMessages.Count > maxLines)
        {
            logMessages.RemoveAt(0);
        }

        logText.text = string.Join("\n", logMessages);
    }

    public void Clear()
    {
        logMessages.Clear();
        if (logText != null) logText.text = "";
    }
}
