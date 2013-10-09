using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;

namespace HabilisX.Tools
{
   public abstract class FilterTool : Tool
   {
      private System.Timers.Timer timer = new System.Timers.Timer(1500);

      public abstract void removeFilter(object filter);
      public abstract void addFilter(object filter, ScatterViewItem tile);

      public void filterTile_TouchLeave(object sender, TouchEventArgs e)
      {
         timer.Enabled = false;
      }
      public void filterTile_MouseUp(object sender, MouseEventArgs e)
      {
         timer.Enabled = false;
      }
      public void filterTile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         timer = new System.Timers.Timer(1500);
         timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate(object delSender, System.Timers.ElapsedEventArgs delE)
         {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
               timer.Enabled = false;
               timer = new System.Timers.Timer(1500);
               ScatterViewItem tile = sender as ScatterViewItem;
               Canvas parent = (Canvas)tile.Parent;
               if (parent == null) {
                  return;
               }
               parent.Dispatcher.BeginInvoke(new Action(() => parent.Children.Remove(tile)));

               ((FilterTool)(parent.Parent)).Dispatcher.BeginInvoke(new Action(() => ((FilterTool)(parent.Parent)).removeFilter(((ScatterViewItem)sender).Tag)));
            }
         });

         timer.Enabled = true;
      }
      public void filterTile_TouchEnter(object sender, TouchEventArgs e)
      {
         timer = new System.Timers.Timer(1500);
         timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate(object delSender, System.Timers.ElapsedEventArgs delE)
         {
            timer.Enabled = false;
            timer = new System.Timers.Timer(1500);
            ScatterViewItem tile = sender as ScatterViewItem;
            Canvas parent = (Canvas)tile.Parent;
            if (parent == null)
            {
               return;
            }

            parent.Dispatcher.BeginInvoke(new Action(() => parent.Children.Remove(tile)));
            ((FilterTool)(parent.Parent)).Dispatcher.BeginInvoke(new Action(() => 
               ((FilterTool)(parent.Parent)).removeFilter(((ScatterViewItem)sender).Tag)));
         });

         timer.Enabled = true;
      }

      public void activateFilterTile(String str, Brush color, object tag)
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
          this.addFilter(tag, filterTile);
      }


   }
}
