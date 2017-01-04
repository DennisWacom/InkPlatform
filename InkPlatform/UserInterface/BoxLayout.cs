using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace InkPlatform.UserInterface
{
    /// <summary>
    /// A 3 by 3 layout to easily add elements without calculating the location and size of the element.
    /// User can just specify the position of the element, and the location and size will be automatically calculated.
    /// When more than 1 elements are added in the same position, the bounds of all the elements in the same
    /// position will be recalculated to share the space equally. 
    /// Setting the flow allows you to control whether the next element will be added to the right or down.
    /// </summary>
    /// <seealso cref="InkPlatform.UserInterface" />
    public class BoxLayout : Layout
    {
        
        /// <summary>
        /// Enum specifying where should the element appear
        /// </summary>
        public enum POSITION
        {
            TOP_LEFT, TOP_CENTRE, TOP_RIGHT,
            MIDDLE_LEFT, MIDDLE_CENTRE, MIDDLE_RIGHT,
            BOTTOM_LEFT, BOTTOM_CENTRE, BOTTOM_RIGHT,
            UNKNOWN
        }

        /// <summary>
        /// Specify whether elements will share the position space vertically or horizontally
        /// </summary>
        public enum FLOW
        {
            /// <summary>
            /// When more than 1 elements are added in the same position, the new element will share the space horizontally to the right
            /// </summary>
            RIGHT,
            /// <summary>
            /// When more than 1 elements are added in the same position, the new element will share the space vertically down
            /// </summary>
            DOWN
        }

        private int _topRatio = 1;
        private int _middleRatio = 1;
        private int _bottomRatio = 1;
        private int _leftRatio = 1;
        private int _centreRatio = 1;
        private int _rightRatio = 1;

        protected FLOW _flow = FLOW.RIGHT;
        /// <summary>
        /// Gets or sets the flow.
        /// </summary>
        /// <value>
        /// The flow.
        /// </value>
        public FLOW Flow
        {
            get
            {
                return _flow;
            }
            set
            {
                _flow = value;
            }
        }

        protected int _spacing = 0;
        /// <summary>
        /// Gets or sets the spacing (0 to 10).
        /// When 2 or more elements share the same position, the spacing will leave a gap between the elements.
        /// The spacing is automatically calculated as a percentage of the width of each element.
        /// When spacing = 1, the space is 10% of the width of the element. The width of the element will be adjusted accordingly too
        /// <example>
        /// When 2 buttons are added in the same position - MIDDLE_CENTRE, Flow = RIGHT, Spacing = 2
        /// Then the width of MIDDLE_CENTRE will be divided into 22 parts. Each button will occupy 10/22 of the MIDDLE_CENTRE width,
        /// and the spacing will be 2/22.
        /// </example>
        /// <example>
        /// When 3 buttons are added in the same position - MIDDLE_CENTRE, Flow = Down, Spacing = 1
        /// Then the height of MIDDLE_CENTRE will be divided into 32 parts. Each button will occupy 10/32 of the MIDDLE_CENTRE height,
        /// and the spacing will be 1/32
        /// </example>
        /// </summary>
        /// <value>
        /// The spacing.
        /// </value>
        public int Spacing
        {
            get { return _spacing; }
            set
            {
                if(value < 0)
                {
                    _spacing = 0;
                }else if(value > 10)
                {
                    _spacing = 10;
                }
                else
                {
                    _spacing = value;
                }
            }
        }

        private const string OUTER_LEFT = "OUTER_LEFT_LINE";
        private const string OUTER_TOP = "OUTER_TOP_LINE";
        private const string OUTER_RIGHT = "OUTER_RIGHT_LINE";
        private const string OUTER_BOTTOM = "OUTER_BOTTOM_LINE";
        private const string INNER_VERTICAL_LEFT = "INNER_VERTICAL_LEFT_LINE";
        private const string INNER_VERTICAL_RIGHT = "INNER_VERTICAL_RIGHT_LINE";
        private const string INNER_HORIZONTAL_TOP = "INNER_HORIZONTAL_TOP_LINE";
        private const string INNER_HORIZONTAL_BOTTOM = "INNER_HORIZONTAL_BOTTOM_LINE";
        private List<ElementLine> _gridLines = new List<ElementLine>();

        protected bool _showGrid;
        /// <summary>
        /// Gets or sets a value indicating whether [show grid].
        /// When true, black gridlines will appear on the screen for debugging purposes.
        /// This will add ElementLines to the layout, but doesnt specify the position. The following keywords
        /// will be used as the names of the ElementLines.
        /// <list type="bullet">
        ///     <item><description>OUTER_LEFT_LINE</description></item>
        ///     <item><description>OUTER_TOP_LINE</description></item>
        ///     <item><description>OUTER_RIGHT_LINE</description></item>
        ///     <item><description>OUTER_BOTTOM_LINE</description></item>
        ///     <item><description>INNER_VERTICAL_LEFT_LINE</description></item>
        ///     <item><description>INNER_VERTICAL_RIGHT_LINE</description></item>
        ///     <item><description>INNER_HORIZONTAL_TOP_LINE</description></item>
        ///     <item><description>INNER_HORIZONTAL_BOTTOM_LINE</description></item>
        /// </list>>
        /// If any of the names are already in used, the method will fail, and the grid lines will be removed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show grid]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGrid
        {
            get
            {
                return _showGrid;
            }
            set
            {
                _showGrid = value;
            }
        }

        private void removeGridLines()
        {
            RemoveElement(OUTER_LEFT);
            RemoveElement(OUTER_TOP);
            RemoveElement(OUTER_RIGHT);
            RemoveElement(OUTER_BOTTOM);
            RemoveElement(INNER_VERTICAL_LEFT);
            RemoveElement(INNER_VERTICAL_RIGHT);
            RemoveElement(INNER_HORIZONTAL_TOP);
            RemoveElement(INNER_HORIZONTAL_BOTTOM);
        }

        private bool createGridLines(int _width, int _height)
        {
            bool result = false;

            ElementLine line1 = new ElementLine(OUTER_LEFT, new Point(0,0), new Point(0, _height));
            result = AddElement(line1);
            ElementLine line2 = new ElementLine(OUTER_TOP, new Point(0, 0), new Point(_width - 1, 0));
            result = AddElement(line2);
            ElementLine line3 = new ElementLine(OUTER_RIGHT, new Point(_width - 1, 0), new Point(_width - 1, _height - 1));
            result = AddElement(line3);
            ElementLine line4 = new ElementLine(OUTER_BOTTOM, new Point(0, _height - 1), new Point(_width - 1, _height - 1));
            result = AddElement(line4);

            Point topCentre = getCellLocationForPosition(POSITION.TOP_CENTRE, _width, _height);
            Point topRight = getCellLocationForPosition(POSITION.TOP_RIGHT, _width, _height);
            Point middleLeft = getCellLocationForPosition(POSITION.MIDDLE_LEFT, _width, _height);
            Point bottomLeft = getCellLocationForPosition(POSITION.BOTTOM_LEFT, _width, _height);

            ElementLine line5 = new ElementLine(INNER_VERTICAL_LEFT, topCentre, new Point(topCentre.X, _height));
            result = AddElement(line5);
            ElementLine line6 = new ElementLine(INNER_VERTICAL_RIGHT, topRight, new Point(topRight.X, _height));
            result = AddElement(line6);
            ElementLine line7 = new ElementLine(INNER_HORIZONTAL_TOP, middleLeft, new Point(_width, middleLeft.Y));
            result = AddElement(line7);
            ElementLine line8 = new ElementLine(INNER_HORIZONTAL_BOTTOM, bottomLeft, new Point(_width, bottomLeft.Y));
            result = AddElement(line8);

            return result;
        }

        private Dictionary<POSITION, List<Element>> boxDict;

        public BoxLayout(string name) : base (name)
        {
            _layoutType = LAYOUT_TYPE.BOX_LAYOUT;

            boxDict = new Dictionary<POSITION, List<Element>>();
            boxDict.Add(POSITION.TOP_LEFT, new List<Element>());
            boxDict.Add(POSITION.TOP_CENTRE, new List<Element>());
            boxDict.Add(POSITION.TOP_RIGHT, new List<Element>());
            boxDict.Add(POSITION.MIDDLE_LEFT, new List<Element>());
            boxDict.Add(POSITION.MIDDLE_CENTRE, new List<Element>());
            boxDict.Add(POSITION.MIDDLE_RIGHT, new List<Element>());
            boxDict.Add(POSITION.BOTTOM_LEFT, new List<Element>());
            boxDict.Add(POSITION.BOTTOM_CENTRE, new List<Element>());
            boxDict.Add(POSITION.BOTTOM_RIGHT, new List<Element>());
        }
        
        /// <summary>
        /// Gets the element list for position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public List<Element> GetElementListForPosition(POSITION position)
        {
            return boxDict[position];
        }

        protected Point getCellLocationForPosition(POSITION position, int width, int height)
        {
            Point result = new Point(0, 0);

            if (position == POSITION.TOP_LEFT || position == POSITION.MIDDLE_LEFT || position == POSITION.BOTTOM_LEFT)
            {
                //Do nothing, X=0;
            }
            else if (position == POSITION.TOP_CENTRE || position == POSITION.MIDDLE_CENTRE || position == POSITION.BOTTOM_CENTRE)
            {
                result.X = getCellWidthForPosition(POSITION.TOP_LEFT, width) - 1;
            }
            else if (position == POSITION.TOP_RIGHT || position == POSITION.MIDDLE_RIGHT || position == POSITION.BOTTOM_RIGHT)
            {
                result.X = getCellWidthForPosition(POSITION.TOP_LEFT, width) + getCellWidthForPosition(POSITION.TOP_CENTRE, width) - 1;
            }

            if (position == POSITION.TOP_LEFT || position == POSITION.TOP_CENTRE || position == POSITION.TOP_RIGHT)
            {
                //Do nothing, y = 0;
            }
            else if (position == POSITION.MIDDLE_LEFT || position == POSITION.MIDDLE_CENTRE || position == POSITION.MIDDLE_RIGHT)
            {
                result.Y = getCellHeightForPosition(POSITION.TOP_LEFT, height) - 1;
            }
            else if (position == POSITION.BOTTOM_LEFT || position == POSITION.BOTTOM_CENTRE || position == POSITION.BOTTOM_RIGHT)
            {
                result.Y = getCellHeightForPosition(POSITION.TOP_LEFT, height) + getCellHeightForPosition(POSITION.MIDDLE_LEFT, height) - 1;
            }

            return result;

        }

        protected int getCellWidthForPosition(POSITION position, int _width)
        {
            float totalRatio = _leftRatio + _centreRatio + _rightRatio;
            float ratio = 1F;

            if (position == POSITION.TOP_LEFT || position == POSITION.MIDDLE_LEFT || position == POSITION.BOTTOM_LEFT)
            {
                ratio = _leftRatio / totalRatio;
            }

            if(position == POSITION.TOP_CENTRE || position == POSITION.MIDDLE_CENTRE || position == POSITION.BOTTOM_CENTRE)
            {
                ratio = _centreRatio / totalRatio;
            }

            if(position == POSITION.TOP_RIGHT || position == POSITION.MIDDLE_RIGHT || position == POSITION.BOTTOM_RIGHT)
            {
                ratio = _rightRatio / totalRatio;
            }

            return (int)(ratio * _width);
        }

        protected int getCellHeightForPosition(POSITION position, int _height)
        {
            float totalRatio = _topRatio + _middleRatio + _bottomRatio;
            float ratio = 1F;

            if(position == POSITION.TOP_LEFT || position == POSITION.TOP_CENTRE || position == POSITION.TOP_RIGHT)
            {
                ratio = _topRatio / totalRatio;
            }

            if(position == POSITION.MIDDLE_LEFT || position == POSITION.MIDDLE_CENTRE || position == POSITION.MIDDLE_RIGHT)
            {
                ratio = _middleRatio / totalRatio;
            }

            if(position == POSITION.BOTTOM_LEFT || position == POSITION.BOTTOM_CENTRE || position == POSITION.BOTTOM_RIGHT)
            {
                ratio = _bottomRatio / totalRatio;
            }

            return (int)(ratio * _height);
        }

        protected Rectangle getBoundsForPosition(POSITION position, int width, int height)
        {
            int cellWidth = getCellWidthForPosition(position, width);
            int cellHeight = getCellHeightForPosition(position, height);
            Point cellLocation = getCellLocationForPosition(position, width, height);

            return new Rectangle(
                    cellLocation.X,
                    cellLocation.Y,
                    cellWidth,
                    cellHeight
                );
            
        }
        
        public void Render(POSITION position, int width, int height)
        {
            List<Element> eList = boxDict[position];

            if (eList.Count == 0) return;

            Rectangle rect = getBoundsForPosition(position, width, height);
            int assignedLengthForCell = 0;
            int elementLength = 0;
            int spacingLength = 0;
            if (_flow == FLOW.RIGHT)
            {
                assignedLengthForCell = getCellWidthForPosition(position, width);
            }
            else
            {
                assignedLengthForCell = getCellHeightForPosition(position, height);
            }

            int parts = (eList.Count * 10) + ((eList.Count - 1) * _spacing);
            elementLength = (int)((float)assignedLengthForCell / (float)parts * 10);
            spacingLength = (int)((float)assignedLengthForCell / (float)parts * _spacing);

            for(int i=0; i<eList.Count; i++)
            {
                if(_flow == FLOW.RIGHT)
                {
                    eList[i].Location = new Point(
                            rect.X + (i * elementLength) + (i * spacingLength),
                            rect.Y
                        );

                    eList[i].Size = new Size(
                            elementLength, 
                            rect.Height
                        );
                    
                }
                else
                {
                    eList[i].Location = new Point(
                            rect.X,
                            rect.Y + (i * elementLength) + (i * spacingLength)
                        );
                    eList[i].Size = new Size(
                            rect.Width, elementLength
                        );
                }
            }
            
        }

        /// <summary>
        /// Get the layout to redraw the gridl lines only if ShowGrid is true.
        /// </summary>
        /// <returns></returns>
        public bool RedrawGridLines(int width, int height)
        {
            if (_showGrid == false) return false;

            if (_gridLines.Count == 0)
            {
                bool result = createGridLines(width, height);
                if (!result)
                {
                    removeGridLines();
                    _showGrid = false;
                    return false;
                }
                return true;
            }
            return false;
        }

        public override void Render(int width, int height)
        {
            Render(POSITION.TOP_LEFT, width, height);
            Render(POSITION.TOP_CENTRE, width, height);
            Render(POSITION.TOP_RIGHT, width, height);
            Render(POSITION.MIDDLE_LEFT, width, height);
            Render(POSITION.MIDDLE_CENTRE, width, height);
            Render(POSITION.MIDDLE_RIGHT, width, height);
            Render(POSITION.BOTTOM_LEFT, width, height);
            Render(POSITION.BOTTOM_CENTRE, width, height);
            Render(POSITION.BOTTOM_RIGHT, width, height);

            if (ShowGrid)
            {
                createGridLines(width, height);
            }
            else
            {
                removeGridLines();
            }
        }
        
        protected POSITION getPositionForElement(Element element)
        {
            if (boxDict[POSITION.TOP_LEFT].Contains(element)) return POSITION.TOP_LEFT;
            if (boxDict[POSITION.TOP_CENTRE].Contains(element)) return POSITION.TOP_CENTRE;
            if (boxDict[POSITION.TOP_RIGHT].Contains(element)) return POSITION.TOP_RIGHT;
            if (boxDict[POSITION.MIDDLE_LEFT].Contains(element)) return POSITION.MIDDLE_LEFT;
            if (boxDict[POSITION.MIDDLE_CENTRE].Contains(element)) return POSITION.MIDDLE_CENTRE;
            if (boxDict[POSITION.MIDDLE_RIGHT].Contains(element)) return POSITION.MIDDLE_RIGHT;
            if (boxDict[POSITION.BOTTOM_LEFT].Contains(element)) return POSITION.BOTTOM_LEFT;
            if (boxDict[POSITION.BOTTOM_CENTRE].Contains(element)) return POSITION.BOTTOM_CENTRE;
            if (boxDict[POSITION.BOTTOM_RIGHT].Contains(element)) return POSITION.BOTTOM_RIGHT;

            return POSITION.UNKNOWN;
        }

        /// <summary>
        /// Creates an ElementText and adds it to a specific position of the box layout. When more than 1 element
        /// is added in the same position, the space will be shared equally among the elements.
        /// </summary>
        /// <param name="text">The name and text of the ElementText</param>
        /// <param name="position">The position to appear</param>
        /// <returns>The ElementText created, null if unsuccessful</returns>
        public ElementText AddElement(string text, POSITION position)
        {
            return AddElement(text, position, StringAlignment.Center);
        }

        /// <summary>
        /// Creates an ElementText and adds it to a specific position of the box layout. When more than 1 element
        /// is added in the same position, the space will be shared equally among the elements.
        /// </summary>
        /// <param name="text">The name and text of the ElementText</param>
        /// <param name="position">The position to appear</param>
        /// <param name="hAlign">The horizontal alignment of the text.</param>
        /// <returns>
        /// The ElementText created, null if unsuccessful
        /// </returns>
        public ElementText AddElement(string text, POSITION position, StringAlignment hAlign)
        {
            string name = generateSafeName(text);

            ElementText ele = new ElementText(name, text);
            ele.HAlign = hAlign;
            bool result = AddElement(ele, position);
            if (result) return ele;
            return null;
        }
        
        /// <summary>
        /// Adds an Element to a specific position of the box layout. When more than 1 element
        /// is added in the same position, the space will be shared equally among the elements.
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <param name="position">The position of the element</param>
        /// <returns>Boolean whether the operation is successful</returns>
        public bool AddElement(Element element, POSITION position)
        {
            if (GetElement(element.Name) != null) return false;

            boxDict[position].Add(element);
            return AddElement(element);
        }

        /// <summary>
        /// Removes the element from the layout. This method overrides the parent method to remove the element
        /// from the position also, and recalculate the position of the elements.
        /// </summary>
        /// <param name="element">The element to remove</param>
        /// <returns>Boolean specifying whether the operation is successful</returns>
        public new bool RemoveElement(Element element)
        {
            return RemoveElement(element.Name);
        }

        /// <summary>
        /// Removes the element from the layout by the name of the element. 
        /// This method overrides the parent method to remove the element 
        /// from the position also, and recalculate the position of the elements.
        /// </summary>
        /// <param name="name">The name of the element to remove</param>
        /// <returns>Boolean specifying whether the operation is successful</returns>
        public new bool RemoveElement(string name)
        {
            if (elementList == null) return false;

            foreach (Element e in elementList)
            {
                if (e.Name == name)
                {
                    elementList.Remove(e);
                    POSITION targetPosition = getPositionForElement(e);
                    if(targetPosition != POSITION.UNKNOWN)
                    {
                        boxDict[targetPosition].Remove(e);
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the ratio between the left, centre, and right position widths. Default is 1:1:1.
        /// <example>
        /// 1:1:1 - the box will be divided equally horizontally
        /// </example>
        /// <example>
        /// 1:3:1 - the centre will be 3 times wider than the sides
        /// </example>
        /// </summary>
        /// <param name="left">The left ratio</param>
        /// <param name="centre">The centre ratio</param>
        /// <param name="right">The right ratio</param>
        public void setHorizontalRatio(int left, int centre, int right)
        {
            _leftRatio = left;
            _centreRatio = centre;
            _rightRatio = right;
            
        }

        /// <summary>
        /// Sets the ratio between the top, middle, and bottom position heights. Default is 1:1:1
        /// <example>
        /// 1:1:1 - the box will be divided equally vertically
        /// </example>
        /// <example>
        /// 1:2:1 - the middle will be 2 times wider than the top and bottom
        /// </example>
        /// </summary>
        /// <param name="top">The top ratio</param>
        /// <param name="middle">The middle ratio</param>
        /// <param name="bottom">The bottom ratio</param>
        public void setVerticalRatio(int top, int middle, int bottom)
        {
            _topRatio = top;
            _middleRatio = middle;
            _bottomRatio = bottom;
            
        }

        private string generateSafeName(string name)
        {
            if (GetElement(name) == null) return name;
            
            int count = 1;
            string safeName = name + count.ToString();
            while (GetElement(safeName) != null)
            {
                count++;
                safeName = name + count.ToString();
            }
            return safeName;
        }
    }
}
