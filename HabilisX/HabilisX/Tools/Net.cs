using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;


namespace Microsoft.Surface.Presentation.Controls
{
   public class Net : ScatterViewItem
   {
      public List<iFilter> filters;

      public Net()
      {
         this.filters = new List<iFilter>();
      }

      public bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
      {
         RectangleGeometry cursorBounds =
             new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
         RectangleGeometry targetBounds =
             new RectangleGeometry(new Rect(0, (this.ActualHeight / 5), this.ActualWidth, (this.ActualHeight / 3)));
         cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);

         return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
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
   }
}
