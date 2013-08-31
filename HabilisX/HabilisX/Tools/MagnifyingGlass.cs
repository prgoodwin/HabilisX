using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Surface.Presentation.Controls
{
   public class MagnifyingGlass : ScatterViewItem
   {
      public List<String> attributes = new List<String>();
      public Label detailsText = new Label();

      public MagnifyingGlass()
      {
         detailsText.FontSize = 24;
        this.Background = Brushes.Transparent;
        this.CanRotate = false;
         this.Center = new Point(275, 450);
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

      public void addAttribute(String str)
      {
         attributes.Add(str);
      }

      public void removeAttribute(String str)
      {
         if (attributes.Contains(str))
         {
            attributes.Remove(str);
         }
      }

      public ScatterViewItem activateMagnifyingGlassFilter(FilterTile tile)
      {


          //Make label out of title
          Label filterTitle = new Label();
          filterTitle.Content = tile.attTag.ToLower();
          filterTitle.Foreground = Brushes.White;



          //Make item that will be attached
          ScatterViewItem Filter = new ScatterViewItem();
          Filter.Tag = tile.attTag;

          //Format visually
          Filter.Background = tile.Background;
          Filter.ShowsActivationEffects = false;
          Filter.MinHeight = 0;
          Filter.Height = 35;
          double y = (40 * (this.attributes.Count - 1)) + 10;

          //Attach 
          Filter.Content = filterTitle;

          ((Canvas)(this.Content)).Children.Add(Filter);
          Canvas.SetRight(Filter, 105);
          Canvas.SetTop(Filter, y);

          this.addAttribute(tile.attTag);

          return Filter;

      }

      public bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
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



