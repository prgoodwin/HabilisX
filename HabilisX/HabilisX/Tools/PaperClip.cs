using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows.Input;


namespace HabilisX.Tools
{
    public class PaperClip : Tool
    {
        public List<iFilter> filters;
        public HashSet<Entry> toOrganize;

        public PaperClip()
        {
            filters = new List<iFilter>();
            this.toOrganize = new HashSet<Entry>();
            this.Background = Brushes.Transparent;
            this.CanRotate = false;
            this.CanScale = false;
            this.Center = new Point(295, 495);
            this.Height = 50;
            this.MinHeight = 5;
            this.MinWidth = 5;
            this.Orientation = 0;
            this.ShowsActivationEffects = false;
            this.Width = 150;
            this.MouseDown += new System.Windows.Input.MouseButtonEventHandler(PaperClip_MouseDown);
            Canvas innerView = new Canvas();
            innerView.Width = 150;
            innerView.MinHeight = 5;
            innerView.MinWidth = 5;
            this.Content = innerView;

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.paperClip.png");
            innerView.Background = ib;

            HabilisX.Utils.RemoveShadow(this);

            this.Tag = 0; //For image

        }

        void PaperClip_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Canvas.SetZIndex(this, 0);
        }

        public void addEntry(Entry entry)
        {
            if (this.toOrganize.Count < 7)
            {
                this.toOrganize.Add(entry);
            }
        }

        public void removeEntry(Entry entry)
        {
            if (this.toOrganize.Contains(entry))
            {
                this.toOrganize.Remove(entry);
            }
        }

       public void activatePaperClipFilter(iFilter query, ScatterViewItem filterTile)
        {
            double y = (40 * (this.filters.Count)) - 10;
            Canvas.SetTop(filterTile, y);
            Canvas.SetRight(filterTile, this.ActualWidth+5);
            this.addFilter(query);
        }

        public void addFilter(iFilter filter)
        {
            this.filters.Add(filter);
        }

        public override void removeFilter(object filt)
        {
            iFilter filter = filt as iFilter;

            if (this.filters.Contains(filter))
            {
                this.filters.Remove(filter);
            }

            UIElementCollection child = ((Canvas)this.Content).Children;
            int childrenCount = 0;
            for (int i = 0; i < ((Canvas)this.Content).Children.Count; i++)
            {
                if (((Canvas)this.Content).Children[i] is ScatterViewItem)
                {
                    ScatterViewItem tile = ((Canvas)this.Content).Children[i] as ScatterViewItem;
                    Canvas.SetTop(tile, (40 * childrenCount) - 10);
                    childrenCount++;
                }
            }

        }

        public override bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
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
