using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Surface.Presentation.Controls
{
    public class IntListFilterTile : FilterTile
    {
        public IntListFilterTile(String attTag)
        {
            init(attTag, 986, Brushes.SlateGray);
        }
        public override iFilter getFilter()
        {
            return new HabilisX.StringFilter(this.getUserInput(), this.attTag);
        }

    }
}
