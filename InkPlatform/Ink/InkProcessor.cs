using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Steganography;
using System.Runtime.Serialization.Json;
using System.IO;

namespace InkPlatform.Ink
{
    /// <summary>
    /// Class to deal with converting the ink data received from various devices to different formats, including bitmaps and WILL
    /// </summary>
    public class InkProcessor
    {
        public enum GenerateImageResult
        {
            Successful, SerializeToJsonFail, PictureSizeTooSmallToEmbedData, SteganographyEmbedFail, Error
        }
        
        public static Point ConvertCoordinate(Point point, Size fromDimension, Size toDimension)
        {
            return new Point(
                    point.X * toDimension.Width / fromDimension.Width,
                    point.Y * toDimension.Height / fromDimension.Height
                );
        }
        
        public static List<InkData> Base64Decode(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);
            string json = Encoding.UTF8.GetString(data);
            List<InkData> penData = DeserializeJsonToInkDataList(json);
            return penData;
        }

        public static string Base64Encode(List<InkData> penData)
        {
            string json = SerializeInkDataListToJson(penData);
            byte[] data = Encoding.UTF8.GetBytes(json);
            string base64 = Convert.ToBase64String(data);
            return base64;
        }
        
        public static GenerateImageResult GenerateImageFromContextPenData(out Bitmap bitmap, ContextPenData contextPenData, Pen pen, Color backgroundColor, bool encodeData, bool autoResizeToFitData)
        {
            Size bitmapSize = new Size(contextPenData.PenDevice.ScreenWidth, contextPenData.PenDevice.ScreenHeight);
            Size tabletSize = new Size(contextPenData.PenDevice.TabletWidth, contextPenData.PenDevice.TabletHeight);

            bitmap = new Bitmap(contextPenData.PenDevice.ScreenWidth, contextPenData.PenDevice.ScreenHeight);
            GenerateImageResult result = GenerateImageFromInkData(out bitmap, contextPenData.PenData, bitmapSize, pen, backgroundColor);
            if (!encodeData) return result;

            string json = "";
            try
            {
                json = contextPenData.ToString();
            }
            catch (Exception)
            {
                return GenerateImageResult.SerializeToJsonFail;
            }

            int dataSizeNeeded = SteganographyHelper2.GetPictureSizeNeededToEmbedText(json);
            int pictureSize = SteganographyHelper2.GetPictureSize(bitmap);

            if (autoResizeToFitData)
            {
                float multiplier = 1.0f;
                while (pictureSize < dataSizeNeeded && multiplier <= 5)
                {
                    multiplier = multiplier + 0.5f;
                    Size biggerSize = new Size((int)(bitmapSize.Width * multiplier), (int)(bitmapSize.Height * multiplier));
                    result = GenerateImageFromInkDataResize(out bitmap, contextPenData.PenData, bitmapSize, biggerSize, pen, backgroundColor);
                    pictureSize = SteganographyHelper2.GetPictureSize(bitmap);
                }

            }

            if (pictureSize < dataSizeNeeded)
            {
                return GenerateImageResult.PictureSizeTooSmallToEmbedData;
            }

            try
            {
                Bitmap sbmp = SteganographyHelper.embedText(json, bitmap);
                bitmap = sbmp;
            }
            catch
            {
                return GenerateImageResult.SteganographyEmbedFail;
            }

            return result;
        }

        /// <summary>
        /// Creates a bitmap image from the ink data. This is a primitive method
        /// using simple drawlines between each pen data points and the image size generated
        /// will be the same size as the screen size.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="inkData">The ink data.</param>
        /// <param name="bitmapSize">Size of the bitmap.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <returns>GenerateImageResult</returns>
        public static GenerateImageResult GenerateImageFromInkData(out Bitmap bitmap, List<InkData> inkData, Size bitmapSize, Pen pen, Color backgroundColor)
        {
            SolidBrush brush = null;
            bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);

            try
            { 
                Graphics graphics = Graphics.FromImage(bitmap);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                brush = new SolidBrush(backgroundColor);
                graphics.FillRectangle(brush, 0, 0, bitmapSize.Width, bitmapSize.Height);

                for (int i = 1; i < inkData.Count; i++)
                {
                    PointF p1 = new Point((int)inkData[i - 1].x, (int)inkData[i - 1].y);
                    PointF p2 = new Point((int)inkData[i].x, (int)inkData[i].y);

                    if (inkData[i - 1].contact && inkData[i].contact)
                    {
                        graphics.DrawLine(pen, p1, p2);
                    }

                }
                
            }
            catch (Exception)
            {
                return GenerateImageResult.Error;
            }
            finally
            {
                if (brush != null)
                {
                    brush.Dispose();
                }
            }

