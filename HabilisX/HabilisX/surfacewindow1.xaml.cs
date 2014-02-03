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
using System.IO;
using System.Reflection;
using HabilisX.Tools;
using System.Threading;




namespace HabilisX
{


    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {


        public const int TOP = 0;
        public const int BOTTOM = 1;
        public const int LEFT = 2;
        public const int RIGHT = 3;

        public Point lastMousePoint = new Point(0, 0);
        public Point lastDelta;
        public bool hasClosedLabel = false;

        public List<String> saveState1 = new List<String>();
        public List<String> saveState2 = new List<String>();
        public List<Entry> saveEntries1 = new List<Entry>();
        public List<Entry> saveEntries2 = new List<Entry>();

        public List<MagnifyingGlass> mags = new List<MagnifyingGlass>();
        public List<PushPin> pushPins = new List<PushPin>();
        public List<Entry> entries = new List<Entry>();
        public List<MagicLens> lenses = new List<MagicLens>();
        public List<Ruler> rulers = new List<Ruler>();
        public List<PaperClip> paperClips = new List<PaperClip>();
        public List<FilterTile> filters = new List<FilterTile>();
        public List<ScatterViewItem> notes = new List<ScatterViewItem>();
        public TrashCan trash;
        Random num = new Random();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            Double ScreenWidth = MyScatterView.Width;
            Console.WriteLine("WIDTH OF SCREEN" + MyScatterView.Width);

            trash = new TrashCan();
            MyScatterView.Items.Add(trash);

            #region make and display the database

            Database dataSet = new Database();
            List<KeyValuePair<string, Type>> myList = dataSet.allAttributes.ToList();

            myList.Sort(
               delegate(KeyValuePair<string, Type> firstPair,
               KeyValuePair<string, Type> nextPair)
               {
                   return firstPair.Key.CompareTo(nextPair.Key);
               });
            #region Make Database Attribute Buttons
            for (int i = 0; i < myList.Count; i++)
            {
                String str = myList.ElementAt(i).Key;


                ScatterViewItem newButton = new ScatterViewItem();
                Double buttonsOnScreen = Math.Floor((MyScatterView.Width - 280) / 180);


                newButton.Orientation = 0;
                newButton.Center = new Point(280 + (180 * (i % buttonsOnScreen)), 40 + 65 * (int)(i / buttonsOnScreen));
                //newButton.Center = new Point(280 + (180 * (i % buttonsOnScreen)), 40 + 75 * (int)(i / buttonsOnScreen));
                newButton.Height = 50;
                newButton.Width = 160;
                //newButton.Background = Brushes.DarkSlateGray;
                newButton.MinHeight = 0;
                newButton.CanMove = false;
                newButton.CanRotate = false;

                SurfaceButton myButton = new SurfaceButton();
                myButton.FontSize = 15;
                myButton.Content = "Add " + str + " label";
                myButton.Height = 50;
                myButton.Width = 160;
                myButton.Foreground = Brushes.White;

                newButton.Content = myButton;
                MyScatterView.Items.Add(newButton);
                myButton.Tag = str;

                if (dataSet.allAttributes[str].Equals(typeof(System.String)))
                {
                    myButton.Background = Brushes.DarkSlateGray;
                    myButton.Click += new RoutedEventHandler(AddStringFilter_Click);

                }
                else if (dataSet.allAttributes[str].Equals(typeof(List<String>)))
                {
                    myButton.Background = Brushes.SlateGray;
                    myButton.Click += new RoutedEventHandler(AddStringListFilter_Click);
                }
                else if (dataSet.allAttributes[str].Equals(typeof(DateTime)))
                {
                    myButton.Background = Brushes.LightGray;
                    myButton.Foreground = Brushes.Black;
                    myButton.Click += new RoutedEventHandler(AddDateFilter_Click);
                }
                else if (dataSet.allAttributes[str].Equals(typeof(int)))
                {
                    myButton.Background = Brushes.LightGray;
                    myButton.Foreground = Brushes.Black;
                    myButton.Click += new RoutedEventHandler(AddIntFilter_Click);
                }
                else if (dataSet.allAttributes[str].Equals(typeof(List<int>)))
                {
                    myButton.Background = Brushes.Gray;
                    myButton.Click += new RoutedEventHandler(AddIntListFilter_Click);
                }


                //Console.WriteLine("ALL ATTRIBUTES: " + str + ", " + dataSet.allAttributes[str].ToString());
            }

            #endregion

            #region make entries and display to screen

            //List<Entry> listy = dataSet.allEntries.ToList();
           // Console.WriteLine("NUMBER OF PAPERS: " + listy.Count());
            //For every paper in the database, make a scatterview that shows the details
            foreach (Entry e in dataSet.allEntries)
            {
                AddToScreen(e);
                //            EventSubscriber.SubscribeAll(e);
            }

            #endregion

            #endregion


            #region Autosave
            System.Timers.Timer timer = new System.Timers.Timer(60000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate(object delSender, System.Timers.ElapsedEventArgs delE)
            {
                this.autoSave("autoSave.txt");
                timer.Enabled = true;
            });

            timer.Enabled = true;



            #endregion

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }


