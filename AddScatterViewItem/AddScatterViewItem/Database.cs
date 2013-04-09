using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddScatterViewItem
{
   class Database
   {
      public List<Entry> allPapers;

      public Database() {
         allPapers = new List<Entry>();
      
      }

      public void addPaper(Entry paper) {
         allPapers.Add(paper);
      }

   }
}
