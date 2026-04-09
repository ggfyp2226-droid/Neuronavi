-- ============================================================
-- Supabase SQL Setup — Run this in the Supabase SQL Editor
--
-- MATCHES YOUR UNITY TaskData:
--   { "time": float, "fails": int, "timesCompleted": int }
--
-- Steps:
--   1. Go to https://supabase.com → Sign in → Open your project
--   2. Click "SQL Editor" in the left sidebar
--   3. Paste this entire script and click "Run"
-- ============================================================

-- Drop existing table if you need a fresh start (uncomment if needed)
-- DROP TABLE IF EXISTS game_stats;

-- Create the game_stats table matching YOUR Unity data
CREATE TABLE IF NOT EXISTS game_stats (
    id                UUID DEFAULT gen_random_uuid() PRIMARY KEY,  -- auto-generated unique ID
    time              REAL NOT NULL,                                -- time taken (seconds)
    fails             INTEGER NOT NULL DEFAULT 0,                   -- number of failed attempts
    "timesCompleted"  INTEGER NOT NULL DEFAULT 0,                   -- times completed count
    created_at        TIMESTAMPTZ DEFAULT now()                     -- auto-set timestamp
);

-- ============================================================
-- ROW LEVEL SECURITY — Open policies for testing
-- ⚠️ In production, lock these down!
-- ============================================================
ALTER TABLE game_stats ENABLE ROW LEVEL SECURITY;

-- Allow anonymous inserts (Unity can POST without login)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_policies WHERE tablename = 'game_stats' AND policyname = 'Allow anonymous inserts'
    ) THEN
        CREATE POLICY "Allow anonymous inserts"
            ON game_stats
            FOR INSERT
            TO anon
            WITH CHECK (true);
    END IF;
END $$;

-- Allow anonymous reads (Dashboard can GET without login)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_policies WHERE tablename = 'game_stats' AND policyname = 'Allow anonymous reads'
    ) THEN
        CREATE POLICY "Allow anonymous reads"
            ON game_stats
            FOR SELECT
            TO anon
            USING (true);
    END IF;
END $$;

-- ============================================================
-- MIGRATION: Add Task 2 Support
-- Adds task_name and pass_count columns.
-- Safe to run multiple times (IF NOT EXISTS / idempotent).
-- Existing rows auto-get default values:
--   task_name  → 'Task 1'
--   pass_count → 0
-- ============================================================

-- Add task_name column (identifies which task: 'Task 1', 'Task 2', etc.)
ALTER TABLE game_stats ADD COLUMN IF NOT EXISTS task_name TEXT NOT NULL DEFAULT 'Task 1';

-- Add pass_count column (used by Task 2 Memory game)
ALTER TABLE game_stats ADD COLUMN IF NOT EXISTS pass_count INTEGER NOT NULL DEFAULT 0;

-- Index for fast filtering by task
CREATE INDEX IF NOT EXISTS idx_game_stats_task_name ON game_stats(task_name);

-- ============================================================
-- Optional: Insert test data to verify everything works
-- ============================================================

-- Task 1 test data (now includes pass_count)
INSERT INTO game_stats (time, fails, "timesCompleted", task_name, pass_count)
VALUES
    (18.82, 1, 1, 'Task 1', 1),
    (45.30, 3, 1, 'Task 1', 1),
    (62.70, 5, 2, 'Task 1', 2),
    (30.10, 0, 1, 'Task 1', 1);

-- Task 2 (Memory) test data
INSERT INTO game_stats (time, fails, "timesCompleted", task_name, pass_count)
VALUES
    (75.98, 0, 2, 'Task 2', 1),
    (52.40, 2, 1, 'Task 2', 1),
    (90.10, 1, 3, 'Task 2', 2);

-- Verify: Run this to see all data
-- SELECT * FROM game_stats ORDER BY created_at DESC;
-- SELECT * FROM game_stats WHERE task_name = 'Task 2' ORDER BY created_at DESC;
