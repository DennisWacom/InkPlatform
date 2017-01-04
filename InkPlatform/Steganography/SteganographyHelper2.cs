using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Steganography
{
    class SteganographyHelper2 : SteganographyHelper
    {
        /// <summary>
        /// Gets the picture size needed to embed text steganographically
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static int GetPictureSizeNeededToEmbedText(string text)
        {
            int result = text.Length * 8;
            return result;
        }

        /// <summary>
        /// Gets the size of the picture.
        /// </summary>
        /// <param name="bmp">The bitmap to measure the size</param>
        /// <returns>size of bitmap (width x height)</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int GetPictureSize(Bitmap bmp)
        {
            if (bmp == null) throw new ArgumentNullException();
            return bmp.Width * bmp.Height;
        }
        
    }
}
