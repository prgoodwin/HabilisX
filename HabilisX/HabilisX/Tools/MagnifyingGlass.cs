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
    public class MagnifyingGlass : Tool
    
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
          Label label = new Label();
          label.Content = tile.attTag.ToLower();
          label.Foreground = Brushes.White;



          //Make item that will be attached
          ScatterViewItem filterTile = new ScatterViewItem();
          filterTile.PreviewMouseDown +=new System.Windows.Input.MouseButtonEventHandler(filterTile_PreviewMouseDown);
         
          filterTile.Tag = tile.attTag;
          filterTile.Background = tile.Background;
          filterTile.ShowsActivationEffects = false;
          filterTile.MinHeight = 0;
          filterTile.Height = 35;
          double y = (40 * (this.attributes.Count - 1)) + 10;

          //Attach 
          filterTile.Content = label;

          ((Canvas)(this.Content)).Children.Add(filterTile);
          Canvas.SetRight(filterTile, 105);
          Canvas.SetTop(filterTile, y);

          this.addAttribute(tile.attTag);

          return filterTile;

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



