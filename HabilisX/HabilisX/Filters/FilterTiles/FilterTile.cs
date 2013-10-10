using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
namespace Microsoft.Surface.Presentation.Controls
{
    public abstract class FilterTile : ScatterViewItem
    {
        public String attTag;
        public Boolean onTextBox = false;

        protected void init(String attTag, Brush color) {
            this.Tag = attTag;
            this.attTag = attTag;
            this.Orientation = 0;
            this.MinHeight = 30;
            this.Content = NewEntryTileTextBox(attTag, this);
            this.Center = new Point(280, 200);
            this.Background = color;        
        }

        public String getContent(){
            return ((SurfaceTextBox)this.Content).Text;
        
        }

        public Boolean hasInput(){
            return this.getContent().Length > attTag.Length+1;
        }


        private SurfaceTextBox NewEntryTileTextBox(String attTag, ScatterViewItem item)
        {
            
            SurfaceTextBox txt = new SurfaceTextBox();
            txt.AcceptsReturn = false;
            txt.Background = item.Background;
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Height = 30;
            txt.Margin = new Thickness(8);

            Thickness bottomMargin = txt.Margin;
            bottomMargin.Bottom = 50;

            txt.Margin = bottomMargin;
            txt.Tag = attTag;
            txt.Text = attTag + "=";
            txt.Width = 150;
            
            txt.TextChanged += new TextChangedEventHandler(FilterTile_TextChanged);
            txt.MouseDoubleClick += new MouseButtonEventHandler(txt_MouseDoubleClick);

            return txt;
        }

        void txt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            onTextBox = true;
            e.Handled = true;
        }


        abstract protected void FilterTile_TextChanged(object sender, TextChangedEventArgs e);
        //{
        //    String str = (String)((TextBox)sender).Tag + "=";
        //    if (((TextBox)sender).Text.Length < str.Length || !(((TextBox)sender).Text.Substring(0, str.Length).Equals(str)))
        //    {
        //        ((TextBox)sender).Text = str;
        //        ((TextBox)sender).Select(str.Length, 0);
        //    }
        //}

        abstract public iFilter getFilter();

        

    }
}
