using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Surface.Presentation.Controls
{


   public class Entry : ScatterViewItem
   {
      public Dictionary<String, object> attributes = new Dictionary<String,object>();

      private const int CHARSPERLINE = 40;
      private Label L;

      public Entry() {
          L = new Label();
          // Set the content of the label.
          L.Content = this.toString();
          L.FontSize = 12;



          Canvas innerView = new Canvas();
          innerView.Background = Brushes.Transparent;
          innerView.Children.Add(L);


          this.Width = 260;
          this.Height = 170;
          this.Content = innerView;
          this.Center = new Point(this.getNewX(), this.getNewY());
          this.Orientation = this.getNewOrientation();
          this.Tag = 0; //highlighting off to begin
      }

      public Entry(Dictionary<String, object> attributes) {
         this.attributes = attributes;
      }

      public void addAttribute(String key, object value) {
         attributes.Add(key, value);
         L.Content = this.toString();

      }

      public void addAllAttributes(Dictionary<String,object> attributes)
      {
         this.attributes = attributes;
         L.Content = this.toString();
      }

      public bool matchesAllFilters(List<iFilter> filters)
      {
         if (filters.Count == 0)
         {
            return false;
         }

         foreach (iFilter filter in filters)
         {
            if (!filter.Matches(this))
            {
               return false;
            }
         }
         return true;
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
      public String printStringAttribute(String att)
      {
         String attribute = (String)(attributes[att]);

         if (attribute.Length != 0)
         {
            return this.addLineBreaks(att + ": " + attribute);
         }
         else
         {
            return att + ": null";
         }
      }

      private String printStringListAttribute(String att)
      {
         List<String> attribute = (List<String>)attributes[att];
         if (attribute.Count == 0)
         {
            return att + ": null";
         }
         else
         {
            String str = att + ": " + attribute[0];

            for (int i = 1; i < attribute.Count; i++)
            {
               str += ", " + attribute[i];
            }

            return this.addLineBreaks(str);
         }
      }

      private String printIntListAttribute(String att)
      {
         List<int> attribute = (List<int>)attributes[att];
         if (attribute.Count == 0)
         {
            return att + ": null";
         }
         else
         {
            String str = att + ": " + attribute[0];

            for (int i = 1; i < attribute.Count; i++)
            {
               str += ", " + attribute[i];
            }

            return this.addLineBreaks(str);
         }
      }


      private String printDateAttribute(String att)
      {
         DateTime publicationDate = (DateTime)attributes[att];
         if (publicationDate.Year == 0)
         {
            return att + ": null";
         }
         else
         {
            return att + ": " + publicationDate.Year;
         }
      }

      private String printIntAttribute(String att)
      {
         return att + ": " + attributes[att].ToString();
      }

      public String printAttribute(String att)
      {
         if (!this.attributes.Keys.Contains(att))
         {
            return att + ": null";
         }

         object value = this.attributes[att];
         if (value is String)
         {
            return printStringAttribute(att);
         }
         else if (value is DateTime) {
            return printDateAttribute(att);
         }
         else if (value is List<String>) {
            return printStringListAttribute(att);
         }
         else if (value is int) 
         {
            return printIntAttribute(att);
         }
         else if (value is List<int>) 
         {
            return printIntListAttribute(att);
         }
         else
         {
            return "attribute type not recognized";
         }


      }

      public String toString() {
         String str = "";

         foreach (String attribute in this.attributes.Keys) {
            str += this.printAttribute(attribute);
            str += "\n\n";
         
         }

         return str;      
      }


      public String addLineBreaks(String oldStr) {
         StringBuilder newStr = new StringBuilder(oldStr);

         
         for (int lineBeginning = CHARSPERLINE; lineBeginning < newStr.Length; lineBeginning += CHARSPERLINE)
         {
            int insertPoint = lineBeginning;
            while (newStr[insertPoint] != ' ')
            {
               insertPoint--;
            }
            newStr.Insert(insertPoint + 1, "\n");
            lineBeginning = insertPoint + 2;
         }


         return newStr.ToString();

      }

      private double getNewX()
      {
          return HabilisX.Utils.nextNum(400, 1200);
      }
      private double getNewY()
      {
          return HabilisX.Utils.nextNum(200, 650);
      }
      private double getNewOrientation()
      {
          return  HabilisX.Utils.nextNum(-60, 60);
      }

   }

}
