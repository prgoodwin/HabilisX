﻿using System;
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
            return new HabilisX.IntListFilter(this.getUserInput(),
                               this.getContent()[attTag.Length], attTag);
        }

        public int getUserInput()
        {
            String userInput = "";
            int queryNumber = -1;

            if (this.getContent().Length > attTag.Length + 1 && this.getContent().Substring(0, attTag.Length).Equals(attTag) && (this.getContent()[attTag.Length].Equals('>') ||
                   this.getContent()[attTag.Length].Equals('<') || this.getContent()[attTag.Length].Equals('=')))
            {
                userInput = this.getContent().Substring(attTag.Length + 1);
                try
                {
                    queryNumber = Convert.ToInt32(userInput);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Not a number in IntTile: " + ex);
                }
            }

            return queryNumber;

        }
    }
}
