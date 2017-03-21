using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using wgssSTU;

namespace InkPlatform.UserInterface
{
    public class Layout
    {
        public enum LAYOUT_TYPE { DEFAULT, BOX_LAYOUT, SIGNATURE_LAYOUT };

        protected string _name = "";
        protected List<Element> elementList;
        protected List<ElementButton> buttonList;
        protected List<ElementImage> imageList;
        protected LAYOUT_TYPE _layoutType = LAYOUT_TYPE.DEFAULT;

        /// <summary>
        /// Gets or sets the name of the Layout for identification
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        /// <summary>
        /// Gets the type of the layout.
        /// </summary>
        /// <value>
        /// The type of the layout.
        /// </value>
        public LAYOUT_TYPE LayoutType
        {
            get { return _layoutType; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Layout" /> class.
        /// </summary>
        public Layout(string name)
        {
            _name = name;
        }
        
        /// <summary>
        /// Adds a text element to the layout at the specified location
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public ElementText AddElement(string text, int x, int y)
        {
            if (GetElement(text) != null) return null;

            ElementText txt = new ElementText(text, text, new Point(x, y));
            
            txt.HAlign = StringAlignment.Near;
            txt.VAlign = StringAlignment.Near;
            bool result = AddElement(txt);
            if (result) return txt;
            return null;
        }

        /// <summary>
        /// Adds a text element to the layout at the specified location
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public ElementText AddElement(string name, string text, int x, int y)
        {
            if (GetElement(text) != null) return null;

            ElementText txt = new ElementText(name, text, new Point(x, y));
            
            txt.HAlign = StringAlignment.Near;
            txt.VAlign = StringAlignment.Near;
            bool result = AddElement(txt);
            if (result) return txt;
            return null;
        }

        public ElementText AddElement(string name, string text, int x, int y, int fontSize)
        {
            if (GetElement(text) != null) return null;

            ElementText txt = new ElementText(name, text, new Point(x, y));

            txt.HAlign = StringAlignment.Near;
            txt.VAlign = StringAlignment.Near;
            txt.FontSize = fontSize;
            bool result = AddElement(txt);
            if (result) return txt;
            return null;
        }

        /// <summary>
        /// Adds an element to the layout
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <returns>Boolean specifying whether the operation is successful</returns>
        public bool AddElement(Element element)
        {
            if(elementList == null)
            {
                elementList = new List<Element>();
            }
            else
            {
                //Check to make sure the same name is not added
                foreach(Element e in elementList)
                {
                    if(e.Name == element.Name)
                    {
                        return false;
                    }
                }
            }
            
            elementList.Add(element);
            return true;
        }

        /// <summary>
        /// Removes the element from the layout
        /// </summary>
        /// <param name="element">The element to remove</param>
        /// <returns>Boolean specifying whether the operation is successful</returns>
        public bool RemoveElement(Element element)
        {
            return RemoveElement(element.Name);
        }

        /// <summary>
        /// Searches an element by name, and remove it from the layout
        /// </summary>
        /// <param name="name">The name of the element to remove</param>
        /// <returns>Boolean specifying whether the operation is successful</returns>
        public bool RemoveElement(string name)
        {
            foreach(Element e in elementList)
            {
                if(e.Name == name)
                {
                    elementList.Remove(e);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the element by name
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <returns>The element found, null if unsuccessful</returns>
        public Element GetElement(string name)
        {
            if (elementList == null || elementList.Count == 0) return null;

            foreach (Element e in elementList)
            {
                if (e.Name == name)
                {
                    return e;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Searches for an ElementButton in the layout by name, and fires a click event if the button is found
        /// </summary>
        /// <param name="name">The name of the ElementButton</param>
        /// <returns>Boolean specifying whether the operation is successful</returns>
        public bool ClickButton(string name)
        {
            Element e = GetElement(name);
            if(e.ElementType == Element.ELEMENT_TYPE.BUTTON)
            {
                ElementButton btn = (ElementButton)e;
                btn.PerformClick();
                return true;
            }
            if (e.ElementType == Element.ELEMENT_TYPE.IMAGE)
            {
                ElementImage btn = (ElementImage)e;
                btn.PerformClick();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves the list of all elements in the layout
        /// </summary>
        /// <value>
        /// The element list.
        /// </value>
        public List<Element> ElementList
        {
            get
            {
                return elementList;
            }
        }
        
        /// <summary>
        /// Retrieves the list of all buttons in the layout
        /// </summary>
        /// <value>
        /// The button list.
        /// </value>
        public List<ElementButton> ButtonList
        {
            get
            {
                buttonList = new List<ElementButton>();
                if (elementList != null)
                {
                    foreach (Element e in elementList)
                    {
                        if (e.ElementType == Element.ELEMENT_TYPE.BUTTON)
                        {
                            buttonList.Add((ElementButton)e);
                        }
                    }
                }
                return buttonList;
            }
        }
        
        /// <summary>
        /// Retrieves the list of all images in the layout
        /// </summary>
        /// <value>
        /// The image list.
        /// </value>
        public List<ElementImage> ImageList
        {
            get
            {
                imageList = new List<ElementImage>();
                if(elementList != null)
                {
                    foreach (Element e in elementList)
                    {
                        if (e.ElementType == Element.ELEMENT_TYPE.IMAGE)
                        {
                            imageList.Add((ElementImage)e);
                        }
                    }
                }                
                return imageList;
            }
        }

        public ElementButton GetButtonOnPoint(Point ScreenPoint)
        {
            try
            {
                if (ButtonList != null && ButtonList.Count > 0)
                {
                    for (int i = 0; i < ButtonList.Count; i++)
                    {
                        if (ButtonList[i].Bounds.Contains(ScreenPoint))
                        {
                            return ButtonList[i];
                        }
                    }
                }
            }
            catch (Exception) { }

            return null;
        }

        public ElementImage GetImageOnPoint(Point ScreenPoint)
        {
            try
            {
                if (ImageList != null && ImageList.Count > 0)
                {
                    for (int i = 0; i < ImageList.Count; i++)
                    {
                        if (ImageList[i].Bounds.Contains(ScreenPoint) && ImageList[i].IsButton)
                        {
                            return ImageList[i];
                        }
                    }
                }
            }
            catch (Exception) { }

            return null;
        }

        public virtual Size GetRequiredSize()
        {
            int maxX = 0;
            int maxY = 0;

            foreach(Element ele in ElementList)
            {
                System.Drawing.Rectangle rect = ele.Bounds;
                if(ele.ElementType == Element.ELEMENT_TYPE.TEXT)
                {
                    ElementText txt = (ElementText)ele;
                    rect = txt.CalculateBounds();
                }

                int x = rect.X + rect.Width;
                int y = rect.Y + rect.Height;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }

            return new Size(maxX, maxY);
        }

        public virtual void Render(int width, int height)
        {
            //virtual function to be called before rendering of the bitmap by the 
            //create bitmap function of the layout manager
            //This function is supposed to layout the location and size of the elements 
            //before converting to bitmap


        }
    }
}
