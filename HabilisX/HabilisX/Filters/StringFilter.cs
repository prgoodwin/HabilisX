using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;



namespace HabilisX
{
   class StringFilter : iFilter
   {
      public String query;
      public String attribute;
      
      public StringFilter(String query, String attribute) {
         this.query = query.ToLower();
         this.attribute = attribute;
      }

      public bool Matches(Entry entry) {
         if (!entry.attributes.Keys.Contains(this.attribute))
         {
            return false;
         }

         String str = (String)entry.attributes[attribute];
         return (str.ToLower()).Contains(query);
      }

      public string getQueryString() {
         return query;
      }

      public Brush getColor() {
         return Brushes.DarkSlateGray;
      }
   }
}
