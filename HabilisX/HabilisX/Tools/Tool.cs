using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;



namespace HabilisX.Tools
{
    public abstract class Tool : ScatterViewItem, IComparable<ScatterViewItem>
    {

        System.Timers.Timer timer = new System.Timers.Timer(1500);
        public abstract Boolean AreBoundaryIntersecting(FrameworkElement cursorVisual);
        public abstract void removeFilter(object filter);

        public void filterTile_TouchLeave(object sender, TouchEventArgs e)
        {
            timer.Enabled = false;
            Console.WriteLine("uppidy touchy");
        }
        public void filterTile_MouseUp(object sender, MouseEventArgs e)
        {
            timer.Enabled = false;
            Console.WriteLine("uppidy");
        }
        public void filterTile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Found the mouse down");
            timer = new System.Timers.Timer(1500);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate(object delSender, System.Timers.ElapsedEventArgs delE)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    Console.WriteLine("merpaderp");
                    timer.Enabled = false;
                    timer = new System.Timers.Timer(1500);
                    ScatterViewItem tile = sender as ScatterViewItem;
                    Canvas parent = (Canvas)tile.Parent;
                    parent.Dispatcher.BeginInvoke(new Action(() => parent.Children.Remove(tile)));

                    ((Tool)(parent.Parent)).Dispatcher.BeginInvoke(new Action(() => ((Tool)(parent.Parent)).removeFilter(((ScatterViewItem)sender).Tag)));
                }
            });

            timer.Enabled = true;
        }
        public void filterTile_TouchEnter(object sender, TouchEventArgs e)
        {
            Console.WriteLine("Found the Touch down");
            timer = new System.Timers.Timer(1500);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate(object delSender, System.Timers.ElapsedEventArgs delE)
            {
                Console.WriteLine("merpaderp");
                timer.Enabled = false;
                timer = new System.Timers.Timer(1500);
                ScatterViewItem tile = sender as ScatterViewItem;
                Canvas parent = (Canvas)tile.Parent;
                parent.Dispatcher.BeginInvoke(new Action(() => parent.Children.Remove(tile)));

                ((Tool)(parent.Parent)).Dispatcher.BeginInvoke(new Action(() => ((Tool)(parent.Parent)).removeFilter(((ScatterViewItem)sender).Tag)));
            });

            timer.Enabled = true;
        }
        public void activateFilter(FilterTile tile)
        {
            initFilterTile(tile.attTag.ToLower(), tile.Background, tile.attTag);
        }
        public void activateFilter(iFilter query)
        {
            initFilterTile(query.getQueryString(), query.getColor(), query);
        }

        private void initFilterTile(String str, Brush color, object tag)
        {
            Label label = new Label();
            label.Foreground = Brushes.White;
            label.Content = str;

            ScatterViewItem filterTile = new ScatterViewItem();

            filterTile.PreviewMouseDown += new MouseButtonEventHandler(filterTile_PreviewMouseDown);
            filterTile.MouseUp += new System.Windows.Input.MouseButtonEventHandler(filterTile_MouseUp);
            filterTile.TouchEnter += new EventHandler<TouchEventArgs>(filterTile_TouchEnter);
            filterTile.TouchLeave += new EventHandler<TouchEventArgs>(filterTile_TouchLeave);


            filterTile.MinHeight = 0;
            filterTile.Height = 35;
            filterTile.Orientation = 0;

            filterTile.CanMove = false;
            filterTile.CanRotate = false;
            filterTile.CanScale = false;
            filterTile.ShowsActivationEffects = false;

            filterTile.Background = color;
            filterTile.Tag = tag;

            filterTile.Content = label;

            ((Canvas)(this.Content)).Children.Add(filterTile);
            if (this is MagnifyingGlass)
            {
                ((MagnifyingGlass)this).activateMagnifyingGlassFilter((String)tag, filterTile);
            }
            else if (this is PaperClip)
            {
                ((PaperClip)this).activatePaperClipFilter((iFilter)tag, filterTile);
            }
            else if (this is MagicLens)
            {
                ((MagicLens)this).activateMagicLensFilter((iFilter)tag, filterTile);
            }
        }


        public int CompareTo(ScatterViewItem item)
        {
            if (Canvas.GetZIndex(this) > Canvas.GetZIndex(item))
            {
                return 1;
            }
            else if (Canvas.GetZIndex(this) == Canvas.GetZIndex(item))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
