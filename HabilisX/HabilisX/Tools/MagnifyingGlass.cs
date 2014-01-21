using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Surface.Presentation.Controls;


namespace HabilisX.Tools
{
    public class MagnifyingGlass : FilterTool
    
   {
      public List<String> attributes = new List<String>();
      public Label detailsText = new Label();

      public MagnifyingGlass()
      {
         detailsText.FontSize = 24;
        this.Background = Brushes.Transparent;
        this.CanRotate = false;
         this.Center = new Point(275, 385);
         this.Orientation = 0;
         this.ShowsActivationEffects = false;
         Canvas innerView = new Canvas();
         innerView.Width = 100;
         innerView.Height = 100;
         this.Content = innerView;

         ImageBrush im = new ImageBrush();
         im.ImageSource = HabilisX.Utils.NewEmbededResource("HabilisX.Resources.magnifyingGlass.png");
         innerView.Background = im;

         HabilisX.Utils.RemoveShadow(this);
         innerView.Children.Add(this.detailsText);
         Canvas.SetLeft(this.detailsText, 100);

      }

      public override void addFilter(object attTag, ScatterViewItem filterTile)
      {
         double y = (40 * (this.attributes.Count)) - 10;
         Canvas.SetRight(filterTile, 105);
         Canvas.SetTop(filterTile, y);

         attributes.Add((String)attTag);
      }

      public override void removeFilter(object filt)
      {
          String str = filt as String;
 
         if (attributes.Contains(str))
         {
            attributes.Remove(str);
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
             new RectangleGeometry(new Rect((this.ActualWidth / 4), (1 * this.ActualHeight / 3), 1, 1));
         cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);

         return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
      }

      public String getDetails(Entry entry)
      {
         String str = "";
         foreach (String att in this.attributes)
         {
            str += entry.printAttribute(att) + "\n";
         }

         return str;
      }
   }
}



