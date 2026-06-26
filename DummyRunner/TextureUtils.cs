using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DummyRunner;

// bir texture'in dolu piksellerini saran dikdortgeni bulur (sonucu saklar)
public static class TextureUtils
{
    private static readonly Dictionary<Texture2D, Rectangle> _cache = new Dictionary<Texture2D, Rectangle>();

    public static Rectangle ContentBounds(Texture2D texture)
    {
        if (_cache.TryGetValue(texture, out Rectangle cached))
            return cached;

        Color[] data = new Color[texture.Width * texture.Height];
        texture.GetData(data);

        int minX = texture.Width, minY = texture.Height, maxX = -1, maxY = -1;
        for (int y = 0; y < texture.Height; y++)
        {
            for (int x = 0; x < texture.Width; x++)
            {
                if (data[y * texture.Width + x].A != 0)
                {
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }
        }

        Rectangle result = (maxX < 0)
            ? new Rectangle(0, 0, texture.Width, texture.Height)
            : new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);

        _cache[texture] = result;
        return result;
    }
}
