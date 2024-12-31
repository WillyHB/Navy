using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System;

namespace Navy
{
    public static class Base64
    {
        public static Texture2D ConvertBase64ToTexture(GraphicsDevice graphicsDevice, string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            Texture2D texture;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                texture = Texture2D.FromStream(graphicsDevice, ms);
            }

            return texture;
        }

        
        public static string ConvertTextureToBase64(Texture2D texture)
        {
            Image image;

            using (MemoryStream mem = new MemoryStream())
            {
                texture.SaveAsPng(mem, texture.Width, texture.Height);

                image = Image.FromStream(mem);
            }
            
           
            string base64 = ConvertImageToBase64(image);

            image.Dispose();

            return base64;

        }
        



        public static string ConvertImageToBase64(Image image)
        {
            string base64;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                base64 = Convert.ToBase64String(ms.ToArray());
            }

            return base64;
        }

        public static Image ConvertBase64ToImage(string image64Bit)
        {
            byte[] imageBytes = Convert.FromBase64String(image64Bit);
            return new Bitmap(new MemoryStream(imageBytes));
        }
    }
}
