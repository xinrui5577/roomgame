using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.ImgPress
{
    public class BitmapData
    {
        public int height;

        private readonly Color[] pixels;
        public int width;

        /**
    * Pull all of our pixels off the texture (Unity stuff isn't thread safe, and this is faster)
    */

        public BitmapData(Color[] _pixels, int _width, int _height)
        {
            height = _height;
            width = _width;

            pixels = _pixels;
        }

        public BitmapData(Texture2D texture)
        {
            height = texture.height;
            width = texture.width;

            pixels = texture.GetPixels();
        }

        /**
    * Mimic the flash function
    */

        public Color getPixelColor(int x, int y)
        {
            if (x >= width)
                x = width - 1;

            if (y >= height)
                y = height - 1;

            if (x < 0)
                x = 0;

            if (y < 0)
                y = 0;

            return pixels[y*width + x];
        }
    }
}