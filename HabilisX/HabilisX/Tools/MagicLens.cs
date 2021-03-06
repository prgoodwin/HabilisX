﻿using System;
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
    public class MagicLens : FilterTool
    
   {
      public List<iFilter> filters;
      public List<UIElement> intersecting;
      public Brush color;

      public MagicLens() { 
         intersecting = new List<UIElement>();
         color = Brushes.Gold;
         filters = new List<iFilter>();


         this.Background = Brushes.Transparent;
         this.Center = new Point(375, 375);
         this.Orientation = 0;
         this.Width = 300;
         this.Height = 200;
         this.SizeChanged += new SizeChangedEventHandler(MagicLens_SizeChanged);
         Canvas innerView = new Canvas();
         this.BorderBrush = Brushes.Black;
         this.BorderThickness = new Thickness(10);
         this.Content = innerView;
      }

      void MagicLens_SizeChanged(object sender, SizeChangedEventArgs e)
      {
         UIElementCollection child = ((Canvas)this.Content).Children;
         for (int i = 0; i < ((Canvas)this.Content).Children.Count; i++)
         {
            if (((Canvas)this.Content).Children[i] is ScatterViewItem)
            {
               ScatterViewItem tile = ((Canvas)this.Content).Children[i] as ScatterViewItem;
               Canvas.SetRight(tile, this.ActualWidth - 10);
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

      public override void addFilter(object query, ScatterViewItem filterTile) {
         double y = (40 * (this.filters.Count)) - 10;
         Canvas.SetRight(filterTile, this.ActualWidth - 10);
         Canvas.SetTop(filterTile, y);

         this.filters.Add((iFilter)query);
      }

      public override void removeFilter(object filt) {
          iFilter filter = filt as iFilter;

         if(this.filters.Contains(filter)){
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
   }
}
