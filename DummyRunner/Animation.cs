using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DummyRunner;

// sprite sheet'i karelere bolup oynatir
public class Animation
{
    private readonly Texture2D _texture;
    private readonly Rectangle[] _frames;
    private readonly double _frameTime;
    private readonly bool _loop;

    private int _current;
    private double _timer;

    public bool Finished { get; private set; }

    public Animation(Texture2D texture, int frameWidth, int frameHeight,
                     int startFrame, int frameCount, double frameTime, bool loop)
    {
        _texture = texture;
        _frameTime = frameTime;
        _loop = loop;

        _frames = new Rectangle[frameCount];
        int columns = texture.Width / frameWidth;

        for (int i = 0; i < frameCount; i++)
        {
            int index = startFrame + i;
            int col = index % columns;
            int row = index / columns;
            _frames[i] = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
        }
    }

    public void Reset()
    {
        _current = 0;
        _timer = 0;
        Finished = false;
    }

    public void Update(GameTime gameTime)
    {
        if (Finished) return;

        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer >= _frameTime)
        {
            _timer -= _frameTime;
            _current++;
            if (_current >= _frames.Length)
            {
                if (_loop)
                {
                    _current = 0;
                }
                else
                {
                    _current = _frames.Length - 1;
                    Finished = true;
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(_texture, position, _frames[_current], Color.White);
    }
}
