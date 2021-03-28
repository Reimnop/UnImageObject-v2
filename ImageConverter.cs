using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UnImageObject_v2
{
    public unsafe class ImageConverter : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        protected struct UnsafeColor
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;

            public override string ToString()
            {
                return "#" + R.ToString("X2") + G.ToString("X2") + B.ToString("X2") + A.ToString("X2");
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                UnsafeColor col = (UnsafeColor)obj;

                return R == col.R && G == col.G && B == col.B && A == col.A;
            }
        }

        private Bitmap bitmap;
        private BitmapData data;

        private UnsafeColor* pixels;

        private int width;
        private int height;

        public ImageConverter(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            LoadBitmap(bitmap);
        }

        public void WriteToStringBuilder(StringBuilder builder)
        {
            builder.Append("<line-height=14><cspace=-1>");

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    long pos = width * i + j;

                    if (pos == 0)
                        builder.Append($"<{pixels[pos].ToString()}>█");
                    else
                    {
                        if (pixels[pos - 1].Equals(pixels[pos]))
                            builder.Append($"█");
                        else
                            builder.Append($"<{pixels[pos].ToString()}>█");
                    }
                }
                if (i != height)
                    builder.Append("<br>");
            }
        }

        private void LoadBitmap(Bitmap bitmap)
        {
            width = bitmap.Width;
            height = bitmap.Height;

            data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            pixels = (UnsafeColor*)data.Scan0;
        }

        public void Dispose()
        {
            bitmap.UnlockBits(data);
            bitmap.Dispose();
        }
    }
}
