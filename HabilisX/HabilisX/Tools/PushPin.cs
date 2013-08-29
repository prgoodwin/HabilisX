﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;



namespace Microsoft.Surface.Presentation.Controls
{
   public class PushPin : ScatterViewItem
   {

      public PushPin() {
          this.Background = Brushes.Transparent;
          this.CanRotate = false;
          this.Center = new Point(250, 235);
          this.Height = 35;
          this.MinHeight = 50;
          this.MinWidth = 50;
          this.Orientation = 0;
          this.ShowsActivationEffects = false;
          HabilisX.Utils.RemoveShadow(this);
          Image im = new Image();
          im.Source = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.pin.gif");
          this.Tag = 1;
          this.Content = im;


      }


      public void SetImageToOccludedPin()
      {
          ((Image)this.Content).Source = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.pinOccluded.gif");
          this.Tag = 0;
      }

      public void SetImageToPin()
      {
          ((Image)this.Content).Source = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.pin.gif");
          this.Tag = 1;
      }

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
