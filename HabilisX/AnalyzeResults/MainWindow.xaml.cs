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
        public MainWindow()
        {
            InitializeComponent();
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
