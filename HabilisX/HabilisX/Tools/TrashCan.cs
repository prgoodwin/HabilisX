using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;


namespace HabilisX.Tools
{
    public class TrashCan : Tool
    {
        public TrashCan() {
            this.Background = Brushes.Transparent;
            this.CanRotate = false;
            this.CanMove = false;
            this.CanScale = false;

            this.Center = new Point(1800, 920);
            this.Height = 200;
            this.Orientation = 0;
            this.ShowsActivationEffects = false;
            HabilisX.Utils.RemoveShadow(this);
            Image im = new Image();
            im.Source = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.trashCan.png");
            //this.Tag = 1;
            this.Content = im;
        }

        public override bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
        {
            try
            {
                RectangleGeometry cursorBounds =
                 new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
                RectangleGeometry targetBounds =
                    new RectangleGeometry(new Rect((this.ActualWidth / 2), (this.ActualHeight / 4), 1, 1));
                cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);

                return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
            }

            catch (Exception e)
            {
                Console.WriteLine("Found exception: " + e);
                return false;
            }
        }
    }


}
