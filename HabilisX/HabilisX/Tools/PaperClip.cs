using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Surface.Presentation.Controls
{
   public class PaperClip : ScatterViewItem
   {
      public List<iFilter> filters;

      public PaperClip()
      {
         filters = new List<iFilter>();
      }

      public void addFilter(iFilter filter)
      {
         this.filters.Add(filter);
      }

      public void removeFilter(iFilter filter)
      {
         if (this.filters.Contains(filter))
         {
            this.filters.Remove(filter);
         }
      }

      public bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
      {
         RectangleGeometry cursorBounds =
             new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
         RectangleGeometry targetBounds =
             new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
         cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);
         return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
      }

   }
}
