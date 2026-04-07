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
CREATE POLICY "Allow anonymous inserts"
    ON game_stats
    FOR INSERT
    TO anon
    WITH CHECK (true);

-- Allow anonymous reads (Dashboard can GET without login)
CREATE POLICY "Allow anonymous reads"
    ON game_stats
    FOR SELECT
    TO anon
    USING (true);

-- ============================================================
-- Optional: Insert test data to verify everything works
-- ============================================================
INSERT INTO game_stats (time, fails, "timesCompleted")
VALUES
    (18.82, 1, 1),
    (45.30, 3, 1),
    (62.70, 5, 2),
    (30.10, 0, 1);

-- Verify: Run this to see the test data
-- SELECT * FROM game_stats ORDER BY created_at DESC;
