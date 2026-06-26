using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DummyRunner;

public class ObstacleDef
{
    public Texture2D Texture { get; }
    public SoundEffect Sound { get; }

    public ObstacleDef(Texture2D texture, SoundEffect sound)
    {
        Texture = texture;
        Sound = sound;
    }
}

public abstract class Obstacle
{
    protected readonly Texture2D Texture;
    private readonly SoundEffect _sound;
    private readonly Rectangle _content;
    public float X;
    public readonly float Y;
    public bool Cleared;

    public int Width => Texture.Width;

    protected Obstacle(Texture2D texture, SoundEffect sound, float spawnX, float groundY)
    {
        Texture = texture;
        _sound = sound;
        _content = TextureUtils.ContentBounds(texture);
        X = spawnX;
        Y = groundY - (_content.Y + _content.Height);
    }

    public void Move(float dx) => X -= dx;

    public bool IsOffScreen => X + Width < 0;

    public Rectangle Bounds
    {
        get
        {
            const int inset = 1;
            return new Rectangle(
                (int)X + _content.X + inset,
                (int)Y + _content.Y + inset,
                _content.Width - inset * 2,
                _content.Height - inset * 2);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
        => spriteBatch.Draw(Texture, new Vector2(X, Y), Color.White);

    public virtual void OnCleared(ObstacleManager manager) => _sound?.Play();
}

public class NormalObstacle : Obstacle
{
    public NormalObstacle(Texture2D texture, SoundEffect sound, float spawnX, float groundY)
        : base(texture, sound, spawnX, groundY) { }
}

// zorluk basinda cikan engel
public class AnnouncerObstacle : Obstacle
{
    public AnnouncerObstacle(Texture2D texture, SoundEffect sound, float spawnX, float groundY)
        : base(texture, sound, spawnX, groundY) { }
}

// asilinca hizi artiran engel
public class SpeedUpObstacle : Obstacle
{
    public SpeedUpObstacle(Texture2D texture, SoundEffect sound, float spawnX, float groundY)
        : base(texture, sound, spawnX, groundY) { }

    public override void OnCleared(ObstacleManager manager)
    {
        base.OnCleared(manager);
        manager.IncreaseSpeed();
    }
}
