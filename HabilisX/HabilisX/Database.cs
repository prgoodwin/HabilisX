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
        public Dictionary<String, Type> allAttributes;

        public Database()
        {
            allEntries = new List<Entry>();
            allAttributes = new Dictionary<String, Type>();
            //this.newInit();
            //newInit();
            parseFromBibtex("HabilisX.Resources.bibtexRelated.txt");
        }

        public Database(String path) {

            allEntries = new List<Entry>();
            allAttributes = new Dictionary<String, Type>();
            //this.newInit();
            //newInit();
            parseFromBibtex(path);
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


        private void parseFromBibtex(String path)
        {
            string[] text = { "a", "b" };
            try
            {
                text = Utils.NewEmbededTextFile(path);
            }
            catch(System.ArgumentNullException e) {
                Console.WriteLine("Embedded file came back null, trying a hard coded path...");
                try
                {
                    text = System.IO.File.ReadAllLines(path);
                }
                catch (Exception ex) {
                    Console.WriteLine("ERROR READING FILE IN DATABASE: " + ex);
                }
            }
            Entry entry = new Entry();
            int entries = 0;
            //foreach (String str in text)
            for(int i=0; i<text.Length; i++)
            {
                //Console.WriteLine(i);
                String cur = text[i].Trim();
                if (cur.Length > 0 && cur[0] == '@')
                {
                    entries++;
                }
                else if (cur.Length > 0 && cur[0] == '}')
                {
                    this.addEntry(entry);
                    entry = new Entry();
                }
                else if (cur.Length > 0 && cur.Contains('='))
                {
                   String[] ignoreAtts = { "url", "isbn", "location", "acmid", "publisher", "address", "doi", 
                                            "issn", "pages", "articleno" };
                   int index = cur.IndexOf('=');
                   String attName = cur.Substring(0, index).Trim();
                   String attValue = cur.Substring(index + 1).Replace('{', ' ').Replace('}', ' ').Trim();
                   if (!ignoreAtts.Contains(attName))
                   {
                      if (attValue[attValue.Length - 1] == ',')
                      {
                         attValue = attValue.Substring(0, attValue.Length - 1).Trim();
                      }
                      //Console.WriteLine("\"" + attName + "\"" + " " + "\"" + attValue + "\"");
                      int number;
                      bool result = Int32.TryParse(attValue, out number);
                      if (result)
                      {
                         entry.addAttribute(attName, number);
                      }
                      else
                      {
                         entry.addAttribute(attName, attValue);
                      }
                   }
                }
            }

            //this.addEntry(entry);
           // Console.WriteLine("ENTRIES: " + entries);
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

            for (int i = 0; i < 90; i++)
            {
                Entry paper = new Entry();
                paper.addAttribute("title", title);
                paper.addAttribute("authors", authors);
                paper.addAttribute("publicationDate", pubDate);
                paper.addAttribute("pages", 10);
                this.addEntry(paper);

            }
        }

        private void newInit()
        {
            List<String> author = new List<String>();
            String title = "";
            String booktitle = "";
            String series = "";
            DateTime year;
            int numpages = 0;
            List<String> keywords = new List<String>();
            Entry entry = new Entry();

            author = new List<String>();
            author.Add("Accot");
            author.Add("Johnny and Zhai");
            author.Add("Shumin");
            title = "More than dotting the i's --- foundations for crossing-based interfaces";
            booktitle = "Proceedings of the SIGCHI Conference on Human Factors in Computing Systems";
            series = "CHI '02";
            year = new DateTime(2002, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("Fitts' law");
            keywords.Add("events");
            keywords.Add("goal crossing");
            keywords.Add("goal passing");
            keywords.Add("graphical user interfaces");
            keywords.Add("input");
            keywords.Add("input performance");
            keywords.Add("interaction techniques");
            keywords.Add("pointing"); keywords.Add("widgets");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Apitz");
            author.Add("Georg and Guimbretiere");
            author.Add("Francois");
            title = "CrossY: a crossing-based drawing application";
            booktitle = "Proceedings of the 17th annual ACM symposium on User interface software and technology";
            series = "UIST '04";
            year = new DateTime(2004, 1, 1);
            numpages = 10;
            keywords = new List<String>();
            keywords.Add("command composition");
            keywords.Add("crossing based interfaces");
            keywords.Add("fluid interaction");
            keywords.Add("pen-computing");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Bauer");
            author.Add("Dan and Fastrez");
            author.Add("Pierre and Hollan");
            author.Add("Jim");
            title = "Computationally-Enriched 'Piles' for Managing Digital Photo Collections";
            booktitle = "Proceedings of the 2004 IEEE Symposium on Visual Languages - Human Centric Computing";
            series = "VLHCC '04";
            year = new DateTime(2004, 1, 1);
            numpages = 3;


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            this.addEntry(entry);





            author = new List<String>();
            author.Add("Beaudouin-Lafon");
            author.Add("Michel");
            title = "Novel interaction techniques for overlapping windows";
            booktitle = "Proceedings of the 14th annual ACM symposium on User interface software and technology";
            series = "UIST '01";
            year = new DateTime(2001, 1, 1);
            numpages = 2;
            keywords = new List<String>();
            keywords.Add("interaction technique");
            keywords.Add("window management");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Bederson");
            author.Add("Benjamin B. and Hollan");
            author.Add("James D.");
            title = "Pad++: a zooming graphical interface for exploring alternate interface physics";
            booktitle = "Proceedings of the 7th annual ACM symposium on User interface software and technology";
            series = "UIST '94";
            year = new DateTime(1994, 1, 1);

            numpages = 10;
            keywords = new List<String>();
            keywords.Add("authoring");
            keywords.Add("hypertext");
            keywords.Add("information navigation");
            keywords.Add("information physics");
            keywords.Add("information visualization");
            keywords.Add("interactive user interfaces");
            keywords.Add("multiscale interfaces");
            keywords.Add("zooming interfaces");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Bell");
            author.Add("Blaine and Feiner");
            author.Add("Steven and Hollerer");
            author.Add("Tobias");
            title = "View management for virtual and augmented reality";
            booktitle = "Proceedings of the 14th annual ACM symposium on User interface software and technology";
            series = "UIST '01";
            year = new DateTime(2001, 1, 1);

            numpages = 10; keywords = new List<String>();
            keywords.Add("annotation");
            keywords.Add("augmented reality");
            keywords.Add("environment management");
            keywords.Add("labeling");
            keywords.Add("view management");
            keywords.Add("virtual environments");
            keywords.Add("wearable computing");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Carpendale");
            author.Add("M. S. T. and Montagnese");
            author.Add("Catherine");
            title = "A framework for unifying presentation space";
            booktitle = "Proceedings of the 14th annual ACM symposium on User interface software and technology";
            series = "UIST '01";
            year = new DateTime(2001, 1, 1);
            numpages = 10;
            keywords = new List<String>();
            keywords.Add("3D interactions");
            keywords.Add("Distortion viewing");
            keywords.Add("information visualization");
            keywords.Add("interface design issues");
            keywords.Add("interface metaphors");
            keywords.Add("screen layout");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Chang");
            author.Add("Bay-Wei and Ungar");
            author.Add("David");
            title = "Animation: from cartoons to the user interface";
            booktitle = "Proceedings of the 6th annual ACM symposium on User interface software and technology";
            series = "UIST '93";
            year = new DateTime(1993, 1, 1);

            numpages = 11;
            keywords = new List<String>();
            keywords.Add("Self");
            keywords.Add("animation");
            keywords.Add("cartoons");
            keywords.Add("motion blur");
            keywords.Add("user interfaces");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Denoue");
            author.Add("Laurent and Nelson");
            author.Add("Les and Churchill");
            author.Add("Elizabeth");
            title = "A fast, interactive 3D paper-flier metaphor for digital bulletin boards";
            booktitle = "Proceedings of the 16th annual ACM symposium on User interface software and technology";
            series = "UIST '03";
            year = new DateTime(2003, 1, 1);
            numpages = 4;


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            this.addEntry(entry);






            author = new List<String>();
            author.Add("DiGioia");
            author.Add("Paul and Dourish");
            author.Add("Paul");
            title = "Social navigation as a model for usable security";
            booktitle = "Proceedings of the 2005 symposium on Usable privacy and security";
            series = "SOUPS '05";
            year = new DateTime(2005, 1, 1);

            numpages = 8;
            keywords = new List<String>();
            keywords.Add("collaborative interfaces");
            keywords.Add("peer-to-peer filesharing");
            keywords.Add("social navigation");
            keywords.Add("visualization");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Dragicevic Pierre");
            title = "Combining crossing-based and paper-based interaction paradigms for dragging and dropping between overlapping windows";
            booktitle = "Proceedings of the 17th annual ACM symposium on User interface software and technology";
            series = "UIST '04";
            year = new DateTime(2004, 1, 1);

            numpages = 4;
            keywords = new List<String>();
            keywords.Add("crossing-based interfaces");

            keywords.Add("drag-and-drop");
            keywords.Add("gestural interaction");
            keywords.Add("paper-based metaphors");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Fitzmaurice");
            author.Add("George and Khan");
            author.Add("Azam and Pieke");
            author.Add("Robert and Buxton");
            author.Add("Bill and Kurtenbach");
            author.Add("Gordon");
            title = "Tracking menus";
            booktitle = "Proceedings of the 16th annual ACM symposium on User interface software and technology";
            series = "UIST '03";
            year = new DateTime(2003, 1, 1);
            numpages = 9;
            keywords = new List<String>();
            keywords.Add("floating palette");
            keywords.Add("graphical user interface");
            keywords.Add("menu system");
            keywords.Add("pen based user interfaces");
            keywords.Add("tablet PC");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Forlines");
            author.Add("Clifton and Shen");
            author.Add("Chia and Buxton");
            author.Add("Bill");
            title = "Glimpse: a novel input model for multi-level devices";
            booktitle = "CHI '05 Extended Abstracts on Human Factors in Computing Systems";
            series = "CHI EA '05";
            year = new DateTime(2005, 1, 1);
            numpages = 4;
            keywords = new List<String>();
            keywords.Add("direct manipulation");
            keywords.Add("navigation");
            keywords.Add("pressure sensitive input");
            keywords.Add("stylus");
            keywords.Add("three-state input");
            keywords.Add("touch screens");
            keywords.Add("undo");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Gonzalez");
            author.Add("Cleotilde");
            title = "Does animation in user interfaces improve decision making?";
            booktitle = "Proceedings of the SIGCHI Conference on Human Factors in Computing Systems";
            series = "CHI '96";
            year = new DateTime(1996, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("animation");
            keywords.Add("decision making");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Grossman");
            author.Add("Tovi and Balakrishnan");
            author.Add("Ravin and Kurtenbach");
            author.Add("Gordon and Fitzmaurice");
            author.Add("George and Khan");
            author.Add("Azam and Buxton");
            author.Add("Bill");
            title = "Interaction techniques for 3D modeling on large displays";
            booktitle = "Proceedings of the 2001 symposium on Interactive 3D graphics";
            series = "I3D '01";
            year = new DateTime(2001, 1, 1);


            numpages = 7;
            keywords = new List<String>();
            keywords.Add("3D modeling");
            keywords.Add("interaction techniques");
            keywords.Add("large scale displays");
            keywords.Add("tape drawing");
            keywords.Add("two-handed input");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Herndon");
            author.Add("Kenneth P. and Zeleznik");
            author.Add("Robert C. and Robbins");
            author.Add("Daniel C. and Conner");
            author.Add("D. Brookshire and Snibbe");
            author.Add("Scott S. and van Dam");
            author.Add("Andries");
            title = "Interactive shadows";
            booktitle = "Proceedings of the 5th annual ACM symposium on User interface software and technology";
            series = "UIST '92";
            year = new DateTime(1992, 1, 1);
            numpages = 6;
            keywords = new List<String>();
            keywords.Add("3D widgets");
            keywords.Add("direct manipulation");
            keywords.Add("interactive systems");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);



            author = new List<String>();
            author.Add("Hinckley");
            author.Add("Ken and Baudisch");
            author.Add("Patrick and Ramos");
            author.Add("Gonzalo and Guimbretiere");
            author.Add("Francois");
            title = "Design and analysis of delimiters for selection-action pen gesture phrases in scriboli";
            booktitle = "Proceedings of the SIGCHI Conference on Human Factors in Computing Systems";
            series = "CHI '05";
            year = new DateTime(2005, 1, 1);
            numpages = 10;
            keywords = new List<String>();
            keywords.Add("delimiters");
            keywords.Add("gestures");
            keywords.Add("marking");
            keywords.Add("pen input");
            keywords.Add("tablets");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Kurtenbach");
            author.Add("Gordon and Buxton");
            author.Add("William");
            title = "Issues in combining marking and direct manipulation techniques";
            booktitle = "Proceedings of the 4th annual ACM symposium on User interface software and technology";
            series = "UIST '91";
            year = new DateTime(1991, 1, 1);
            numpages = 8;

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            this.addEntry(entry);







            author = new List<String>();
            author.Add("Malone");
            author.Add("Thomas W.");
            title = "How do people organize their desks?: Implications for the design of office information systems";
            year = new DateTime(1983, 1, 1);
            numpages = 14;


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            this.addEntry(entry);






            author = new List<String>();
            author.Add("Mander");
            author.Add("Richard and Salomon");
            author.Add("Gitta and Wong");
            author.Add("Yin Yin");
            title = "A \"pile\" metaphor for supporting casual organization of information";
            booktitle = "Proceedings of the SIGCHI Conference on Human Factors in Computing Systems";
            series = "CHI '92";
            year = new DateTime(1992, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("design process");
            keywords.Add("desktop metaphor");
            keywords.Add("end-user programming");
            keywords.Add("information organization");
            keywords.Add("information visualization");
            keywords.Add("interactive systems");
            keywords.Add("interface design");
            keywords.Add("interface metaphors");
            keywords.Add("pile metaphor");
            keywords.Add("user observation");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Miller");
            author.Add("Lynn");
            title = "Case Study of Customer Input For a Successful Product";
            booktitle = "Proceedings of the Agile Development Conference";
            series = "ADC '05";
            year = new DateTime(2005, 1, 1);


            numpages = 10;



            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            this.addEntry(entry);





            author = new List<String>();
            author.Add("Pook");
            author.Add("Stuart and Lecolinet");
            author.Add("Eric and Vaysseix");
            author.Add("Guy and Barillot");
            author.Add("Emmanuel");
            title = "Control menus: excecution and control in a single interactor";
            booktitle = "CHI '00 Extended Abstracts on Human Factors in Computing Systems";
            series = "CHI EA '00";
            year = new DateTime(2000, 1, 1);
            numpages = 2; keywords = new List<String>();
            keywords.Add("gestures");
            keywords.Add("interaction");
            keywords.Add("interactors");
            keywords.Add("marking menus");
            keywords.Add("menu access");
            keywords.Add("user interface design");
            keywords.Add("zoomable user interfaces");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Ramos");
            author.Add("Gonzalo and Boulos");
            author.Add("Matthew and Balakrishnan");
            author.Add("Ravin");
            title = "Pressure widgets";
            booktitle = "Proceedings of the SIGCHI Conference on Human Factors in Computing Systems";
            series = "CHI '04";
            year = new DateTime(2004, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("isometric input");
            keywords.Add("pen-based interfaces");
            keywords.Add("pressure input");
            keywords.Add("pressure widgets");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Robertson");
            author.Add("George and Czerwinski");
            author.Add("Mary and Larson");
            author.Add("Kevin and Robbins");
            author.Add("Daniel C. and Thiel");
            author.Add("David and van Dantzich");
            author.Add("Maarten");
            title = "Data mountain: using spatial memory for document management";
            booktitle = "Proceedings of the 11th annual ACM symposium on User interface software and technology";
            series = "UIST '98";
            year = new DateTime(1998, 1, 1);
            numpages = 10; keywords = new List<String>();
            keywords.Add("3D user interfaces");
            keywords.Add("desktop VR");
            keywords.Add("document mangement");
            keywords.Add("information visualization");
            keywords.Add("spatial cognition");
            keywords.Add("spatial memory");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);


            author = new List<String>();
            author.Add("Robertson");
            author.Add("George and van Dantzich");
            author.Add("Maarten and Robbins");
            author.Add("Daniel and Czerwinski");
            author.Add("Mary and Hinckley");
            author.Add("Ken and Risden");
            author.Add("Kirsten and Thiel");
            author.Add("David and Gorokhovsky");
            author.Add("Vadim");
            title = "The Task Gallery: a 3D window manager";
            booktitle = "Proceedings of the SIGCHI conference on Human Factors in Computing Systems";
            series = "CHI '00";
            year = new DateTime(2000, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("3D user interfaces");
            keywords.Add("spatial cognition");
            keywords.Add("spatial memory");
            keywords.Add("window managers");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Robertson");

            author.Add("George G. and Mackinlay");
            author.Add("Jock D. and Card");
            author.Add("Stuart K.");
            title = "Cone Trees: animated 3D visualizations of hierarchical information";
            booktitle = "Proceedings of the SIGCHI Conference on Human Factors in Computing Systems";
            series = "CHI '91";
            year = new DateTime(1991, 1, 1);

            numpages = 6;


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            this.addEntry(entry);






            author = new List<String>();
            author.Add("Sellen");
            author.Add("Abigail J. and Harper");
            author.Add("Richard H.R.");
            title = "The Myth of the Paperless Office";
            year = new DateTime(2003, 1, 1);



            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("year", year);
            this.addEntry(entry);



            author = new List<String>();
            author.Add("Sonnet");
            author.Add("Henry and Carpendale");
            author.Add("Sheelagh and Strothotte");
            author.Add("Thomas");
            title = "Integrating expanding annotations with a 3D explosion probe";
            booktitle = "Proceedings of the working conference on Advanced visual interfaces";
            series = "AVI '04";
            year = new DateTime(2004, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("3D model exploration");
            keywords.Add("expanding annotations");
            keywords.Add("explosion diagram");
            keywords.Add("interaction design");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Sonnet");
            author.Add("Henry and Carpendale");
            author.Add("Sheelagh and Strothotte");
            author.Add("Thomas");
            title = "Integrating expanding annotations with a 3D explosion probe";
            booktitle = "Proceedings of the working conference on Advanced visual interfaces";
            series = "AVI '04";
            year = new DateTime(2004, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("3D model exploration");
            keywords.Add("expanding annotations");
            keywords.Add("explosion diagram");
            keywords.Add("interaction design");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Streitz");
            author.Add("Norbert A. and Geissler");
            author.Add("Jorg and Holmer");
            author.Add("Torsten and Konomi");
            author.Add("Shin'ichi and M\"uller-Tomfelde");
            author.Add("Christian and Reischl");
            author.Add("Wolfgang and Rexroth");
            author.Add("Petra and Seitz");
            author.Add("Peter and Steinmetz");
            author.Add("Ralf");
            title = "i-LAND: an interactive landscape for creativity and innovation";
            booktitle = "Proceedings of the SIGCHI conference on Human Factors in Computing Systems";
            series = "CHI '99";
            year = new DateTime(1999, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("CSCW");
            keywords.Add("architectural space");
            keywords.Add("augmented reality");
            keywords.Add("cooperative rooms");
            keywords.Add("creativity support");
            keywords.Add("dynmic team work");
            keywords.Add("integrated design");
            keywords.Add("interactive landscape");
            keywords.Add("roomware");
            keywords.Add("ubiquitous computing");
            keywords.Add("virtual information space");
            keywords.Add("workspaces of the future");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Thorne");
            author.Add("Matthew and Burke");
            author.Add("David and van de Panne");
            author.Add("Michiel");
            title = "Motion doodles: an interface for sketching character motion";
            booktitle = "ACM SIGGRAPH 2004 Papers";
            series = "SIGGRAPH '04";
            year = new DateTime(2004, 1, 1);
            numpages = 8;
            keywords = new List<String>();
            keywords.Add("Animation");
            keywords.Add("Computer Puppetry");
            keywords.Add("Gestural Interfaces");
            keywords.Add("Sketching");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Whittaker");
            author.Add("Steve and Hirschberg");
            author.Add("Julia");
            title = "The character,value,and management of personal paper archives";
            year = new DateTime(2001, 1, 1);
            numpages = 21;
            keywords = new List<String>();
            keywords.Add("archiving");
            keywords.Add("document management");
            keywords.Add("filing");
            keywords.Add("information retrieval");
            keywords.Add("paper");
            keywords.Add("personal information management");


            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);

            author = new List<String>();
            author.Add("Woods");

            author.Add("David D.");
            title = "Visual momentum:  a concept to improve the cognitive coupling of person and computer";
            year = new DateTime(1984, 1, 1);
            numpages = 16;



            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            this.addEntry(entry);





            author = new List<String>();
            author.Add("Yatani");
            author.Add("Koji and Tamura");
            author.Add("Koiti and Hiroki");
            author.Add("Keiichi and Sugimoto");
            author.Add("Masanori and Hashizume");
            author.Add("Hiromichi");
            title = "Toss-it: intuitive information transfer techniques for mobile devices";
            booktitle = "CHI '05 Extended Abstracts on Human Factors in Computing Systems";
            series = "CHI EA '05";
            year = new DateTime(2005, 1, 1);
            numpages = 4;
            keywords = new List<String>();
            keywords.Add("gesture recognition");
            keywords.Add("information transfer");
            keywords.Add("location recognition");
            keywords.Add("mobile devices");

            entry = new Entry();
            entry.addAttribute("author", author);
            entry.addAttribute("title", title);
            entry.addAttribute("booktitle", booktitle);
            entry.addAttribute("series", series);
            entry.addAttribute("year", year);
            entry.addAttribute("numpages", numpages);
            entry.addAttribute("keywords", keywords);
            this.addEntry(entry);









        }
    }
}
