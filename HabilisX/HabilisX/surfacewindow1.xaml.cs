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


namespace HabilisX
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        Assembly _assembly;
        Stream _imageStream;

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
        public List<Net> nets = new List<Net>();
        public List<Ruler> rulers = new List<Ruler>();
        public List<PaperClip> paperClips = new List<PaperClip>();
        Random num = new Random();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            #region make and display the database

            //Initialize the database.  Currently (March '13) it is 10 hardcoded papers
            Database dataSet = new Database();

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



            //For every paper in the database, make a scatterview that shows the details
            foreach (Entry e in dataSet.allEntries)
            {
                Label L = new Label();
                // Set the content of the label.
                L.Content = e.toString();
                L.FontSize = 12;
                //L.Height = 200;
                //L.Width = 300;



                Canvas innerView = new Canvas();
                innerView.Background = Brushes.Transparent;
                innerView.Children.Add(L);


                e.Width = 260;
                e.Height = 170;
                e.Content = innerView;
                e.Center = new Point(this.getNewX(), this.getNewY());
                e.Orientation = this.getNewOrientation();
                e.MouseMove += new MouseEventHandler(entry_MouseMove);
                MyScatterView.Items.Add(e);
                this.entries.Add(e);
            }


            #endregion


            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        #region Add Buttons & MouseEventHandlers
        private void AddStringFilter_Click(object sender, RoutedEventArgs e)
        {
            ScatterViewItem item = new ScatterViewItem();
            item.Center = new Point(290, 130);
            item.Orientation = 0;
            item.Background = Brushes.DarkSlateGray;
            item.MinHeight = 30;
            item.MouseMove += new MouseEventHandler(StringFilterTile_MouseMove);
            item.MouseDoubleClick += new MouseButtonEventHandler(FilterTile_MouseDoubleClick);


            TextBox txt = new TextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.Height = 30;
            txt.Width = 150;
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = (String)((SurfaceButton)sender).Tag + "=";
            txt.AcceptsReturn = false;
            txt.TextChanged += new TextChangedEventHandler(FilterTile_TextChanged);
            txt.Tag = ((SurfaceButton)sender).Tag;
            item.Content = txt;
            item.Tag = ((SurfaceButton)sender).Tag;


            Thickness bottomMargin = txt.Margin;
            bottomMargin.Bottom = 50;
            txt.Margin = bottomMargin;


            MyScatterView.Items.Add(item);
        }
        private void AddStringListFilter_Click(object sender, RoutedEventArgs e)
        {
            ScatterViewItem item = new ScatterViewItem();
            item.Center = new Point(300, 105);
            item.Orientation = 0;
            item.Background = Brushes.SlateGray;
            item.MinHeight = 30;
            item.MouseMove += new MouseEventHandler(StringListTile_MouseMove);
            item.MouseDoubleClick += new MouseButtonEventHandler(FilterTile_MouseDoubleClick);


            TextBox txt = new TextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.Height = 30;
            txt.Width = 150;
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = (String)((SurfaceButton)sender).Tag + "=";
            txt.AcceptsReturn = false;
            txt.TextChanged += new TextChangedEventHandler(FilterTile_TextChanged);
            txt.Tag = ((SurfaceButton)sender).Tag;

            item.Content = txt;
            item.Tag = ((SurfaceButton)sender).Tag;
            MyScatterView.Items.Add(item);
        }
        private void AddDateFilter_Click(object sender, RoutedEventArgs e)
        {
            ScatterViewItem item = new ScatterViewItem();
            item.Center = new Point(300, 170);
            item.Orientation = 0;
            item.Background = Brushes.LightGray;
            item.MinHeight = 30;

            item.MouseMove += new MouseEventHandler(DateTile_MouseMove);
            item.MouseDoubleClick += new MouseButtonEventHandler(FilterTile_MouseDoubleClick);


            TextBox txt = new TextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.Height = 30;
            txt.Width = 150;
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = (String)((SurfaceButton)sender).Tag + "=";
            txt.AcceptsReturn = false;
            txt.Tag = ((SurfaceButton)sender).Tag;
            txt.TextChanged += new TextChangedEventHandler(IntTile_TextChanged);

            item.Content = txt;
            item.Tag = ((SurfaceButton)sender).Tag;
            MyScatterView.Items.Add(item);
        }
        private void AddIntFilter_Click(object sender, RoutedEventArgs e)
        {
            ScatterViewItem item = new ScatterViewItem();
            item.Center = new Point(300, 170);
            item.Orientation = 0;
            item.Background = new SolidColorBrush(Color.FromRgb(191, 191, 191));
            item.MinHeight = 30;

            item.MouseMove += new MouseEventHandler(IntTile_MouseMove);
            item.MouseDoubleClick += new MouseButtonEventHandler(FilterTile_MouseDoubleClick);


            TextBox txt = new TextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.Height = 30;
            txt.Width = 150;
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = (String)((SurfaceButton)sender).Tag + "=";
            txt.AcceptsReturn = false;
            txt.Tag = ((SurfaceButton)sender).Tag;
            txt.TextChanged += new TextChangedEventHandler(IntTile_TextChanged);

            item.Content = txt;
            item.Tag = ((SurfaceButton)sender).Tag;
            MyScatterView.Items.Add(item);

        }
        private void AddIntListFilter_Click(object sender, RoutedEventArgs e)
        {
            ScatterViewItem item = new ScatterViewItem();
            item.Center = new Point(300, 105);
            item.Orientation = 0;
            item.Background = Brushes.SlateGray;
            item.MinHeight = 30;
            item.MouseMove += new MouseEventHandler(IntListTile_MouseMove);
            item.MouseDoubleClick += new MouseButtonEventHandler(FilterTile_MouseDoubleClick);


            TextBox txt = new TextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.Height = 30;
            txt.Width = 150;
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = (String)((SurfaceButton)sender).Tag + "=";
            txt.AcceptsReturn = false;
            txt.TextChanged += new TextChangedEventHandler(IntTile_TextChanged);
            txt.Tag = ((SurfaceButton)sender).Tag;

            item.Content = txt;
            item.Tag = ((SurfaceButton)sender).Tag;
            MyScatterView.Items.Add(item);
        }

        private void AddPushPinButton_Click(object sender, RoutedEventArgs e)
        {
            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.pin.gif");


            Image im = new Image();

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = _imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            im.Source = image;

            PushPin pushPin = new PushPin();
            pushPin.Orientation = 0;
            pushPin.Center = new Point(250, 235);
            pushPin.Background = Brushes.Transparent;
            pushPin.Height = 35;
            pushPin.MinHeight = 50;
            pushPin.MinWidth = 50;
            pushPin.Content = im;
            pushPin.CanRotate = false;
            pushPin.MouseDoubleClick += new MouseButtonEventHandler(pushPin_MouseDoubleClick);
            pushPin.MouseMove += new MouseEventHandler(pushPin_MouseMove);
            pushPin.ShowsActivationEffects = false;
            pushPin.BorderBrush = System.Windows.Media.Brushes.Transparent;

            RoutedEventHandler loadedEventHandler = null;
            loadedEventHandler = new RoutedEventHandler(delegate
            {
                pushPin.Loaded -= loadedEventHandler;
                Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
                ssc = pushPin.Template.FindName("shadow", pushPin) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
                ssc.Visibility = Visibility.Hidden;
            });
            pushPin.Loaded += loadedEventHandler;
            MyScatterView.Items.Add(pushPin);
            this.pushPins.Add(pushPin);

        }
        private void AddRulerButton_Click(object sender, RoutedEventArgs e)
        {

            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.ruler.png");

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = _imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();





            ImageBrush ib = new ImageBrush();
            ib.ImageSource = image;

            ScatterView innerView = new ScatterView();
            innerView.Background = ib;

            Ruler ruler = new Ruler();
            ruler.Content = innerView;
            ruler.Orientation = 0;
            ruler.MinHeight = 0;
            ruler.Height = 75;
            ruler.Width = 412;
            ruler.MaxHeight = 1000;
            ruler.MaxWidth = 1000;
            ruler.Center = new Point(431, 300);
            //ruler.Background = Brushes.Transparent;
            ruler.MouseMove += new MouseEventHandler(ruler_MouseMove);
            ruler.MouseDoubleClick += new MouseButtonEventHandler(ruler_MouseDoubleClick);


            MyScatterView.Items.Add(ruler);
            this.rulers.Add(ruler);

        }
        private void AddMagicLensButton_Click(object sender, RoutedEventArgs e)
        {
            MagicLens magicLens = new MagicLens();
            magicLens.Background = Brushes.Transparent;
            magicLens.Center = new Point(350, 415);
            magicLens.Orientation = 0;
            magicLens.MouseMove += new MouseEventHandler(scatterFrame_MouseMove);
            magicLens.MouseDoubleClick += new MouseButtonEventHandler(magicLens_MouseDoubleClick);

            ScatterView innerView = new ScatterView();
            innerView.BorderBrush = Brushes.Black;
            innerView.BorderThickness = new Thickness(10);

            magicLens.Content = innerView;

            MyScatterView.Items.Add(magicLens);
            this.lenses.Add(magicLens);
            // <s:ScatterViewItem Name="ScatterFrame" Background="Transparent" MouseMove="scatterFrame_MouseMove">
            //     <s:ScatterView Name="FrameFilters" BorderBrush="Black" BorderThickness="10"/>
            // </s:ScatterViewItem>
        }
        private void AddMagnifyingGlass_Click(object sender, RoutedEventArgs e)
        {
            Canvas innerView = new Canvas();
            innerView.Width = 100;
            innerView.Height = 100;

            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.magnifyingGlass.png");

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = _imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();

            ImageBrush im = new ImageBrush();
            im.ImageSource = image;

            innerView.Background = im;

            MagnifyingGlass magnifier = new MagnifyingGlass();
            magnifier.Orientation = 0;
            magnifier.Center = new Point(275, 450);
            magnifier.Background = Brushes.Transparent;
            magnifier.Content = innerView;
            magnifier.CanRotate = false;
            magnifier.MouseDoubleClick += new MouseButtonEventHandler(magnifyingGlass_MouseDoubleClick);
            magnifier.MouseMove += new MouseEventHandler(magnifier_MouseMove);

            magnifier.ShowsActivationEffects = false;
            magnifier.BorderBrush = System.Windows.Media.Brushes.Transparent;

            RoutedEventHandler loadedEventHandler = null;
            loadedEventHandler = new RoutedEventHandler(delegate
            {
                magnifier.Loaded -= loadedEventHandler;
                Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
                ssc = magnifier.Template.FindName("shadow", magnifier) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
                ssc.Visibility = Visibility.Hidden;
            });
            magnifier.Loaded += loadedEventHandler;
            MyScatterView.Items.Add(magnifier);
            mags.Add(magnifier);

            innerView.Children.Add(magnifier.detailsText);
            Canvas.SetLeft(magnifier.detailsText, 100);


        }
        private void AddPaperClip_Click(object sender, RoutedEventArgs e)
        {
            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.paperClip.png");

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = _imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = image;
            ScatterView innerView = new ScatterView();
            innerView.Background = ib;
            innerView.Width = 150;
            innerView.MinHeight = 5;
            innerView.MinWidth = 5;

            PaperClip paperClip = new PaperClip();
            paperClip.Content = innerView;
            paperClip.Orientation = 0;
            paperClip.Center = new Point(295, 495);
            paperClip.Background = Brushes.Transparent;
            paperClip.MinHeight = 5;
            paperClip.MinWidth = 5;
            paperClip.Height = 50;
            paperClip.Width = 150;
            paperClip.CanRotate = false;
            paperClip.MouseDoubleClick += new MouseButtonEventHandler(paperClip_MouseDoubleClick);
            paperClip.MouseMove += new MouseEventHandler(paperClip_MouseMove);
            paperClip.ShowsActivationEffects = false;
            paperClip.BorderBrush = System.Windows.Media.Brushes.Transparent;

            RoutedEventHandler loadedEventHandler = null;
            loadedEventHandler = new RoutedEventHandler(delegate
            {
                paperClip.Loaded -= loadedEventHandler;
                Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
                ssc = paperClip.Template.FindName("shadow", paperClip) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
                ssc.Visibility = Visibility.Hidden;
            });
            paperClip.Loaded += loadedEventHandler;
            MyScatterView.Items.Add(paperClip);
            this.paperClips.Add(paperClip);


        }
        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            Canvas innerView = new Canvas();
            innerView.Width = 260;
            innerView.Height = 170;
            innerView.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));

            //Titie text box
            TextBox titleText = new TextBox();
            titleText.Foreground = Brushes.Black;
            titleText.Margin = new Thickness(8);
            titleText.Height = 30;
            titleText.Width = 243;
            titleText.FontSize = 14;
            titleText.FontWeight = FontWeights.Bold;
            titleText.Text = "Note Title";
            titleText.AcceptsReturn = false;
            titleText.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            titleText.BorderBrush = Brushes.Transparent;

            //Content text box
            TextBox txt = new TextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = "Note Text";
            txt.Width = 243;
            txt.Height = 100;
            txt.AcceptsReturn = true;
            txt.AcceptsTab = true;
            txt.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            txt.BorderBrush = Brushes.Transparent;

            //Attach
            innerView.Children.Add(titleText);
            Canvas.SetTop(titleText, 0);
            innerView.Children.Add(txt);
            Canvas.SetTop(txt, 52);

            //Final item
            ScatterViewItem note = new ScatterViewItem();
            note.Content = innerView;
            note.Orientation = 0;
            note.MinHeight = 0;
            note.Height = 170;
            note.MaxHeight = 1000;
            note.MaxWidth = 1000;
            note.Center = new Point(350, 620);
            //note.MouseUp += new MouseButtonEventHandler(note_MouseMove);
            note.MouseMove += new MouseEventHandler(note_MouseMove);
            note.MouseDoubleClick += new MouseButtonEventHandler(note_MouseDoubleClick);

            MyScatterView.Items.Add(note);
        }

        private void AddNetButton_Click(object sender, RoutedEventArgs e)
        {

            _assembly = Assembly.GetExecutingAssembly();
            _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.net.png");

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = _imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = image;

            ScatterView innerView = new ScatterView();
            innerView.Background = ib;

            Net net = new Net();
            net.Content = innerView;
            net.Orientation = 0;
            net.Center = new Point(250, 500);
            net.Background = Brushes.Transparent;
            net.Height = 250;
            net.Width = 200;
            net.MouseMove += new MouseEventHandler(net_MouseMove);
            net.MouseDoubleClick += new MouseButtonEventHandler(net_MouseDoubleClick);
            net.ShowsActivationEffects = false;
            net.BorderBrush = System.Windows.Media.Brushes.Transparent;

            RoutedEventHandler loadedEventHandler = null;
            loadedEventHandler = new RoutedEventHandler(delegate
            {
                net.Loaded -= loadedEventHandler;
                Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
                ssc = net.Template.FindName("shadow", net) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
                ssc.Visibility = Visibility.Hidden;
            });
            net.Loaded += loadedEventHandler;

            MyScatterView.Items.Add(net);
            this.nets.Add(net);
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
                MyScatterView.Items.Remove(entry);
            }
            this.entries = new List<Entry>();

            for (int i = 0; i < saveState1.Count; i++)
            {
                String str = saveState1[i];
                Entry savedEntry = saveEntries1[i];
                Entry newEntry = (Entry)System.Windows.Markup.XamlReader.Parse(str);
                newEntry.addAllAttributes(savedEntry.attributes);
                MyScatterView.Items.Add(newEntry);
                this.entries.Add(newEntry);
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
                MyScatterView.Items.Remove(entry);
            }
            this.entries = new List<Entry>();

            for (int i = 0; i < saveState2.Count; i++)
            {
                String str = saveState2[i];
                Entry savedEntry = saveEntries2[i];
                Entry newEntry = (Entry)System.Windows.Markup.XamlReader.Parse(str);
                newEntry.addAllAttributes(savedEntry.attributes);
                MyScatterView.Items.Add(newEntry);
                this.entries.Add(newEntry);
            }
            ((SurfaceButton)(Load2.Content)).FontSize = 15;
            ((SurfaceButton)(Load2.Content)).Content = "Loaded";
        }


        void ruler_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!hasClosedLabel)
            {
                if (MyScatterView.Items.Contains(sender))
                {
                    MyScatterView.Items.Remove(sender);
                    this.rulers.Remove((Ruler)sender);
                }
            }

            hasClosedLabel = false;
        }
        void FilterTile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyScatterView.Items.Contains(sender))
            {
                MyScatterView.Items.Remove(sender);
            }
        }
        void magicLens_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!hasClosedLabel)
            {
                if (MyScatterView.Items.Contains(sender))
                {
                    MyScatterView.Items.Remove(sender);
                    this.lenses.Remove((MagicLens)sender);
                }
            }

            hasClosedLabel = false;
        }
        void net_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!hasClosedLabel)
            {
                if (MyScatterView.Items.Contains(sender))
                {
                    MyScatterView.Items.Remove(sender);
                    this.nets.Remove((Net)sender);
                }
            }

            hasClosedLabel = false;
        }
        void pushPin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyScatterView.Items.Contains(sender))
            {
                MyScatterView.Items.Remove(sender);
                this.pushPins.Remove((PushPin)sender);
            }
        }
        void magnifyingGlass_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!hasClosedLabel)
            {
                if (MyScatterView.Items.Contains(sender))
                {
                    MyScatterView.Items.Remove(sender);
                    this.mags.Remove((MagnifyingGlass)sender);
                }
            }

            hasClosedLabel = false;
        }

        void paperClip_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!hasClosedLabel)
            {
                if (MyScatterView.Items.Contains(sender))
                {
                    MyScatterView.Items.Remove(sender);
                    this.paperClips.Remove((PaperClip)sender);
                }
            }

            hasClosedLabel = false;
        }
        void annotation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Canvas innerView = new Canvas();
            innerView.Width = 260;
            innerView.Height = 170;
            innerView.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));

            //Titie text box
            TextBox titleText = new TextBox();
            titleText.Foreground = Brushes.Black;
            titleText.Margin = new Thickness(8);
            titleText.Height = 30;
            titleText.Width = 243;
            titleText.FontSize = 14;
            titleText.FontWeight = FontWeights.Bold;
            titleText.Text = (String)((Label)((ScatterViewItem)sender).Content).Content;
            titleText.AcceptsReturn = false;
            titleText.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            titleText.BorderBrush = Brushes.Transparent;

            //Content text box
            TextBox txt = new TextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = (String)((ScatterViewItem)sender).Tag;
            txt.Width = 243;
            txt.Height = 100;
            txt.AcceptsReturn = true;
            txt.AcceptsTab = true;
            txt.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            txt.BorderBrush = Brushes.Transparent;

            //Attach
            innerView.Children.Add(titleText);
            Canvas.SetTop(titleText, 0);
            innerView.Children.Add(txt);
            Canvas.SetTop(txt, 52);

            //Final item
            ScatterViewItem note = new ScatterViewItem();
            note.Content = innerView;
            note.Orientation = 0;
            note.MinHeight = 0;
            note.Height = 170;
            note.MaxHeight = 1000;
            note.MaxWidth = 1000;
            note.Center = new Point(325, 625);
            note.MouseMove += new MouseEventHandler(note_MouseMove);
            note.MouseDoubleClick += new MouseButtonEventHandler(note_MouseDoubleClick);

            MyScatterView.Items.Add(note);

            Canvas FrameFilters = (Canvas)((ScatterViewItem)sender).Parent;
            ScatterViewItem tool = (ScatterViewItem)FrameFilters.Parent;

            FrameFilters.Children.Remove((UIElement)sender);
        }
        void note_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyScatterView.Items.Contains(sender))
            {
                MyScatterView.Items.Remove(sender);
            }
        }
        void activeFilter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.hasClosedLabel = true;
            ScatterView FrameFilters = (ScatterView)((ScatterViewItem)sender).Parent;
            ScatterViewItem tool = (ScatterViewItem)FrameFilters.Parent;

            FrameFilters.Items.Remove(sender);
            if (tool is MagicLens)
            {
                ((MagicLens)tool).removeFilter((iFilter)((ScatterViewItem)sender).Tag);
            }

            if (tool is Net)
            {
                ((Net)tool).removeFilter((iFilter)((ScatterViewItem)sender).Tag);
            }
            if (tool is PaperClip)
            {
                ((PaperClip)tool).removeFilter((iFilter)((ScatterViewItem)sender).Tag);
            }

            e.Handled = true;
        }
        void AttributeFilter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.hasClosedLabel = true;

            String att = (String)((ScatterViewItem)sender).Tag;
            Canvas FrameFilters = (Canvas)((ScatterViewItem)sender).Parent;
            ScatterViewItem tool = (ScatterViewItem)FrameFilters.Parent;

            FrameFilters.Children.Remove((UIElement)sender);
            if (tool is MagnifyingGlass)
            {
                ((MagnifyingGlass)tool).removeAttribute(att);
            }
            e.Handled = true;

        }

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
        private void FilterTile_TextChanged(object sender, TextChangedEventArgs e)
        {
            String str = (String)((TextBox)sender).Tag + "=";
            if (((TextBox)sender).Text.Length < str.Length || !(((TextBox)sender).Text.Substring(0, str.Length).Equals(str)))
            {
                ((TextBox)sender).Text = str;
                ((TextBox)sender).Select(str.Length, 0);
            }
        }

        #endregion

        #region mouseMoved Events & activeFilterEventHandlers

        private void entry_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (PushPin item in this.pushPins)
            {
                if (item.AreBoundaryIntersecting((FrameworkElement)sender))
                {
                    ((Entry)sender).CanMove = false;
                    ((Entry)sender).CanScale = false;
                    ((Entry)sender).CanRotate = false;
                    item.SetRelativeZIndex(0);
                    Image im = new Image();
                    _assembly = Assembly.GetExecutingAssembly();
                    _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.pinOccluded.gif");

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = _imageStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    im.Source = image;
                    item.Content = im;

                    //item.bring

                    Console.Out.WriteLine("Stopping a move");
                    return;
                }
            }
            ((Entry)sender).CanMove = true;
            ((Entry)sender).CanRotate = true;
            ((Entry)sender).CanScale = true;
        }
        private void note_MouseMove(object sender, MouseEventArgs e)
        {

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    foreach (Entry entry in this.entries)
                    {
                        if (entry.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {
                            activateNote(entry, (ScatterViewItem)sender);
                            return;
                        }
                    }
                }
            }


        }
        private void magnifier_MouseMove(object sender, MouseEventArgs e)
        {
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
                            detailedEntry.Background = new SolidColorBrush(Color.FromArgb(230, 191, 191, 191));
                            detailedEntry = entry;
                            detailedEntry.Background = new SolidColorBrush(Color.FromArgb(230, 128, 128, 128));
                            details = ((MagnifyingGlass)sender).getDetails(entry);
                        }
                    }
                    else
                    {
                        entry.Background = new SolidColorBrush(Color.FromArgb(230, 191, 191, 191));

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

        private void StringFilterTile_MouseMove(object sender, MouseEventArgs e)
        {
            String attribute = (String)((ScatterViewItem)sender).Tag;
            String filterContent = ((TextBox)(((ScatterViewItem)sender).Content)).Text;
            String userInput = "";

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Released)
                {
                    foreach (MagnifyingGlass glass in this.mags)
                    {
                        if (glass.AreBoundaryIntersecting((ScatterViewItem)sender))
                        {
                            this.activateMagnifyingGlassFilter(sender, attribute, glass, Brushes.DarkSlateGray);
                            return;
                        }
                    }
                }
            }

            if (filterContent.Length > attribute.Length + 1 && filterContent.Substring(0, attribute.Length + 1).Equals(attribute + "="))
            {
                userInput = filterContent.Substring(attribute.Length + 1);
            }
            else
            {
                return;
            }

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Released)
                {
                    foreach (MagicLens lens in this.lenses)
                    {
                        if (lens.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {
                            this.activateMagicLensFilter(sender, new StringFilter(userInput, attribute), lens);
                            return;
                        }
                    }

                    foreach (Net net in this.nets)
                    {
                        if (net.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {

                            activateNetFilter(sender, new StringFilter(userInput, attribute), net);
                            return;
                        }
                    }

                    foreach (PaperClip paperClip in this.paperClips)
                    {
                        if (paperClip.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {

                            activatePaperClipFilter(sender, new StringFilter(userInput, attribute), paperClip);
                            return;
                        }
                    }
                }
            }
        }
        private void StringListTile_MouseMove(object sender, MouseEventArgs e)
        {
            String attribute = (String)((ScatterViewItem)sender).Tag;
            String filterContent = ((TextBox)(((ScatterViewItem)sender).Content)).Text;
            String userInput = "";

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Released)
                {
                    foreach (MagnifyingGlass glass in this.mags)
                    {
                        if (glass.AreBoundaryIntersecting((ScatterViewItem)sender))
                        {
                            this.activateMagnifyingGlassFilter(sender, attribute, glass, Brushes.SlateGray);
                            return;
                        }
                    }
                }
            }



            if (filterContent.Length > attribute.Length + 1 && filterContent.Substring(0, attribute.Length + 1).Equals(attribute + "="))
            {
                userInput = filterContent.Substring(attribute.Length + 1);
            }
            else
            {
                return;
            }
            if (MyScatterView.Items.Contains(sender))
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    foreach (MagicLens lens in this.lenses)
                    {
                        if (lens.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {
                            this.activateMagicLensFilter(sender, new StringListFilter(userInput, attribute), lens);
                            return;
                        }
                    }

                    foreach (Net net in this.nets)
                    {
                        if (net.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {

                            activateNetFilter(sender, new StringListFilter(userInput, attribute), net);
                            return;
                        }
                    }
                    foreach (PaperClip paperClip in this.paperClips)
                    {
                        if (paperClip.AreBoundaryIntersecting((FrameworkElement)sender)) //this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)ScatterFrame))
                        {

                            activatePaperClipFilter(sender, new StringListFilter(userInput, attribute), paperClip);
                            return;
                        }
                    }

                }
            }
        }
        private void DateTile_MouseMove(object sender, MouseEventArgs e)
        {
            String attribute = (String)((ScatterViewItem)sender).Tag;
            String filterContent = ((TextBox)(((ScatterViewItem)sender).Content)).Text;
            String userInput = "";

            int year = 0;

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Released)
                {
                    foreach (MagnifyingGlass glass in this.mags)
                    {
                        if (glass.AreBoundaryIntersecting((ScatterViewItem)sender))
                        {
                            this.activateMagnifyingGlassFilter(sender, attribute, glass, Brushes.LightGray);
                            return;
                        }
                    }
                }
            }


            if (filterContent.Length > attribute.Length + 1 && filterContent.Substring(0, attribute.Length).Equals(attribute) && (filterContent[attribute.Length].Equals('>') ||
               filterContent[attribute.Length].Equals('<') || filterContent[attribute.Length].Equals('=')))
            {
                userInput = filterContent.Substring(attribute.Length + 1);
                try
                {
                    year = Convert.ToInt32(userInput);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Not a number in DateTile: " + ex);
                };
            }
            else
            {
                return;
            }


            if (MyScatterView.Items.Contains(sender))
            {
                if (e.LeftButton == MouseButtonState.Released)
                {

                    foreach (MagicLens lens in this.lenses)
                    {
                        if (lens.AreBoundaryIntersecting((FrameworkElement)sender))
                        {
                            this.activateMagicLensFilter(sender, new DateFilter(new DateTime(year, 1, 1),
                               filterContent[attribute.Length], attribute), lens);
                            return;
                        }
                    }

                    foreach (Net net in this.nets)
                    {
                        if (net.AreBoundaryIntersecting((FrameworkElement)sender))
                        {

                            activateNetFilter(sender, new DateFilter(new DateTime(year, 1, 1),
                               filterContent[attribute.Length], attribute), net);
                            return;
                        }
                    }
                    foreach (PaperClip paperClip in this.paperClips)
                    {
                        if (paperClip.AreBoundaryIntersecting((FrameworkElement)sender))
                        {

                            activatePaperClipFilter(sender, new DateFilter(new DateTime(year, 1, 1),
                               filterContent[attribute.Length], attribute), paperClip);
                            return;
                        }
                    }
                }
            }
        }
        private void IntTile_MouseMove(object sender, MouseEventArgs e)
        {
            String attribute = (String)((ScatterViewItem)sender).Tag;
            String filterContent = ((TextBox)(((ScatterViewItem)sender).Content)).Text;
            String userInput = "";

            int queryNumber = 0;

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Released)
                {
                    foreach (MagnifyingGlass glass in this.mags)
                    {
                        if (glass.AreBoundaryIntersecting((ScatterViewItem)sender))
                        {
                            this.activateMagnifyingGlassFilter(sender, attribute, glass, new SolidColorBrush(Color.FromRgb(191, 191, 191)));
                            return;
                        }
                    }
                }
            }


            if (filterContent.Length > attribute.Length + 1 && filterContent.Substring(0, attribute.Length).Equals(attribute) && (filterContent[attribute.Length].Equals('>') ||
               filterContent[attribute.Length].Equals('<') || filterContent[attribute.Length].Equals('=')))
            {
                userInput = filterContent.Substring(attribute.Length + 1);
                try
                {
                    queryNumber = Convert.ToInt32(userInput);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Not a number in IntTile: " + ex);
                }
            }
            else
            {
                return;
            }


            if (MyScatterView.Items.Contains(sender))
            {
                if (e.LeftButton == MouseButtonState.Released)
                {

                    foreach (MagicLens lens in this.lenses)
                    {
                        if (lens.AreBoundaryIntersecting((FrameworkElement)sender))
                        {
                            this.activateMagicLensFilter(sender, new IntFilter(queryNumber,
                               filterContent[attribute.Length], attribute), lens);
                            return;
                        }
                    }

                    foreach (Net net in this.nets)
                    {
                        if (net.AreBoundaryIntersecting((FrameworkElement)sender))
                        {

                            activateNetFilter(sender, new IntFilter(queryNumber,
                               filterContent[attribute.Length], attribute), net);
                            return;
                        }
                    }
                    foreach (PaperClip paperClip in this.paperClips)
                    {
                        if (paperClip.AreBoundaryIntersecting((FrameworkElement)sender))
                        {
                            activatePaperClipFilter(sender, new IntFilter(queryNumber,
                               filterContent[attribute.Length], attribute), paperClip);
                            return;
                        }
                    }
                }
            }
        }
        private void IntListTile_MouseMove(object sender, MouseEventArgs e)
        {
            String attribute = (String)((ScatterViewItem)sender).Tag;
            String filterContent = ((TextBox)(((ScatterViewItem)sender).Content)).Text;
            String userInput = "";

            int queryNumber = 0;

            if (MyScatterView.Items.Contains(sender))
            {

                if (e.LeftButton == MouseButtonState.Released)
                {
                    foreach (MagnifyingGlass glass in this.mags)
                    {
                        if (glass.AreBoundaryIntersecting((ScatterViewItem)sender))
                        {
                            this.activateMagnifyingGlassFilter(sender, attribute, glass, new SolidColorBrush(Color.FromRgb(191, 191, 191)));
                            return;
                        }
                    }
                }
            }


            if (filterContent.Length > attribute.Length + 1 && filterContent.Substring(0, attribute.Length).Equals(attribute) && (filterContent[attribute.Length].Equals('>') ||
               filterContent[attribute.Length].Equals('<') || filterContent[attribute.Length].Equals('=')))
            {
                userInput = filterContent.Substring(attribute.Length + 1);
                try
                {
                    queryNumber = Convert.ToInt32(userInput);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Not a number in IntTile: " + ex);
                }
            }
            else
            {
                return;
            }


            if (MyScatterView.Items.Contains(sender))
            {
                if (e.LeftButton == MouseButtonState.Released)
                {

                    foreach (MagicLens lens in this.lenses)
                    {
                        if (lens.AreBoundaryIntersecting((FrameworkElement)sender))
                        {
                            this.activateMagicLensFilter(sender, new IntListFilter(queryNumber,
                               filterContent[attribute.Length], attribute), lens);
                            return;
                        }
                    }

                    foreach (Net net in this.nets)
                    {
                        if (net.AreBoundaryIntersecting((FrameworkElement)sender))
                        {

                            activateNetFilter(sender, new IntListFilter(queryNumber,
                               filterContent[attribute.Length], attribute), net);
                            return;
                        }
                    }
                    foreach (PaperClip paperClip in this.paperClips)
                    {
                        if (paperClip.AreBoundaryIntersecting((FrameworkElement)sender))
                        {
                            activatePaperClipFilter(sender, new IntListFilter(queryNumber,
                               filterContent[attribute.Length], attribute), paperClip);
                            return;
                        }
                    }
                }
            }
        }

        private void scatterFrame_MouseMove(object sender, MouseEventArgs e)
        {
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
                }
                else
                {
                    ((Canvas)(item.Content)).Background = Brushes.Transparent;
                }
            }
        }
        private void pushPin_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((PushPin)sender).SetRelativeZIndex(0);
                Image im = new Image();
                _assembly = Assembly.GetExecutingAssembly();
                _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.pin.gif");

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = _imageStream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                im.Source = image;
                ((PushPin)sender).Content = im;
            }
            else
            {
                foreach (Entry entry in this.entries)
                {
                    if (((PushPin)sender).AreBoundaryIntersecting(entry))
                    {
                        Image im = new Image();
                        _assembly = Assembly.GetExecutingAssembly();
                        _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.pinOccluded.gif");

                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = _imageStream;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        image.Freeze();
                        im.Source = image;
                        ((Image)((PushPin)(sender)).Content).Source = image;

                        entry.CanMove = false;
                        entry.CanRotate = false;
                        entry.CanScale = false;
                    }
                }
            }

        }

        private void paperClip_MouseMove(object sender, MouseEventArgs e)
        {
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

            List<Entry> toOrganize = new List<Entry>();

            foreach (Entry entry in this.entries)
            {
                if (((PaperClip)sender).AreBoundaryIntersecting(entry) && (entry.matchesAllFilters(((PaperClip)sender).filters) ||
                   ((PaperClip)sender).filters.Count == 0))
                {
                    toOrganize.Add(entry);
                }
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {

                if (toOrganize.Count != 0)
                {

                    for (int i = 0; i < toOrganize.Count; i++)
                    {
                        Entry cur = toOrganize[i];
                        cur.SetRelativeZIndex(0);
                        double offset = cur.ActualWidth / 2 - (((PaperClip)sender).ActualWidth / 2);
                        cur.Orientation = 0;
                        cur.Center = new Point(((PaperClip)sender).Center.X + offset + 8, ((PaperClip)sender).Center.Y - (cur.ActualHeight / 4) + 20 * i);
                    }
                    ((PaperClip)sender).SetRelativeZIndex(0);
                    _assembly = Assembly.GetExecutingAssembly();
                    _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.paperClipOccluded.png");

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = _imageStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = image;



                    ((ScatterView)((PaperClip)sender).Content).Background = ib;

                }
            }
            else
            {
                foreach (Entry entry in toOrganize)
                {
                    entry.SetRelativeZIndex(0);
                    entry.Center = new Point(entry.Center.X + deltaX, entry.Center.Y + deltaY);
                }
                ((PaperClip)sender).SetRelativeZIndex(0);
            }

            lastDelta = new Point(deltaX, deltaY);
            lastMousePoint = e.GetPosition(MyScatterView);
        }
        private void ruler_MouseMove(object sender, MouseEventArgs e)
        {
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

        private void net_MouseMove(object sender, MouseEventArgs e)
        {
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
                    if (((Net)sender).AreBoundaryIntersecting((FrameworkElement)item) &&//this.AreBoundaryIntersecting((FrameworkElement)sender, (FrameworkElement)item) &&
                       item.matchesAllFilters(((Net)sender).filters))
                    {
                        //Move Entry
                        //This is a terrible solution.  It mostly works;
                        int side = this.isCollidingOn((Net)sender, item);
                        Point netCenter = ((Net)sender).Center;
                        Point itemCenter = item.Center;

                        //double distance = ((Net)sender).ActualHeight / 2 + item.ActualHeight / 2;

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


        #endregion

        #region Helper Methods

        private void checkForPins(Entry entry)
        {
            bool foundPin = false;
            foreach (PushPin pin in this.pushPins)
            {
                if (pin.AreBoundaryIntersecting(entry))
                {
                    Image im = new Image();
                    _assembly = Assembly.GetExecutingAssembly();
                    _imageStream = _assembly.GetManifestResourceStream("HabilisX.Resources.pinOccluded.gif");

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = _imageStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    im.Source = image;
                    ((Image)pin.Content).Source = image;

                    entry.CanMove = false;
                    entry.CanRotate = false;
                    entry.CanScale = false;
                    foundPin = true;
                }
            }

            if (!foundPin)
            {
                entry.CanMove = true;
                entry.CanRotate = true;
                entry.CanScale = true;

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
        private int isCollidingOn(Net tool, Entry entry)
        {
            Point difference = new Point(entry.Center.X - tool.Center.X, entry.Center.Y - tool.Center.Y);

            if (difference.X > 0 && Math.Abs(difference.X) > Math.Abs(difference.Y))
            {
                return RIGHT;
            }
            else if (difference.X < 0 && Math.Abs(difference.X) > Math.Abs(difference.Y))
            {
                return LEFT;
            }
            else if (difference.Y < 0 && Math.Abs(difference.Y) > Math.Abs(difference.X))
            {
                return TOP;
            }
            else
            {
                return BOTTOM;
            }
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
        private double getNewX()
        {
            return num.Next(400, 1200);
        }
        private double getNewY()
        {
            return num.Next(50, 650);
        }
        private double getNewOrientation()
        {
            return num.Next(-60, 60);
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

        private void activateMagnifyingGlassFilter(object sender, String att, MagnifyingGlass e, Brush color)
        {


            //Make label out of title
            Label filterTitle = new Label();
            filterTitle.Content = att.ToString().ToLower();
            filterTitle.Foreground = Brushes.White;

            //remove big note
            MyScatterView.Items.Remove(sender);

            //Make item that will be attached
            ScatterViewItem Filter = new ScatterViewItem();
            Filter.MouseDoubleClick += new MouseButtonEventHandler(AttributeFilter_MouseDoubleClick);
            Filter.Tag = att;

            //Format visually
            Filter.Background = color;
            Filter.ShowsActivationEffects = false;
            Filter.MinHeight = 0;
            Filter.Height = 35;
            double y = (40 * (e.attributes.Count - 1)) + 10;

            //Attach 
            Filter.Content = filterTitle;

            ((Canvas)(e.Content)).Children.Add(Filter);
            Canvas.SetRight(Filter, 105);
            Canvas.SetTop(Filter, y);

            e.addAttribute(att);

        }
        private void activateNetFilter(object sender, iFilter query, Net net)
        {
            MyScatterView.Items.Remove(sender);

            ScatterViewItem filterTile = new ScatterViewItem();
            filterTile.MinHeight = 0;
            filterTile.Background = Brushes.Transparent;
            filterTile.ShowsActivationEffects = false;
            filterTile.MouseDoubleClick += new MouseButtonEventHandler(activeFilter_MouseDoubleClick);
            filterTile.Tag = query;

            Label filter = new Label();
            filter.Content = query.getQueryString();
            filter.Foreground = Brushes.White;
            filter.Background = query.getColor();

            ((ScatterView)(net.Content)).Items.Add(filterTile);

            filterTile.Content = filter;
            double x = (15 * net.filters.Count) + 17;
            double y = (40 * net.filters.Count) + 35;
            filterTile.Center = new Point(2 * net.ActualWidth / 3 - x, net.ActualHeight / 2 + y);
            filterTile.Orientation = 20;
            filterTile.CanMove = false;
            filterTile.CanRotate = false;
            filterTile.CanScale = false;
            net.addFilter(query);
        }
        private void activateMagicLensFilter(object sender, iFilter query, MagicLens lens)
        {
            MyScatterView.Items.Remove(sender);

            ScatterViewItem filterTile = new ScatterViewItem();
            filterTile.MinHeight = 0;
            filterTile.Background = Brushes.Transparent;
            filterTile.ShowsActivationEffects = false;
            filterTile.MouseDoubleClick += new MouseButtonEventHandler(activeFilter_MouseDoubleClick);
            filterTile.Tag = query;

            Label filter = new Label();
            filter.Content = query.getQueryString();
            filter.Foreground = Brushes.White;
            filter.Background = query.getColor();

            ((ScatterView)(lens.Content)).Items.Add(filterTile);

            filterTile.Content = filter;
            double y = (50 * lens.filters.Count) + 10;
            filterTile.Center = new Point(-50, y);
            filterTile.Orientation = 0;
            filterTile.CanMove = false;
            filterTile.CanRotate = false;
            filterTile.CanScale = false;
            lens.addFilter(query);
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
            MyScatterView.Items.Remove(note);

            //Make item that will be attached
            ScatterViewItem annotation = new ScatterViewItem();
            annotation.MouseDoubleClick += new MouseButtonEventHandler(annotation_MouseDoubleClick);

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
        private void activatePaperClipFilter(object sender, iFilter query, PaperClip paperClip)
        {
            MyScatterView.Items.Remove(sender);

            ScatterViewItem filterTile = new ScatterViewItem();
            filterTile.MinHeight = 0;
            filterTile.Background = Brushes.Transparent;
            filterTile.ShowsActivationEffects = false;
            filterTile.MouseDoubleClick += new MouseButtonEventHandler(activeFilter_MouseDoubleClick);
            filterTile.Tag = query;

            Label filter = new Label();
            filter.Content = query.getQueryString();
            filter.Foreground = Brushes.White;
            filter.Background = query.getColor();

            ((ScatterView)(paperClip.Content)).Items.Add(filterTile);

            filterTile.Content = filter;
            double y = (50 * paperClip.filters.Count) + 10;
            filterTile.Center = new Point(-50, y);
            filterTile.Orientation = 0;
            filterTile.CanMove = false;
            filterTile.CanRotate = false;
            filterTile.CanScale = false;
            paperClip.addFilter(query);
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
    }
}