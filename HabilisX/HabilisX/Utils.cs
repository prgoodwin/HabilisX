using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;

namespace HabilisX
{
    static class Utils
    {
        private static readonly Random num = new Random();
        public static double nextNum(int lower, int higher)
        {
            return num.Next(lower, higher);
        }

        public static void RemoveShadow(ScatterViewItem tool)
        {
            RoutedEventHandler loadedEventHandler = null;
            loadedEventHandler = new RoutedEventHandler(delegate
            {
                tool.Loaded -= loadedEventHandler;
                Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
                ssc = tool.Template.FindName("shadow", tool) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
                ssc.Visibility = Visibility.Hidden;
            });
            tool.Loaded += loadedEventHandler;
        }
        public static BitmapImage NewEmbededResource(String path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream(path);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }

        public static String[] NewEmbededTextFile(String path) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(path));
            char[] pars = {'\n'};

            return reader.ReadToEnd().Split(pars);


        }
    }
}
