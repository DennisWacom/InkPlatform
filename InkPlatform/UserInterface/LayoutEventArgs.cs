using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InkPlatform.Ink;

namespace InkPlatform.UserInterface
{
    public class LayoutEventArgs : EventArgs
    {
        private Layout _layout;
        private List<InkData> _penData;

        public LayoutEventArgs(Layout layout)
        {
            _layout = layout;
        }

        public LayoutEventArgs(Layout layout, List<InkData> penData)
        {
            _layout = layout;
            _penData = penData;
        }

        public List<InkData> PenData
        {
            get { return _penData; }
        }

        public Layout Layout
        {
            get { return _layout; }
        }
    }
}
