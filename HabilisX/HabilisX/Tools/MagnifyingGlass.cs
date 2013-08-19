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