            return GenerateImageResult.Successful;
        }

        /// <summary>
        /// Creates a bitmap image from the ink data. This is a primitive method
        /// using simple drawlines between each pen data points and the image size generated
        /// will be the same size as the screen size.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="inkData">The ink data.</param>
        /// <param name="originalSize">Size of the tablet capture.</param>
        /// <param name="newSize">Size of the bitmap.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <returns>GenerateImageResult</returns>
        public static GenerateImageResult GenerateImageFromInkDataResize(out Bitmap bitmap, List<InkData> inkData, Size originalSize, Size newSize, Pen pen, Color backgroundColor)
        {
            SolidBrush brush = null;
            bitmap = new Bitmap(newSize.Width, newSize.Height);

            try
            {
                Graphics graphics = Graphics.FromImage(bitmap);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                brush = new SolidBrush(backgroundColor);
                graphics.FillRectangle(brush, 0, 0, newSize.Width, newSize.Height);

                for (int i = 1; i < inkData.Count; i++)
                {
                    
                    PointF p1 = ConvertCoordinate(
                            new Point((int)inkData[i - 1].x, (int)inkData[i - 1].y),
                            originalSize,
                            newSize
                            );

                    PointF p2 = ConvertCoordinate(
                            new Point((int)inkData[i].x, (int)inkData[i].y),
                            originalSize,
                            newSize
                            );
                    
                    if (inkData[i - 1].contact && inkData[i].contact)
                    {
                        graphics.DrawLine(pen, p1, p2);
                    }

                }

            }
            catch (Exception)
            {
                return GenerateImageResult.Error;
            }
            finally
            {
                if (brush != null)
                {
                    brush.Dispose();
                }
            }

            return GenerateImageResult.Successful;
        }

        /// <summary>
        /// Creates a bitmap image from the ink data. This is a primitive method 
        /// using simple drawlines between each pen data points and the image size generated 
        /// will be the same size as the screen size. 
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="inkData">The ink data.</param>
        /// <param name="bitmapSize">Size of the bitmap.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="encodeData">if set to <c>true</c> [encode data].</param>
        /// <param name="autoResizeToFitData">if set to <c>true</c> [automatic resize to fit data].</param>
        /// <returns>Status of GenerateImageResult</returns>
        public static GenerateImageResult GenerateImageFromInkData(out Bitmap bitmap, List<InkData> inkData, Size bitmapSize, Pen pen, Color backgroundColor, bool encodeData, bool autoResizeToFitData)
        {
            bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            GenerateImageResult result = GenerateImageFromInkData(out bitmap, inkData, bitmapSize, pen, backgroundColor);
            if (!encodeData) return result;

            string json = "";
            try
            {
                json = SerializeInkDataListToJson(inkData);
            }
            catch (Exception)
            {
                return GenerateImageResult.SerializeToJsonFail;
            }

            int dataSizeNeeded = SteganographyHelper2.GetPictureSizeNeededToEmbedText(json);
            int pictureSize = SteganographyHelper2.GetPictureSize(bitmap);

            if (autoResizeToFitData)
            {
                float multiplier = 1.0f;
                while (pictureSize < dataSizeNeeded && multiplier <= 5)
                {
                    multiplier = multiplier + 0.5f;
                    Size biggerSize = new Size((int)(bitmapSize.Width * multiplier), (int)(bitmapSize.Height * multiplier));
                    result = GenerateImageFromInkDataResize(out bitmap, inkData, bitmapSize, biggerSize, pen, backgroundColor);
                    pictureSize = SteganographyHelper2.GetPictureSize(bitmap);
                }

            }

            if (pictureSize < dataSizeNeeded)
            {
                return GenerateImageResult.PictureSizeTooSmallToEmbedData;
            }

            try
            {
                Bitmap sbmp = SteganographyHelper.embedText(json, bitmap);
                bitmap = sbmp;
            }
            catch
            {
                return GenerateImageResult.SteganographyEmbedFail;
            }

            return result;
        }

        /// <summary>
        /// Generates the image from ink data.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <param name="inkData">The ink data.</param>
        /// <param name="tabletCaptureSize">Size of the tablet capture.</param>
        /// <param name="bitmapSize">Size of the bitmap.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="encodeData">if set to <c>true</c> [encode data].</param>
        /// <returns>Status of the GenerateImageResult</returns>
        public static GenerateImageResult GenerateImageFromInkData(out Bitmap bmp, List<InkData> inkData, Size bitmapSize, Pen pen, Color backgroundColor, bool encodeData)
        {
            bmp = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            return GenerateImageFromInkData(out bmp, inkData, bitmapSize, pen, backgroundColor, encodeData, false);
        }

        /// <summary>
        /// Serializes the ink data list to json.
        /// </summary>
        /// <param name="inkData">The ink data to serialize</param>
        /// <returns></returns>
        public static string SerializeInkDataListToJson(List<InkData> inkData)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<InkData>));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, inkData);
                ms.Position = 0;
                StreamReader reader = new StreamReader(ms);
                return reader.ReadToEnd();
            }
        }
        
        public static List<InkData> DeserializeJsonToInkDataList(string json)
        {
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(List<InkData>));
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(ms);
                writer.Write(json);
                writer.Flush();
                ms.Position = 0;
                List<InkData> penData = (List<InkData>)deserializer.ReadObject(ms);
                return penData;
            }
                
        }


    }
}
