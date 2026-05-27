using UnityEngine;

namespace GhostSignal.UI
{
    public static class LevelProgress
    {
        private const string KeyStatus = "GhostSignal_LevelStatus_";
        private const string KeyTime = "GhostSignal_BestTime_";
        private const string KeyFragments = "GhostSignal_Fragments_";

        public static LevelData.LevelStatus GetStatus(LevelData level)
        {
            // TODOS los niveles disponibles (selector libre)
            return LevelData.LevelStatus.Available;
        }

        public static void SetStatus(LevelData level, LevelData.LevelStatus status)
        {
            PlayerPrefs.SetInt(KeyStatus + level.id, (int)status);
            PlayerPrefs.Save();
        }

        public static float GetBestTime(LevelData level)
        {
            return PlayerPrefs.GetFloat(KeyTime + level.id, 0f);
        }

        public static void SetBestTime(LevelData level, float seconds)
        {
            float currentBest = GetBestTime(level);
            if (currentBest == 0f || seconds < currentBest)
            {
                PlayerPrefs.SetFloat(KeyTime + level.id, seconds);
                PlayerPrefs.Save();
            }
        }

        public static int GetFragments(LevelData level)
        {
            return PlayerPrefs.GetInt(KeyFragments + level.id, 0);
        }

        public static void SetFragments(LevelData level, int count)
        {
            PlayerPrefs.SetInt(KeyFragments + level.id, Mathf.Clamp(count, 0, level.fragmentsTotal));
            PlayerPrefs.Save();
        }

        public static void UnlockNext(LevelData currentLevel, LevelData[] allLevels)
        {
            SetStatus(currentLevel, LevelData.LevelStatus.Completed);
            int nextId = currentLevel.id + 1;

            foreach (var level in allLevels)
            {
                if (level.id == nextId)
                {
                    if (GetStatus(level) == LevelData.LevelStatus.Locked)
                    {
                        SetStatus(level, LevelData.LevelStatus.Available);
                    }
                    break;
                }
            }
        }
    }
}