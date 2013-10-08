using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;


namespace HabilisX.Tools
{
    public class Ruler : Tool
    
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


      public override bool AreBoundaryIntersecting(FrameworkElement item)
      {
         RectangleGeometry itemBounds =
             new RectangleGeometry(new Rect(0, 0, item.ActualWidth, item.ActualHeight));
         RectangleGeometry rulerBounds =
             new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
         itemBounds.Transform = (Transform)item.TransformToVisual(this);
         return itemBounds.FillContainsWithDetail(rulerBounds) != IntersectionDetail.Empty;
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
             new RectangleGeometry(new Rect(0, 5, 1, this.ActualHeight-10));
         RectangleGeometry targetBoundsRight =
             new RectangleGeometry(new Rect(this.ActualWidth - 1, 5, 1, this.ActualHeight-10));


         cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);


         if (cursorBounds.FillContainsWithDetail(targetBoundsLeft) != IntersectionDetail.Empty)
         {
             Console.WriteLine("FOUND A LEFT");
            return LEFT;
         }
         else if (cursorBounds.FillContainsWithDetail(targetBoundsRight) != IntersectionDetail.Empty)
         {
             Console.WriteLine("FOUND A RIGHT");
            return RIGHT;
         }
         else if(cursorBounds.FillContainsWithDetail(targetBoundsTop) != IntersectionDetail.Empty)
         {
             Console.WriteLine("FOUND A TOP");
            return TOP;
         }
         else //if (cursorBounds.FillContainsWithDetail(targetBoundsBottom) != IntersectionDetail.Empty)
         {
             Console.WriteLine("FOUND A BOTTOM");
            return BOTTOM;
         }


      }


      public void addFilter(iFilter filter)
      {
         this.filters.Add(filter);
      }

      public override void removeFilter(object filter)
      {
          throw new NotImplementedException();
      }

   }
}
