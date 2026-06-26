using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DummyRunner;

public class Game1 : Game
{
    private const int VirtualWidth = 240;
    private const int VirtualHeight = 160;
    private const int Scale = 4;
    private const int GroundTop = VirtualHeight - 32;

    private const int PanelHeight = 18;
    private static readonly Vector2 MedalsPos = new Vector2(6, 6);
    private static readonly Vector2 ScorePos  = new Vector2(92, 6);
    private static readonly Vector2 TopPos    = new Vector2(178, 6);
    private const int NumberCenterX = 42;

    private static readonly Vector2 GameOverScorePos = new Vector2(60, 104);
    private static readonly Vector2 GameOverTopPos    = new Vector2(180, 104);

    private const float BgScrollFactor = 0.3f;

    private const float BirdY = 32f;
    private const float BirdSpeed = 130f;
    private const int BirdSize = 16;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Matrix _scaleMatrix;

    private enum GameState { Menu, Playing, Dying, GameOver }
    private GameState _state = GameState.Menu;
    private KeyboardState _prevKeyboard;

    private Texture2D _background;
    private Texture2D _foreground;
    private Texture2D _menuScreen;
    private Texture2D _gameoverScreen;
    private Texture2D _birdTexture;
    private Texture2D _medalsPanel, _scorePanel, _topPanel;
    private NumberRenderer _numbers;
    private Player _player;
    private ObstacleManager _obstacles;
    private HighScore _highScore;
    private SoundEffect _crashSound;
    private SoundEffect _medalSound;
    private SoundEffectInstance _music;

