using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Surface.Presentation.Controls
{
    public class StringListFilterTile : FilterTile
    {
        public StringListFilterTile(String attTag)
        {
            init(attTag, 464, Brushes.SlateGray);
        }

        public override iFilter getFilter() {
            return new HabilisX.StringListFilter(this.getUserInput(), this.attTag);
        }

        public String getUserInput()
        {
            String input = "";

            if (this.getContent().Length > this.attTag.Length + 1 && this.getContent().Substring(0, this.attTag.Length + 1).Equals(this.attTag + "="))
            {
                input = this.getContent().Substring(this.attTag.Length + 1);
            }

            return input;
        }

        protected override void FilterTile_TextChanged(object sender, TextChangedEventArgs e)
        {
            String str = (String)((TextBox)sender).Tag + "=";
            if (((TextBox)sender).Text.Length < str.Length || !(((TextBox)sender).Text.Substring(0, str.Length).Equals(str)))
            {
                ((TextBox)sender).Text = str;
                ((TextBox)sender).Select(str.Length, 0);
            }
        }

    }
}