        private void ScatterView_LayoutUpdated(object sender, EventArgs e)
        {
            //this.mags.Sort();
            //this.pushPins.Sort();
            //this.entries.Sort();
            //this.lenses.Sort();
            //this.rulers.Sort();
            //this.paperClips.Sort();
            //this.filters.Sort();
            //this.notes.Sort();



            #region Trash Interactions
            foreach (MagnifyingGlass item in this.mags)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            foreach (PushPin item in this.pushPins)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            foreach (Entry item in this.entries)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            foreach (MagicLens item in this.lenses)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            foreach (Ruler item in this.rulers)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            foreach (PaperClip item in this.paperClips)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            foreach (FilterTile item in this.filters)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            foreach (ScatterViewItem item in this.notes)
            {
                if (trash.AreBoundaryIntersecting((ScatterViewItem)item))
                {
                    RemoveFromScreen((ScatterViewItem)item);
                    return;
                }
            }
            #endregion




            ISet<PushPin> set = new HashSet<PushPin>();
            foreach (Entry entry in this.entries)
            {
                #region pushPin Interactions
                Boolean pinned = false;
                foreach (PushPin pin in this.pushPins)
                {
                    if (pin.AreBoundaryIntersecting(entry))
                    {
                        pinned = true;
                        set.Add(pin);
                    }

                }

                if (pinned)
                {
                    entry.CanMove = false;
                    entry.CanRotate = false;

                }
                else
                {
                    entry.CanMove = true;
                    entry.CanRotate = true;
                }
                #endregion

                #region MagicLens Interactions
                Boolean highlighted = false;
                foreach (MagicLens lens in this.lenses)
                {
                    if (lens.AreBoundaryIntersecting(entry) && entry.matchesAllFilters(lens.filters) && (Canvas.GetZIndex(lens) > Canvas.GetZIndex(entry)))
                    {
                        highlighted = true;
                    }
                }

                if (highlighted && (int)entry.Tag != 1)
                {
                    ((Canvas)(entry.Content)).Background = Brushes.Gold;
                    entry.SetRelativeZIndex(0);

                    entry.Tag = 1;
                }
                else if (!highlighted && (int)entry.Tag != 0)
                {

                    ((Canvas)(entry.Content)).Background = Brushes.Transparent;
                    entry.Tag = 0;
                }

                foreach (MagicLens lens in this.lenses)
                {
                    lens.SetRelativeZIndex(0);
                }
                #endregion
            }

            #region activate filter tiles if intersecting with tool
            List<FilterTile> toBeRemoved = new List<FilterTile>();
            foreach (FilterTile tile in this.filters)
            {
                foreach (MagnifyingGlass glass in this.mags)
                {
                    if (MyScatterView.Items.Contains(tile) && glass.AreBoundaryIntersecting(tile) && !toBeRemoved.Contains(tile))
                    {
                        glass.activateFilterTile(tile.attTag.ToLower(), tile.Background, tile.attTag);//tile, tile.attTag, tile.Background);
                        toBeRemoved.Add(tile);

                    }
                }

                foreach (MagicLens lens in lenses)
                {
                    if (MyScatterView.Items.Contains(tile) && lens.AreBoundaryIntersecting(tile) && !toBeRemoved.Contains(tile))
                    {
                        if (tile.hasInput())
                        {
                            iFilter query = tile.getFilter();
                            lens.activateFilterTile((String)tile.attTag + ": " + query.getQueryString(), query.getColor(), query);
                            toBeRemoved.Add(tile);
                        }
                    }
                }

                foreach (PaperClip clip in this.paperClips)
                {
                    if (MyScatterView.Items.Contains(tile) && clip.AreBoundaryIntersecting(tile) && !toBeRemoved.Contains(tile))
                    {
                        if (tile.hasInput())
                        {
                            iFilter query = tile.getFilter();
                            clip.activateFilterTile((String)tile.attTag + ": " + query.getQueryString(), query.getColor(), query);
                            toBeRemoved.Add(tile);
                        }
                    }
                }
            }

            foreach (FilterTile tile in toBeRemoved)
            {
                RemoveFromScreen(tile);
            }

            #endregion


            #region make pushpin always on top
            foreach (PushPin pin in this.pushPins)
            {
                pin.SetRelativeZIndex(0);
                if (set.Contains(pin) && (int)pin.Tag == 1)
                {
                    pin.SetImageToOccludedPin();
                }
                else if (!set.Contains(pin) && (int)pin.Tag == 0)
                {
                    pin.SetImageToPin();
                }
            }

            #endregion
        }


        #region Add Buttons & MouseEventHandlers
        private void AddStringFilter_Click(object sender, RoutedEventArgs e)
        {
            AddNewFilterTile(new StringFilterTile((String)((SurfaceButton)sender).Tag));
        }
        private void AddStringListFilter_Click(object sender, RoutedEventArgs e)
        {
            AddNewFilterTile(new StringListFilterTile((String)((SurfaceButton)sender).Tag));
        }
        private void AddDateFilter_Click(object sender, RoutedEventArgs e)
        {
            AddNewFilterTile(new DateFilterTile((String)((SurfaceButton)sender).Tag));
        }
        private void AddIntFilter_Click(object sender, RoutedEventArgs e)
        {
            AddNewFilterTile(new IntFilterTile((String)((SurfaceButton)sender).Tag));
        }
        private void AddIntListFilter_Click(object sender, RoutedEventArgs e)
        {
            AddNewFilterTile(new IntListFilterTile((String)((SurfaceButton)sender).Tag));
        }

        private void AddNewFilterTile(FilterTile tile)
        {
            AddToScreen(tile);
        }
        private void AddNewTool(Tool tool)
        {
            AddToScreen(tool);
        }

        private void AddPushPinButton_Click(object sender, RoutedEventArgs e)
        {
            PushPin pushPin = new PushPin();
            AddNewTool(pushPin);
        }
        private void AddRulerButton_Click(object sender, RoutedEventArgs e)
        {
            Ruler ruler = new Ruler();
            ruler.MouseMove += new MouseEventHandler(ruler_MouseMove);
            ruler.PreviewTouchMove += new EventHandler<TouchEventArgs>(ruler_PreviewTouchMove);
            AddNewTool(ruler);
        }
        private void AddMagicLensButton_Click(object sender, RoutedEventArgs e)
        {
            MagicLens magicLens = new MagicLens();
            AddNewTool(magicLens);
        }

        private void AddMagnifyingGlass_Click(object sender, RoutedEventArgs e)
        {
            MagnifyingGlass magnifier = new MagnifyingGlass();
            magnifier.PreviewTouchMove += new EventHandler<TouchEventArgs>(magnifier_PreviewTouchMove);
            magnifier.MouseMove += new MouseEventHandler(magnifier_MouseMove);
            AddNewTool(magnifier);
        }

