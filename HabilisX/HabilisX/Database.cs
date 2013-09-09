using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;

namespace HabilisX
{
   class Database
   {
      public List<Entry> allEntries;
      public Dictionary<String,Type> allAttributes;

      public Database()
      {
         allEntries = new List<Entry>();
         allAttributes = new Dictionary<String, Type>();
         this.init();
      }

      public void addEntry(Entry entry)
      {
         foreach (String key in entry.attributes.Keys)
         {
            if (!(this.allAttributes.Keys).Contains(key))
            {
               Type objectType = (entry.attributes[key]).GetType();
               allAttributes.Add(key, objectType);
            }
         }
         allEntries.Add(entry);
      }

      private void init()
      {
         String title = "Improving Continuous Gesture Recognition with spoken Prosody";
         List<String> authors = new List<String>();
         authors.Add("Kettebekov");
         authors.Add("Sanshzar");
         DateTime pubDate = new DateTime(2003, 1, 1);
         List<int> intList = new List<int>();
         intList.Add(1);
         intList.Add(2);
         intList.Add(3);

         Entry paper1 = new Entry();
         paper1.addAttribute("title", title);
         paper1.addAttribute("authors", authors);
         paper1.addAttribute("publicationDate", pubDate);
         paper1.addAttribute("pages", 1);
         paper1.addAttribute("abstract", "This is the abstract");
         this.addEntry(paper1);

         title = "High level data fusion on a multimodal interactive application platform";
         authors = new List<String>();
         authors.Add("Mendonça");
         pubDate = new DateTime(2009, 1, 1);

         Entry paper2 = new Entry();
         paper2.addAttribute("title", title);
         paper2.addAttribute("authors", authors);
         paper2.addAttribute("publicationDate", pubDate);
         paper2.addAttribute("pages", 2);
         this.addEntry(paper2);

         title = "Spoken and Multimodal Communication Systems in Mobile Settings";
         authors = new List<String>();
         authors.Add("Turunen");
         authors.Add("Hakulinen");
         pubDate = new DateTime(2007, 1, 1);


         Entry paper3 = new Entry();
         paper3.addAttribute("title", title);
         paper3.addAttribute("authors", authors);
         paper3.addAttribute("publicationDate", pubDate);
         paper3.addAttribute("pages", 3);
         this.addEntry(paper3);

         title = "Put-that-there”: Voice and gesture at the graphics interface";
         authors.Clear();
         authors.Add("Bolt");
         pubDate = new DateTime(1980, 1, 1);

         Entry paper4 = new Entry();
         paper4.addAttribute("title", title);
         paper4.addAttribute("authors", authors);
         paper4.addAttribute("publicationDate", pubDate);
         paper4.addAttribute("pages", 4);
         this.addEntry(paper4);

         title = "QuickSet: Multimodal Interaction for Distributed Applications";
         authors = new List<String>();
         authors.Add("Cohen");
         authors.Add("Johnston");
         authors.Add("Mcgee");
         authors.Add("Oviatt");
         authors.Add("Pittman");
         authors.Add("Smith");
         authors.Add("Chen");
         authors.Add("Clow");
         pubDate = new DateTime(1997, 1, 1);

         Entry paper5 = new Entry();
         paper5.addAttribute("title", title);
         paper5.addAttribute("authors", authors);
         paper5.addAttribute("publicationDate", pubDate);
         paper5.addAttribute("pages", 5);
         this.addEntry(paper5);

         title = "What is that? Gesturing to determine device identity";
         authors = new List<String>();
         authors.Add("Swindells");
         authors.Add("Inkpen");
         pubDate = new DateTime(2002, 1, 1);

         Entry paper6 = new Entry();
         paper6.addAttribute("title", title);
         paper6.addAttribute("authors", authors);
         paper6.addAttribute("publicationDate", pubDate);
         paper6.addAttribute("pages", 6);
         this.addEntry(paper6);
         this.addEntry(new Entry());

         title = "100,000,000 taps: analysis and improvement of touch performance in the large";
         authors = new List<String>();
         authors.Add("Henze");
         authors.Add("Rukzio");
         authors.Add("Boll");
         pubDate = new DateTime(2011, 1, 1);


         Entry paper7 = new Entry();
         paper7.addAttribute("title", title);
         paper7.addAttribute("authors", authors);
         paper7.addAttribute("publicationDate", pubDate);
         paper7.addAttribute("pages", 7);
         this.addEntry(paper7);

         title = "UI on the Fly: Generating a Multimodal User Interface";
         authors = new List<String>();
         authors.Add("Reitter");
         authors.Add("Panttaja");
         pubDate = new DateTime(2004, 1, 1);

         Entry paper8 = new Entry();
         paper8.addAttribute("title", title);
         paper8.addAttribute("authors", authors);
         paper8.addAttribute("publicationDate", pubDate);
         paper8.addAttribute("pages", 8);
         this.addEntry(paper8);

         title = "AirMouse: Finger Gesture for 2D and 3D Interaction";
         authors = new List<String>();
         authors.Add("Ortega");
         authors.Add("Nigay");
         pubDate = new DateTime(2009, 1, 1);

         Entry paper9 = new Entry();
         paper9.addAttribute("title", title);
         paper9.addAttribute("authors", authors);
         paper9.addAttribute("publicationDate", pubDate);
         paper9.addAttribute("pages", 9);
         this.addEntry(paper9);

         title = "SiMPE: 7th Workshop on Speech and Sound in Mobile and Pervasive Environments";
         authors = new List<String>();
         authors.Add("Nanavati");
         authors.Add("Rajput");
         authors.Add("Rudnicky");
         authors.Add("Turunen");
         authors.Add("Sandholm");
         authors.Add("Munteanu");
         authors.Add("Penn");
         pubDate = new DateTime(2012, 1, 1);

         Entry paper10 = new Entry();
         paper10.addAttribute("title", title);
         paper10.addAttribute("authors", authors);
         paper10.addAttribute("publicationDate", pubDate);
         paper10.addAttribute("pages", 10);
         this.addEntry(paper10);

         for (int i = 0; i < 90; i++) {
             Entry paper = new Entry();
             paper.addAttribute("title", title);
             paper.addAttribute("authors", authors);
             paper.addAttribute("publicationDate", pubDate);
             paper.addAttribute("pages", 10);
             this.addEntry(paper);

         }
      }
   }
}
