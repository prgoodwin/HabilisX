using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;



namespace HabilisX.Tools
{
    public abstract class Tool : ScatterViewItem, IComparable<ScatterViewItem>
    {
        public int numFingers = 0;

        public abstract Boolean AreBoundaryIntersecting(FrameworkElement cursorVisual);


        public Tool() {
           this.TouchEnter += new EventHandler<TouchEventArgs>(Tool_TouchEnter);
           this.TouchLeave += new EventHandler<TouchEventArgs>(Tool_TouchLeave);
        }

        void Tool_TouchLeave(object sender, TouchEventArgs e)
        {
           this.numFingers--;

        }
        void Tool_TouchEnter(object sender, TouchEventArgs e)
        {
           this.numFingers++;

        }


        public int CompareTo(ScatterViewItem item)
        {
            if (Canvas.GetZIndex(this) > Canvas.GetZIndex(item))
            {
                return 1;
            }
            else if (Canvas.GetZIndex(this) == Canvas.GetZIndex(item))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
