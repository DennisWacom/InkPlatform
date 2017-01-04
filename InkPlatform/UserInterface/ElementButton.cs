using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace InkPlatform.UserInterface
{
    /// <summary>
    /// Represents a button element to include in a layout
    /// </summary>
    /// <seealso cref="WacomSign.Element" />
    public class ElementButton : Element
    {
        public static int DEFAULT_BUTTON_FONT_SIZE = 40;

        /// <summary>
        /// The text to display on the button
        /// </summary>
        public string Text;
        /// <summary>
        /// The brush of the text. Default is Brushes.Black.
        /// </summary>
        public Brush TextBrush = Brushes.Black;
        /// <summary>
        /// The brush of the filling of the button. Default is Brushes.LightGray.
        /// </summary>
        public Brush FillBrush = Brushes.LightGray;
        /// <summary>
        /// The pen to use to draw the border. Default is Pens.Black
        /// </summary>
        public Pen BorderPen = new Pen(Color.Black);
        /// <summary>
        /// The font to draw the text. Default fontsize is half the height of the button, black, GenericSansSerif.
        /// When AutoResizeText is true (default), the font size will be adjusted accordingly to fit the 
        /// whole length of text inside the button
        /// </summary>
        public Font TextFont = new Font(FontFamily.GenericSansSerif, DEFAULT_BUTTON_FONT_SIZE);

        /// <summary>
        /// Gets or sets the size of the font. Will adjust the TextFont accordingly
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public float FontSize
        {
            get
            {
                return TextFont.Size;
            }
            set
            {
                TextFont = new Font(TextFont.FontFamily, value, GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// When true, the fontsize of the text will be adjusted automatically to fit the whole length
        /// of the text into the button.
        /// </summary>
        public bool AutoResizeText = true;

        /// <summary>
        /// The EventHandler to perform when the button is clicked
        /// </summary>
        public EventHandler Click;

        /// <summary>
        /// The identifier of the EventHandler Click, if the event is 1 of the 3 default actions in ISignPadDisplay
        /// </summary>
        public DEFAULT_ACTIONS Action = DEFAULT_ACTIONS.None;
        
        public string NextScreenName = "";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementButton"/> class.
        /// </summary>
        /// <param name="name">The unique identifier of the element in the layout</param>
        /// <param name="text">The text to display on the button</param>
        public ElementButton(string name, string text) : base(name, ELEMENT_TYPE.BUTTON)
        {
            Text = text;
            Click = new EventHandler(DoNothing);
        }

        public ElementButton(string name, string text, DEFAULT_ACTIONS action) : base(name, ELEMENT_TYPE.BUTTON)
        {
            Text = text;
            Action = action;
        }

        public ElementButton(string name, string text, string nextScreenName) : base(name, ELEMENT_TYPE.BUTTON)
        {
            Text = text;
            Action = DEFAULT_ACTIONS.Done;
            NextScreenName = nextScreenName;
        }

        public ElementButton(string name, string text, Point location) : base(name, ELEMENT_TYPE.BUTTON, location)
        {
            Text = text;
            Click = new EventHandler(DoNothing);
        }

        public ElementButton(string name, string text, Point location, DEFAULT_ACTIONS action) : base(name, ELEMENT_TYPE.BUTTON, location)
        {
            Text = text;
            Action = action;
        }

        public ElementButton(string name, string text, Point location, string nextScreenName) : base(name, ELEMENT_TYPE.BUTTON, location)
        {
            Text = text;
            Action = DEFAULT_ACTIONS.Done;
            NextScreenName = nextScreenName;
        }

        public ElementButton(string name, string text, Point location, Size size) : base(name, ELEMENT_TYPE.BUTTON, location, size)
        {
            Text = text;
            Click = new EventHandler(DoNothing);
        }

        public ElementButton(string name, string text, Point location, Size size, DEFAULT_ACTIONS action) : base(name, ELEMENT_TYPE.BUTTON, location, size)
        {
            Text = text;
            Action = action;
        }

        public ElementButton(string name, string text, Point location, Size size, string nextScreenName) : base(name, ELEMENT_TYPE.BUTTON, location, size)
        {
            Text = text;
            Action = DEFAULT_ACTIONS.Done;
            NextScreenName = nextScreenName;
        }

        private void DoNothing(object sender, EventArgs e)
        {
            //this is here so that there won't be error when the user calls the Perform click method
            //when the click eventhandler is not defined.
        }

        /// <summary>
        /// Fires the EventHandler specified in the Click property of the button
        /// </summary>
        public void PerformClick()
        {
            Click(this, null);
        }
        
    }

}
