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
         this.Background = Brushes.Transparent;
         this.CanRotate = false;
         this.Center = new Point(295, 495);
         this.Height = 50;
         this.MinHeight = 5;
         this.MinWidth = 5;
         this.Orientation = 0;
         this.ShowsActivationEffects = false;
         this.Width = 150;

         ScatterView innerView = new ScatterView();
         innerView.Width = 150;
         innerView.MinHeight = 5;
         innerView.MinWidth = 5;
         this.Content = innerView;

         ImageBrush ib = new ImageBrush();
         ib.ImageSource = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.paperClip.png");
         innerView.Background = ib;

         HabilisX.Utils.RemoveShadow(this);

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