    private float _groundScrollX;
    private float _bgScrollX;
    private int _prevMedals;
    private int _prevHops;
    private int _startBest;
    private bool _recordBeaten;
    private bool _birdActive;
    private float _birdX;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = VirtualWidth * Scale;
        _graphics.PreferredBackBufferHeight = VirtualHeight * Scale;
    }

    protected override void Initialize()
    {
        _scaleMatrix = Matrix.CreateScale(Scale);
        Window.Title = "DummyRunner";
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _background     = Content.Load<Texture2D>("wr_background");
        _foreground     = Content.Load<Texture2D>("wr_foreground");
        _menuScreen     = Content.Load<Texture2D>("wr_menu");
        _gameoverScreen = Content.Load<Texture2D>("wr_gameover");
        _birdTexture    = Content.Load<Texture2D>("wr_bird");

        _medalsPanel = Content.Load<Texture2D>("wr_medals");
        _scorePanel  = Content.Load<Texture2D>("wr_score");
        _topPanel    = Content.Load<Texture2D>("wr_topscore");
        _numbers = new NumberRenderer(Content.Load<Texture2D>("wr_digits"), 0.75f);

        SoundEffect jumpSound = Content.Load<SoundEffect>("wr_jump");
        _crashSound = Content.Load<SoundEffect>("wr_crash");
        _medalSound = Content.Load<SoundEffect>("wr_medal");

        _music = Content.Load<SoundEffect>("wr_music").CreateInstance();
        _music.IsLooped = true;
        _music.Volume = 0.5f;

        Texture2D playerSheet = Content.Load<Texture2D>("wr_playeranimations");
        Texture2D ballSheet   = Content.Load<Texture2D>("wr_ballanimation");
        _player = new Player(playerSheet, ballSheet, jumpSound);

        ObstacleDef[] normals = new ObstacleDef[]
        {
            new ObstacleDef(Content.Load<Texture2D>("wr_rat"),       Content.Load<SoundEffect>("wr_snd_rat")),
            new ObstacleDef(Content.Load<Texture2D>("wr_rocks"),     Content.Load<SoundEffect>("wr_snd_rocks")),
            new ObstacleDef(Content.Load<Texture2D>("wr_cone"),      Content.Load<SoundEffect>("wr_snd_cone")),
            new ObstacleDef(Content.Load<Texture2D>("wr_thrashcan"), Content.Load<SoundEffect>("wr_snd_trashcan")),
            new ObstacleDef(Content.Load<Texture2D>("wr_taxi"),      Content.Load<SoundEffect>("wr_snd_taxi")),
        };
        ObstacleDef police   = new ObstacleDef(Content.Load<Texture2D>("wr_police1"),  Content.Load<SoundEffect>("wr_snd_police"));
        ObstacleDef musician = new ObstacleDef(Content.Load<Texture2D>("wr_musician"), Content.Load<SoundEffect>("wr_snd_musician"));
        _obstacles = new ObstacleManager(normals, police, musician, Player.GroundY);

        _highScore = new HighScore();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            keyboard.IsKeyDown(Keys.Escape))
            Exit();

        switch (_state)
        {
            case GameState.Menu:
                if (WasKeyPressed(keyboard, Keys.Space))
                    StartGame();
                break;

            case GameState.Playing:
                UpdatePlaying(gameTime, keyboard);
                break;

            case GameState.Dying:
                _player.Update(gameTime);
                if (_player.DeathFinished)
                    _state = GameState.GameOver;
                break;

            case GameState.GameOver:
                if (WasKeyPressed(keyboard, Keys.R))
                    _state = GameState.Menu;
                break;
        }

        _prevKeyboard = keyboard;
        base.Update(gameTime);
    }

    private void StartGame()
    {
        _player.Reset();
        _obstacles.Reset();
        _prevMedals = 0;
        _prevHops = 0;
        _startBest = _highScore.Best;
        _recordBeaten = false;
        _birdActive = false;
        _music.Play();
        _state = GameState.Playing;
    }

    private void UpdatePlaying(GameTime gameTime, KeyboardState keyboard)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (WasKeyPressed(keyboard, Keys.Space))
            _player.Jump();

        _player.Update(gameTime);
        bool hit = _obstacles.Update(gameTime, _player);

        _groundScrollX -= _obstacles.Speed * dt;
        if (_groundScrollX <= -VirtualWidth) _groundScrollX += VirtualWidth;
        _bgScrollX -= _obstacles.Speed * BgScrollFactor * dt;
        if (_bgScrollX <= -VirtualWidth) _bgScrollX += VirtualWidth;

        if (_obstacles.Medals > _prevMedals)
        {
            _prevMedals = _obstacles.Medals;
            _medalSound?.Play();
        }

        // rekoru gecince ve her 100 hop'ta kus ucur
        int h = _obstacles.Hops;
        if (h != _prevHops)
        {
            _prevHops = h;
            bool beatRecord = !_recordBeaten && _startBest > 0 && h > _startBest;
            if (beatRecord) _recordBeaten = true;
            if (beatRecord || (h > 0 && h % 100 == 0))
                TriggerBird();
        }
        UpdateBird(dt);

        if (hit)
        {
            _player.Die();
            _music.Stop();
            _crashSound?.Play();
            _highScore.TrySet(_obstacles.Hops);
            _state = GameState.Dying;
        }
    }

    private void TriggerBird()
    {
        _birdActive = true;
        _birdX = VirtualWidth;
    }

    private void UpdateBird(float dt)
    {
        if (!_birdActive) return;
        _birdX -= BirdSpeed * dt;
        if (_birdX < -BirdSize) _birdActive = false;
    }

    private bool WasKeyPressed(KeyboardState current, Keys key)
        => current.IsKeyDown(key) && _prevKeyboard.IsKeyUp(key);

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _scaleMatrix);

        switch (_state)
        {
            case GameState.Menu:
                _spriteBatch.Draw(_menuScreen, Vector2.Zero, Color.White);
                break;

            case GameState.Playing:
            case GameState.Dying:
                DrawScene();
                DrawHud();
                break;

            case GameState.GameOver:
                _spriteBatch.Draw(_gameoverScreen, Vector2.Zero, Color.White);
                _numbers.DrawCentered(_spriteBatch, _obstacles.Hops, GameOverScorePos, Color.White);
                _numbers.DrawCentered(_spriteBatch, _highScore.Best, GameOverTopPos, Color.White);
                break;
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawScene()
    {
        _spriteBatch.Draw(_background, new Vector2(_bgScrollX, 0), Color.White);
        _spriteBatch.Draw(_background, new Vector2(_bgScrollX + VirtualWidth, 0), Color.White);
        _spriteBatch.Draw(_foreground, new Vector2(_groundScrollX, GroundTop), Color.White);
        _spriteBatch.Draw(_foreground, new Vector2(_groundScrollX + VirtualWidth, GroundTop), Color.White);

        if (_birdActive)
            _spriteBatch.Draw(_birdTexture, new Vector2(_birdX, BirdY), Color.White);

        _obstacles.Draw(_spriteBatch);
        _player.Draw(_spriteBatch);
    }

    private void DrawHud()
    {
        DrawPanel(_medalsPanel, MedalsPos, _obstacles.Medals);
        DrawPanel(_scorePanel,  ScorePos,  _obstacles.Hops);
        DrawPanel(_topPanel,    TopPos,    _highScore.Best);
    }

    private void DrawPanel(Texture2D panel, Vector2 pos, int value)
    {
        _spriteBatch.Draw(panel, pos, Color.White);
        var center = new Vector2(pos.X + NumberCenterX, pos.Y + PanelHeight / 2f);
        _numbers.DrawCentered(_spriteBatch, value, center, Color.White);
    }
}
