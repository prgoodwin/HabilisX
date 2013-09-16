using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows;



namespace HabilisX.Tools
{
    public abstract class Tool : ScatterViewItem, IComparable<ScatterViewItem>
    {
        public abstract Boolean AreBoundaryIntersecting(FrameworkElement cursorVisual);

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
