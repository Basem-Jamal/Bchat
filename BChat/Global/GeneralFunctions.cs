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
    }
}
