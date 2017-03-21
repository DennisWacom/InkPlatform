using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace InkPlatform.UserInterface
{
    /// <summary>
    /// Generic elements (e.g. lines, text, buttons) to add into a layout
    /// </summary>
    public abstract class Element
    {

        /// <summary>
        /// Enum type specifying the element type
        /// </summary>
        public enum ELEMENT_TYPE { TEXT, BUTTON, LINE, IMAGE }

        /// <summary>
        /// Enum type specifying the default actions applicable for the buttons/images
        /// </summary>
        public enum DEFAULT_ACTIONS { None, Done, Refresh, Cancel };

        protected ELEMENT_TYPE _elementType;
        protected string _name;
        protected Size _size;
        protected Point _location;

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public ELEMENT_TYPE ElementType
        {
            get { return _elementType; }
        }
        
        /// <summary>
        /// The Name is the unique identifier of the elements in a layout.
        /// No 2 elements can have the same name in the same layout
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
        }
        
        public Point Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public Size Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Location, Size);
            }
            set
            {
                _location = new Point(value.X, value.Y);
                _size = new Size(value.Width, value.Height);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="name">Unique identifier of the element</param>
        /// <param name="elementType">Type of the element.</param>
        public Element(string name, ELEMENT_TYPE elementType)
        {
            _name = name;
            _elementType = elementType;
        }

        public Element(string name, ELEMENT_TYPE elementType, Point location)
        {
            _name = name;
            _elementType = elementType;
            _location = location;
        }

        public Element(string name, ELEMENT_TYPE elementType, Point location, Size size)
        {
            _name = name;
            _elementType = elementType;
            _location = location;
            _size = size;
        }

        public virtual void ResizeToNewDimension(Size originalDimension, Size newDimension)
        {
            Point newLocation = new Point(
                    (int)((float)_location.X * ((float)newDimension.Width / (float)originalDimension.Width)),
                    (int)((float)_location.Y * ((float)newDimension.Height / (float)originalDimension.Height))
                );
            _location = newLocation;

            Size newSize = new Size(
                    (int)((float)_size.Width * ((float)newDimension.Width / (float)originalDimension.Width)),
                    (int)((float)_size.Height * ((float)newDimension.Height / (float)originalDimension.Height))
                );
            _size = newSize;
        }

    }
}
