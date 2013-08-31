using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

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
    }
}
