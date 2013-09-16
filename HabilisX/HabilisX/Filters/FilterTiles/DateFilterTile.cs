﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Surface.Presentation.Controls
{
    public class DateFilterTile : FilterTile
    {
        public DateFilterTile(String attTag)
        {
            init(attTag, 638, Brushes.LightGray);
        }

        public override iFilter getFilter()
        {
            return new HabilisX.DateFilter(this.getUserInput(),
                               this.getContent()[attTag.Length], attTag);
        }

        public DateTime getUserInput()
        {
            int year = 0;
            String userInput = "";
            if (this.getContent().Length > attTag.Length + 1 && this.getContent().Substring(0, attTag.Length).Equals(attTag) && (this.getContent()[attTag.Length].Equals('>') ||
       this.getContent()[attTag.Length].Equals('<') || this.getContent()[attTag.Length].Equals('=')))
            {
                userInput = this.getContent().Substring(attTag.Length + 1);
                try
                {
                    year = Convert.ToInt32(userInput);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Not a number in DateTile: " + ex);
                };
            }

            return new DateTime(year, 1, 1);

        }

    }
}