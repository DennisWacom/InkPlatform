using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace InkPlatform.UserInterface
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="WacomSign.Element" />
    public class ElementImage : Element
    {

        private bool _isButton = false;

        /// <summary>
        /// Gets a value indicating whether this image is a button. This is determined by if the Click event is assigned
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is button; otherwise, <c>false</c>.
        /// </value>
        public bool IsButton
        {
            get { return _isButton; }
        }
        
        private EventHandler _click;

        /// <summary>
        /// The EventHandler to perform when the image is clicked
        /// </summary>
        public EventHandler Click
        {
            get { return _click; }
            set
            {
                _click = value;
                _isButton = true;
            }
        }

        public Image Picture;

        /// <summary>
        /// The identifier of the EventHandler Click, if the event is 1 of the 3 default actions in ISignPadDisplay
        /// </summary>
        public DEFAULT_ACTIONS Action = DEFAULT_ACTIONS.None;
        
        public string NextScreenName = "";
        
        public ElementImage(string name, string filename) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = Image.FromFile(filename);
            Location = new Point(0, 0);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
        }

        public ElementImage(string name, string filename, int x, int y) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = Image.FromFile(filename);
            Location = new Point(x, y);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
        }

        public ElementImage(string name, string filename, int x, int y, DEFAULT_ACTIONS action) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = Image.FromFile(filename);
            Location = new Point(x, y);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
            Action = action;
        }

        public ElementImage(string name, string filename, int x, int y, string nextScreenName) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = Image.FromFile(filename);
            Location = new Point(x, y);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
            Action = DEFAULT_ACTIONS.Done;
            NextScreenName = nextScreenName;
        }

        public ElementImage(string name, Image picture) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = picture;
            Location = new Point(0, 0);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
        }

        public ElementImage(string name, Image picture, int x, int y) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = picture;
            Location = new Point(x, y);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
        }

        public ElementImage(string name, Image picture, int x, int y, DEFAULT_ACTIONS action) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = picture;
            Location = new Point(x, y);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
            Action = action;
        }

        public ElementImage(string name, Image picture, int x, int y, string nextScreenName) : base(name, ELEMENT_TYPE.IMAGE)
        {
            _click = null;
            Picture = picture;
            Location = new Point(x, y);
            Size = new Size(Picture.Size.Width, Picture.Size.Height);
            Action = DEFAULT_ACTIONS.Done;
            NextScreenName = nextScreenName;
        }
        
        /// <summary>
        /// Fires the EventHandler specified in the Click property of the image
        /// </summary>
        public void PerformClick()
        {
            if(Click != null)
            {
                Click(this, null);
            }
   
        }
    }
}