        private void AddPaperClip_Click(object sender, RoutedEventArgs e)
        {
            PaperClip paperClip = new PaperClip();
            paperClip.MouseMove += new MouseEventHandler(paperClip_MouseMove);
            paperClip.PreviewTouchMove += new EventHandler<TouchEventArgs>(paperClip_PreviewTouchMove);
            AddNewTool(paperClip);
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {

            Note note = new Note();
            note.MouseMove += new MouseEventHandler(note_MouseMove);
            note.PreviewTouchMove += new EventHandler<TouchEventArgs>(note_PreviewTouchMove);
            AddNewTool(note);
        }

        private void autoSave(String fileName) {
            List<String> bibtexEntries = new List<String>();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.entries.Sort(
                    delegate(Entry p1, Entry p2)
                    {
                        if (Canvas.GetZIndex(p1) > Canvas.GetZIndex(p2)) return 1;
                        else return -1;
                    });

                foreach (Entry entry in this.entries)
                {
                    String bibTex = "Entry{\n";
                    foreach (KeyValuePair<string, object> pair in entry.attributes)
                    {
                        if (!pair.Key.Equals("x") && !pair.Key.Equals("y") && !pair.Key.Equals("orientation"))
                        {
                            bibTex += pair.Key + "=" + pair.Value.ToString() + "\n";
                        }
                    }
                    bibTex += "x=" + entry.Center.X + "\n";
                    bibTex += "y=" + entry.Center.Y + "\n";
                    bibTex += "orientation=" + entry.Orientation + "\n";
                    bibTex += "zIndex=" + Canvas.GetZIndex(entry) + "\n";
                    bibTex += "}\n";
                    bibtexEntries.Add(bibTex);
                }


                String str = System.IO.Directory.GetCurrentDirectory();
                str = str.Substring(0, str.LastIndexOf("\\"));
                str = str.Substring(0, str.LastIndexOf("\\"));
                str = str + "\\Resources\\" + fileName;

                Console.WriteLine("saving...");
                System.IO.File.WriteAllLines(str, bibtexEntries.ToArray());
            }));
            }

        private void save1_Click(object sender, RoutedEventArgs e)
        {
            saveState1 = new List<string>();
            List<String> bibtexEntries = new List<string>();

            this.entries.Sort(
                delegate(Entry p1, Entry p2)
            {
                if (Canvas.GetZIndex(p1) > Canvas.GetZIndex(p2)) return 1;
                else return -1; 
            });
            foreach (Entry entry in this.entries)
            {
                String bibTex = "Entry{\n";
                foreach(KeyValuePair<string, object> pair in entry.attributes){
                    bibTex += pair.Key + "=" + pair.Value.ToString() + "\n";
                }
                bibTex += "x=" + entry.Center.X + "\n";
                bibTex += "y=" + entry.Center.Y + "\n";
                bibTex += "orientation=" + entry.Orientation + "\n";
                bibTex += "zIndex=" + Canvas.GetZIndex(entry) + "\n";
                bibTex += "}\n";
                bibtexEntries.Add(bibTex);
            }


            String str = System.IO.Directory.GetCurrentDirectory();
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str + "\\Resources\\saveState.txt";

            System.IO.File.WriteAllLines(str, bibtexEntries.ToArray());
            
            ((SurfaceButton)(Save1.Content)).Content = "Saved";
        }
        private void load1_Click(object sender, RoutedEventArgs e)
        {
            foreach (Entry entry in this.entries)
            {
                MyScatterView.Items.Remove(entry);
            }
            this.entries = new List<Entry>();

            String str = System.IO.Directory.GetCurrentDirectory();
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str + "\\Resources\\saveState.txt";

            Database loadedData = new Database(str);
            Database originalData = new Database();

            foreach (Entry data in originalData.allEntries)
            {
                if (!loadedData.Contains(data))
                {

                    loadedData.addEntry(data);
                }
            }


            Console.WriteLine("Number of cards in saved data: " + loadedData.allEntries.Count);
            foreach (Entry entry in loadedData.allEntries)
            {

                if (entry.attributes.ContainsKey("x") && entry.attributes.ContainsKey("y") && entry.attributes.ContainsKey("orientation"))
                {
                    Double x = Double.Parse(entry.attributes["x"].ToString());
                    Double y = Double.Parse(entry.attributes["y"].ToString()); ;

                    if (x > 1800)
                    {
                        x = 500;
                    }

                    if (y > 900)
                    {
                        y = 500;

                    }
                    entry.Center = new Point(x, y);
                    entry.Orientation = Double.Parse(entry.attributes["orientation"].ToString());
                    AddToScreen(entry);
                }
                else
                {
                    AddToScreen(entry);
                }
            } 

            ((SurfaceButton)(Load1.Content)).FontSize = 15;
            ((SurfaceButton)(Load1.Content)).Content = "Loaded";

        }
        private void save2_Click(object sender, RoutedEventArgs e)
        {
            saveState2 = new List<string>();
            List<String> bibtexEntries = new List<string>();

            this.entries.Sort(
                delegate(Entry p1, Entry p2)
                {
                    if (Canvas.GetZIndex(p1) > Canvas.GetZIndex(p2)) return 1;
                    else return -1;
                });
            foreach (Entry entry in this.entries)
            {
                String bibTex = "Entry{\n";
                foreach (KeyValuePair<string, object> pair in entry.attributes)
                {
                    bibTex += pair.Key + "=" + pair.Value.ToString() + "\n";
                }
                bibTex += "x=" + entry.Center.X + "\n";
                bibTex += "y=" + entry.Center.Y + "\n";
                bibTex += "orientation=" + entry.Orientation + "\n";
                bibTex += "zIndex=" + Canvas.GetZIndex(entry) + "\n";
                bibTex += "}\n";
                bibtexEntries.Add(bibTex);
            }


            String str = System.IO.Directory.GetCurrentDirectory();
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str + "\\Resources\\saveState2.txt";

            System.IO.File.WriteAllLines(str, bibtexEntries.ToArray());

            ((SurfaceButton)(Save2.Content)).Content = "Saved";
        }
        private void load2_Click(object sender, RoutedEventArgs e)
        {
            foreach (Entry entry in this.entries)
            {
                MyScatterView.Items.Remove(entry);
            }
            this.entries = new List<Entry>();

            String str = System.IO.Directory.GetCurrentDirectory();
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str + "\\Resources\\saveState2.txt";

            Database loadedData = new Database(str);
            Database originalData = new Database();

            foreach (Entry data in originalData.allEntries) {
                if (!loadedData.Contains(data)) {
                    
                    loadedData.addEntry(data);
                }
            }


            Console.WriteLine("Number of cards in saved data: " +  loadedData.allEntries.Count);
            foreach (Entry entry in loadedData.allEntries)
            {

                if (entry.attributes.ContainsKey("x") && entry.attributes.ContainsKey("y") && entry.attributes.ContainsKey("orientation"))
                {
                    Double x = Double.Parse(entry.attributes["x"].ToString());
                    Double y = Double.Parse(entry.attributes["y"].ToString()); ;

                    if (x > 1800)
                    {
                        x = 500;
                    }

                    if (y > 900)
                    {
                        y = 500;

                    }
                    entry.Center = new Point(x, y);
                    entry.Orientation = Double.Parse(entry.attributes["orientation"].ToString());
                    AddToScreen(entry);
                }
                else {
                    AddToScreen(entry);
                }
            } 

            ((SurfaceButton)(Load2.Content)).FontSize = 15;
            ((SurfaceButton)(Load2.Content)).Content = "Loaded";
        }
        private void AutoSaveButton_Click_1(object sender, RoutedEventArgs e) {
            foreach (Entry entry in this.entries)
            {
                MyScatterView.Items.Remove(entry);
            }
            this.entries = new List<Entry>();

            String str = System.IO.Directory.GetCurrentDirectory();
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str.Substring(0, str.LastIndexOf("\\"));
            str = str + "\\Resources\\autoSave.txt";

            Database loadedData = new Database(str);

            foreach (Entry entry in loadedData.allEntries)
            {
                Double x = Double.Parse(entry.attributes["x"].ToString());
                Double y = Double.Parse(entry.attributes["y"].ToString()); ;
                entry.Center = new Point(x, y);
                entry.Orientation = Double.Parse(entry.attributes["orientation"].ToString());
                AddToScreen(entry);
            }

            ((SurfaceButton)(AutoSaveButton.Content)).Content = "Loaded";
        
        }

        //void ruler_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (!hasClosedLabel)
        //    {
        //        if (MyScatterView.Items.Contains(sender))
        //        {
        //            MyScatterView.Items.Remove(sender);
        //            this.rulers.Remove((Ruler)sender);
        //        }
        //    }

        //    hasClosedLabel = false;
        //}
        //void FilterTile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (!((FilterTile)sender).onTextBox && MyScatterView.Items.Contains(sender))
        //    {
        //        this.filters.Remove((FilterTile)sender);
        //        MyScatterView.Items.Remove(sender);
        //    }

        //    ((FilterTile)sender).onTextBox = false;
        //}
        //void magicLens_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (!hasClosedLabel)
        //    {
        //        if (MyScatterView.Items.Contains(sender))
        //        {
        //            MyScatterView.Items.Remove(sender);
        //            this.lenses.Remove((MagicLens)sender);
        //        }
        //    }

        //    hasClosedLabel = false;
        //}
        //void pushPin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (MyScatterView.Items.Contains(sender))
        //    {
        //        MyScatterView.Items.Remove(sender);
        //        this.pushPins.Remove((PushPin)sender);
        //    }
        //}
        //void magnifyingGlass_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (!hasClosedLabel)
        //    {
        //        if (MyScatterView.Items.Contains(sender))
        //        {
        //            MyScatterView.Items.Remove(sender);
        //            this.mags.Remove((MagnifyingGlass)sender);
        //        }
        //    }

        //    hasClosedLabel = false;
        //}


        //void paperClip_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (!hasClosedLabel)
        //    {
        //        if (MyScatterView.Items.Contains(sender))
        //        {
        //            MyScatterView.Items.Remove(sender);
        //            this.paperClips.Remove((PaperClip)sender);
        //        }
        //    }

        //    hasClosedLabel = false;
        //}
        //void annotation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Canvas innerView = new Canvas();
        //    innerView.Width = 260;
        //    innerView.Height = 170;
        //    innerView.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));

        //    //Titie text box
        //    SurfaceTextBox titleText = new SurfaceTextBox();
        //    titleText.Foreground = Brushes.Black;
        //    titleText.Margin = new Thickness(8);
        //    titleText.Height = 30;
        //    titleText.Width = 243;
        //    titleText.FontSize = 14;
        //    titleText.FontWeight = FontWeights.Bold;
        //    titleText.Text = (String)((Label)((ScatterViewItem)sender).Content).Content;
        //    titleText.AcceptsReturn = false;
        //    titleText.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
        //    titleText.BorderBrush = Brushes.Transparent;

        //    //Content text box
        //    SurfaceTextBox txt = new SurfaceTextBox();
        //    txt.Foreground = Brushes.Black;
        //    txt.Margin = new Thickness(8);
        //    txt.FontSize = 14;
        //    txt.FontWeight = FontWeights.Bold;
        //    txt.Text = (String)((ScatterViewItem)sender).Tag;
        //    txt.Width = 243;
        //    txt.Height = 100;
        //    txt.AcceptsReturn = true;
        //    txt.AcceptsTab = true;
        //    txt.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
        //    txt.BorderBrush = Brushes.Transparent;

        //    //Attach
        //    innerView.Children.Add(titleText);
        //    Canvas.SetTop(titleText, 0);
        //    innerView.Children.Add(txt);
        //    Canvas.SetTop(txt, 52);

        //    //Final item
        //    ScatterViewItem note = new ScatterViewItem();
        //    note.Content = innerView;
        //    note.Orientation = 0;
        //    note.MinHeight = 0;
        //    note.Height = 170;
        //    note.MaxHeight = 1000;
        //    note.MaxWidth = 1000;
        //    note.Center = new Point(325, 625);
        //    note.MouseDoubleClick += new MouseButtonEventHandler(note_MouseDoubleClick);

        //    AddToScreen(note);
        //    //this.notes.Add(note);
        //    //MyScatterView.Items.Add(note);

        //    Canvas FrameFilters = (Canvas)((ScatterViewItem)sender).Parent;
        //    ScatterViewItem tool = (ScatterViewItem)FrameFilters.Parent;

        //    FrameFilters.Children.Remove((UIElement)sender);
        //}
        //void note_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (MyScatterView.Items.Contains(sender))
        //    {
        //        notes.Remove((ScatterViewItem)sender);
        //        MyScatterView.Items.Remove(sender);
        //    }
        //}
        //public void activeFilter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    this.hasClosedLabel = true;
        //    ScatterView FrameFilters = (ScatterView)((ScatterViewItem)sender).Parent;
        //    ScatterViewItem tool = (ScatterViewItem)FrameFilters.Parent;

        //    FrameFilters.Items.Remove(sender);
        //    if (tool is MagicLens)
        //    {
        //        ((MagicLens)tool).removeFilter((iFilter)((ScatterViewItem)sender).Tag);
        //    }
        //    if (tool is PaperClip)
        //    {
        //        ((PaperClip)tool).removeFilter((iFilter)((ScatterViewItem)sender).Tag);
        //    }

        //    e.Handled = true;
        //}
        //void AttributeFilter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    this.hasClosedLabel = true;

        //    String att = (String)((ScatterViewItem)sender).Tag;
        //    Canvas FrameFilters = (Canvas)((ScatterViewItem)sender).Parent;
        //    ScatterViewItem tool = (ScatterViewItem)FrameFilters.Parent;

        //    FrameFilters.Children.Remove((UIElement)sender);
        //    if (tool is MagnifyingGlass)
        //    {
        //        ((MagnifyingGlass)tool).removeAttribute(att);
        //    }
        //    e.Handled = true;

        //}



        #endregion

        #region Mouse Move
        private void entry_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            foreach (PushPin item in this.pushPins)
            {
                if (item.AreBoundaryIntersecting((FrameworkElement)sender))
                {
                    ((Entry)sender).CanMove = false;
                    ((Entry)sender).CanRotate = false;
                    item.SetRelativeZIndex(0);
                    Image im = new Image();

                    im.Source = Utils.NewEmbededResource("HabilisX.Resources.pinOccluded.gif");
                    item.Content = im;

                    Console.Out.WriteLine("Stopping a move");
                    return;
                }
            }
            ((Entry)sender).CanMove = true;
            ((Entry)sender).CanRotate = true;
        }
        private void entry_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            foreach (PushPin item in this.pushPins)
            {
                if (item.AreBoundaryIntersecting((FrameworkElement)sender))
                {
                    ((Entry)sender).CanMove = false;
                    ((Entry)sender).CanRotate = false;
                    item.SetRelativeZIndex(0);
                    Image im = new Image();

                    im.Source = Utils.NewEmbededResource("HabilisX.Resources.pinOccluded.gif");
                    item.Content = im;

                    Console.Out.WriteLine("Stopping a move");
                    return;
                }
            }
            ((Entry)sender).CanMove = true;
            ((Entry)sender).CanRotate = true;
        }

        private void note_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            if (MyScatterView.Items.Contains(sender))
            {

                foreach (Entry entry in this.entries)
                {
                    if (entry.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                    {
                        activateNote(entry, (Note)sender);
                        return;
                    }
                }
            }
        }
        private void note_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    foreach (Entry entry in this.entries)
                    {
                        if (entry.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {
                            activateNote(entry, (Note)sender);
                            return;
                        }
                    }
                }
            }
        }

        private void magnifier_PreviewTouchMove(object sender, TouchEventArgs e)
        {

            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            if (MyScatterView.Items.Contains(sender))
            {
                Entry detailedEntry = new Entry();
                Boolean foundOne = false;
                String details = "";
                foreach (Entry entry in this.entries)
                {
                    if (((MagnifyingGlass)sender).AreBoundaryIntersecting(entry) && !foundOne)
                    {
                        //Add to list and get one with highest z index;
                        detailedEntry = entry;
                        detailedEntry.Background = new SolidColorBrush(Color.FromArgb(230, 128, 128, 128));
                        details = ((MagnifyingGlass)sender).getDetails(entry);
                        //((MagnifyingGlass)sender).detailsText.Content = details;
                        foundOne = true;
                    }
                    else if (((MagnifyingGlass)sender).AreBoundaryIntersecting(entry) && foundOne)
                    {
                        if (Canvas.GetZIndex(entry) > Canvas.GetZIndex(detailedEntry))
                        {
                            detailedEntry.Background = new SolidColorBrush(Color.FromArgb(245, 191, 191, 191));
                            detailedEntry = entry;
                            detailedEntry.Background = new SolidColorBrush(Color.FromArgb(230, 128, 128, 128));
                            details = ((MagnifyingGlass)sender).getDetails(entry);
                        }
                    }
                    else
                    {
                        entry.Background = new SolidColorBrush(Color.FromArgb(245, 191, 191, 191));

                    }
                }

                if (foundOne && details.Length > 0)
                {
                    ((MagnifyingGlass)sender).detailsText.Background = new SolidColorBrush(Color.FromArgb(180, 128, 128, 128));
                }
                else
                {
                    ((MagnifyingGlass)sender).detailsText.Background = Brushes.Transparent;
                }

                ((MagnifyingGlass)sender).detailsText.Content = details;


            }

        }
        private void magnifier_MouseMove(object sender, MouseEventArgs e)
        {

            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            if (MyScatterView.Items.Contains(sender))
            {
                Entry detailedEntry = new Entry();
                Boolean foundOne = false;
                String details = "";
                foreach (Entry entry in this.entries)
                {
                    if (((MagnifyingGlass)sender).AreBoundaryIntersecting(entry) && !foundOne)
                    {
                        //Add to list and get one with highest z index;
                        detailedEntry = entry;
                        detailedEntry.Background = new SolidColorBrush(Color.FromArgb(230, 128, 128, 128));
                        details = ((MagnifyingGlass)sender).getDetails(entry);
                        //((MagnifyingGlass)sender).detailsText.Content = details;
                        foundOne = true;
                    }
                    else if (((MagnifyingGlass)sender).AreBoundaryIntersecting(entry) && foundOne)
                    {
                        if (Canvas.GetZIndex(entry) > Canvas.GetZIndex(detailedEntry))
                        {
                            detailedEntry.Background = new SolidColorBrush(Color.FromArgb(245, 191, 191, 191));
                            detailedEntry = entry;
                            detailedEntry.Background = new SolidColorBrush(Color.FromArgb(230, 128, 128, 128));
                            details = ((MagnifyingGlass)sender).getDetails(entry);
                        }
                    }
                    else
                    {
                        entry.Background = new SolidColorBrush(Color.FromArgb(245, 191, 191, 191));

                    }
                }

                if (foundOne && details.Length > 0)
                {
                    ((MagnifyingGlass)sender).detailsText.Background = new SolidColorBrush(Color.FromArgb(180, 128, 128, 128));
                }
                else
                {
                    ((MagnifyingGlass)sender).detailsText.Background = Brushes.Transparent;
                }

                ((MagnifyingGlass)sender).detailsText.Content = details;


            }
        }

        private void FilterTile_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            if (MyScatterView.Items.Contains(sender))
            {

                foreach (MagnifyingGlass glass in this.mags)
                {
                    if (glass.AreBoundaryIntersecting((ScatterViewItem)sender))
                    {
                        FilterTile tile = sender as FilterTile;
                        glass.activateFilterTile(tile.attTag.ToLower(), tile.Background, tile.attTag);
                        RemoveFromScreen((FilterTile)sender);
                        return;
                    }
                }
            }



            if (MyScatterView.Items.Contains(sender) && ((FilterTile)sender).hasInput())
            {

                foreach (MagicLens lens in this.lenses)
                {
                    if (lens.AreBoundaryIntersecting((FrameworkElement)sender))
                    {
                        iFilter query = ((FilterTile)sender).getFilter();
                        lens.activateFilterTile((String)((FilterTile)sender).attTag + ": " + query.getQueryString(), query.getColor(), query);
                        RemoveFromScreen((FilterTile)sender);
                        return;
                    }
                }

                foreach (PaperClip paperClip in this.paperClips)
                {
                    if (paperClip.AreBoundaryIntersecting((FrameworkElement)sender))
                    {
                        iFilter query = ((FilterTile)sender).getFilter();
                        paperClip.activateFilterTile((String)((FilterTile)sender).attTag + ": " + query.getQueryString(), query.getColor(), query);
                        RemoveFromScreen((FilterTile)sender);
                        return;
                    }
                }
            }
        }
        private void FilterTile_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }


            if (MyScatterView.Items.Contains(sender))
            {

                foreach (MagnifyingGlass glass in this.mags)
                {
                    if (glass.AreBoundaryIntersecting((ScatterViewItem)sender))
                    {
                        FilterTile tile = sender as FilterTile;
                        glass.activateFilterTile(tile.attTag.ToLower(), tile.Background, tile.attTag);
                        RemoveFromScreen((FilterTile)sender);
                        return;
                    }
                }
            }



            if (MyScatterView.Items.Contains(sender) && ((FilterTile)sender).hasInput())
            {

                foreach (MagicLens lens in this.lenses)
                {
                    if (lens.AreBoundaryIntersecting((FrameworkElement)sender))
                    {
                        iFilter query = ((FilterTile)sender).getFilter();
                        lens.activateFilterTile((String)((FilterTile)sender).attTag + ": " + query.getQueryString(), query.getColor(), query);
                        RemoveFromScreen((FilterTile)sender);
                        return;
                    }
                }

                foreach (PaperClip paperClip in this.paperClips)
                {
                    if (paperClip.AreBoundaryIntersecting((FrameworkElement)sender))
                    {
                        iFilter query = ((FilterTile)sender).getFilter();
                        paperClip.activateFilterTile((String)((FilterTile)sender).attTag + ": " + query.getQueryString(), query.getColor(), query);
                        RemoveFromScreen((FilterTile)sender);
                        return;
                    }
                }
            }
        }

        private void magicLens_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            //Console.WriteLine("num " + ((Tool)sender).numFingers);
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }


            foreach (Entry item in this.entries)
            {
                if (((MagicLens)sender).AreBoundaryIntersecting((FrameworkElement)item) &&//this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)item) &&
                   item.matchesAllFilters(((MagicLens)sender).filters))
                {
                    ((Canvas)(item.Content)).Background = ((MagicLens)sender).color;
                    item.SetRelativeZIndex(0);
                    ((MagicLens)sender).SetRelativeZIndex(0);
                }
                else
                {
                    ((Canvas)(item.Content)).Background = Brushes.Transparent;
                }
            }
        }
        private void magicLens_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            foreach (Entry item in this.entries)
            {
                if (((MagicLens)sender).AreBoundaryIntersecting((FrameworkElement)item) &&//this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)item) &&
                   item.matchesAllFilters(((MagicLens)sender).filters))
                {
                    ((Canvas)(item.Content)).Background = ((MagicLens)sender).color;
                    item.SetRelativeZIndex(0);
                    ((MagicLens)sender).SetRelativeZIndex(0);

                }
                else
                {
                    ((Canvas)(item.Content)).Background = Brushes.Transparent;
                }
            }
        }

        private void pushPin_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            foreach (Entry entry in this.entries)
            {
                if (((PushPin)sender).AreBoundaryIntersecting(entry))
                {
                    ((Image)((PushPin)(sender)).Content).Source = Utils.NewEmbededResource("HabilisX.Resources.pinOccluded.gif");

                    entry.CanMove = false;
                    entry.CanRotate = false;
                }
            }
        }
        private void pushPin_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((PushPin)sender).SetRelativeZIndex(0);
                Image im = new Image();
                im.Source = Utils.NewEmbededResource("HabilisX.Resources.pin.gif");
                ((PushPin)sender).Content = im;
            }
            else
            {
                foreach (Entry entry in this.entries)
                {
                    if (((PushPin)sender).AreBoundaryIntersecting(entry))
                    {
                        ((Image)((PushPin)(sender)).Content).Source = Utils.NewEmbededResource("HabilisX.Resources.pinOccluded.gif");

                        entry.CanMove = false;
                        entry.CanRotate = false;
                    }
                }
            }

        }

        private void paperClip_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            //Console.WriteLine(((Tool)sender).numFingers);
            if (!MyScatterView.Items.Contains(sender) || ((Tool)sender).numFingers >= 2)
            {
                return;
            }

            PaperClip clip = sender as PaperClip;
            foreach (Entry entry in this.entries)
            {

                if (clip.AreBoundaryIntersecting(entry) && (entry.matchesAllFilters(clip.filters) ||
               clip.filters.Count == 0) && !isAlreadyClipped(clip, entry))
                {
                    clip.addEntry(entry);
                }
                else
                {
                    clip.removeEntry(entry);
                }
            }



            int listoffset = -1;
            List<Entry> toOrganize = clip.toOrganize.ToList();
            if (toOrganize.Count() > 0)
            {
                String att = toOrganize.First().attributes.Keys.First();
                toOrganize.Sort((x, y) => string.Compare(x.printAttribute(att), y.printAttribute(att)));
            }

            foreach (Entry cur in toOrganize)
            {
                listoffset++;
                if (cur.CanMove)
                {
                    cur.SetRelativeZIndex(0);
                    double offset = cur.ActualWidth / 2 - (clip.ActualWidth / 2);
                    cur.Orientation = 0;
                    cur.Center = new Point(clip.Center.X + cur.Width / 2 - offset + 33, clip.Center.Y + cur.Height / 2 - 150 + 20 * listoffset);

                    //new Point(clip.Center.X + offset + 8, clip.Center.Y - (cur.ActualHeight / 4) + 20 * listoffset);
                }
            }

            clip.SetRelativeZIndex(0);
            if ((int)clip.Tag == 0 && clip.toOrganize.Count > 0)
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = Utils.NewEmbededResource("HabilisX.Resources.paperClipOccluded.png");
                ((Canvas)clip.Content).Background = ib;
                clip.Tag = 1;
            }
            else if ((int)clip.Tag == 1 && clip.toOrganize.Count == 0)
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = Utils.NewEmbededResource("HabilisX.Resources.paperClip.png");
                ((Canvas)clip.Content).Background = ib;
                clip.Tag = 0;


            }

        }
        private void paperClip_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            PaperClip clip = sender as PaperClip;
            foreach (Entry entry in this.entries)
            {

                if (clip.AreBoundaryIntersecting(entry) && (entry.matchesAllFilters(clip.filters) ||
               clip.filters.Count == 0) && !isAlreadyClipped(clip, entry))
                {
                    clip.addEntry(entry);
                }
                else
                {
                    clip.removeEntry(entry);
                }
            }



            int listoffset = -1;
            List<Entry> toOrganize = clip.toOrganize.ToList();
            if (toOrganize.Count() > 0)
            {
                String att = toOrganize.First().attributes.Keys.First();
                toOrganize.Sort((x, y) => string.Compare(x.printAttribute(att), y.printAttribute(att)));
            }
            foreach (Entry cur in toOrganize)
            {
                listoffset++;
                if (cur.CanMove)
                {
                    cur.SetRelativeZIndex(0);
                    double offset = cur.ActualWidth / 2 - (clip.ActualWidth / 2);
                    cur.Orientation = 0;
                    cur.Center = new Point(clip.Center.X + cur.Width / 2 - offset + 33, clip.Center.Y + cur.Height / 2 - 150 + 20 * listoffset);

                    //new Point(clip.Center.X + offset + 8, clip.Center.Y - (cur.ActualHeight / 4) + 20 * listoffset);
                }
            }
            clip.SetRelativeZIndex(0);
            if ((int)clip.Tag == 0 && clip.toOrganize.Count > 0)
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = Utils.NewEmbededResource("HabilisX.Resources.paperClipOccluded.png");
                ((Canvas)clip.Content).Background = ib;
                clip.Tag = 1;
            }
            else if ((int)clip.Tag == 1 && clip.toOrganize.Count == 0)
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = Utils.NewEmbededResource("HabilisX.Resources.paperClip.png");
                ((Canvas)clip.Content).Background = ib;
                clip.Tag = 0;


            }
        }

        private void ruler_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender) || ((Tool)sender).numFingers > 1)
            {
                return;
            }

            double deltaX = e.GetTouchPoint(MyScatterView).Position.X - lastMousePoint.X;
            //deltaX += Math.Sign(deltaX);

            double deltaY = e.GetTouchPoint(MyScatterView).Position.Y - lastMousePoint.Y;
            //deltaY += Math.Sign(deltaY);

            if (!MyScatterView.Items.Contains(sender))
            {
                lastDelta = new Point(deltaX, deltaY);
                lastMousePoint = e.GetTouchPoint(MyScatterView).Position;
                return;
            }

            foreach (Entry item in this.entries)
            {
                if (item.CanMove && ((Ruler)sender).AreBoundaryIntersecting((FrameworkElement)item))
                {
                    //Move Entry
                    //This is a terrible solution.  It mostly works;
                    int side = this.isCollidingOn((Ruler)sender, item);
                    Point rulerCenter = ((Ruler)sender).Center;
                    Point itemCenter = item.Center;

                    switch (side)
                    {
                        case TOP:
                            if (deltaY < 0)
                            {
                                moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                ((Canvas)(item.Content)).Background = Brushes.Blue;
                            }
                            break;
                        case RIGHT:
                            if (deltaX > 0)
                            {
                                moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                ((Canvas)(item.Content)).Background = Brushes.Blue;
                            }
                            break;
                        case BOTTOM:
                            if (deltaY > 0)
                            {
                                moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                ((Canvas)(item.Content)).Background = Brushes.Blue;
                            }
                            break;
                        case LEFT:
                            if (deltaX < 0)
                            {
                                moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                ((Canvas)(item.Content)).Background = Brushes.Blue;
                            }

                            break;
                    }

                    bool isClipped = false;
                    foreach (PaperClip clip in this.paperClips)
                    {
                        if (clip.AreBoundaryIntersecting(item))
                        {
                            isClipped = true;
                        }
                    }

                    if (!isClipped)
                    {
                        item.Orientation = this.findNewOrientation((Ruler)sender, item);
                    }
                    checkForPins(item);

                }
                else
                {
                    ((Canvas)(item.Content)).Background = Brushes.Transparent;
                }
            }

            lastDelta = new Point(deltaX, deltaY);
            lastMousePoint = e.GetTouchPoint(MyScatterView).Position;


        }
        private void ruler_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MyScatterView.Items.Contains(sender))
            {
                return;
            }

            double deltaX = e.GetPosition(MyScatterView).X - lastMousePoint.X;
            //deltaX += Math.Sign(deltaX);

            double deltaY = (e.GetPosition(MyScatterView)).Y - lastMousePoint.Y;
            //deltaY += Math.Sign(deltaY);

            if (!MyScatterView.Items.Contains(sender))
            {
                lastDelta = new Point(deltaX, deltaY);
                lastMousePoint = e.GetPosition(MyScatterView);
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                foreach (Entry item in this.entries)
                {
                    if (item.CanMove && ((Ruler)sender).AreBoundaryIntersecting((FrameworkElement)item))
                    {
                        //Move Entry
                        //This is a terrible solution.  It mostly works;
                        int side = this.isCollidingOn((Ruler)sender, item);
                        Point rulerCenter = ((Ruler)sender).Center;
                        Point itemCenter = item.Center;

                        switch (side)
                        {
                            case TOP:
                                if (deltaY < 0)
                                {
                                    moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                    ((Canvas)(item.Content)).Background = Brushes.Blue;
                                }
                                break;
                            case RIGHT:
                                if (deltaX > 0)
                                {
                                    moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                    ((Canvas)(item.Content)).Background = Brushes.Blue;
                                }
                                break;
                            case BOTTOM:
                                if (deltaY > 0)
                                {
                                    moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                    ((Canvas)(item.Content)).Background = Brushes.Blue;
                                }
                                break;
                            case LEFT:
                                if (deltaX < 0)
                                {
                                    moveEntry(item, new Point(item.Center.X + deltaX, item.Center.Y + deltaY));
                                    ((Canvas)(item.Content)).Background = Brushes.Blue;
                                }

                                break;
                        }

                        bool isClipped = false;
                        foreach (PaperClip clip in this.paperClips)
                        {
                            if (clip.AreBoundaryIntersecting(item))
                            {
                                isClipped = true;
                            }
                        }

                        if (!isClipped)
                        {
                            item.Orientation = this.findNewOrientation((Ruler)sender, item);
                        }
                        checkForPins(item);

                    }
                    else
                    {
                        ((Canvas)(item.Content)).Background = Brushes.Transparent;
                    }
                }
            }
            lastDelta = new Point(deltaX, deltaY);
            lastMousePoint = e.GetPosition(MyScatterView);
        }


        private void checkForPins(Entry entry)
        {
            bool foundPin = false;
            foreach (PushPin pin in this.pushPins)
            {
                if (pin.AreBoundaryIntersecting(entry))
                {
                    ((Image)pin.Content).Source = Utils.NewEmbededResource("HabilisX.Resources.pinOccluded.gif");

                    entry.CanMove = false;
                    entry.CanRotate = false;
                    foundPin = true;
                }
            }

            if (!foundPin)
            {
                entry.CanMove = true;
                entry.CanRotate = true;

            }
        }
        private void moveEntry(Entry entry, Point center)
        {
            double deltaX = center.X - entry.Center.X;
            double deltaY = center.Y - entry.Center.Y;

            foreach (PushPin pin in this.pushPins)
            {
                if (pin.AreBoundaryIntersecting(entry))
                {
                    return;
                }
            }

            foreach (PaperClip p in this.paperClips)
            {
                if (p.AreBoundaryIntersecting(entry))
                {
                    foreach (Entry e in this.entries)
                    {
                        if (p.AreBoundaryIntersecting(e))
                        {
                            if (!e.CanMove)
                            {
                                return;
                            }
                            else
                            {
                                e.Center = new Point(e.Center.X + deltaX, e.Center.Y + deltaY);
                            }
                        }
                    }
                    p.Center = new Point(p.Center.X + deltaX, p.Center.Y + deltaY);

                    return;
                }
            }

            entry.Center = new Point(entry.Center.X + deltaX, entry.Center.Y + deltaY);


        }
        #endregion

        #region Helper Methods




        public void AddToScreen(ScatterViewItem item)
        {
            if (item is MagnifyingGlass)
            {
                this.mags.Add((MagnifyingGlass)item);
            }
            else if (item is PushPin)
            {
                this.pushPins.Add((PushPin)item);
            }
            else if (item is Entry)
            {
                this.entries.Add((Entry)item);
            }
            else if (item is MagicLens)
            {
                this.lenses.Add((MagicLens)item);
            }
            else if (item is Ruler)
            {
                this.rulers.Add((Ruler)item);
            }
            else if (item is PaperClip)
            {
                this.paperClips.Add((PaperClip)item);
            }
            else if (item is FilterTile)
            {
                this.filters.Add((FilterTile)item);

            }
            else if (item is Note)
            {
                this.notes.Add(item);
            }
            else
            {
                Console.WriteLine("Error trying to add something to global access of type: " + item.GetType().ToString());
            }

            MyScatterView.Items.Add(item);
        }
        public void RemoveFromScreen(ScatterViewItem item)
        {
            if (!hasClosedLabel)
            {
                if (MyScatterView.Items.Contains(item))
                {

                    if (item is MagnifyingGlass)
                    {
                        this.mags.Remove((MagnifyingGlass)item);
                    }
                    else if (item is PushPin)
                    {
                        this.pushPins.Remove((PushPin)item);
                    }
                    else if (item is Entry)
                    {
                        this.entries.Remove((Entry)item);
                    }
                    else if (item is MagicLens)
                    {
                        this.lenses.Remove((MagicLens)item);
                    }
                    else if (item is Ruler)
                    {
                        this.rulers.Remove((Ruler)item);
                    }
                    else if (item is PaperClip)
                    {
                        this.paperClips.Remove((PaperClip)item);
                    }
                    else if (item is FilterTile)
                    {
                        this.filters.Remove((FilterTile)item);

                    }
                    else if (item is Note)
                    {
                        this.notes.Remove(item);
                    }
                    else
                    {
                        Console.WriteLine("Error trying to Remove something to global access of type: " + item.GetType().ToString());
                    }

                    MyScatterView.Dispatcher.BeginInvoke(new Action(() => MyScatterView.Items.Remove(item)));
                    //MyScatterView.Items.Remove(item);
                }
            }

            hasClosedLabel = false;

        }


        public Boolean isAlreadyClipped(PaperClip clip, Entry e)
        {
            foreach (PaperClip paperClip in this.paperClips)
            {
                if (paperClip != clip && paperClip.toOrganize.Contains(e) && (Canvas.GetZIndex(clip) < Canvas.GetZIndex(paperClip)))
                {
                    return true;
                }
            }

            return false;
        }

        private int isCollidingOn(Ruler tool, Entry entry)
        {
            Point difference = new Point(entry.Center.X - tool.Center.X, entry.Center.Y - tool.Center.Y);

            if (difference.X > 0 && Math.Abs(difference.X) > Math.Abs(difference.Y))
            {
                //Console.WriteLine("RIGHT");
                return RIGHT;
            }
            else if (difference.X < 0 && Math.Abs(difference.X) > Math.Abs(difference.Y))
            {
                //Console.WriteLine("LEFT");
                return LEFT;
            }
            else if (difference.Y < 0 && Math.Abs(difference.Y) > Math.Abs(difference.X))
            {
                //Console.WriteLine("TOP");
                return TOP;
            }
            else
            {
                //Console.WriteLine("BOTTOM");
                return BOTTOM;
            }
        }
        private double findNewOrientation(Ruler sender, Entry item)
        {


            double entryOrientation = item.Orientation;
            if (entryOrientation < -180)
            {
                entryOrientation += 360;
            }
            else if (entryOrientation > 180)
            {
                entryOrientation -= 360;
            }
            List<double> orientations = new List<double>();
            orientations.Add(((Ruler)sender).Orientation);
            orientations.Add((((Ruler)sender).Orientation + 90) % 360);
            orientations.Add((((Ruler)sender).Orientation + 180) % 360);
            orientations.Add((((Ruler)sender).Orientation + 270) % 360);
            orientations.Add(((Ruler)sender).Orientation - 360);
            orientations.Add(((((Ruler)sender).Orientation + 90) % 360) - 360);
            orientations.Add(((((Ruler)sender).Orientation + 180) % 360) - 360);
            orientations.Add(((((Ruler)sender).Orientation + 270) % 360) - 360);

            orientations.Sort();

            double orDiff = Math.Abs(entryOrientation - orientations[0]);
            double newOr = orientations[0];
            foreach (double d in orientations)
            {
                if (Math.Abs(entryOrientation - d) < orDiff)
                {
                    newOr = d;
                    orDiff = Math.Abs(entryOrientation - d);
                }
            }

            return newOr;

        }

        private void activateNote(Entry e, Note note)
        {
            e.addAnnotation(note);



            //remove big note
            RemoveFromScreen(note);

            //annotation.MouseDoubleClick += new MouseButtonEventHandler(annotation_MouseDoubleClick);


        }

        #endregion

        #region surface template functions
        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            bool isFinger = e.TouchDevice.GetIsFingerRecognized();
            bool isTag = e.TouchDevice.GetIsTagRecognized();
            if (isFinger == false && isTag == false)
            {
                e.Handled = true;
                return;
            }
            base.OnPreviewTouchDown(e);
        }

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
    }


    #region eventSubscriber
    class EventSubscriber
    {
        private static readonly MethodInfo HandleMethod =
            typeof(EventSubscriber)
                .GetMethod("HandleEvent",
                           BindingFlags.Instance |
                           BindingFlags.NonPublic);

        private readonly EventInfo evt;

        private EventSubscriber(EventInfo evt)
        {
            this.evt = evt;
        }

        private void HandleEvent(object sender, EventArgs args)
        {
            String str = "null";
            if (sender != null)
            {
                str = sender.GetType().Name;
            }
            Console.WriteLine("Event {0} fired {1}", evt.Name, str);
        }

        private void Subscribe(object target)
        {
            try
            {
                Delegate handler = Delegate.CreateDelegate(
                 evt.EventHandlerType, this, HandleMethod);
                evt.AddEventHandler(target, handler);

            }
            catch
            {
                try
                {
                    Delegate handler = Delegate.CreateDelegate(evt.EventHandlerType, this, HandleMethod, true);
                    evt.AddEventHandler(target, handler);
                }
                catch (Exception f)
                {
                    Console.WriteLine("exception: " + f);
                }
            }
        }

        public static void SubscribeAll(object target)
        {
            foreach (EventInfo evt in target.GetType().GetEvents())
            {
                EventSubscriber subscriber = new EventSubscriber(evt);
                subscriber.Subscribe(target);
            }
        }
    }
    #endregion


}