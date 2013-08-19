using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;



namespace Microsoft.Surface.Presentation.Controls
{
   public interface iFilter
   {
      bool Matches(Entry entry);
      String getQueryString();
      Brush getColor();
   }
}
