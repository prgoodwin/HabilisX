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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace AddScatterViewItem
{
   /// <summary>
   /// Interaction logic for SurfaceWindow1.xaml
   /// </summary>
   public partial class SurfaceWindow1 : SurfaceWindow
   {
      public String dateString;
      public String authorString;
      public String titleString;

      /// <summary>
      /// Default constructor.
      /// </summary>
      public SurfaceWindow1()
      {
         InitializeComponent();

         #region make and display the database

         //Initialize the database.  Currently (March '13) it is 10 hardcoded papers
         Database dataSet = this.initDatabase();


         //For every paper in the database, make a scatterview that showes the title
         foreach (Entry e in dataSet.allPapers)
         {
            Label L = new Label();
            // Set the content of the label.
            L.Content = e.toString();
            L.FontSize = 12;
            L.Height = 200;
            L.Width = 300;
            L.Tag = e;
            // Add the label to the ScatterView contr
            // It is automatically wrapped in a ScatterViewItem control.
            MyScatterView.Items.Add(L);
         }

         #endregion

         //Tools
         dateString = DateTile.Text;
         authorString = AuthorTile.Text;
         titleString = TitleTile.Text;
         //DragDrop.

         //frame.Source = "C:\Users\User\documents\visual studio 2010\Projects\AddScatterViewItem\AddScatterViewItem\Resources\pictureFrame.png";

         //#region ConnectEventHandlers
         //SurfaceButtonAddImplicit.Click += new RoutedEventHandler(AddImplicit_Click);
         //SurfaceButtonAddExplicit.Click += new RoutedEventHandler(AddExplicit_Click);
         //#endregion

         // Add handlers for window availability events
         AddWindowAvailabilityHandlers();


      }

      #region AddImplicit
      private void AddImplicit_Click(object sender, RoutedEventArgs e)
      {
         Label L = new Label();
         // Set the content of the label.
         L.Content = "Item THIS AND THAT AND ONE OTHER THING \n AND OH YEAH< THAT THING TOO";
         // Add the label to the ScatterView control.
         // It is automatically wrapped in a ScatterViewItem control.
         MyScatterView.Items.Add(L);
      }
      #endregion

      #region AddExplicit
      private void AddExplicit_Click(object sender, RoutedEventArgs e)
      {
         ScatterViewItem item = new ScatterViewItem();

         item.Content = "Item THIS AND THAT AND ONE OTHER THING \n AND OH YEAH< THAT THING TOO";
         item.Orientation = 0.0;
         MyScatterView.Items.Add(item);
      }
      #endregion

      #region surface template functions
      /// <summary>
      /// Occurs when the window is about to close. 
      /// </summary>
      /// <param name="e"></param>
      protected override void OnClosed(EventArgs e)
      {
         base.OnClosed(e);

         // Remove handlers for window availability events
         RemoveWindowAvailabilityHandlers();
      }

      /// <summary>
      /// Adds handlers for window availability events.
      /// </summary>
      private void AddWindowAvailabilityHandlers()
      {
         // Subscribe to surface window availability events
         ApplicationServices.WindowInteractive += OnWindowInteractive;
         ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
         ApplicationServices.WindowUnavailable += OnWindowUnavailable;
      }

      /// <summary>
      /// Removes handlers for window availability events.
      /// </summary>
      private void RemoveWindowAvailabilityHandlers()
      {
         // Unsubscribe from surface window availability events
         ApplicationServices.WindowInteractive -= OnWindowInteractive;
         ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
         ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
      }

      /// <summary>
      /// This is called when the user can interact with the application's window.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void OnWindowInteractive(object sender, EventArgs e)
      {
         //TODO: enable audio, animations here
      }

      /// <summary>
      /// This is called when the user can see but not interact with the application's window.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void OnWindowNoninteractive(object sender, EventArgs e)
      {
         //TODO: Disable audio here if it is enabled

         //TODO: optionally enable animations here
      }

      /// <summary>
      /// This is called when the application's window is not visible or interactive.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void OnWindowUnavailable(object sender, EventArgs e)
      {
         //TODO: disable audio, animations here
      }
      #endregion

      #region database functions
      private Database initDatabase()
      {
         Database database = new Database();
         String title = "Improving Continuous Gesture Recognition with spoken Prosody";
         List<String> authors = new List<String>();
         authors.Add("Kettebekov");
         authors.Add("Sanshzar");
         DateTime pubDate = new DateTime(2003, 1, 1);


         Entry paper1 = new Entry(title, authors, pubDate);
         database.addPaper(paper1);

         title = "High level data fusion on a multimodal interactive application platform";
         authors = new List<String>();
         authors.Add("Mendonça");
         pubDate = new DateTime(2009, 1, 1);

         Entry paper2 = new Entry(title, authors, pubDate);
         database.addPaper(paper2);

         title = "Spoken and Multimodal Communication Systems in Mobile Settings";
         authors = new List<String>();
         authors.Add("Turunen");
         authors.Add("Hakulinen");
         pubDate = new DateTime(2007, 1, 1);


         Entry paper3 = new Entry(title, authors, pubDate);
         database.addPaper(paper3);

         title = "Put-that-there”: Voice and gesture at the graphics interface";
         authors.Clear();
         authors.Add("Bolt");
         pubDate = new DateTime(1980, 1, 1);

         Entry paper4 = new Entry(title, authors, pubDate);
         database.addPaper(paper4);

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

         Entry paper5 = new Entry(title, authors, pubDate);
         database.addPaper(paper5);

         title = "What is that? Gesturing to determine device identity";
         authors = new List<String>();
         authors.Add("Swindells");
         authors.Add("Inkpen");
         pubDate = new DateTime(2002, 1, 1);

         Entry paper6 = new Entry(title, authors, pubDate);
         database.addPaper(paper6);

         title = "100,000,000 taps: analysis and improvement of touch performance in the large";
         authors = new List<String>();
         authors.Add("Henze");
         authors.Add("Rukzio");
         authors.Add("Boll");
         pubDate = new DateTime(2011, 1, 1);


         Entry paper7 = new Entry(title, authors, pubDate);
         database.addPaper(paper7);

         title = "UI on the Fly: Generating a Multimodal User Interface";
         authors = new List<String>();
         authors.Add("Reitter");
         authors.Add("Panttaja");
         pubDate = new DateTime(2004, 1, 1);

         Entry paper8 = new Entry(title, authors, pubDate);
         database.addPaper(paper8);

         title = "AirMouse: Finger Gesture for 2D and 3D Interaction";
         authors = new List<String>();
         authors.Add("Ortega");
         authors.Add("Nigay");
         pubDate = new DateTime(2009, 1, 1);

         Entry paper9 = new Entry(title, authors, pubDate);
         database.addPaper(paper9);

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

         Entry paper10 = new Entry(title, authors, pubDate);
         database.addPaper(paper10);

         return database;

      }

      #endregion

      #region frame events
      private void frame_TouchMove(object sender, TouchEventArgs e)
      {
         Console.Out.WriteLine("in event");
         foreach (Label l in MyScatterView.Items)
         {
            Console.Out.WriteLine("Item: " + l.Content);

         }
      }
      private void frame_MouseMove(object sender, MouseEventArgs e)
      {
         //foreach (Object item in MyScatterView.Items)
         //{
         //   if (item is Label)
         //   {
         //      if (this.AreBoundaryIntersecting((FrameworkElement)item, (FrameworkElement)sender))
         //      //if (this.GetBounds((FrameworkElement)item, MyScatterView).IntersectsWith(this.GetBounds((FrameworkElement)sender, MyScatterView)))
         //      {
         //         ((Label)item).Background = Brushes.Red;
         //      }
         //      else
         //      {
         //         ((Label)item).Background = Brushes.Transparent;
         //      }
         //   }
         //}
      }

      private void frame_MouseDown(object sender, MouseButtonEventArgs e)
      {

         //foreach (Object item in MyScatterView.Items)
         //{
         //   if (item is Label)
         //   {
         //      if (this.GetBounds((FrameworkElement)item, MyScatterView).IntersectsWith(this.GetBounds((FrameworkElement)sender, MyScatterView)))
         //      {
         //         ((Label)item).Background = Brushes.Red;
         //      }
         //      else
         //      {
         //         ((Label)item).Background = Brushes.Transparent;
         //      }
         //   }
         //}
      }
      #endregion

      #region Scatterview Item events
      private void ScatterViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
      }
      private void ScatterViewItem_DragEnter(object sender, DragEventArgs e)
      {
         Console.Out.WriteLine("Entering a Drag contest");
      }
      #endregion


      private void DateTile_DragOver(object sender, DragEventArgs e)
      {
         Console.Out.WriteLine("In Date drag event");
      }


      #region mouseMoved Events
      private void titleTile_MouseMove(object sender, MouseEventArgs e)
      {

         String title = "";
         if (titleString.Length > 6)
         {
            title = titleString.Substring(6);
         }
         else
         {
            foreach (Object item in MyScatterView.Items)
            {
               if (item is Label && ((Label)item).Background == Brushes.Gold)
               {
                  ((Label)item).Background = Brushes.Transparent;
               }
               else if (item is Label && ((Label)item).Background == Brushes.Lime)
               {
                  ((Label)item).Background = Brushes.Blue;
               }
               else if (item is Label && ((Label)item).Background == Brushes.DarkOrange)
               {
                  ((Label)item).Background = Brushes.Red;
               }
               else if (item is Label && ((Label)item).Background == Brushes.Black)
               {
                  ((Label)item).Background = Brushes.DarkOrchid;
               }
            }

            return;
         }

         foreach (Object item in MyScatterView.Items)
         {
            if (item is Label)
            {
               Entry entry = (Entry)((Label)item).Tag;
               String entryTitle = entry.title;

               if (this.AreBoundaryIntersecting((FrameworkElement)item, (FrameworkElement)sender) && entryTitle.ToLower().Contains(title.ToLower()))
               {
                  if (item is Label && ((Label)item).Background == Brushes.Transparent)
                  {
                     ((Label)item).Background = Brushes.Gold;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Blue)
                  {
                     ((Label)item).Background = Brushes.Lime;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Red)
                  {
                     ((Label)item).Background = Brushes.DarkOrange;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.DarkOrchid)
                  {
                     ((Label)item).Background = Brushes.Black;
                  }
               }
               else
               {
                  if (item is Label && ((Label)item).Background == Brushes.Gold)
                  {
                     ((Label)item).Background = Brushes.Transparent;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Lime)
                  {
                     ((Label)item).Background = Brushes.Blue;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.DarkOrange)
                  {
                     ((Label)item).Background = Brushes.Red;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Black)
                  {
                     ((Label)item).Background = Brushes.DarkOrchid;
                  }
               }
            }
         }

      }

      private void AuthorTile_MouseMove(object sender, MouseEventArgs e)
      {

         String author = "";
         if (authorString.Length > 7)
         {
            author = authorString.Substring(7);
         }
         else
         {
            foreach (Object item in MyScatterView.Items)
            {
               if (item is Label && ((Label)item).Background == Brushes.Blue)
               {
                  ((Label)item).Background = Brushes.Transparent;
               }
               else if (item is Label && ((Label)item).Background == Brushes.DarkOrchid)
               {
                  ((Label)item).Background = Brushes.Red;
               }
               else if (item is Label && ((Label)item).Background == Brushes.Lime)
               {
                  ((Label)item).Background = Brushes.Gold;
               }
               else if (item is Label && ((Label)item).Background == Brushes.Black)
               {
                  ((Label)item).Background = Brushes.DarkOrange;
               }
            }

            return;
         }

         foreach (Object item in MyScatterView.Items)
         {
            if (item is Label)
            {
               Entry entry = (Entry)((Label)item).Tag;
               List<String> entryAuthors = entry.Authors;
               for (int i = 0; i < entryAuthors.Count; i++) {
                  entryAuthors[i] = entryAuthors[i].ToLower();
               }

               if (this.AreBoundaryIntersecting((FrameworkElement)item, (FrameworkElement)sender) && entryAuthors.Contains(author.ToLower()))
               {
                  if (item is Label && ((Label)item).Background == Brushes.Transparent)
                  {
                     ((Label)item).Background = Brushes.Blue;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Red)
                  {
                     ((Label)item).Background = Brushes.DarkOrchid;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Gold)
                  {
                     ((Label)item).Background = Brushes.Lime;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.DarkOrange)
                  {
                     ((Label)item).Background = Brushes.Black;
                  }
               }
               else
               {
                  if (item is Label && ((Label)item).Background == Brushes.Blue)
                  {
                     ((Label)item).Background = Brushes.Transparent;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.DarkOrchid)
                  {
                     ((Label)item).Background = Brushes.Red;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Lime)
                  {
                     ((Label)item).Background = Brushes.Gold;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Black)
                  {
                     ((Label)item).Background = Brushes.DarkOrange;
                  }
               }
            }
         }
      }

      private void DateTile_MouseMove(object sender, MouseEventArgs e)
      {
         int year = 0;
         String yearStr = "";
         if (dateString.Length > 5)
         {
            yearStr = dateString.Substring(5);
            year = Convert.ToInt32(yearStr);
         }
         else
         {
            foreach (Object item in MyScatterView.Items)
            {
               if (item is Label && ((Label)item).Background == Brushes.Red)
               {
                  ((Label)item).Background = Brushes.Transparent;
               }
               else if (item is Label && ((Label)item).Background == Brushes.DarkOrchid)
               {
                  ((Label)item).Background = Brushes.Blue;
               }
               else if (item is Label && ((Label)item).Background == Brushes.DarkOrange)
               {
                  ((Label)item).Background = Brushes.Gold;
               }
               else if (item is Label && ((Label)item).Background == Brushes.Black)
               {
                  ((Label)item).Background = Brushes.Lime;
               }
            }

            return;
         }

         foreach (Object item in MyScatterView.Items)
         {
            if (item is Label)
            {
               Entry entry = (Entry)((Label)item).Tag;
               int pubYear = entry.publicationDate.Year;

               if (this.AreBoundaryIntersecting((FrameworkElement)item, (FrameworkElement)sender) && year == pubYear)
               {
                  if (item is Label && ((Label)item).Background == Brushes.Transparent)
                  {
                     ((Label)item).Background = Brushes.Red;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Blue)
                  {
                     ((Label)item).Background = Brushes.DarkOrchid;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Gold)
                  {
                     ((Label)item).Background = Brushes.DarkOrange;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Lime)
                  {
                     ((Label)item).Background = Brushes.Black;
                  }
               }
               else
               {
                  if (item is Label && ((Label)item).Background == Brushes.Red)
                  {
                     ((Label)item).Background = Brushes.Transparent;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.DarkOrchid)
                  {
                     ((Label)item).Background = Brushes.Blue;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.DarkOrange)
                  {
                     ((Label)item).Background = Brushes.Gold;
                  }
                  else if (item is Label && ((Label)item).Background == Brushes.Black)
                  {
                     ((Label)item).Background = Brushes.Lime;
                  }
               }
            }
         }
      }
      #endregion 

      #region TextChanged events
      private void DateTile_TextChanged(object sender, TextChangedEventArgs e)
      {
         dateString = ((TextBox)sender).Text;
      }

      private void AuthorTile_TextChanged(object sender, TextChangedEventArgs e)
      {
         authorString = ((TextBox)sender).Text;
      }

      private void titleTile_TextChanged(object sender, TextChangedEventArgs e)
      {
         titleString = ((TextBox)sender).Text;
      }
      #endregion

      public Rect GetBounds(FrameworkElement of, FrameworkElement from)
      {
         // Might throw an exception if of and from are not in the same visual tree
         GeneralTransform transform = of.TransformToVisual(from);

         return transform.TransformBounds(new Rect(0, 0, of.ActualWidth, of.ActualHeight));
      }

      private bool AreBoundaryIntersecting(FrameworkElement cursorVisual, FrameworkElement target)
      {
         RectangleGeometry cursorBounds =
             new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
         RectangleGeometry targetBounds =
             new RectangleGeometry(new Rect(0, 0, target.ActualWidth, target.ActualHeight));
         cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(target);
         return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
      }

      private void ScatterViewItem_DragEnter(object sender, SurfaceDragDropEventArgs e)
      {
         Console.Out.WriteLine("In a drag event");
         
      }
   }
}