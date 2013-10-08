using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;



namespace HabilisX.Tools
{
    public abstract class Tool : ScatterViewItem, IComparable<ScatterViewItem>
    {
        System.Timers.Timer timer = new System.Timers.Timer(1500);
        public abstract Boolean AreBoundaryIntersecting(FrameworkElement cursorVisual);

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
                    //timer = new System.Timers.Timer(1500);
                    //ScatterViewItem tile = sender as ScatterViewItem;
                    //Canvas parent = (Canvas)tile.Parent;
                    //parent.Dispatcher.BeginInvoke(new Action(() => parent.Children.Remove(tile)));


                    //if (parent.Parent is PaperClip) {
                    //    ((PaperClip)(parent.Parent)).Dispatcher.BeginInvoke(new Action(() => ((PaperClip)(parent.Parent)).removeFilter((iFilter)((ScatterViewItem)sender).Tag)));
                    //}
                    //else if (parent.Parent is MagicLens) {
                    //    ((MagicLens)(parent.Parent)).Dispatcher.BeginInvoke(new Action(() => ((MagicLens)(parent.Parent)).removeFilter((iFilter)((ScatterViewItem)sender).Tag)));
                    //} else if(parent.Parent is MagnifyingGlass){
                    //    ((MagnifyingGlass)(parent.Parent)).Dispatcher.BeginInvoke(new Action(() => ((MagnifyingGlass)(parent.Parent)).removeAttribute((String)((ScatterViewItem)sender).Tag)));
                    //}

                }
            });

            timer.Enabled = true;
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
