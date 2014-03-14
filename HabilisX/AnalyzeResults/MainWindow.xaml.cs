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
            String ThomasRelatedMendeley = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\ThomasRelatedMendeley.bib";
            String AdamRelatedMendeley = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\AdamRelatedMendeley.bib";
            String MikeRelatedMendeley = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\MikeRelatedMendeley.bib";
            String StephenRelatedMendeley = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\StephenRelatedMendeley.bib";
            String TonyRelatedMendeley = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\TonyRelatedMendeley.bib";

            String ThomasRelatedHabilis = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\ThomasRelatedHabilis.bib";
            String AdamRelatedHabilis = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\AdamRelatedHabilis.bib";
            String MikeRelatedHabilis = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\MikeRelatedHabilis.bib";
            String StephenRelatedHabilis = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\StephenRelatedHabilis.bib";
            String TonyRelatedHabilis = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\TonyRelatedHabilis.bib";
            String WillRelatedHabilis = "C:\\Users\\prairierose\\Documents\\GitHub\\HabilisX\\HabilisX\\AnalyzeResults\\Results\\WillRelatedHabilis.bib";

            String[] files = {ThomasRelatedMendeley,ThomasRelatedHabilis, AdamRelatedMendeley, AdamRelatedHabilis, MikeRelatedMendeley, MikeRelatedHabilis, 
                                 StephenRelatedMendeley, StephenRelatedHabilis, TonyRelatedMendeley, TonyRelatedHabilis, WillRelatedHabilis};
            String[] Names = { "Thomas", "Thomas", "Adam", "Adam", "Mike", "Mike", "Stephen", "Stephen", "Tony", "Tony", "Will" };
            Database sources = new Database(0);
            Database distractors = new Database(1);
            Database dataset1 = new Database(2);
            Database dataset2 = new Database(3);


            for (int i = 0; i < files.Length; i++)
            {
                String FileName = files[i];


                Database cur;
                Database results = new Database(FileName);
                if (dataset1.Contains(results.allEntries[1]))
                {
                    cur = dataset1;
                    Console.WriteLine("Dataset 1");
                }
                else
                {
                    cur = dataset2;
                    Console.WriteLine("Dataset 2");
                }



                if (i % 2 == 0)
                {
                    Console.WriteLine(Names[i] + "'s Mendeley Results");
                }
                else
                {
                    Console.WriteLine(Names[i] + "'s Habilis Results");
                }

                Console.WriteLine("Related Size: " + results.Count());


                int truePositive = 0;
                int trueNegative = 0;
                int falsePositive = 0;
                int falseNegative = 0;

                foreach (Entry e in results.allEntries)
                {
                    if (sources.Contains(e))
                        truePositive++;
                    if (distractors.Contains(e))
                        falsePositive++;
                }

                foreach (Entry e in sources.allEntries)
                {
                    if (cur.Contains(e) && !results.Contains(e) )
                    {
                        falseNegative++;
                    }
                }

                foreach (Entry e in distractors.allEntries)
                {
                    if (cur.Contains(e) && !results.Contains(e))
                    {
                        trueNegative++;
                    }
                }

                Console.WriteLine("     TRUE || FALSE");
                Console.WriteLine("POS:   " + truePositive + " || " + falsePositive);
                Console.WriteLine("NEG:   " + trueNegative + " || " + falseNegative);

            }

        }
    }
}
