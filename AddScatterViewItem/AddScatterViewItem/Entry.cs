using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddScatterViewItem
{
   class Entry
   {
      public String title;
      public List<String> Authors;
      public DateTime publicationDate;
      public String venue;
      public String paperAbstract;
      public int pages;
      public List<String> references;

      private const int CHARLIMITPERLINE = 40;


      public Entry(String title, List<String> Authors, DateTime publicationDate,
                  String venue, String paperAbstract, int pages, List<String> references)
      {

         this.title = title;
         this.Authors = Authors;
         this.publicationDate = publicationDate;
         this.venue = venue;
         this.paperAbstract = paperAbstract;
         this.pages = pages;
         this.references = references;

      }

      public Entry(String title, List<String> Authors, DateTime publicationDate) {

         this.title = title;
         this.Authors = Authors;
         this.publicationDate = publicationDate;
         this.venue = null;
         this.paperAbstract = null;
         this.pages = 0;
         this.references = null;
      
      }

      public String toString() {
         String str = "";

         //title(year)
         String titleString = title + " (" + publicationDate.Year + ")";

         //author, author, author
         String authorString = Authors[0];
         for (int i = 1; i < Authors.Count; i++) {
            authorString += ", ";
            authorString += Authors[i];
         }

         //title(year)
         //
         //author, author, author
         str += this.addLineBreaks(titleString);
         str += "\n\n";
         str += this.addLineBreaks(authorString);

         return str;      
      }


      public String addLineBreaks(String oldStr) {
         StringBuilder newStr = new StringBuilder(oldStr);

         
         //
         for (int lineBeginning = CHARLIMITPERLINE; lineBeginning < newStr.Length; lineBeginning += CHARLIMITPERLINE)
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
   }


}
