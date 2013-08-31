using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
namespace Microsoft.Surface.Presentation.Controls
{
    public class StringFilterTile : FilterTile
    {
        public StringFilterTile(String attTag) {
            init(attTag, 290, Brushes.DarkSlateGray);
        }
        public override iFilter getFilter()
        {
            return new HabilisX.StringFilter(this.getUserInput(), this.attTag);
        }

        public String getUserInput()
        {
            String input = "";

            if (this.hasInput() && this.getContent().Substring(0, this.attTag.Length + 1).Equals(this.attTag + "="))
            {
                input = this.getContent().Substring(this.attTag.Length + 1);
            }

            return input;
        }


    }
}
