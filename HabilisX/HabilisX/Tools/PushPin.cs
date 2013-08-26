using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;




namespace Microsoft.Surface.Presentation.Controls
{
   public class PushPin : ScatterViewItem
   {

      public PushPin() { }


      public bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
      {
         try
         {
            RectangleGeometry cursorBounds =
             new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
            RectangleGeometry targetBounds =
                new RectangleGeometry(new Rect((this.ActualWidth / 4), (2 * this.ActualHeight / 3), 1, 1));
            cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);

            return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
         }
         //catch (NullReferenceException e)
         //{
         //   return false;
         //}
         catch (Exception e) {
            Console.WriteLine("Found exception: " + e);
            return false;
         }
      }

   
   }

}
