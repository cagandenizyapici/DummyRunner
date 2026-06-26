using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DummyRunner;

public class Player
{
    public const float X = 40f;
    public const float GroundY = 132f;

    private const float Gravity = 0.25f;
    private const float JumpStrength = -5.5f;

    private const int FrameSize = 16;

    public enum State { Running, Jumping, Dead }
    public State Current { get; private set; } = State.Running;

    public bool DeathFinished => Current == State.Dead && _death.Finished;

    private readonly Animation _run;
    private readonly Animation _jump;
    private readonly Animation _death;
    private readonly Animation _ball;
    private readonly SoundEffect _jumpSound;

    private float _footY = GroundY;
    private float _velocityY;
    private bool _grounded = true;

    public Player(Texture2D playerSheet, Texture2D ballSheet, SoundEffect jumpSound)
    {
        _jumpSound = jumpSound;
        // 14 kare: 0-4 kosma, 5-7 ziplama, 8-13 olme
        _run   = new Animation(playerSheet, FrameSize, FrameSize, startFrame: 0, frameCount: 5, frameTime: 0.10, loop: true);
        _jump  = new Animation(playerSheet, FrameSize, FrameSize, startFrame: 5, frameCount: 3, frameTime: 0.12, loop: false);
        _death = new Animation(playerSheet, FrameSize, FrameSize, startFrame: 8, frameCount: 6, frameTime: 0.12, loop: false);
        _ball  = new Animation(ballSheet,   FrameSize, FrameSize, startFrame: 0, frameCount: 4, frameTime: 0.08, loop: true);
    }

    public Rectangle Bounds
    {
        get
        {
            const int inset = 3;
            int left = (int)X + inset;
            int top = (int)(_footY - FrameSize * 2) + inset;
            return new Rectangle(left, top, FrameSize - inset * 2, FrameSize * 2 - inset * 2);
        }
    }

    public void Jump()
    {
        if (_grounded && Current != State.Dead)
        {
            _velocityY = JumpStrength;
            _grounded = false;
            Current = State.Jumping;
            _jump.Reset();
            _jumpSound?.Play();
        }
    }

    public void Die()
    {
        if (Current != State.Dead)
        {
            Current = State.Dead;
            _death.Reset();
        }
    }

    public void Reset()
    {
        Current = State.Running;
        _footY = GroundY;
        _velocityY = 0;
        _grounded = true;
        _run.Reset();
    }

    public void Update(GameTime gameTime)
    {
        if (Current != State.Dead)
        {
            _velocityY += Gravity;
            _footY += _velocityY;

            if (_footY >= GroundY)
            {
                _footY = GroundY;
                _velocityY = 0;
                if (!_grounded)
                {
                    _grounded = true;
                    Current = State.Running;
                }
            }

            _ball.Update(gameTime);
        }

        switch (Current)
        {
            case State.Running: _run.Update(gameTime); break;
            case State.Jumping: _jump.Update(gameTime); break;
            case State.Dead:    _death.Update(gameTime); break;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _ball.Draw(spriteBatch, new Vector2(X, _footY - FrameSize));

        Animation body;
        switch (Current)
        {
            case State.Jumping: body = _jump; break;
            case State.Dead:    body = _death; break;
            default:            body = _run; break;
        }
        body.Draw(spriteBatch, new Vector2(X, _footY - FrameSize * 2));
    }
}
