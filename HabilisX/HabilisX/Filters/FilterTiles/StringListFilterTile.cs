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
            return new HabilisX.StringFilter(this.getUserInput(), this.attTag);
        }
    }
}
