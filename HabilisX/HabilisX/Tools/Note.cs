﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;
using System.Windows;

namespace HabilisX.Tools
{
    public class Note : Tool
    {

        public SurfaceTextBox titleText;
        public SurfaceTextBox txt;

        public Note() {
            Canvas innerView = new Canvas();
            innerView.Width = 260;
            innerView.Height = 170;
            innerView.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));

            //Titie text box
            titleText = new SurfaceTextBox();
            titleText.AcceptsReturn = false;
            titleText.FontSize = 14;
            titleText.FontWeight = FontWeights.Bold;
            titleText.Foreground = Brushes.Black;
            titleText.Height = 30;
            titleText.Margin = new Thickness(8);
            titleText.Text = "Note Title";
            titleText.Width = 243;
            titleText.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            titleText.BorderBrush = Brushes.Gray;
            titleText.GotFocus += new RoutedEventHandler(titleText_GotFocus);

            //Content text box
            txt = new SurfaceTextBox();
            txt.Foreground = Brushes.Black;
            txt.Margin = new Thickness(8);
            txt.FontSize = 14;
            txt.FontWeight = FontWeights.Bold;
            txt.Text = "Note Text";
            txt.Width = 243;
            txt.Height = 100;
            txt.AcceptsReturn = true;
            txt.AcceptsTab = true;
            txt.TextWrapping = TextWrapping.Wrap;
            txt.Background = new SolidColorBrush(Color.FromRgb(252, 240, 173));
            txt.BorderBrush = Brushes.Gray;
            txt.GotFocus += new RoutedEventHandler(txt_GotFocus);

            //Attach
            innerView.Children.Add(titleText);
            Canvas.SetTop(titleText, 0);
            innerView.Children.Add(txt);
            Canvas.SetTop(txt, 52);

            //Final item
            
            this.Content = innerView;
            this.Orientation = 0;
            this.MinHeight = 0;
            this.Height = 170;
            this.MaxHeight = 1000;
            this.MaxWidth = 1000;
            this.Center = new Point(152, 620);
        }

        void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt.Text == "Note Text") {
                txt.Text = "";
            }
        }

        void titleText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (titleText.Text == "Note Title")
            {
                titleText.Text = "";
            }
        }


        public override bool AreBoundaryIntersecting(FrameworkElement cursorVisual)
        {
            RectangleGeometry cursorBounds =
                new RectangleGeometry(new Rect(0, 0, cursorVisual.ActualWidth, cursorVisual.ActualHeight));
            RectangleGeometry targetBounds =
                new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            cursorBounds.Transform = (Transform)cursorVisual.TransformToVisual(this);
            return cursorBounds.FillContainsWithDetail(targetBounds) != IntersectionDetail.Empty;
        }
    }
}
