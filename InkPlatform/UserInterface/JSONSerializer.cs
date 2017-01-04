using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace InkPlatform.UserInterface
{
    public class JSONSerializer
    {
        public const int DEFAULT_SCREEN_WIDTH = 800;
        public const int DEFAULT_SCREEN_HEIGHT = 480;

        /// <summary>
        /// The signature layout types
        /// </summary>
        public static string[] SignatureLayoutTypes = { "STANDARD", "SMALL" };

        /// <summary>
        /// 
        /// </summary>
        public enum SignatureLayoutTypeEnum { STANDARD, SMALL };
        
        /// <summary>
        /// The layout types
        /// </summary>
        public static string[] LayoutTypes = { "DEFAULT", "BOX_LAYOUT", "SIGNATURE_LAYOUT" };

        /// <summary>
        /// Enum of the layouts 
        /// </summary>
        public enum LayoutTypeEnum { DEFAULT, BOX_LAYOUT, SIGNATURE_LAYOUT };

        /// <summary>
        /// The box layout flows
        /// </summary>
        public static string[] BoxLayoutFlows = { "RIGHT", "DOWN" };

        /// <summary>
        /// The actions
        /// </summary>
        public static string[] Actions = { "DONE", "REFRESH", "CANCEL" };

        /// <summary>
        /// Enum of the default actions of button/image
        /// </summary>
        public enum ActionEnum { Done, Refresh, Cancel };

        /// <summary>
        /// Enum of the box layout flow
        /// </summary>
        public enum BoxLayoutFlowEnum { RIGHT, DOWN };

        /// <summary>
        /// Enum of the element types
        /// </summary>
        public enum ElementTypeEnum { TEXT, BUTTON, LINE, IMAGE};

        /// <summary>
        /// The list of element types
        /// </summary>
        public static string[] ElementTypeList = { "TEXT", "BUTTON", "LINE", "IMAGE" };

        /// <summary>
        /// The list of horizontal alignments
        /// </summary>
        public static string[] AlignmentList = { "LEFT", "CENTRE", "RIGHT" };

        /// <summary>
        /// The list of vertical alignments
        /// </summary>
        public static string[] VAlignmentList = { "TOP", "MIDDLE", "BOTTOM" };

        /// <summary>
        /// The default font size
        /// </summary>
        public static int DefaultFontSize = 40;

        /// <summary>
        /// The default black color RGB
        /// </summary>
        public static int[] DefaultBlackColor = { 0, 0, 0 };

        /// <summary>
        /// The light gray color RGB
        /// </summary>
        public static int[] LightGrayColor = { 211, 211, 211 };


        public static List<string> SplitJsonArray(string json)
        {
            List<string> result = new List<string>();

            json = json.Trim();
            if(json.Substring(0, 1) == "[" && json.Substring(json.Length - 1, 1) == "]")
            {
                int level = 0;
                string item = "";
                foreach (char c in json)
                {
                    if(c == ',' && level == 1 && item != "")
                    {
                        result.Add(item);
                        item = "";
                    }

                    if (c == ']' || c == '}')
                    {
                        if (level == 1 && item != "")
                        {
                            result.Add(item);
                            item = "";
                        }
                        level--;
                    }

                    if (level >= 1)
                    {
                        if(level != 1 || c != ',')
                        {
                            item += c;
                        }
                    }

                    if (c == '[' || c == '{') { level++; }
                    
                }
            }
            else
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Serializes the layout.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string SerializeLayout(Layout obj)
        {
            InterfaceLayout inf;
            
            if (obj.GetType() == typeof(BoxLayout))
            {
                inf = InterfaceBoxLayout.Serialize((BoxLayout)obj);
            }
            else if (obj.GetType() == typeof(SignatureLayout))
            {
                inf = InterfaceSignatureLayout.Serialize((SignatureLayout)obj);
            }
            else
            {
                inf = InterfaceLayout.Serialize(obj);
            }
                       
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(inf.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, inf);
                ms.Position = 0;
                StreamReader reader = new StreamReader(ms);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Deserializes the layout.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static object DeserializeLayout(string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(InterfaceLayout));
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                ms.Position = 0;
                InterfaceLayout inf = (InterfaceLayout)serializer.ReadObject(ms);
                if(inf.Layout == LayoutTypes[(int)LayoutTypeEnum.BOX_LAYOUT])
                {
                    ms.Position = 0;
                    serializer = new DataContractJsonSerializer(typeof(InterfaceBoxLayout));
                    InterfaceBoxLayout infLayout = (InterfaceBoxLayout)serializer.ReadObject(ms);
                    BoxLayout layout = infLayout.Deserialize(inf.Name);
                    return layout;
                }
                else if(inf.Layout == LayoutTypes[(int)LayoutTypeEnum.SIGNATURE_LAYOUT])
                {
                    ms.Position = 0;
                    serializer = new DataContractJsonSerializer(typeof(InterfaceSignatureLayout));
                    InterfaceSignatureLayout infLayout = (InterfaceSignatureLayout)serializer.ReadObject(ms);
                    SignatureLayout layout = infLayout.Deserialize(inf.Name);
                    return layout;
                }
                else
                {
                    ms.Position = 0;
                    serializer = new DataContractJsonSerializer(typeof(InterfaceLayout));
                    InterfaceLayout infLayout = (InterfaceLayout)serializer.ReadObject(ms);
                    Layout layout = infLayout.Deserialize(inf.Name);
                    return layout;
                }
            }
            
        }

        /// <summary>
        /// Deserializes the element.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static object DeserializeElement(string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(InterfaceElement));
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                ms.Position = 0;
                InterfaceElement inf = (InterfaceElement)serializer.ReadObject(ms);
                if(inf.ElementType == ElementTypeList[(int)ElementTypeEnum.TEXT])
                {
                    ms.Position = 0;
                    serializer = new DataContractJsonSerializer(typeof(InterfaceElementText));
                    InterfaceElementText infTxt = (InterfaceElementText)serializer.ReadObject(ms);
                    ElementText ele = infTxt.Deserialize();
                    return ele;
                }
                else if(inf.ElementType == ElementTypeList[(int)ElementTypeEnum.BUTTON])
                {
                    ms.Position = 0;
                    serializer = new DataContractJsonSerializer(typeof(InterfaceElementButton));
                    InterfaceElementButton infBtn = (InterfaceElementButton)serializer.ReadObject(ms);
                    ElementButton ele = infBtn.Deserialize();
                    return ele;
                }
                else if (inf.ElementType == ElementTypeList[(int)ElementTypeEnum.LINE])
                {
                    ms.Position = 0;
                    serializer = new DataContractJsonSerializer(typeof(InterfaceElementLine));
                    InterfaceElementLine infLn = (InterfaceElementLine)serializer.ReadObject(ms);
                    ElementLine ele = infLn.Deserialize();
                    return ele;
                }
                else if (inf.ElementType == ElementTypeList[(int)ElementTypeEnum.IMAGE])
                {
                    ms.Position = 0;
                    serializer = new DataContractJsonSerializer(typeof(InterfaceElementImage));
                    InterfaceElementImage infLn = (InterfaceElementImage)serializer.ReadObject(ms);
                    ElementImage ele = infLn.Deserialize();
                    return ele;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Serializes the element.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string SerializeElement(object obj)
        {
            InterfaceElement inf;
            if (obj.GetType() == typeof(ElementText))
            {
                inf = InterfaceElementText.Serialize((ElementText)obj);
            }
            else if(obj.GetType() == typeof(ElementButton))
            {
                inf = InterfaceElementButton.Serialize((ElementButton)obj);
            }
            else if (obj.GetType() == typeof(ElementLine))
            {
                inf = InterfaceElementLine.Serialize((ElementLine)obj);
            }
            else if (obj.GetType() == typeof(ElementImage))
            {
                inf = InterfaceElementImage.Serialize((ElementImage)obj);
            }
            else
            {
                return null;
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(inf.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, inf);
                ms.Position = 0;
                StreamReader reader = new StreamReader(ms);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Converts the horizontal alignments (in default String alignment after deserialization from json)
        /// to the string alignment as in the layout
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        /// <returns></returns>
        public static string convertAlignmentHorizontal(StringAlignment alignment)
        {
            if (alignment == StringAlignment.Near) return AlignmentList[0];
            if (alignment == StringAlignment.Far) return AlignmentList[2];
            return AlignmentList[1];
        }

        /// <summary>
        /// Converts the vertical alignments (in default String alignment after deserialization from json)
        /// to the string alignment as in the layout
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        /// <returns></returns>
        public static string convertAlignmentVertical(StringAlignment alignment)
        {
            if (alignment == StringAlignment.Near) return VAlignmentList[0];
            if (alignment == StringAlignment.Far) return VAlignmentList[2];
            return VAlignmentList[1];
        }

        /// <summary>
        /// Converts the alignments (in default String alignment after deserialization from json)
        /// to the string alignment as in the layout
        /// </summary>
        /// <param name="alignmentText">The alignment text.</param>
        /// <returns></returns>
        public static StringAlignment ConvertAlignment(string alignmentText)
        {
            if (alignmentText == null) return StringAlignment.Center;
            if (alignmentText.ToUpper() == AlignmentList[0]) return StringAlignment.Near;
            if (alignmentText.ToUpper() == AlignmentList[2]) return StringAlignment.Far;
            if (alignmentText.ToUpper() == VAlignmentList[0]) return StringAlignment.Near;
            if (alignmentText.ToUpper() == VAlignmentList[2]) return StringAlignment.Far;

            return StringAlignment.Center;
        }

        /// <summary>
        /// Intermediate class to serialize BoxLayout to JSON
        /// </summary>
        /// <seealso cref="InkPlatform.UserInterface.JSONSerializer.InterfaceLayout" />
        [DataContract]
        public class InterfaceBoxLayout : InterfaceLayout
        {
            [DataMember]
            public int TopRatio = 1;
            [DataMember]
            public int MiddleRatio = 1;
            [DataMember]
            public int BottomRatio = 1;
            [DataMember]
            public int LeftRatio = 1;
            [DataMember]
            public int CentreRatio = 1;
            [DataMember]
            public int RightRatio = 1;
            [DataMember]
            public string Flow = "RIGHT";
            [DataMember]
            public int Spacing = 0;
            [DataMember]
            public bool ShowGrid = false;

            [DataMember]
            public List<InterfaceElement> TopLeft = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> TopCentre = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> TopRight = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> MiddleLeft = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> MiddleCentre = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> MiddleRight = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> BottomLeft = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> BottomCentre = new List<InterfaceElement>();
            [DataMember]
            public List<InterfaceElement> BottomRight = new List<InterfaceElement>();

            /// <summary>
            /// Serializes a boxlayout to json
            /// </summary>
            /// <param name="layout">The layout.</param>
            /// <returns></returns>
            public static InterfaceBoxLayout Serialize(BoxLayout layout)
            {
                InterfaceBoxLayout infLayout = new InterfaceBoxLayout();
                if (layout.Name != null) infLayout.Name = layout.Name;
                infLayout.Layout = LayoutTypes[(int)LayoutTypeEnum.BOX_LAYOUT];
                foreach(Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.TOP_LEFT))
                {
                    infLayout.TopLeft.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.TOP_CENTRE))
                {
                    infLayout.TopCentre.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.TOP_RIGHT))
                {
                    infLayout.TopRight.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.MIDDLE_LEFT))
                {
                    infLayout.MiddleLeft.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.MIDDLE_CENTRE))
                {
                    infLayout.MiddleCentre.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.MIDDLE_RIGHT))
                {
                    infLayout.MiddleRight.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.BOTTOM_LEFT))
                {
                    infLayout.BottomLeft.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.BOTTOM_CENTRE))
                {
                    infLayout.BottomCentre.Add(GetInterfaceElement(ele));
                }
                foreach (Element ele in layout.GetElementListForPosition(BoxLayout.POSITION.BOTTOM_RIGHT))
                {
                    infLayout.BottomRight.Add(GetInterfaceElement(ele));
                }
                
                return infLayout;
            }

            /// <summary>
            /// Gets the intermediate element class 
            /// </summary>
            /// <param name="ele">The ele.</param>
            /// <returns></returns>
            public static InterfaceElement GetInterfaceElement(Element ele)
            {
                if (ele.GetType().Equals(typeof(ElementText)))
                {
                    InterfaceElementText eleTxt = InterfaceElementText.Serialize((ElementText)ele);
                    return eleTxt;
                }
                else if (ele.GetType().Equals(typeof(ElementButton)))
                {
                    InterfaceElementButton eleBtn = InterfaceElementButton.Serialize((ElementButton)ele);
                    return eleBtn;
                }
                else if (ele.GetType().Equals(typeof(ElementLine)))
                {
                    InterfaceElementLine eleLn = InterfaceElementLine.Serialize((ElementLine)ele);
                    return eleLn;
                }
                else if (ele.GetType().Equals(typeof(ElementImage)))
                {
                    InterfaceElementImage eleImg = InterfaceElementImage.Serialize((ElementImage)ele);
                    return eleImg;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Deserializes the current InterfaceBoxLayout to a BoxLayout
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns></returns>
            public new BoxLayout Deserialize(string name)
            {
                BoxLayout layout = new BoxLayout(name);
                
                if(Flow != null)
                {
                    if (Flow.ToUpper().Equals(BoxLayoutFlows[(int)BoxLayoutFlowEnum.DOWN]))
                    {
                        layout.Flow = BoxLayout.FLOW.DOWN;
                    }
                    else
                    {
                        layout.Flow = BoxLayout.FLOW.RIGHT;
                    }
                }
                
                if(TopLeft != null)
                {
                    foreach (InterfaceElement infEle in TopLeft)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.TOP_LEFT);
                    }
                }
                if(TopCentre != null)
                {
                    foreach (InterfaceElement infEle in TopCentre)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.TOP_CENTRE);
                    }
                }
                if(TopRight != null)
                {
                    foreach (InterfaceElement infEle in TopRight)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.TOP_RIGHT);
                    }
                }
                if(MiddleLeft != null)
                {
                    foreach (InterfaceElement infEle in MiddleLeft)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.MIDDLE_LEFT);
                    }
                }
                if(MiddleCentre != null)
                {
                    foreach (InterfaceElement infEle in MiddleCentre)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.MIDDLE_CENTRE);
                    }
                }
                if(MiddleRight != null)
                {
                    foreach (InterfaceElement infEle in MiddleRight)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.MIDDLE_RIGHT);
                    }
                }
                if(BottomLeft != null)
                {
                    foreach (InterfaceElement infEle in BottomLeft)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.BOTTOM_LEFT);
                    }
                }
                if(BottomCentre != null)
                {
                    foreach (InterfaceElement infEle in BottomCentre)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.BOTTOM_CENTRE);
                    }
                }
                if(BottomRight != null)
                {
                    foreach (InterfaceElement infEle in BottomRight)
                    {
                        Element ele = InterfaceElement.Deserialize(infEle);
                        layout.AddElement(ele, BoxLayout.POSITION.BOTTOM_RIGHT);
                    }
                }
                
                if (LeftRatio == 0) LeftRatio = 1;
                if (CentreRatio == 0) CentreRatio = 1;
                if (RightRatio == 0) RightRatio = 1;
                layout.setHorizontalRatio(LeftRatio, CentreRatio, RightRatio);

                if (TopRatio == 0) TopRatio = 1;
                if (MiddleRatio == 0) MiddleRatio = 1;
                if (BottomRatio == 0) BottomRatio = 1;
                layout.setVerticalRatio(TopRatio, MiddleRatio, BottomRatio);

                layout.Spacing = Spacing;
                layout.ShowGrid = ShowGrid;

                return layout;
            }
        }

        /// <summary>
        /// Intermediate class to transform a SignatureLayout to JSON
        /// </summary>
        /// <seealso cref="InkPlatform.UserInterface.JSONSerializer.InterfaceLayout" />
        [DataContract]
        public class InterfaceSignatureLayout : InterfaceLayout
        {
            [DataMember]
            public string OkText;
            [DataMember]
            public string ClearText;
            [DataMember]
            public string CancelText;
            [DataMember]
            public string Who;
            [DataMember]
            public string Why;
            
            /// <summary>
            /// Serializes the signature layout to the intermediate class
            /// </summary>
            /// <param name="layout">The layout.</param>
            /// <returns></returns>
            public static InterfaceSignatureLayout Serialize(SignatureLayout layout)
            {
                InterfaceSignatureLayout infLayout = new InterfaceSignatureLayout();
                if (layout.Name != null) infLayout.Name = layout.Name;
                infLayout.Layout = LayoutTypes[(int)LayoutTypeEnum.SIGNATURE_LAYOUT];
                infLayout.OkText = layout.OkText;
                infLayout.CancelText = layout.CancelText;
                infLayout.ClearText = layout.ClearText;
                infLayout.Who = layout.Who;
                infLayout.Why = layout.Why;
                
                return infLayout;
            }

            /// <summary>
            /// Deserializes the current InterfaceSignatureLayout to an actual SignatureLayoyt
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns></returns>
            public new SignatureLayout Deserialize(string name)
            {
                SignatureLayout layout = new SignatureLayout(name);
                layout.OkText = this.OkText;
                layout.ClearText = this.ClearText;
                layout.CancelText = this.CancelText;
                layout.Who = this.Why;
                layout.Why = this.Why;
                return layout;
            }

        }

        /// <summary>
        /// Intermediate class to transform a Layout to JSON
        /// </summary>
        [DataContract]
        [KnownType(typeof(InterfaceSignatureLayout))]
        [KnownType(typeof(InterfaceElement))]
        [KnownType(typeof(InterfaceElementText))]
        [KnownType(typeof(InterfaceElementLine))]
        [KnownType(typeof(InterfaceElementButton))]
        [KnownType(typeof(InterfaceElementImage))]
        public class InterfaceLayout
        {
            [DataMember]
            public List<InterfaceElement> ElementList = new List<InterfaceElement>();
            [DataMember]
            public string Layout = "DEFAULT";
            [DataMember]
            public string Name;

            /// <summary>
            /// Serializes the layout to the intermediate class
            /// </summary>
            /// <param name="layout">The layout.</param>
            /// <returns></returns>
            public static InterfaceLayout Serialize(Layout layout)
            {
                InterfaceLayout infLayout = new InterfaceLayout();
                if (layout.Name != null) infLayout.Name = layout.Name;
                if(layout.ElementList != null && layout.ElementList.Count > 0)
                {
                    foreach(Element ele in layout.ElementList)
                    {
                        if (ele.GetType().Equals(typeof(ElementText)))
                        {
                            InterfaceElementText eleTxt = InterfaceElementText.Serialize((ElementText)ele);
                            infLayout.ElementList.Add(eleTxt);
                        }else if (ele.GetType().Equals(typeof(ElementButton)))
                        {
                            InterfaceElementButton eleBtn = InterfaceElementButton.Serialize((ElementButton)ele);
                            infLayout.ElementList.Add(eleBtn);
                        }else if (ele.GetType().Equals(typeof(ElementLine)))
                        {
                            InterfaceElementLine eleLn = InterfaceElementLine.Serialize((ElementLine)ele);
                            infLayout.ElementList.Add(eleLn);
                        }else if (ele.GetType().Equals(typeof(ElementImage)))
                        {
                            InterfaceElementImage eleImg = InterfaceElementImage.Serialize((ElementImage)ele);
                            infLayout.ElementList.Add(eleImg);
                        }
                    }
                }
                return infLayout;
            }

            /// <summary>
            /// Deserializes the current InterfaceLayout to an actual layout
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns></returns>
            public Layout Deserialize(string name)
            {
                Layout layout = new Layout(name);
                if(ElementList != null && ElementList.Count > 0)
                {
                    foreach(InterfaceElement infEle in ElementList)
                    {
                        if(infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.TEXT])
                        {
                            ElementText eleTxt = ((InterfaceElementText)infEle).Deserialize();
                            layout.AddElement(eleTxt);
                        }
                        else if(infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.LINE])
                        {
                            ElementLine eleLn = ((InterfaceElementLine)infEle).Deserialize();
                            layout.AddElement(eleLn);
                        }
                        else if(infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.IMAGE])
                        {
                            ElementImage eleImg = ((InterfaceElementImage)infEle).Deserialize();
                            layout.AddElement(eleImg);
                        }
                        else if (infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.BUTTON])
                        {
                            ElementButton eleBtn = ((InterfaceElementButton)infEle).Deserialize();
                            layout.AddElement(eleBtn);
                        }
                    }
                }

                return layout;
            }
        }

        /// <summary>
        /// Intermediate class to transform ElementLine to JSON
        /// </summary>
        /// <seealso cref="InkPlatform.UserInterface.JSONSerializer.InterfaceElement" />
        [DataContract]
        public class InterfaceElementLine : InterfaceElement
        {
            [DataMember]
            public int[] LineColor = DefaultBlackColor;
            [DataMember]
            public int X1;
            [DataMember]
            public int Y1;
            [DataMember]
            public int X2;
            [DataMember]
            public int Y2;

            /// <summary>
            /// Serializes the ElementLine to the intermediate class.
            /// </summary>
            /// <param name="ln">The ElementLine.</param>
            /// <returns></returns>
            public static InterfaceElementLine Serialize(ElementLine ln)
            {
                InterfaceElementLine result = new InterfaceElementLine();
                result.Name = ln.Name;
                result.ElementType = ElementTypeList[(int)ElementTypeEnum.LINE];

                result.X = (int)ln.Location.X;
                result.Y = (int)ln.Location.Y;
                result.Width = (int)ln.Size.Width;
                result.Height = (int)ln.Size.Height;
                result.X1 = result.X;
                result.Y1 = result.Y;
                result.X2 = Math.Abs(result.Width - result.X);
                result.Y2 = Math.Abs(result.Height - result.Y);
                
                return result;
            }

            /// <summary>
            /// Deserializes this instance to an ElementLine
            /// </summary>
            /// <returns></returns>
            public ElementLine Deserialize()
            {
                if (Name == null || Name == "") return null;

                ElementLine txt = new ElementLine(Name);
                txt.Location = new Point((int)X1, (int)Y1);
                txt.Size = new Size((int)(Math.Abs(X2 - X1)), (int)(Math.Abs(Y2 - Y1)));
                if (LineColor != null && !LineColor.SequenceEqual(DefaultBlackColor))
                {
                    txt.LinePen = new Pen(Color.FromArgb(LineColor[0], LineColor[1], LineColor[2]));
                }
                
                return txt;
            }
        }

        /// <summary>
        /// Intermediate class to transform ElementImage to JSON
        /// </summary>
        /// <seealso cref="InkPlatform.UserInterface.JSONSerializer.InterfaceElement" />
        [DataContract]
        public class InterfaceElementImage : InterfaceElement
        {
            [IgnoreDataMember]
            public EventHandler Click;
            [DataMember]
            public string PictureFilename;
            [DataMember]
            public string NextScreen;
            [DataMember]
            public string Action;

            /// <summary>
            /// Serializes the ElementImage to the intermediate class
            /// </summary>
            /// <param name="img">The img.</param>
            /// <returns></returns>
            public static InterfaceElementImage Serialize(ElementImage img)
            {
                InterfaceElementImage result = new InterfaceElementImage();
                result.Name = img.Name;
                result.ElementType = ElementTypeList[(int)ElementTypeEnum.IMAGE];
                result.X = (int)img.Location.X;
                result.Y = (int)img.Location.Y;
                result.Width = (int)img.Size.Width;
                result.Height = (int)img.Size.Height;

                bool save = true;
                string tmpFilename = "";
                try
                {
                    if(img.Picture != null)
                    {
                        tmpFilename = Path.GetTempFileName() + Guid.NewGuid().ToString();
                        img.Picture.Save(tmpFilename, ImageFormat.Png);
                    }
                }
                catch (Exception) { save = false; }
                if (save) result.PictureFilename = tmpFilename;

                return result;
            }

            /// <summary>
            /// Deserializes this instance.
            /// </summary>
            /// <returns></returns>
            public ElementImage Deserialize()
            {
                if (Name == null || Name == "") return null;

                if (PictureFilename != null && PictureFilename != "")
                {
                    ElementImage img = new ElementImage(Name, PictureFilename);
                    img.Location = new Point(X, Y);
                    img.Size = new Size(Width, Height);

                    if (NextScreen != null && NextScreen != "")
                    {
                        img.NextScreenName = NextScreen;
                    }
                    if (Action != null)
                    {
                        if (Action.ToUpper().Equals(Actions[(int)ActionEnum.Done]))
                        {
                            img.Action = Element.DEFAULT_ACTIONS.Done;
                        }
                        else if (Action.ToUpper().Equals(Actions[(int)ActionEnum.Refresh]))
                        {
                            img.Action = Element.DEFAULT_ACTIONS.Refresh;
                        }
                        else if (Action.ToUpper().Equals(Actions[(int)ActionEnum.Cancel]))
                        {
                            img.Action = Element.DEFAULT_ACTIONS.Cancel;
                        }
                    }

                    return img;
                }
                
                return null;
            }
        }

        /// <summary>
        /// Intermediate class to transform ElementButton to JSON
        /// </summary>
        /// <seealso cref="InkPlatform.UserInterface.JSONSerializer.InterfaceElement" />
        [DataContract]
        public class InterfaceElementButton : InterfaceElement
        {
            [DataMember]
            public string Text;
            [DataMember]
            public int[] TextColor = DefaultBlackColor;
            [DataMember]
            public int[] FillColor = LightGrayColor;
            [DataMember]
            public int[] BorderColor = DefaultBlackColor;
            [DataMember]
            public int FontSize = DefaultFontSize;
            [DataMember]
            public bool AutoResize = true;
            [IgnoreDataMember]
            public EventHandler Click;
            [DataMember]
            public string NextScreen;
            [DataMember]
            public string Action;

            /// <summary>
            /// Serializes the specified ElementButton to this intermediate class
            /// </summary>
            /// <param name="btn">The BTN.</param>
            /// <returns></returns>
            public static InterfaceElementButton Serialize(ElementButton btn)
            {
                InterfaceElementButton result = new InterfaceElementButton();
                result.Name = btn.Name;
                result.ElementType = ElementTypeList[(int)ElementTypeEnum.BUTTON];
                if (!(btn.Location.X == 0 && btn.Location.Y == 0 && btn.Size.Width == 0 && btn.Size.Height == 0))
                {
                    result.X = (int)btn.Location.X;
                    result.Y = (int)btn.Location.Y;
                    result.Width = (int)btn.Size.Width;
                    result.Height = (int)btn.Size.Height;
                }
                result.Text = btn.Text;
                
                result.NextScreen = btn.NextScreenName;
                if(btn.TextBrush != null)
                {
                    result.TextColor[0] = ((SolidBrush)btn.TextBrush).Color.R;
                    result.TextColor[1] = ((SolidBrush)btn.TextBrush).Color.G;
                    result.TextColor[2] = ((SolidBrush)btn.TextBrush).Color.B;
                }
                if(btn.FillBrush != null)
                {
                    result.FillColor[0] = ((SolidBrush)btn.FillBrush).Color.R;
                    result.FillColor[1] = ((SolidBrush)btn.FillBrush).Color.G;
                    result.FillColor[2] = ((SolidBrush)btn.FillBrush).Color.B;
                }
                if(btn.BorderPen != null)
                {
                    result.BorderColor[0] = ((Pen)btn.BorderPen).Color.R;
                    result.BorderColor[1] = ((Pen)btn.BorderPen).Color.G;
                    result.BorderColor[2] = ((Pen)btn.BorderPen).Color.B;
                }
                if (btn.TextFont != null) result.FontSize = (int)btn.FontSize;
                if (btn.AutoResizeText != true) result.AutoResize = btn.AutoResizeText;

                return result;
            }

            /// <summary>
            /// Deserializes this instance.
            /// </summary>
            /// <returns></returns>
            public ElementButton Deserialize()
            {
                if (Name == null || Name == "") return null;
                if (Text == null) Text = "";

                ElementButton btn = new ElementButton(Name, Text);
                btn.Location = new Point(X, Y);
                btn.Size = new Size(Width, Height);

                if(NextScreen != null && NextScreen != "")
                {
                    btn.NextScreenName = NextScreen;
                }
                if(Action != null)
                {
                    if (Action.ToUpper().Equals(Actions[(int)ActionEnum.Done]))
                    {
                        btn.Action = Element.DEFAULT_ACTIONS.Done;
                    }
                    else if (Action.ToUpper().Equals(Actions[(int)ActionEnum.Refresh]))
                    {
                        btn.Action = Element.DEFAULT_ACTIONS.Refresh;
                    }
                    else if (Action.ToUpper().Equals(Actions[(int)ActionEnum.Cancel]))
                    {
                        btn.Action = Element.DEFAULT_ACTIONS.Cancel;
                    }
                }
                
                if (TextColor != null && !TextColor.SequenceEqual(DefaultBlackColor))
                {
                    btn.TextBrush = new SolidBrush(Color.FromArgb(TextColor[0], TextColor[1], TextColor[2]));
                }
                if (FillColor != null && !FillColor.SequenceEqual(LightGrayColor))
                {
                    btn.TextBrush = new SolidBrush(Color.FromArgb(TextColor[0], TextColor[1], TextColor[2]));
                }
                if (BorderColor != null && !BorderColor.SequenceEqual(DefaultBlackColor))
                {
                    btn.BorderPen = new Pen(Color.FromArgb(TextColor[0], TextColor[1], TextColor[2]));
                }
                if (FontSize != 0 && FontSize != DefaultFontSize) btn.FontSize = FontSize;
                btn.AutoResizeText = AutoResize;

                return btn;
            }
        }

        /// <summary>
        /// Intermediate class to serialize ElementText to JSON
        /// </summary>
        /// <seealso cref="InkPlatform.UserInterface.JSONSerializer.InterfaceElement" />
        [DataContract]
        public class InterfaceElementText : InterfaceElement
        {
            [DataMember]
            public string Text;
            [DataMember]
            public int[] TextColor = DefaultBlackColor;
            [DataMember]
            public string Align = AlignmentList[1];
            [DataMember]
            public string VAlign = VAlignmentList[1];
            [DataMember]
            public int FontSize = DefaultFontSize;
            [DataMember]
            public bool AutoResize = true;

            /// <summary>
            /// Serializes the specified ElementText
            /// </summary>
            /// <param name="txt">The text.</param>
            /// <returns></returns>
            public static InterfaceElementText Serialize(ElementText txt)
            {
                InterfaceElementText result = new InterfaceElementText();
                result.Name = txt.Name;
                result.ElementType = ElementTypeList[(int)ElementTypeEnum.TEXT];
                if(!(txt.Location.X == 0 && txt.Location.Y == 0 && txt.Size.Width == 0 && txt.Size.Height == 0))
                {
                    result.X = (int)txt.Location.X;
                    result.Y = (int)txt.Location.Y;
                    result.Width = (int)txt.Size.Width;
                    result.Height = (int)txt.Size.Height;
                }
                result.Text = txt.Text;
                if(txt.Brush != null)
                {
                    result.TextColor[0] = ((SolidBrush)txt.Brush).Color.R;
                    result.TextColor[1] = ((SolidBrush)txt.Brush).Color.G;
                    result.TextColor[2] = ((SolidBrush)txt.Brush).Color.B;
                }
                if (txt.HAlign != StringAlignment.Center) result.Align = convertAlignmentHorizontal(txt.HAlign);
                if (txt.VAlign != StringAlignment.Center) result.Align = convertAlignmentVertical(txt.VAlign);
                if (txt.TextFont != null) result.FontSize = (int)txt.FontSize;
                if (txt.AutoResizeText != true) result.AutoResize = txt.AutoResizeText;

                return result;
            }

            /// <summary>
            /// Deserializes this instance.
            /// </summary>
            /// <returns></returns>
            public ElementText Deserialize()
            {
                if (Name == null || Name == "") return null;
                if (Text == null) Text = "";
                
                ElementText txt = new ElementText(Name, Text);
                txt.Location = new Point(X, Y);
                txt.Size = new Size(Width, Height);

                if (TextColor != null && !TextColor.SequenceEqual(DefaultBlackColor))
                {
                    txt.Brush = new SolidBrush(Color.FromArgb(TextColor[0], TextColor[1], TextColor[2]));
                }
                else
                {
                    txt.Brush = new SolidBrush(Color.FromArgb(DefaultBlackColor[0], 
                                                                DefaultBlackColor[1], 
                                                                DefaultBlackColor[2]));
                }
                txt.HAlign = ConvertAlignment(Align);
                txt.VAlign = ConvertAlignment(VAlign);
                if (FontSize != 0)
                {
                    txt.FontSize = FontSize;
                }
                else
                {
                    txt.FontSize = DefaultFontSize;
                }

                txt.AutoResizeText = AutoResize;

                return txt;
            }
        }

        /// <summary>
        /// Intermediate class to serialize Element to JSON
        /// </summary>
        [DataContract]
        public class InterfaceElement
        {
            [DataMember]
            public string ElementType;
            [DataMember]
            public string Name;
            [DataMember]
            public int X;
            [DataMember]
            public int Y;
            [DataMember]
            public int Width;
            [DataMember]
            public int Height;

            /// <summary>
            /// Deserializes the InterfaceElement to Element
            /// </summary>
            /// <param name="infEle">The inf ele.</param>
            /// <returns></returns>
            public static Element Deserialize(InterfaceElement infEle)
            {
                if (infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.TEXT])
                {
                    ElementText eleTxt = ((InterfaceElementText)infEle).Deserialize();
                    return eleTxt;
                }
                else if (infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.LINE])
                {
                    ElementLine eleLn = ((InterfaceElementLine)infEle).Deserialize();
                    return eleLn;
                }
                else if (infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.IMAGE])
                {
                    ElementImage eleImg = ((InterfaceElementImage)infEle).Deserialize();
                    return eleImg;
                }
                else if (infEle.ElementType == ElementTypeList[(int)ElementTypeEnum.BUTTON])
                {
                    ElementButton eleBtn = ((InterfaceElementButton)infEle).Deserialize();
                    return eleBtn;
                }

                return null;
            }
               
        }

    }

    
}
