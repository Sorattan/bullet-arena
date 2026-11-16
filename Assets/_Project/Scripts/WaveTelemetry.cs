using System;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>Prototype CSV log; does not collect participant identity.</summary>
public static class WaveTelemetry
{
    public static string FilePath => Path.Combine(Application.persistentDataPath, "bullet_arena_waves.csv");

    public static void Append(int wave, int enemyCount, float spawnInterval,
        float durationSeconds, int damageTaken, bool success)
    {
        try
        {
            bool newFile = !File.Exists(FilePath);
            using (var writer = new StreamWriter(FilePath, append: true))
            {
                if (newFile)
                    writer.WriteLine("timestamp_utc,wave,enemy_count,spawn_interval_s,duration_s,damage_taken,success");
                var ci = CultureInfo.InvariantCulture;
                writer.WriteLine(string.Join(",",
                    DateTime.UtcNow.ToString("o", ci),
                    wave.ToString(ci),
                    enemyCount.ToString(ci),
                    spawnInterval.ToString("F2", ci),
                    durationSeconds.ToString("F2", ci),
                    damageTaken.ToString(ci),
                    success ? "1" : "0"));
            }
            Debug.Log($"[Bullet Arena] Wave {wave} kaydedildi: {FilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[Bullet Arena] CSV yazılamadı: {ex.Message}");
        }
    }
}
