using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Surface.Presentation.Controls
{
   public class Ruler : ScatterViewItem
   {
      public List<iFilter> filters;
      public const int TOP = 0;
      public const int BOTTOM = 1;
      public const int LEFT = 2;
      public const int RIGHT = 3;

      public Ruler() {
         filters = new List<iFilter>();
         this.Center = new Point(431, 300);
         this.Height = 75;
         this.MinHeight = 0;
         this.Orientation = 0;
         this.Width = 412;
         this.MaxHeight = 1000;
         this.MaxWidth = 1000;

         ScatterView innerView = new ScatterView();
         this.Content = innerView;
         ImageBrush ib = new ImageBrush();
         ib.ImageSource = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.ruler.png");
         innerView.Background = ib;

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


      public int BoundaryIntersectingOnSide(FrameworkElement cursorVisual)
      {
         RectangleGeometry cursorBounds =
             new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
         RectangleGeometry targetBoundsTop =
             new RectangleGeometry(new Rect(0, 0, this.ActualWidth, 1));
         RectangleGeometry targetBoundsBottom =
             new RectangleGeometry(new Rect(0, this.ActualHeight-1, this.ActualWidth, 1));
         RectangleGeometry targetBoundsLeft =
             new RectangleGeometry(new Rect(0, 0, 1, this.ActualHeight));
         RectangleGeometry targetBoundsRight =
             new RectangleGeometry(new Rect(0, this.ActualWidth-1, 1, this.ActualHeight));


         cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);


         if (cursorBounds.FillContainsWithDetail(targetBoundsTop) != IntersectionDetail.Empty)
         {
            return TOP;
         }
         else if (cursorBounds.FillContainsWithDetail(targetBoundsBottom) != IntersectionDetail.Empty)
         {
            return BOTTOM;
         }

         else if (cursorBounds.FillContainsWithDetail(targetBoundsLeft) != IntersectionDetail.Empty)
         {
            return LEFT;
         }
         else {
            return RIGHT;
         }
      }


      public void addFilter(iFilter filter)
      {
         this.filters.Add(filter);
      }

   }
}
