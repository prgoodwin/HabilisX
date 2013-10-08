using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Surface.Presentation.Controls;


namespace HabilisX.Tools
{
   // Summary:
   //     Represents an item that users can manipulate in a Microsoft.Surface.Presentation.Controls.ScatterView
   //     control.
    public class MagicLens : Tool
    
   {
      public List<iFilter> filters;
      public List<UIElement> intersecting;
      public Brush color;

      public MagicLens() { 
         intersecting = new List<UIElement>();
         color = Brushes.Gold;
         filters = new List<iFilter>();


         this.Background = Brushes.Transparent;
         this.Center = new Point(350, 415);
         this.Orientation = 0;
         this.Width = 300;
         this.Height = 200;

         Canvas innerView = new Canvas();
         this.BorderBrush = Brushes.Black;
         this.BorderThickness = new Thickness(10);
         this.Content = innerView;
      }

      public ScatterViewItem activateMagicLensFilter(iFilter query)
      {

          Label label = new Label();
          label.Content = query.getQueryString();
          label.Foreground = Brushes.White;
          label.Background = query.getColor();

          ScatterViewItem filterTile = new ScatterViewItem();
          filterTile.PreviewMouseDown += new MouseButtonEventHandler(filterTile_PreviewMouseDown);
          filterTile.MinHeight = 0;
          filterTile.Background = Brushes.Transparent;
          filterTile.ShowsActivationEffects = false;
          filterTile.Tag = query;

          double y = (50 * (this.filters.Count))-10;
          ((Canvas)(this.Content)).Children.Add(filterTile);
          Canvas.SetRight(filterTile, this.ActualWidth - 10);
          Canvas.SetTop(filterTile, y);

          filterTile.Content = label;
          
          filterTile.Center = new Point(-100, y);
          filterTile.Orientation = 0;
          filterTile.CanMove = false;
          filterTile.CanRotate = false;
          filterTile.CanScale = false;
          this.addFilter(query);

          return filterTile;
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
      public void addFilter(iFilter filter) {
         this.filters.Add(filter);
      }

      public void removeFilter(iFilter filter) {
         if(this.filters.Contains(filter)){
            this.filters.Remove(filter);
         }
      }

      
   }
}
