using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DummyRunner;

// 0-9 rakam sheet'inden sayi cizer (esit genislikte hucreler)
public class NumberRenderer
{
    private readonly Texture2D _sheet;
    private readonly int _cellWidth;
    private readonly int _cellHeight;
    private readonly float _scale;

    public NumberRenderer(Texture2D sheet, float scale = 0.75f)
    {
        _sheet = sheet;
        _cellWidth = sheet.Width / 10;
        _cellHeight = sheet.Height;
        _scale = scale;
    }

    public float DigitHeight => _cellHeight * _scale;

    public float MeasureWidth(int number)
        => number.ToString().Length * _cellWidth * _scale;

    public void Draw(SpriteBatch sb, int number, Vector2 pos, Color color)
    {
        string s = number.ToString();
        float x = (float)Math.Round(pos.X);
        float y = (float)Math.Round(pos.Y);
        foreach (char c in s)
        {
            int d = c - '0';
            var src = new Rectangle(d * _cellWidth, 0, _cellWidth, _cellHeight);
            sb.Draw(_sheet, new Vector2(x, y), src, color, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            x += _cellWidth * _scale;
        }
    }

    public void DrawRight(SpriteBatch sb, int number, Vector2 rightTop, Color color)
        => Draw(sb, number, new Vector2(rightTop.X - MeasureWidth(number), rightTop.Y), color);

    public void DrawCentered(SpriteBatch sb, int number, Vector2 center, Color color)
        => Draw(sb, number, new Vector2(center.X - MeasureWidth(number) / 2f, center.Y - DigitHeight / 2f), color);
}
