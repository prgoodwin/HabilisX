using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Surface.Presentation.Controls
{
    public class IntFilterTile : FilterTile
    {
        public IntFilterTile(String attTag) {
            init(attTag, 812, new SolidColorBrush(Color.FromRgb(191, 191, 191)));
        }

        public override iFilter getFilter()
        {
            return new HabilisX.StringFilter(this.getUserInput(), this.attTag);
        }

    }

}
