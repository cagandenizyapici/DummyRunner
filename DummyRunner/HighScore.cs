using System;
using System.IO;
using System.Text.Json;

namespace DummyRunner;

// en yuksek skoru dosyada saklar
public class HighScore
{
    private class Data { public int Best { get; set; } }

    private readonly string _path;
    public int Best { get; private set; }

    public HighScore()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DummyRunner");
        _path = Path.Combine(dir, "highscore.json");
        Load();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(_path))
            {
                string json = File.ReadAllText(_path);
                Data d = JsonSerializer.Deserialize<Data>(json);
                Best = d?.Best ?? 0;
            }
        }
        catch
        {
            Best = 0;
        }
    }

    public bool TrySet(int score)
    {
        if (score <= Best) return false;
        Best = score;
        Save();
        return true;
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            string json = JsonSerializer.Serialize(new Data { Best = Best });
            File.WriteAllText(_path, json);
        }
        catch
        {
        }
    }
}
