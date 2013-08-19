using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;


namespace HabilisX
{
   class IntFilter : iFilter
   {
      public enum Quantifier { LESSTHAN, GREATERTHAN, EQUALTO }
      public int query;
      public string attribute;
      public Quantifier q;

      public IntFilter(int query, Char q, String attribute)
      {
         this.query = query;
         this.attribute = attribute;
         switch (q)
         {
            case '>':
               this.q = Quantifier.GREATERTHAN;
               break;
            case '<':
               this.q = Quantifier.LESSTHAN;
               break;
            case '=':
               this.q = Quantifier.EQUALTO;
               break;
         }
      }

      public bool Matches(Entry entry)
      {
         if (!entry.attributes.Keys.Contains(this.attribute))
         {
            return false;
         }
         int value = (int)entry.attributes[this.attribute];
         switch (this.q)
         {
            case Quantifier.LESSTHAN:
               return value < query;
            case Quantifier.GREATERTHAN:
               return value > query;
            case Quantifier.EQUALTO:
               return value == query;
            default:
               return false;
         }
      }

      public string getQueryString()
      {

         return this.q.ToString().ToLower() + " " + this.query;
      }

      public Brush getColor()
      {
         return new SolidColorBrush(Color.FromRgb(191,191,191));
      }
   }
}
