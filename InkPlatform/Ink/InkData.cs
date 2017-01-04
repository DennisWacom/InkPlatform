using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wgssSTU;
using System.Runtime.Serialization;
using System.Drawing;

namespace InkPlatform.Ink
{
    /// <summary>
    /// ink data format for each point captured on the device
    /// </summary>
    [DataContract]
    public class InkData
    {
        /// <summary>
        /// The sequence number of the point
        /// </summary>
        [DataMember]
        public uint seq;

        /// <summary>
        /// The x coordinate
        /// </summary>
        [DataMember]
        public uint x;

        /// <summary>
        /// The y coordinate
        /// </summary>
        [DataMember]
        public uint y;

        /// <summary>
        /// The pen pressure
        /// </summary>
        [DataMember]
        public uint p;

        /// <summary>
        /// The timing data
        /// </summary>
        [DataMember]
        public uint t;

        /// <summary>
        /// Boolean specifying if the pen is in contact with the tablet surface at the specific point
        /// </summary>
        [IgnoreDataMember]
        public bool contact;

        /// <summary>
        /// boolean specifying if the pen is within proximity with the tablet surface at the specific point
        /// </summary>
        [IgnoreDataMember]
        public bool proximity;

        [DataMember]
        public int ct
        {
            get { return contact ? 1 : 0; }
            set { contact = (value == 1 ? true : false); }
        }

        [DataMember]
        public int pr
        {
            get { return proximity ? 1 : 0; }
            set { proximity = (value == 1 ? true : false); }
        }

        [DataMember]
        public string tag = "";
        
        [IgnoreDataMember]
        public Point coordinates
        {
            get
            {
                return new Point((int)x, (int)y);
            }
        }

        public InkData Duplicate()
        {
            InkData result = new InkData();
            
            result.x = x;
            result.y = y;
            result.p = p;
            result.proximity = proximity;
            result.contact = contact;
            result.t = t;
            result.seq = seq;
            result.tag = tag;
            
            return result;
        }

        public InkData Duplicate(Size originalDimension, Size newDimension)
        {
            InkData result = this.Duplicate();
            result.x = (uint)(result.x * (float)newDimension.Width / (float)originalDimension.Width);
            result.y = (uint)(result.y * (float)newDimension.Height / (float)originalDimension.Height);

            return result;
        }

    }
}
