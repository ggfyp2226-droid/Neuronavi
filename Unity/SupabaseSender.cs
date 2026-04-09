// ============================================================
// SupabaseSender.cs — Reusable Unity Script
// Sends gameplay stats to Supabase via REST API (HTTP POST).
//
// SETUP:
//   1. Attach this script to any GameObject in your scene
//      (e.g., an empty "SupabaseManager" object).
//   2. Fill in your Supabase URL and API Key in the Inspector.
//   3. Call SupabaseSender.Instance.SendStats(...) from anywhere.
//
// SUPPORTS:
//   • Task 1 — SendStats(time, fails, timesCompleted)
//   • Task 2 — SendStats(time, fails, timesCompleted, "Task 2", passCount)
//
// LOCATION: Assets/Scripts/SupabaseSender.cs
// ============================================================

using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SupabaseSender : MonoBehaviour
{
    // === SINGLETON (so you can call it from any script) ===
    public static SupabaseSender Instance { get; private set; }

    // === FILL THESE IN THE INSPECTOR ===
    [Header("Supabase Configuration")]
    [Tooltip("Your Supabase project URL, e.g. https://abcdefg.supabase.co")]
    public string supabaseUrl = "https://armtgguvlpmkkjniqvli.supabase.co";

    [Tooltip("Your Supabase anon/public API key")]
    public string supabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFybXRnZ3V2bHBta2tqbmlxdmxpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzU1Nzg2ODUsImV4cCI6MjA5MTE1NDY4NX0.zWCAnJOio1UkQl1Xq8KARx-YSdvaY8lNLGa9OEHvw40";

    // Table name in Supabase
    private const string TABLE_NAME = "game_stats";

    private void Awake()
    {
        // Singleton pattern — keeps one instance alive across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ==========================================================
    // PUBLIC METHOD — Original (backward-compatible for Task 1)
    // ==========================================================
    /// <summary>
    /// Sends gameplay stats to Supabase (defaults to "Task 1").
    /// </summary>
    /// <param name="time">Time taken to complete the task (seconds)</param>
    /// <param name="fails">Number of failed attempts</param>
    /// <param name="timesCompleted">Number of times completed</param>
    public void SendStats(float time, int fails, int timesCompleted)
    {
        // Backward compatible — defaults to Task 1, pass_count 0
        SendStats(time, fails, timesCompleted, "Task 1", 0);
    }

    // ==========================================================
    // PUBLIC METHOD — Full version (supports any task)
    // ==========================================================
    /// <summary>
    /// Sends gameplay stats to Supabase with task name and pass count.
    /// </summary>
    /// <param name="time">Time taken to complete the task (seconds)</param>
    /// <param name="fails">Number of failed attempts</param>
    /// <param name="timesCompleted">Number of times completed</param>
    /// <param name="taskName">Task identifier, e.g. "Task 1" or "Task 2"</param>
    /// <param name="passCount">Number of passes (used by Task 2 Memory)</param>
    public void SendStats(float time, int fails, int timesCompleted, string taskName, int passCount = 0)
    {
        StartCoroutine(PostToSupabase(time, fails, timesCompleted, taskName, passCount));
    }

    // ==========================================================
    // COROUTINE — Handles the actual HTTP POST request
    // ==========================================================
    private IEnumerator PostToSupabase(float time, int fails, int timesCompleted, string taskName, int passCount)
    {
        // Build the REST API endpoint URL
        string url = $"{supabaseUrl}/rest/v1/{TABLE_NAME}";

        // Create JSON payload
        string jsonPayload = JsonUtility.ToJson(new GameStatsPayload
        {
            time = time,
            fails = fails,
            timesCompleted = timesCompleted,
            task_name = taskName,
            pass_count = passCount
        });

        Debug.Log($"[SupabaseSender] Sending data: {jsonPayload}");

        // Create the POST request
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            // Attach the JSON body
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // === REQUIRED HEADERS ===
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");

            // Prefer: return=minimal → faster, doesn't return the inserted row
            request.SetRequestHeader("Prefer", "return=minimal");

            // Send the request and wait for response
            yield return request.SendWebRequest();

            // Check result
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[SupabaseSender] ✅ {taskName} data sent to Supabase successfully!");
            }
            else
            {
                Debug.LogError($"[SupabaseSender] ❌ Error: {request.error}");
                Debug.LogError($"[SupabaseSender] Response: {request.downloadHandler.text}");
            }
        }
    }

    // ==========================================================
    // DATA CLASS — Matches the Supabase table columns
    // ==========================================================
    [System.Serializable]
    private class GameStatsPayload
    {
        public float time;
        public int fails;
        public int timesCompleted;
        public string task_name;
        public int pass_count;
    }
}
