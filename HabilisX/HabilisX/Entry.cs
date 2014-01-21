using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using HabilisX.Tools;

namespace Microsoft.Surface.Presentation.Controls
{


    public class Entry : Tool
    {
        public Dictionary<String, object> attributes = new Dictionary<String, object>();
        public Boolean isClipped = false;
        public List<ScatterViewItem> annotations;

        private const int CHARSPERLINE = 48;
        private int numLines = 1;
        private Label L;

        public Entry()
        {
            L = new Label();
            // Set the content of the label.
            L.Content = this.toString();
            L.FontSize = 14;
            L.Foreground = Brushes.Black;
            //L.FontWeight = FontWeights.Bold;
           


            Canvas innerView = new Canvas();
            innerView.Background = Brushes.Transparent;
            innerView.Children.Add(L);


            this.Width = 350;
            //Console.WriteLine("Going to height...");
            //this.Height = this.numLines*12;
            this.Content = innerView;
            this.Center = new Point(this.getNewX(), this.getNewY());
            this.Orientation = this.getNewOrientation();
            this.Tag = 0; //highlighting off to begin
            this.CanScale = false;
            this.Background = new SolidColorBrush(Color.FromArgb(245, 191, 191, 191));

            this.annotations = new List<ScatterViewItem>();
        }

        public Entry(Dictionary<String, object> attributes)
        {
            this.attributes = attributes;
        }

        public void addAttribute(String key, object value)
        {
            attributes.Add(key, value);
            L.Content = this.toString();

        }

        public void addAllAttributes(Dictionary<String, object> attributes)
        {
            this.attributes = attributes;
            L.Content = this.toString();
        }

        public bool matchesAllFilters(List<iFilter> filters)
        {
            if (filters.Count == 0)
            {
                return false;
            }

            foreach (iFilter filter in filters)
            {
                if (!filter.Matches(this))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
        {
            RectangleGeometry cursorBounds =
                new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
            RectangleGeometry targetBounds =
                new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);
            return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
        }
        public String printStringAttribute(String att)
        {
            String attribute = (String)(attributes[att]);

            if (attribute.Length != 0)
            {
                return this.addLineBreaks(att + ": " + attribute);
            }
            else
            {
                return att + ": null";
            }
        }

        private String printStringListAttribute(String att)
        {
            List<String> attribute = (List<String>)attributes[att];
            if (attribute.Count == 0)
            {
                return att + ": null";
            }
            else
            {
                String str = att + ": " + attribute[0];

                for (int i = 1; i < attribute.Count; i++)
                {
                    str += ", " + attribute[i];
                }

                return this.addLineBreaks(str);
            }
        }

        private String printIntListAttribute(String att)
        {
            List<int> attribute = (List<int>)attributes[att];
            if (attribute.Count == 0)
            {
                return att + ": null";
            }
            else
            {
                String str = att + ": " + attribute[0];

                for (int i = 1; i < attribute.Count; i++)
                {
                    str += ", " + attribute[i];
                }

                return this.addLineBreaks(str);
            }
        }


        private String printDateAttribute(String att)
        {
            DateTime publicationDate = (DateTime)attributes[att];
            if (publicationDate.Year == 0)
            {
                return att + ": null";
            }
            else
            {
                return att + ": " + publicationDate.Year;
            }
        }

        private String printIntAttribute(String att)
        {
            return att + ": " + attributes[att].ToString();
        }

        public String printAttribute(String att)
        {
            if (!this.attributes.Keys.Contains(att))
            {
                return att + ": null";
            }

            object value = this.attributes[att];
            if (value is String)
            {
                return printStringAttribute(att);
            }
            else if (value is DateTime)
            {
                return printDateAttribute(att);
            }
            else if (value is List<String>)
            {
                return printStringListAttribute(att);
            }
            else if (value is int)
            {
                return printIntAttribute(att);
            }
            else if (value is List<int>)
            {
                return printIntListAttribute(att);
            }
            else
            {
                return "attribute type not recognized";
            }


        }

        public String toString()
        {
            String str = "";

            int counter = 0;
            foreach (String attribute in this.attributes.Keys)
            {
                counter++;
                if (counter > 4) {
                    break;
                }

                str += this.printAttribute(attribute);
                str += "\n\n";

            }

           this.numLines = str.Count(f => f == '\n') + 1;
           //Console.WriteLine("numLines: " + this.numLines);
           this.Height = this.numLines * 20;
            return str;


        }


        public String addLineBreaks(String oldStr)
        {
            StringBuilder newStr = new StringBuilder(oldStr);


            for (int lineBeginning = CHARSPERLINE; lineBeginning < newStr.Length; lineBeginning += CHARSPERLINE)
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

        private double getNewX()
        {
            return HabilisX.Utils.nextNum(700, 1500);
        }
        private double getNewY()
        {
            return HabilisX.Utils.nextNum(350, 750);
        }
        private double getNewOrientation()
        {
            return HabilisX.Utils.nextNum(-60, 60);
        }
    
        public void addAnnotation(Note note){
            //Save the fields of the two text boxes
            String titleString = note.titleText.Text;
            String noteString = note.txt.Text;

            //Make label out of title
            Label noteTitle = new Label();
            noteTitle.Content = titleString;
            noteTitle.Foreground = Brushes.Black;

            //Make item that will be attached
            ScatterViewItem annotation = new ScatterViewItem();

            //Format visually
            annotation.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            annotation.ShowsActivationEffects = false;
            annotation.CanScale = false;
            annotation.MinHeight = 25;
            annotation.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(annotation_PreviewMouseDown);
            annotation.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(annotation_PreviewMouseUp);
            annotation.TouchEnter += new EventHandler<System.Windows.Input.TouchEventArgs>(annotation_TouchEnter);
            annotation.TouchLeave += new EventHandler<System.Windows.Input.TouchEventArgs>(annotation_TouchLeave);
            annotation.PreviewMouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(annotation_PreviewMouseDoubleClick);

            //Attach 
            annotation.Content = noteTitle;
            annotation.Tag = noteString;

            this.annotations.Add(annotation);

            ((Canvas)this.Content).Children.Add(annotation);
            Canvas.SetRight(annotation, this.Width);
            Canvas.SetTop(annotation, annotations.IndexOf(annotation) * 50); 



        }


        int fingers = 0;
        void annotation_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        {
            fingers--;
            eventRelease((ScatterViewItem)sender);
        }

        void annotation_TouchEnter(object sender, System.Windows.Input.TouchEventArgs e)
        {
            fingers++;
            eventHold((ScatterViewItem)sender);

            if (fingers >= 3) {
            }
        }

        void annotation_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Canvas p = (Canvas)((ScatterViewItem)sender).Parent;
            p.Children.Remove((ScatterViewItem)sender);
        }

        void annotation_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            eventRelease((ScatterViewItem)sender);
        }

        void eventRelease(ScatterViewItem sender) {
            String txt = (String)((Label)sender.Content).Content;
            String title = (String)sender.Tag;


            ((Label)sender.Content).Content = title;
            sender.Tag = txt;

            sender.Width = ((Label)sender.Content).Width;
            sender.Height = ((Label)sender.Content).Height;
        }
        void eventHold(ScatterViewItem sender) {
            String title = (String)((Label)sender.Content).Content;
            String txt = (String)sender.Tag;


            ((Label)sender.Content).Content = txt;
            sender.Tag = title;

            sender.Width = 243;//Math.Min(243, ((Label)((ScatterViewItem)sender).Content).Width);
            sender.Height = ((Label)sender.Content).Height;

        }

        void annotation_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            eventHold((ScatterViewItem)sender);
        }
    }
}
