using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace InkPlatform.UserInterface
{
    /// <summary>
    /// Represents a line element to include in a layout
    /// </summary>
    /// <seealso cref="WacomSign.Element" />
    public class ElementLine : Element
    {
        /// <summary>
        /// The pen to draw the line, default is black color, 1 pixel width.
        /// </summary>
        public Pen LinePen;

        public Point Start;
        public Point End;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementLine"/> class. Default with pen width of 1 pixel, black.
        /// </summary>
        /// <param name="name">The name.</param>
        public ElementLine(string name) : base(name, ELEMENT_TYPE.LINE)
        {
            LinePen = new Pen(Color.Black);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementLine"/> class, specifying the start and end points
        /// </summary>
        /// <param name="name">The unique identifier</param>
        /// <param name="start">The origin of the line</param>
        /// <param name="end">The end position of the line</param>
        public ElementLine(string name, Point start, Point end) : base(name, ELEMENT_TYPE.LINE)
        {
            LinePen = new Pen(Color.Black);
            Location = start;
            Start = start;
            End = end;
        }

        public ElementLine(string name, Point start, Point end, Color color) : base(name, ELEMENT_TYPE.LINE)
        {
            LinePen = new Pen(color);
            Location = start;
            Start = start;
            End = end;
        }

        public ElementLine(string name, Point start, Point end, float width) : base(name, ELEMENT_TYPE.LINE)
        {
            LinePen = new Pen(Color.Black, width);
            Location = start;
            Start = start;
            End = end;
        }

        public ElementLine(string name, Point start, Point end, Color color, float width) : base(name, ELEMENT_TYPE.LINE)
        {
            LinePen = new Pen(color, width);
            Location = start;
            Start = start;
            End = end;
        }
    }
}
