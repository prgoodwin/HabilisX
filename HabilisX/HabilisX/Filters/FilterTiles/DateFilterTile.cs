using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Surface.Presentation.Controls
{
    public class DateFilterTile : FilterTile
    {
        public DateFilterTile(String attTag) {
            init(attTag, 638, Brushes.LightGray);
        }

        public override iFilter getFilter()
        {
            return new HabilisX.StringFilter(this.getUserInput(), this.attTag);
        }
 
    }
}
