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
        private SurfaceTextBox txt = new SurfaceTextBox();
        protected void init(String attTag, int X, Brush color) {
            this.Tag = attTag;
            this.attTag = attTag;
            this.Orientation = 0;
            this.MinHeight = 30;
            this.Content = NewEntryTileTextBox(attTag, this);
            this.Center = new Point(X, 130);
            this.Background = color;        
        }

        public String getContent(){
            return txt.Text;
        
        }

        public Boolean hasInput(){
            return txt.Text.Length > attTag.Length+1;
        }

        public String getUserInput() {
            String input = "";

            if (this.getContent().Length > this.attTag.Length + 1 && this.getContent().Substring(0, this.attTag.Length + 1).Equals(this.attTag + "="))
            {
                input = this.getContent().Substring(this.attTag.Length + 1);
            }

            return input;
        }
        private SurfaceTextBox NewEntryTileTextBox(String attTag, ScatterViewItem item)
        {
            txt.Background = item.Background;
            txt.Margin = new Thickness(8);
            Thickness bottomMargin = txt.Margin;
            bottomMargin.Bottom = 50;
            txt.Margin = bottomMargin;
            txt.Height = 30;
            txt.Width = 150;
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = attTag + "=";
            txt.AcceptsReturn = false;
            txt.TextChanged += new TextChangedEventHandler(FilterTile_TextChanged);
            txt.Tag = attTag;

            return txt;
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

        abstract public iFilter getFilter();

        

    }
}
