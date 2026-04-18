using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Global
{
    public class GeneralFunctions
    {
        public GeneralFunctions()
        {

        }

        public static byte[] ResizeImage(string path, int width, int height)
        {
            using (var original = Image.FromFile(path))
            using (var resized = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(resized))
            using (var ms = new MemoryStream())
            {
                // 1. ارسم الصورة الأصلية على resized بالحجم الجديد
                graphics.DrawImage(original, 0, 0, width, height);

                // 2. احفظ resized في ms كـ PNG
                resized.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
                // 3. رجّع ms.ToArray()
            }

          
        }
        public static string ImageToBase64(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static Image Base64ToImage(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            byte[] bytes = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }
        public static Image LoadImageSafe(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return null;

                if (!System.IO.File.Exists(path))
                    return null;

                using (var img = Image.FromFile(path))
                {
                    return (Image)img.Clone();
                }
            }
            catch
            {
                return null;
            }
        }




        public static string ColorToHex(Color c, bool withAlpha = false)
        {
            return withAlpha
                ? $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}"
                : $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Invalid hex color");

            hex = hex.Replace("#", "");

            // RGB (6 خانات)
            if (hex.Length == 6)
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);

                return Color.FromArgb(r, g, b);
            }
            // ARGB (8 خانات)
            else if (hex.Length == 8)
            {
                int a = Convert.ToInt32(hex.Substring(0, 2), 16);
                int r = Convert.ToInt32(hex.Substring(2, 2), 16);
                int g = Convert.ToInt32(hex.Substring(4, 2), 16);
                int b = Convert.ToInt32(hex.Substring(6, 2), 16);

                return Color.FromArgb(a, r, g, b);
            }

            throw new ArgumentException("Hex color must be 6 or 8 characters long.");
        }
    }
}
