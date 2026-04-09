using UnityEngine;

[System.Serializable]
public class TaskData
{
    public float time;
    public int fails;
    public int timesCompleted;
}

public class TaskManager : MonoBehaviour
{
    public static bool taskActive = false;

    public static float startTime;
    public static int failCount = 0;
    public static int totalFailCount = 0;
    public static int timesCompleted = 0;
    public static string currentTaskName = "";
    public static int passCount = 0; // Track correct passes (used by Task 2 Memory)

    // START TASK
    public static void StartTask(string taskName)
    {
        taskActive = true;
        startTime = Time.time;
        failCount = 0;
        passCount = 0;

        currentTaskName = taskName;

        Debug.Log("Task Started: " + taskName);
    }

    // FAIL (wrong step)
    public static void RegisterFail()
    {
        if (taskActive)
        {
            failCount++;
            totalFailCount++;
        }
    }

    // PASS (correct step — call this from Task 2 Memory when player gets one right)
    public static void RegisterPass()
    {
        if (taskActive)
        {
            passCount++;
        }
    }

    // COMPLETE TASK
    public static void CompleteTask()
    {
        if (taskActive)
        {
            float timeTaken = Time.time - startTime;

            timesCompleted++;
            passCount++; // Completing the task = 1 pass

            TaskData data = new TaskData();
            data.time = timeTaken;
            data.fails = totalFailCount;
            data.timesCompleted = timesCompleted;

            // ── Send stats to Supabase based on task type ──
            if (currentTaskName == "Footprint")
            {
                // Task 1 — original call (no task name needed, defaults to "Task 1")
                SupabaseSender.Instance.SendStats(timeTaken, totalFailCount, timesCompleted);
            }
            else if (currentTaskName == "Memory")
            {
                // Task 2 — includes task name and pass count
                SupabaseSender.Instance.SendStats(timeTaken, totalFailCount, timesCompleted, "Task 2", passCount);
            }

            //  CLEAN REPORT (SINGLE BLOCK)
            string report =
            "TASK REPORT\n\n" +
            "Task: " + currentTaskName + "\n" +
            "Time Taken : " + data.time.ToString("F2") + " sec\n" +
            "Task Completed : Yes\n" +
            "Fail Count : " + data.fails + "\n" +
            "Pass Count : " + passCount;

            Debug.Log(report);

            //  JSON (FOR BACKEND)
            string json = JsonUtility.ToJson(data);
            Debug.Log("DATA: " + json);

            taskActive = false;
        }
    }
}
