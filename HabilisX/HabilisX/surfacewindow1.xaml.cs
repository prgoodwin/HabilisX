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

            trash = new TrashCan();
            MyScatterView.Items.Add(trash);

            #region make and display the database

            //Initialize the database.  Currently (March '13) it is 10 hardcoded papers
            Database dataSet = new Database();

            #region Make Database Attribute Buttons
            for (int i = 0; i < dataSet.allAttributes.Keys.Count; i++)
            {
                String str = dataSet.allAttributes.Keys.ElementAt(i);

                ScatterViewItem newButton = new ScatterViewItem();
                newButton.Orientation = 0;
                newButton.Center = new Point(100 + (180 * i), 40);
                newButton.Height = 50;
                newButton.Width = 160;
                //newButton.Background = Brushes.DarkSlateGray;
                newButton.MinHeight = 0;
                newButton.CanMove = false;
                newButton.CanScale = false;
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
                    myButton.Click += new RoutedEventHandler(AddDateFilter_Click);
                }
                else if (dataSet.allAttributes[str].Equals(typeof(int)))
                {
                    myButton.Background = Brushes.LightGray;
                    myButton.Click += new RoutedEventHandler(AddIntFilter_Click);
                }
                else if (dataSet.allAttributes[str].Equals(typeof(List<int>)))
                {
                    myButton.Background = Brushes.Gray;
                    myButton.Click += new RoutedEventHandler(AddIntListFilter_Click);
                }


                Console.WriteLine("ALL ATTRIBUTES: " + str + ", " + dataSet.allAttributes[str].ToString());
            }

            #endregion

            #region make entries and display to screen

            //For every paper in the database, make a scatterview that shows the details
            foreach (Entry e in dataSet.allEntries)
            {
                AddToScreen(e);
                //MyScatterView.Items.Add(e);
                //this.entries.Add(e);
            }

            #endregion

            #endregion


            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }


        private void ScatterView_LayoutUpdated(object sender, EventArgs e)
        {
            this.mags.Sort();
            this.pushPins.Sort();
            this.entries.Sort();
            this.lenses.Sort();
            this.rulers.Sort();
            this.paperClips.Sort();
            this.filters.Sort();
            this.notes.Sort();

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
                    entry.CanScale = false;
                    entry.CanRotate = false;

                }
                else
                {
                    entry.CanMove = true;
                    entry.CanRotate = true;
                    entry.CanScale = true;
                }
                #endregion

                #region Ruler Interactions
                foreach (Ruler ruler in this.rulers)
                {
                    if (entry.CanMove && ruler.AreBoundaryIntersecting(entry))
                    {
                        if (entry.Orientation != this.findNewOrientation(ruler, entry))
                        {
                            entry.Orientation = this.findNewOrientation(ruler, entry);
                        }
                        Point center = entry.Center;
                        int side = ruler.BoundaryIntersectingOnSide(entry);//this.isCollidingOn(ruler, entry);


                        switch (side)
                        {
                            case TOP:
                                center.Y -= 1;
                                break;
                            case BOTTOM:
                                center.Y += 1;
                                break;
                            case LEFT:
                                center.X -= 1;
                                break;
                            case RIGHT:
                                center.X += 1;
                                break;
                            default:
                                break;
                        }

                        entry.Center = center;

                    }
                }
                #endregion

                #region PaperClip Interactions
                foreach (PaperClip clip in this.paperClips)
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
                #endregion

                #region MagicLens Interactions
                Boolean highlighted = false;
                foreach (MagicLens lens in this.lenses)
                {
                    if (lens.AreBoundaryIntersecting(entry) && entry.matchesAllFilters(lens.filters))
                    {
                        highlighted = true;
                    }
                }

                if (highlighted && (int)entry.Tag != 1)
                {
                    ((Canvas)(entry.Content)).Background = Brushes.Gold;
                    entry.Tag = 1;
                }
                else if (!highlighted && (int)entry.Tag != 0)
                {

                    ((Canvas)(entry.Content)).Background = Brushes.Transparent;
                    entry.Tag = 0;
                }
                #endregion

                #region Note Interactions
                foreach (ScatterViewItem note in this.notes)
                {
                    if (MyScatterView.Items.Contains(note) && entry.AreBoundaryIntersecting(note))
                    {
                        this.activateNote(entry, note);
                        return;
                    }
                }
                #endregion

            }

            foreach (PaperClip clip in this.paperClips)
            {
                int listoffset = -1;
                foreach (Entry cur in clip.toOrganize)
                {
                    listoffset++;
                    if (cur.CanMove)
                    {
                        cur.SetRelativeZIndex(0);
                        double offset = cur.ActualWidth / 2 - (clip.ActualWidth / 2);
                        cur.Orientation = 0;
                        cur.Center = new Point(clip.Center.X + offset + 8, clip.Center.Y - (cur.ActualHeight / 4) + 20 * listoffset);
                    }
                }
                clip.SetRelativeZIndex(0);
                if ((int)clip.Tag == 0 && clip.toOrganize.Count > 0)
                {
                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = Utils.NewEmbededResource("HabilisX.Resources.paperClipOccluded.png");
                    ((ScatterView)clip.Content).Background = ib;
                    clip.Tag = 1;
                }
                else if ((int)clip.Tag == 1 && clip.toOrganize.Count == 0)
                {
                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = Utils.NewEmbededResource("HabilisX.Resources.paperClip.png");
                    ((ScatterView)clip.Content).Background = ib;
                    clip.Tag = 0;


                }
            }
            #region Magnifying Glass interactions
            List<Entry> detailedEntries = new List<Entry>();
            //List<MagnifyingGlass> glasses = new List<MagnifyingGlass>();
            foreach (MagnifyingGlass glass in this.mags)
            {
                if (MyScatterView.Items.Contains(glass))
                {
                    Entry detailedEntry = new Entry();
                    Boolean foundOne = false;
                    foreach (Entry entry in this.entries)
                    {
                        if (glass.AreBoundaryIntersecting(entry) && !foundOne)
                        {
                            //Add to list and get one with highest z index;
                            detailedEntry = entry;
                            foundOne = true;
                        }
                        else if (glass.AreBoundaryIntersecting(entry) && foundOne)
                        {
                            if (Canvas.GetZIndex(entry) > Canvas.GetZIndex(detailedEntry))
                            {
                                detailedEntry = entry;
                            }
                        }

                    }
                    if (!foundOne)
                    {
                        //glasses.Add(glass);
                        glass.detailsText.Background = Brushes.Transparent;
                        glass.detailsText.Content = "";
                    }
                    detailedEntries.Add(detailedEntry);
                }

            }


            foreach (Entry entry in this.entries)
            {
                if (detailedEntries.Contains(entry) && !((SolidColorBrush)(entry.Background)).Color.ToString().Equals("#E6808080"))
                {
                    entry.Background = new SolidColorBrush(Color.FromArgb(230, 128, 128, 128));

                    int index = detailedEntries.IndexOf(entry);
                    MagnifyingGlass glass = this.mags[index];
                    glass.detailsText.Content = glass.getDetails(entry);
                    if (glass.getDetails(entry).Length > 0)
                    {
                        glass.detailsText.Background = new SolidColorBrush(Color.FromArgb(180, 128, 128, 128));
                    }
                }
                else if (!detailedEntries.Contains(entry) && ((SolidColorBrush)(entry.Background)).Color.ToString().Equals("#E6808080"))
                {
                    entry.Background = new SolidColorBrush(Color.FromArgb(230, 191, 191, 191));
                }
            }

            #endregion

            #region activate filter tiles if intersecting with tool
            List<FilterTile> toBeRemoved = new List<FilterTile>();
            foreach (FilterTile tile in this.filters)
            {
                foreach (MagnifyingGlass glass in this.mags)
                {
                    if (MyScatterView.Items.Contains(tile) && glass.AreBoundaryIntersecting(tile) && !toBeRemoved.Contains(tile))
                    {
                        ScatterViewItem filterTile = glass.activateMagnifyingGlassFilter(tile);//tile, tile.attTag, tile.Background);
                        //filterTile.MouseDoubleClick += new MouseButtonEventHandler(AttributeFilter_MouseDoubleClick);

                        toBeRemoved.Add(tile);
                        //MyScatterView.Items.Remove(tile);
                    }
                }

                foreach (MagicLens lens in lenses)
                {
                    if (MyScatterView.Items.Contains(tile) && lens.AreBoundaryIntersecting(tile) && !toBeRemoved.Contains(tile))
                    {
                        if (tile.hasInput())
                        {
                            ScatterViewItem filterTile = lens.activateMagicLensFilter(tile.getFilter());
                            //filterTile.MouseDoubleClick += new MouseButtonEventHandler(activeFilter_MouseDoubleClick);
                            toBeRemoved.Add(tile);
                            //RemoveFromScreen(tile);
                            //MyScatterView.Items.Remove(tile);
                        }
                    }
                }

                foreach (PaperClip clip in this.paperClips)
                {
                    if (MyScatterView.Items.Contains(tile) && clip.AreBoundaryIntersecting(tile) && !toBeRemoved.Contains(tile))
                    {
                        if (tile.hasInput())
                        {
                            ScatterViewItem filterTile = clip.activatePaperClipFilter(tile.getFilter());
                            //filterTile.MouseDoubleClick += new MouseButtonEventHandler(activeFilter_MouseDoubleClick);
                            toBeRemoved.Add(tile);
                            //MyScatterView.Items.Remove(tile);
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


        private void AddNewTool(Tool tool) {
            AddToScreen(tool);
            tool.MouseDown += new MouseButtonEventHandler(tool_MouseDown);
            //tool.TouchDown += new EventHandler<TouchEventArgs>(tool_TouchDown);
        }

        void tool_TouchDown(object sender, TouchEventArgs e)
        {
            throw new NotImplementedException();
        }
        void tool_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("start delete process");
            ThreadPool.QueueUserWorkItem(delegate
            {
                Thread.Sleep(1000);
                if (e.RightButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed)
                {

                    RemoveFromScreen((ScatterViewItem)sender);
                    Console.WriteLine("Time to delete");
                }
                else
                {
                    Console.WriteLine("delete false");
                }
            });


        }


        private void AddPushPinButton_Click(object sender, RoutedEventArgs e)
        {
            PushPin pushPin = new PushPin();
            AddNewTool(pushPin);
        }
        private void AddRulerButton_Click(object sender, RoutedEventArgs e)
        {
            Ruler ruler = new Ruler();
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
            AddNewTool(magnifier);
        }

        private void AddPaperClip_Click(object sender, RoutedEventArgs e)
        {
            PaperClip paperClip = new PaperClip();
            AddNewTool(paperClip);
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {

            Note note = new Note();
            AddNewTool(note);
        }

        private void save1_Click(object sender, RoutedEventArgs e)
        {
            saveState1 = new List<string>();

            foreach (Entry entry in this.entries)
            {
                saveState1.Add(System.Windows.Markup.XamlWriter.Save(entry));
                saveEntries1.Add(new Entry(entry.attributes));
            }

            ((SurfaceButton)(Save1.Content)).Content = "Saved";
        }
        private void load1_Click(object sender, RoutedEventArgs e)
        {
            if (saveState1.Count == 0)
            {
                return;
            }
            foreach (Entry entry in this.entries)
            {
                RemoveFromScreen(entry);
                //MyScatterView.Items.Remove(entry);
            }
            this.entries = new List<Entry>();

            for (int i = 0; i < saveState1.Count; i++)
            {
                String str = saveState1[i];
                Entry savedEntry = saveEntries1[i];
                Entry newEntry = (Entry)System.Windows.Markup.XamlReader.Parse(str);
                newEntry.addAllAttributes(savedEntry.attributes);
                AddToScreen(newEntry);
                //MyScatterView.Items.Add(newEntry);
                //this.entries.Add(newEntry);
            }
            ((SurfaceButton)(Load1.Content)).FontSize = 15;
            ((SurfaceButton)(Load1.Content)).Content = "Loaded";

        }
        private void save2_Click(object sender, RoutedEventArgs e)
        {
            saveState2 = new List<string>();
            foreach (Entry entry in this.entries)
            {
                saveState2.Add(System.Windows.Markup.XamlWriter.Save(entry));
                saveEntries2.Add(new Entry(entry.attributes));
            }

            ((SurfaceButton)(Save2.Content)).Content = "Saved";
        }
        private void load2_Click(object sender, RoutedEventArgs e)
        {

            if (saveState2.Count == 0)
            {
                return;
            }
            foreach (Entry entry in this.entries)
            {
                RemoveFromScreen(entry);
                //MyScatterView.Items.Remove(entry);
            }
            this.entries = new List<Entry>();

            for (int i = 0; i < saveState2.Count; i++)
            {
                String str = saveState2[i];
                Entry savedEntry = saveEntries2[i];
                Entry newEntry = (Entry)System.Windows.Markup.XamlReader.Parse(str);
                newEntry.addAllAttributes(savedEntry.attributes);
                AddToScreen(newEntry);
                //MyScatterView.Items.Add(newEntry);
                //this.entries.Add(newEntry);
            }
            ((SurfaceButton)(Load2.Content)).FontSize = 15;
            ((SurfaceButton)(Load2.Content)).Content = "Loaded";
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

        private void IntTile_TextChanged(object sender, TextChangedEventArgs e)
        {
            String str = (String)((TextBox)sender).Tag;

            if (((TextBox)sender).Text.Length < str.Length || !(((TextBox)sender).Text.Substring(0, str.Length).Equals(str)))
            {
                ((TextBox)sender).Text = str;
                ((TextBox)sender).Select(str.Length, 0);
            }

            if (((TextBox)sender).Text.Length >= str.Length + 1 && !(((TextBox)sender).Text[str.Length] == '=' || ((TextBox)sender).Text[str.Length] == '>' || ((TextBox)sender).Text[str.Length] == '<'))
            {
                ((TextBox)sender).Text = str;
                ((TextBox)sender).Select(str.Length, 0);

            }

        }

        #endregion

        #region Helper Methods


        private void clearDebug(object sender, RoutedEventArgs e)
        {
            ((SurfaceButton)(Debug.Content)).Content = "This is the debug";

        }
        void debugText(String str)
        {
            ((SurfaceButton)(Debug.Content)).Content = str;
            Console.WriteLine(str);

        }

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
            else if (item is ScatterViewItem)
            {
                Console.WriteLine("God I hope this is a note");
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
                    else if (item is ScatterViewItem)
                    {
                        Console.WriteLine("God I hope this is a note");
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
                Console.WriteLine("RIGHT");
                return RIGHT;
            }
            else if (difference.X < 0 && Math.Abs(difference.X) > Math.Abs(difference.Y))
            {
                Console.WriteLine("LEFT");
                return LEFT;
            }
            else if (difference.Y < 0 && Math.Abs(difference.Y) > Math.Abs(difference.X))
            {
                Console.WriteLine("TOP");
                return TOP;
            }
            else
            {
                Console.WriteLine("BOTTOM");
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

        private void activateNote(Entry e, ScatterViewItem note)
        {
            //Save the fields of the two text boxes
            String titleString = ((TextBox)((Canvas)(note.Content)).Children[0]).Text;
            String noteString = ((TextBox)((Canvas)(note.Content)).Children[1]).Text;

            //Make label out of title
            Label noteTitle = new Label();
            noteTitle.Content = titleString;
            noteTitle.Foreground = Brushes.Black;

            //remove big note
            RemoveFromScreen(note);

            //Make item that will be attached
            ScatterViewItem annotation = new ScatterViewItem();
            //annotation.MouseDoubleClick += new MouseButtonEventHandler(annotation_MouseDoubleClick);

            //Format visually
            annotation.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            annotation.ShowsActivationEffects = false;
            annotation.MinHeight = 0;

            //Attach 
            annotation.Content = noteTitle;
            annotation.Tag = noteString;

            ((Canvas)(e.Content)).Children.Add(annotation);
            Canvas.SetRight(annotation, 260);
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