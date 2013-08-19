using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;


namespace HabilisX
{
   class StringListFilter : iFilter
   {
     public String query;
     public String attribute;
      
      public StringListFilter(String query, String attribute) {
         this.query = query.ToLower();
         this.attribute = attribute;
      }

      public bool Matches(Entry entry) {
         if (!entry.attributes.Keys.Contains(this.attribute))
         {
            return false;
         }

         List<String> lst = (List<String>)entry.attributes[attribute];
         for (int i = 0; i < lst.Count; i++ ) {
            lst[i] = lst[i].ToLower();
         }

         return lst.Contains(query);
      }

      public string getQueryString() {
         return query;
      }

      public Brush getColor() {
         return Brushes.SlateGray;      
      }
   }
}
