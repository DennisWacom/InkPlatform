using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace InkPlatform.UserInterface
{
    /// <summary>
    /// A standard layout to display to capture signature, showing the name and reason of signing, together 
    /// with the timestamp. 3 buttons (OK, Clear, Cancel) is displayed at the bottom of the screen.
    /// If the name and reason is not specified, the screen will show only the 3 buttons.
    /// </summary>
    /// <seealso cref="WacomSign.Layout" />
    public class SignatureLayout : Layout
    {
        
        protected ElementButton btnOk;
        protected ElementButton btnCancel;
        protected ElementButton btnClear;
        
        /// <summary>
        /// The text to display on the Ok button
        /// </summary>
        public string OkText = strings.OK_TEXT;

        /// <summary>
        /// The text to display on the clear button
        /// </summary>
        public string ClearText = strings.CLEAR_TEXT;

        /// <summary>
        /// The text to display on the cancel button
        /// </summary>
        public string CancelText = strings.CANCEL_TEXT;

        /// <summary>
        /// The text to display as the signer, below the signature line
        /// </summary>
        public string Who = "";

        /// <summary>
        /// The text to display as the reason of signing, on the top left corner of the screen
        /// </summary>
        public string Why = "";
        
        /// <summary>
        /// Gets the ok button.
        /// </summary>
        /// <value>
        /// The ok button.
        /// </value>
        public ElementButton OkButton
        {
            get
            {
                return btnOk;   
            }
        }

        /// <summary>
        /// Gets the clear button.
        /// </summary>
        /// <value>
        /// The clear button.
        /// </value>
        public ElementButton ClearButton
        {
            get { return btnClear; }
        }

        /// <summary>
        /// Gets the cancel button.
        /// </summary>
        /// <value>
        /// The cancel button.
        /// </value>
        public ElementButton CancelButton
        {
            get { return btnCancel; }
        }

        
        public SignatureLayout(string name) : base (name)
        {
            _layoutType = LAYOUT_TYPE.SIGNATURE_LAYOUT;
        }
        
        public SignatureLayout(string name, string who, string why) : base(name)
        {
            _layoutType = LAYOUT_TYPE.SIGNATURE_LAYOUT;
            Who = who;
            Why = why;
            
        }
        
        public SignatureLayout(string name, string okText, string clearText, string cancelText) : base (name)
        {
            _layoutType = LAYOUT_TYPE.SIGNATURE_LAYOUT;
            OkText = okText;
            ClearText = clearText;
            CancelText = cancelText;
            
        }
        
        public SignatureLayout(string name, string okText, string clearText, string cancelText, string who, string why): base (name)
        {
            _layoutType = LAYOUT_TYPE.SIGNATURE_LAYOUT;
            
            OkText = okText;
            ClearText = clearText;
            CancelText = cancelText;

            Who = who;
            Why = why;
            
        }

        /// <summary>
        /// Calculates the layout of the buttons and text
        /// </summary>
        public override void Render(int width, int height)
        {
            Size _screenSize = new Size(width, height);
            
            btnOk = new ElementButton("btnOk", OkText);
            btnCancel = new ElementButton("btnCancel", CancelText);
            btnClear = new ElementButton("btnClear", ClearText);

            ElementLine dottedLine = new ElementLine("dottedLine");
            dottedLine.LinePen.DashStyle = DashStyle.Dot;

            ElementText txtWho = new ElementText("txtWho", Who);
            ElementText txtWhy = new ElementText("txtWhy", Why);
            ElementText txtTime = new ElementText("txtTime", DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToLongDateString());

            float aspectRatio = (float)width / (float)height;

            if (aspectRatio < 2.5)
            {
                // Place the buttons across the bottom of the screen.

                int w2 = _screenSize.Width / 3;
                int w3 = _screenSize.Width / 3;
                int w1 = _screenSize.Width - w2 - w3;
                int y = _screenSize.Height * 8 / 9;
                int h = _screenSize.Height - y;

                //btnOk.Bounds = new Rectangle(0, y, w1, h);
                btnOk.Location = new Point(0, y);
                btnOk.Size = new Size(w1, h);

                //btnClear.Bounds = new Rectangle(w1, y, w2, h);
                btnClear.Location = new Point(w1, y);
                btnClear.Size = new Size(w2, h);

                //btnCancel.Bounds = new Rectangle(w1 + w2, y, w3, h);
                btnCancel.Location = new Point(w1 + w2, y);
                btnCancel.Size = new Size(w3, h);

                if(Who != "" || Why != "")
                {
                    int lw = _screenSize.Width * 5 / 7;
                    int lx = (_screenSize.Width - lw) / 2;
                    int ly = _screenSize.Height * 6 / 9;
                    //int lh = 0;
                    //dottedLine.Bounds = new Rectangle(lx, ly, lw, lh);
                    dottedLine.Start = new Point(lx, ly);
                    dottedLine.End = new Point(lx + lw, ly);

                    if (Why != "")
                    {
                        txtWhy.HAlign = StringAlignment.Near;
                        txtWhy.VAlign = StringAlignment.Center;
                        //txtWhy.Bounds = new Rectangle(0, 0, _screenSize.Width, h);
                        txtWhy.Location = new Point(0, 0);
                        txtWhy.Size = new Size(_screenSize.Width, h);
                    }

                    if(Who != "")
                    {
                        txtWho.HAlign = StringAlignment.Near;
                        txtWho.VAlign = StringAlignment.Far;
                        //txtWho.Bounds = new Rectangle(lx, ly, lw, h);
                        txtWho.Location = new Point(lx, ly);
                        txtWho.Size = new Size(lw, h);
                    }

                    txtTime.HAlign = StringAlignment.Near;
                    txtTime.VAlign = StringAlignment.Near;
                    //txtTime.Bounds = new Rectangle(lx, ly + h, lw, h);
                    txtTime.Location = new Point(lx, ly + h);
                    txtTime.Size = new Size(lw, h);

                    txtTime.TextFont = new Font(FontFamily.GenericSansSerif, txtTime.Size.Height / 2F, GraphicsUnit.Pixel);
                    txtWho.TextFont = new Font(FontFamily.GenericSansSerif, txtWho.Size.Height / 2F, GraphicsUnit.Pixel);
                    txtWhy.TextFont = new Font(FontFamily.GenericSansSerif, txtWhy.Size.Height / 2F, GraphicsUnit.Pixel);
                }
                
            }
            else
            {
                // The STU-300 is very shallow, so it is better to utilise
                // the buttons to the side of the display instead.

                int x = _screenSize.Width * 4 / 5;
                int w = _screenSize.Width - x;

                int h2 = _screenSize.Height / 3;
                int h3 = _screenSize.Height / 3;
                int h1 = _screenSize.Height - h2 - h3;

                //btnOk.Bounds = new Rectangle(x, 0, w - 1, h1);
                btnOk.Location = new Point(x, 0);
                btnOk.Size = new Size(w - 1, h1);

                //btnClear.Bounds = new Rectangle(x, h1, w - 1, h2);
                btnClear.Location = new Point(x, h1);
                btnClear.Size = new Size(w - 1, h2);

                //btnCancel.Bounds = new Rectangle(x, h1 + h2, w - 1, h3 - 1);
                btnCancel.Location = new Point(x, h1 + h2);
                btnCancel.Size = new Size(w - 1, h3 - 1);

                if (Who != "" || Why != "")
                {
                    int lw = x * 7 / 8;
                    int lx = x / 16;
                    int ly = h1 + h2;
                    int lh = 0;

                    //dottedLine.Bounds = new Rectangle(lx, ly, lw, lh);
                    dottedLine.Start = new Point(lx, ly);
                    dottedLine.End = new Point(lx + lw, ly + lh);

                    if (Why != "")
                    {
                        txtWhy.HAlign = StringAlignment.Near;
                        txtWhy.VAlign = StringAlignment.Center;
                        //txtWhy.Bounds = new Rectangle(0, 0, _screenSize.Width, h1 / 2);
                        txtWhy.Location = new Point(0, 0);
                        txtWhy.Size = new Size(_screenSize.Width, h1 / 2);
                    }

                    if (Who != "")
                    {
                        txtWho.HAlign = StringAlignment.Far;
                        txtWho.VAlign = StringAlignment.Center;
                        //txtWho.Bounds = new Rectangle(lx, ly, lw, h3 / 2);
                        txtWho.Location = new Point(lx, ly);
                        txtWho.Size = new Size(lw, h3 / 2);
                    }

                    txtTime.HAlign = StringAlignment.Far;
                    txtTime.VAlign = StringAlignment.Near;
                    //txtTime.Bounds = new Rectangle(lx, ly + (h3 / 2), lw, h3 / 2);
                    txtTime.Location = new Point(lx, ly + (h3 / 2));
                    txtTime.Size = new Size(lw, h3 / 2);
                }

            }

            btnOk.Action = Element.DEFAULT_ACTIONS.Done;
            btnClear.Action = Element.DEFAULT_ACTIONS.Refresh;
            btnCancel.Action = Element.DEFAULT_ACTIONS.Cancel;

            btnOk.FontSize = 20;
            btnCancel.FontSize = 20;
            btnClear.FontSize = 20;
            
            AddElement(btnOk);
            AddElement(btnClear);
            AddElement(btnCancel);

            if(Who != "" || Why != "")
            {
                AddElement(dottedLine);
                AddElement(txtTime);
                if (Who != "") AddElement(txtWho);
                if (Why != "") AddElement(txtWhy);
            }
            
        }
    }
}
