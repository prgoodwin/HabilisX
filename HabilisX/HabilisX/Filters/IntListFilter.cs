using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;



namespace HabilisX
{
   class IntListFilter : iFilter
   {

      public enum Quantifier { LESSTHAN, GREATERTHAN, EQUALTO }
      public int query;
      public string attribute;
      public Quantifier q;


      public IntListFilter(int query, Char q, String attribute)
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
         List<int> values = (List<int>)entry.attributes[this.attribute];
         foreach (int value in values) {
            if (this.q == Quantifier.LESSTHAN && value < query) 
            {
               return true;
            }
            else if (this.q == Quantifier.GREATERTHAN && value > query) 
            {
               return true;
            }
            else if (this.q == Quantifier.EQUALTO && value == query) 
            {
               return true;
            }
         }

         return false;
      }

      public string getQueryString()
      {

         return this.q.ToString().ToLower() + " " + this.query;
      }


      public Brush getColor()
      {
         return Brushes.SlateGray;
      }
   }
}
