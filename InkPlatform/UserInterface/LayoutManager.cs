using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Net;

namespace InkPlatform.UserInterface
{
    
    public class LayoutManager
    {
        public static Bitmap CreateBitmap(Layout layout, int width, int height, bool supportColor)
        {
            // Size the bitmap to the size of the LCD screen.
            // This application uses the same bitmap for both the screen and client (window).
            // However, at high DPI, this bitmap will be stretch and it would be better to 
            // create individual bitmaps for screen and client at native resolutions.

            layout.Render(width, height);

            Bitmap _bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            {
                Graphics gfx = Graphics.FromImage(_bitmap);
                gfx.Clear(Color.White);

                if (supportColor)
                {
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                }
                else
                {
                    // Anti-aliasing should be turned off for monochrome devices.
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                }

                if (layout.ElementList != null && layout.ElementList.Count > 0)
                {
                    for (int i = 0; i < layout.ElementList.Count; i++)
                    {
                        switch (layout.ElementList[i].ElementType)
                        {
                            case Element.ELEMENT_TYPE.TEXT:
                                drawString(gfx, (ElementText)layout.ElementList[i]);
                                break;
                            case Element.ELEMENT_TYPE.BUTTON:
                                drawButton(gfx, (ElementButton)layout.ElementList[i], supportColor);
                                break;
                            case Element.ELEMENT_TYPE.LINE:
                                drawLine(gfx, (ElementLine)layout.ElementList[i]);
                                break;
                            case Element.ELEMENT_TYPE.IMAGE:
                                drawImage(gfx, (ElementImage)layout.ElementList[i]);
                                break;
                            default:
                                break;
                        }
                    }
                }

                gfx.Dispose();
                //font.Dispose();
            }

            return _bitmap;
        }

        private static void drawLine(Graphics gfx, ElementLine line)
        {
            gfx.DrawLine(line.LinePen, line.Start, line.End);
        }

        private static void drawString(Graphics gfx, ElementText txt)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = txt.HAlign;
            sf.LineAlignment = txt.VAlign;

            Size intendedSize = new Size((int)txt.Bounds.Width, (int)txt.Bounds.Height);
            Size calSize = TextRenderer.MeasureText(txt.Text, txt.TextFont, intendedSize);

            if (intendedSize.Width == 0 && intendedSize.Height == 0)
            {
                intendedSize = calSize;
                txt.Bounds = new Rectangle(txt.Bounds.X, txt.Bounds.Y, intendedSize.Width, intendedSize.Height);
            }

            if (txt.AutoResizeText)
            {
                if (intendedSize.Width != 0 && intendedSize.Height != 0)
                {
                    while (calSize.Width > intendedSize.Width || calSize.Height > intendedSize.Height)
                    {
                        txt.FontSize = txt.FontSize - 1;
                        calSize = TextRenderer.MeasureText(txt.Text, txt.TextFont, intendedSize);
                        txt.Bounds = new Rectangle(txt.Bounds.X, txt.Bounds.Y, calSize.Width, calSize.Height);
                    }
                    
                }
            }
            
            //gfx.DrawRectangle(Pens.Black, Rectangle.Truncate(txt.Bounds));
            gfx.DrawString(txt.Text, txt.TextFont, txt.Brush, txt.Bounds, sf);

        }

        private static void drawImage(Graphics gfx, ElementImage img)
        {
            gfx.DrawImageUnscaled(
                img.Picture,
                new Rectangle(
                    (int)img.Location.X,
                    (int)img.Location.Y,
                    (int)img.Size.Width,
                    (int)img.Size.Height
                )
            );

        }

        private static void drawButton(Graphics gfx, ElementButton btn, bool supportColor)
        {
            // Uses pixels for units as DPI won't be accurate for tablet LCD.
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            Size intendedSize = new Size((int)btn.Bounds.Width, (int)btn.Bounds.Height);
            Size calSize = TextRenderer.MeasureText(btn.Text, btn.TextFont, intendedSize);

            if (intendedSize.Width == 0 && intendedSize.Height == 0)
            {
                intendedSize = calSize;
                btn.Bounds = new Rectangle(btn.Bounds.X, btn.Bounds.Y, intendedSize.Width, intendedSize.Height);
            }

            if (btn.AutoResizeText)
            {
                if (intendedSize.Width != 0 && intendedSize.Height != 0)
                {
                    while (calSize.Width > intendedSize.Width || calSize.Height > intendedSize.Height)
                    {
                        btn.FontSize = btn.FontSize - 1;
                        calSize = TextRenderer.MeasureText(btn.Text, btn.TextFont, intendedSize);
                        btn.Bounds = new Rectangle(btn.Bounds.X, btn.Bounds.Y, calSize.Width, calSize.Height);
                    }
                    
                }
            }

            if (supportColor)
            {
                gfx.FillRectangle(btn.FillBrush, btn.Bounds);
            }

            gfx.DrawRectangle(btn.BorderPen, Rectangle.Truncate(btn.Bounds));
            gfx.DrawString(btn.Text, btn.TextFont, btn.TextBrush, btn.Bounds, sf);
        }

        public static List<Layout> ReadLayoutFiles(string[] layoutFiles)
        {
            List<Layout> layouts = new List<Layout>();

            for (int j = 0; j < layoutFiles.Length; j++)
            {
                Layout layout = ReadLayoutFile(layoutFiles[j]);
                layouts.Add(layout);
            }

            return layouts;
        }

        public static List<Layout> ReadLayoutFiles(List<string> layoutFiles)
        {
            List<Layout> layouts = new List<Layout>();
            
            for (int j = 0; j < layoutFiles.Count; j++)
            {
                Layout layout = ReadLayoutFile(layoutFiles[j]);
                layouts.Add(layout);
            }

            return layouts;
        }

        public static Layout ReadLayoutFile(string path)
        {
            string currLayout = "";
            string line = "";
            string readJson = "";

            if (File.Exists(path))
            {
                //This code sets the current directory for the images in the layout file
                //so that they are automatically assumed to be in the same folder as the layout file
                string OriginalCurrDir = Environment.CurrentDirectory;

                Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                try
                {
                    Environment.CurrentDirectory = Path.GetDirectoryName(path);
                    currLayout = Path.GetFileName(path);
                }
                catch (Exception)
                {
                    currLayout = path;
                }
                //--------------------------------------------------------------
                
                try
                {
                    StreamReader sr = new StreamReader(currLayout);
                    while ((line = sr.ReadLine()) != null)
                    {
                        readJson = readJson + line + System.Environment.NewLine;
                    }
                    sr.Close();
                }
                catch (Exception ex)
                {
                    Exception ex2 = new Exception("Error reading file " + path, ex);
                    throw ex2;
                }
            }
            else
            {
                //try if it is a url
                try
                {
                    Uri layoutUri = new Uri(path);
                    WebClient webClient = new WebClient();
                    readJson = webClient.DownloadString(layoutUri);
                }
                catch (Exception ex)
                {
                    Exception ex2 = new Exception("Error reading uri " + path, ex);
                    throw ex2;
                }
            }
            
            try
            {
                Layout layout = (Layout)JSONSerializer.DeserializeLayout(readJson);

                return layout;
            }
            catch(Exception ex)
            {
                //Environment.CurrentDirectory = OriginalCurrDir;
                //Obselete (setting the current directory, as above comments)
                Exception ex2 = new Exception("Format for JSON is incorrect - " + path, ex);
                throw ex2;
            }
            
        }

    }
}
