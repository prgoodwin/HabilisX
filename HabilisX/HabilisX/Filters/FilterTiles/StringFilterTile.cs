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

    }
}
