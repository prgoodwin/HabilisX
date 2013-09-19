using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;


namespace HabilisX
{
   class DateFilter : iFilter
   {
      public enum Quantifier { BEFORE, AFTER, IN }
      public DateTime query;
      public string attribute;
      public Quantifier q;

      public DateFilter(DateTime query, Char q, String attribute) {
         this.query = query;
         this.attribute = attribute;
         switch (q)
         {
            case '>':
               this.q = Quantifier.AFTER;
               break;
            case '<':
               this.q = Quantifier.BEFORE;
               break;
            case '=':
               this.q = Quantifier.IN;
               break;
         }
      }

      public bool Matches(Entry entry) {
         if (!entry.attributes.Keys.Contains(this.attribute)) {
            return false;
         }
         DateTime date = (DateTime)entry.attributes[this.attribute];
         switch(this.q){
            case Quantifier.BEFORE:
               return (query.CompareTo(date) > 0);
            case Quantifier.AFTER:
               return (query.CompareTo(date) < 0);
            case Quantifier.IN:
               return (query.CompareTo(date) == 0);
            default:
               return false;
         }
      }

      public string getQueryString() {
         
         return this.q.ToString().ToLower() + " " + this.query.Year;
      }

      public Brush getColor() {
         return Brushes.LightGray;
      }
   }
}
