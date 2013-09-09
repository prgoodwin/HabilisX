using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Surface.Presentation.Controls
{
    public class PaperClip : ScatterViewItem
    {
        public List<iFilter> filters;
        public HashSet<Entry> toOrganize;

        public PaperClip()
        {
            filters = new List<iFilter>();
            this.toOrganize = new HashSet<Entry>();
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

            this.Tag = 0; //For image

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

        public ScatterViewItem activatePaperClipFilter(iFilter query)
        {
            ScatterViewItem filterTile = new ScatterViewItem();
            filterTile.MinHeight = 0;
            filterTile.Background = Brushes.Transparent;
            filterTile.ShowsActivationEffects = false;
            filterTile.Tag = query;

            Label filter = new Label();
            filter.Content = query.getQueryString();
            filter.Foreground = Brushes.White;
            filter.Background = query.getColor();

            ((ScatterView)(this.Content)).Items.Add(filterTile);

            filterTile.Content = filter;
            double y = (50 * this.filters.Count) + 10;
            filterTile.Center = new Point(-50, y);
            filterTile.Orientation = 0;
            filterTile.CanMove = false;
            filterTile.CanRotate = false;
            filterTile.CanScale = false;
            this.addFilter(query);

            return filterTile;
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
