using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DummyRunner;

public class ObstacleManager
{
    private const float SpawnX = 240f;
    private const float SpawnGap = 120f;
    private const float BaseSpeed = 90f;
    private const float SpeedStep = 30f;

    private readonly ObstacleDef[] _normals;
    private readonly ObstacleDef _announcer;
    private readonly ObstacleDef _speedUp;
    private readonly float _groundY;
    private readonly Random _rng = new Random();

    private readonly List<Obstacle> _obstacles = new List<Obstacle>();
    private int _spawnedCount;

    public float Speed { get; private set; } = BaseSpeed;
    public int Hops { get; private set; }
    public int Medals => Hops / 15;

    public ObstacleManager(ObstacleDef[] normals, ObstacleDef announcer, ObstacleDef speedUp, float groundY)
    {
        _normals = normals;
        _announcer = announcer;
        _speedUp = speedUp;
        _groundY = groundY;
    }

    public void Reset()
    {
        _obstacles.Clear();
        _spawnedCount = 0;
        Hops = 0;
        Speed = BaseSpeed;
        SpawnNext();
    }

    public void IncreaseSpeed() => Speed += SpeedStep;

    private void SpawnNext()
    {
        int n = _spawnedCount + 1;

        Obstacle obstacle;
        if (n == 37 || n == 97)
            obstacle = new SpeedUpObstacle(_speedUp.Texture, _speedUp.Sound, SpawnX, _groundY);
        else if (n == 1 || n == 38 || n == 98)
            obstacle = new AnnouncerObstacle(_announcer.Texture, _announcer.Sound, SpawnX, _groundY);
        else
        {
            ObstacleDef d = _normals[_rng.Next(_normals.Length)];
            obstacle = new NormalObstacle(d.Texture, d.Sound, SpawnX, _groundY);
        }

        _obstacles.Add(obstacle);
        _spawnedCount++;
    }

    // engelleri ilerletir, carparsa true doner
    public bool Update(GameTime gameTime, Player player)
    {
        float dx = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (Obstacle o in _obstacles)
        {
            o.Move(dx);

            if (!o.Cleared && o.X + o.Width < Player.X)
            {
                o.Cleared = true;
                Hops++;
                o.OnCleared(this);
            }
        }

        _obstacles.RemoveAll(o => o.IsOffScreen);

        if (_obstacles.Count == 0 || _obstacles[_obstacles.Count - 1].X <= SpawnX - SpawnGap)
            SpawnNext();

        Rectangle playerBox = player.Bounds;
        foreach (Obstacle o in _obstacles)
            if (o.Bounds.Intersects(playerBox))
                return true;

        return false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Obstacle o in _obstacles)
            o.Draw(spriteBatch);
    }
}
