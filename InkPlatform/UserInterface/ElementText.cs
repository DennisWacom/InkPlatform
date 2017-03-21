using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace InkPlatform.UserInterface
{
    /// <summary>
    /// Represents a text string to add on the layout
    /// </summary>
    /// <seealso cref="WacomSign.Element" />
    public class ElementText : Element
    {
        public static int DEFAULT_FONT_SIZE = 40;

        /// <summary>
        /// The text to display
        /// </summary>
        public string Text;

        /// <summary>
        /// The brush to draw the text, default is Brushes.Black.
        /// </summary>
        public Brush Brush;

        /// <summary>
        /// Vertical alignment of the text within its bounds
        /// </summary>
        public StringAlignment VAlign;

        /// <summary>
        /// Horizontal alignment of the text within its bounds
        /// </summary>
        public StringAlignment HAlign;

        /// <summary>
        /// The font of the text
        /// </summary>
        public Font TextFont = new Font(FontFamily.GenericSansSerif, DEFAULT_FONT_SIZE,GraphicsUnit.Pixel);

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public float FontSize
        {
            get
            {
                if(TextFont != null)
                {
                    return TextFont.Size;
                }
                else
                {
                    return DEFAULT_FONT_SIZE;
                }
            }
            set
            {
                TextFont = new Font(TextFont.FontFamily, value, GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// When set to true, the font size will be adjusted automatically to fit the whole length of the
        /// text within the bounds
        /// </summary>
        public bool AutoResizeText = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementText"/> class.
        /// </summary>
        /// <param name="name">The unique identifier of the element</param>
        /// <param name="text">The text to display</param>
        public ElementText(string name, string text) : base(name, ELEMENT_TYPE.TEXT)
        {
            Text = text;
            Brush = Brushes.Black;
            VAlign = StringAlignment.Center;
            HAlign = StringAlignment.Center;
            Location = new Point(0, 0);
        }

        public ElementText(string name, string text, Point location) : base (name, ELEMENT_TYPE.TEXT, location)
        {
            Text = text;
            Brush = Brushes.Black;
            VAlign = StringAlignment.Center;
            HAlign = StringAlignment.Center;
        }

        public ElementText(string name, string text, Point location, Size size) : base(name, ELEMENT_TYPE.TEXT, location, size)
        {
            Text = text;
            Brush = Brushes.Black;
            VAlign = StringAlignment.Center;
            HAlign = StringAlignment.Center;
        }

        public Rectangle CalculateBounds()
        {
            Size intendedSize = new Size((int)this.Bounds.Width, (int)this.Bounds.Height);
            Size calSize = TextRenderer.MeasureText(this.Text, this.TextFont, intendedSize);

            Rectangle result = this.Bounds;

            if (intendedSize.Width == 0 && intendedSize.Height == 0)
            {
                intendedSize = calSize;
                result = new Rectangle(this.Bounds.X, this.Bounds.Y, intendedSize.Width, intendedSize.Height);
            }

            if (this.AutoResizeText)
            {
                while (calSize.Width > intendedSize.Width || calSize.Height > intendedSize.Height)
                {
                    this.FontSize = this.FontSize - 1;
                    calSize = TextRenderer.MeasureText(this.Text, this.TextFont, intendedSize);
                    result = new Rectangle(this.Bounds.X, this.Bounds.Y, calSize.Width, calSize.Height);
                }
            }

            return result;
        }

    }
}
