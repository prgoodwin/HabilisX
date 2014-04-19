//#define histogram
//#define vocab
#define excel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HabilisX;

using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using System.IO;
using System.Reflection;
using HabilisX.Tools;
using System.Threading;



namespace AnalyzeResults
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public void makeExcel(String filePath) {
            Database data = new Database(filePath);
            String[] attNames = data.allAttributes.Keys.ToArray<String>();
            int columns = attNames.Length + 1;

            Console.WriteLine("DATASIZE: " + data.Count());
            Console.WriteLine("Atts: " );
            String[][] spreadsheet = new String[data.Count() + 1][];
            for(int i=0; i<spreadsheet.Length; i++){
                spreadsheet[i] = new String[columns];
            }
            //String[,] spreadsheet = new String[data.Count() + 1, columns];

            //fill in first row of attributes
            spreadsheet[0][0] = "index";
            for (int i = 1; i < columns; i++) {
                spreadsheet[0][i] = attNames[i - 1];
            }

            //fill in columns
            for (int i = 1; i < spreadsheet.Length; i++)
            {
                spreadsheet[i][0] = "" + i;
            }

            for (int i=0; i< data.allEntries.Count; i++){
                Entry e = data.allEntries[i];

                foreach (KeyValuePair<String, object> pair in e.attributes) {
                    int index = Array.IndexOf(spreadsheet[0], pair.Key);
                //    if(index > columns || index < 0)
                //    Console.WriteLine(pair.Key + ": " + index);

                        Console.WriteLine(i + "," + index);
                        String noCommas = pair.Value.ToString().Replace(",", " ");
                        spreadsheet[i + 1][index] = noCommas;
                }
            
            }
            //join for string array to be written
            String[] toWrite = new String[spreadsheet.Length];
            for(int i=0; i < spreadsheet.Length; i++){
                toWrite[i] = String.Join(",", spreadsheet[i]);
            }


            System.IO.File.WriteAllLines("C:\\Users\\prairierose\\Downloads\\NLP\\Habilis.csv", toWrite);
            

        }
        public void getHistogram(String filePath){
            String text = System.IO.File.ReadAllText(filePath);
            text = text.ToLower();
            text.Replace("\n", "");
            Dictionary<String, int> hist = new Dictionary<String, int>();
            string[] split = text.Split(' ', ':', '=', '@', '{', '}', ',', '.','-','/','\\','\"', '\'', '(', ')', '+', '&','[', ']');
            foreach (String s in split) {
                if (hist.ContainsKey(s))
                {
                    hist[s]++;
                }
                else if(!s.Contains("1") && !s.Contains("2") && !s.Contains("3") && !s.Contains("4") && !s.Contains("5") && !s.Contains("6") && !s.Contains('7') && !s.Contains('8') && !s.Contains('9') && !s.Contains('0')) {
                    hist.Add(s, 1);
                }
             }



            
//            List<KeyValuePair<String,int>> alphabet = hist.ToList();
            List<String> Keys = hist.Keys.ToList<String>();
            String[] toWrite = new String[Keys.Count()];
            Keys.Sort();
            for(int i=0; i<Keys.Count(); i++){
                toWrite[i] = "" + (i+1) + ":" + hist[Keys[i]];
            }

            foreach (String s in toWrite) {
                Console.WriteLine(s);
            }
            System.IO.File.WriteAllLines("C:\\Users\\PrairieRose\\Documents\\GitHub\\HabilisX\\Infer.NET 2.5\\Samples\\C#\\LDA\\TestLDA\\890Histobram.txt", toWrite);
#if vocab
            System.IO.File.WriteAllLines("C:\\Users\\PrairieRose\\Documents\\GitHub\\HabilisX\\Infer.NET 2.5\\Samples\\C#\\LDA\\TestLDA\\890Vocab.txt", Keys.ToArray<String>());
#endif
        }
        public MainWindow()
        {
            InitializeComponent();

#if histogram
            getHistogram("C:\\Users\\PrairieRose\\Documents\\GitHub\\HabilisX\\Infer.NET 2.5\\Samples\\C#\\LDA\\TestLDA\\890.bib");
            return;
#endif

#if excel
            makeExcel("C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\HabilisX\\Resources\\bibtexUnrelated.txt");
            return;

#endif


            String Path = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\";


            String[] files = {
            "ThomasRelatedMendeley.bib",
            "ThomasRelatedHabilis.bib",
            "AdamRelatedMendeley.bib",
            "AdamRelatedHabilis.bib",
            "MikeRelatedMendeley.bib",
            "MikeRelatedHabilis.bib",
            "StephenRelatedMendeley.bib",
            "StephenRelatedHabilis.bib",
            "TonyRelatedMendeley.bib",
            "TonyRelatedHabilis.bib",
            "WillRelatedMendeley.bib",
            "WillRelatedHabilis.bib",
            "JimRelatedMendeley.bib",
            "JimRelatedHabilis.bib",
            "ChrisRelatedMendeley.bib",
            "ChrisRelatedHabilis.bib",

                             };
            Database sources = new Database(0);
            Database distractors = new Database(1);
            Database dataset1 = new Database(2);
            Database dataset2 = new Database(3);

            String TabDeliniatedOutput = "MTP\tMFP\tMTN\tMFN\tHTP\tHFP\tHTN\tHFN\n";



            for (int i = 0; i < files.Length; i++)
            {
                String FileName = files[i];
                int end = FileName.LastIndexOf("Related");
                int period = FileName.LastIndexOf(".");
                String Name = FileName.Substring(0, end);
                Console.WriteLine(Name + "'s " + FileName.Substring(end + 7, (period - (end + 7))) + " Results");

                Database cur;
                Database results = new Database(Path + FileName);
                if (dataset1.Contains(results.allEntries[0]))
                {
                    cur = dataset1;
                    Console.WriteLine("Dataset 1");
                }
                else
                {
                    cur = dataset2;
                    Console.WriteLine("Dataset 2");
                }

                Console.WriteLine("Related Size: " + results.Count());


                int truePositive = 0;
                int trueNegative = 0;
                int falsePositive = 0;
                int falseNegative = 0;

                foreach(Entry e in cur.allEntries){
                    if (results.Contains(e) && sources.Contains(e)) {
                        truePositive++;
                    } else if(results.Contains(e) && distractors.Contains(e)){
                        falsePositive++;
                    }
                    else if (sources.Contains(e) && !results.Contains(e)) {
                        falseNegative++;
                    }
                    else if (distractors.Contains(e) && !results.Contains(e))
                    {
                        trueNegative++;
                    }
                    else {
                        Console.WriteLine("found something uncategorizable: " + e.printAttribute("title"));
                    }
                }




                Console.WriteLine("     TRUE || FALSE");
                if (truePositive > 9)
                {
                    Console.WriteLine("POS:   " + truePositive + " || " + falsePositive);
                }
                else
                {
                    Console.WriteLine("POS:    " + truePositive + " || " + falsePositive);
                }

                if (trueNegative > 9)
                {
                    Console.WriteLine("NEG:   " + trueNegative + " || " + falseNegative);
                }
                else
                {
                    Console.WriteLine("NEG:    " + trueNegative + " || " + falseNegative);
                }

                if (trueNegative + truePositive + falseNegative + falsePositive != 37)
                {
                    Console.WriteLine("There may be a problem here: " + (trueNegative + truePositive + falseNegative + falsePositive));
                    if (truePositive + falsePositive != results.allEntries.Count) Console.WriteLine("problem with positive");
                }


                TabDeliniatedOutput += Name + "\t" + truePositive + "\t" + falsePositive + "\t" + trueNegative + "\t" + falseNegative + "\t";

                if (i % 2 != 0) {
                    TabDeliniatedOutput += "\n";
                    Console.WriteLine("_________________________");
                }
                Console.WriteLine();

            }

            Console.WriteLine(TabDeliniatedOutput);

        }
    }
}
