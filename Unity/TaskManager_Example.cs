// ============================================================
// TaskManager Integration — HOW TO CALL SupabaseSender
//
// In your EXISTING TaskManager at:
//   D:\GG\UNITY\NAVI ISLAND M.F\Assets\Scripts\TaskManager.cs
//
// Add this ONE LINE inside CompleteTask(), after your TaskData setup:
// ============================================================

// YOUR EXISTING CODE (don't change this):
//
//   public static void CompleteTask()
//   {
//       if (taskActive)
//       {
//           float timeTaken = Time.time - startTime;
//           timesCompleted++;
//
//           TaskData data = new TaskData();
//           data.time = timeTaken;
//           data.fails = totalFailCount;
//           data.timesCompleted = timesCompleted;
//
//           // ⬇️ ADD THIS ONE LINE RIGHT HERE ⬇️
//           SupabaseSender.Instance.SendStats(timeTaken, totalFailCount, timesCompleted);
//
//           // ... rest of your code (report, JSON, etc.)
//       }
//   }

// ============================================================
// PARAMETERS EXPLAINED:
//
//   timeTaken        → float (Time.time - startTime)  = your "time"
//   totalFailCount   → int                            = your "fails"
//   timesCompleted   → int                            = your "timesCompleted"
//
// That's it! No player_id needed for now.
// ============================================================
