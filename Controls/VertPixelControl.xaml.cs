using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ghost.Controls
{
    /// <summary>
    /// Interaction logic for VertPixelControl.xaml
    /// </summary>
    public partial class VertPixelControl : UserControl
    {
        public VertPixelControl()
        {
            InitializeComponent();

            Canvas.SetTop(this, 20);
            Canvas.SetLeft(this, 80);
        }

        private double origHorizOffset;
        private double origVertOffset;
        private bool modifyLeftOffset;
        private bool modifyTopOffset;
        private Point origCursorLocation;
        private double PixelSize;
        private bool IsResize = false;

        private void InitilizeMouseDown(MouseButtonEventArgs args)
        {
            origCursorLocation = args.GetPosition(null);

            UIElement control;

            if (IsResize)
            {
                control = image_size;
                Cursor = Cursors.SizeNS;
            }
            else
            {
                control = this;
                Cursor = Cursors.SizeAll;
            }

            double left = Canvas.GetLeft(control);
            double right = Canvas.GetRight(control);
            double top = Canvas.GetTop(control);
            double bottom = Canvas.GetBottom(control);

            origHorizOffset = ResolveOffset(left, right, out modifyLeftOffset);
            origVertOffset = ResolveOffset(top, bottom, out modifyTopOffset);
            CaptureMouse();

            args.Handled = true;
        }

        private void PreviewMouseLeftButtonDown_image_move(object sender, MouseButtonEventArgs e)
        {
            InitilizeMouseDown(e);
            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void PreviewMouseLeftButtonDown_image_size(object sender, MouseButtonEventArgs e)
        {
            IsResize = true;
            InitilizeMouseDown(e);
            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void PreviewMouseUp_UserControl(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                if (IsResize)
                {
                    IsResize = false;
                }

                Cursor = Cursors.Arrow;
                ReleaseMouseCapture();
            }
            base.OnPreviewMouseUp(e);
        }

        private void PreviewMouseMove_UserControl(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                // Get the new mouse position.
                Point cursorLocation = e.GetPosition(null);
                double newHorizontalOffset, newVerticalOffset;

                #region Calculate Offsets

                // Determine the horizontal offset.
                if (modifyLeftOffset)
                {
                    newHorizontalOffset =
                      origHorizOffset + (cursorLocation.X - origCursorLocation.X);
                }
                else
                {
                    newHorizontalOffset =
                      origHorizOffset - (cursorLocation.X - origCursorLocation.X);
                }

                // Determine the vertical offset.
                if (modifyTopOffset)
                {
                    newVerticalOffset =
                      origVertOffset + (cursorLocation.Y - origCursorLocation.Y);
                }
                else
                {
                    newVerticalOffset =
                      origVertOffset - (cursorLocation.Y - origCursorLocation.Y);
                }

                #endregion Calculate Offsets

                if (IsResize)
                {
                    // DON'T LET HEIGHT GO NEGATIVE
                    if (newVerticalOffset >= 0)
                    {
                        if (modifyTopOffset)
                        {
                            Canvas.SetTop(image_size, newVerticalOffset);
                        }
                        else
                        {
                            Canvas.SetBottom(image_size, newVerticalOffset);
                        }

                        // POSITION TEXT BLOCK
                        Canvas.SetTop(border_textblock_main, ((grid_main.ActualHeight / 2) - (border_textblock_main.ActualHeight / 2)));

                        if (newVerticalOffset > border_textblock_main.ActualHeight)
                        {
                            Canvas.SetLeft(border_textblock_main, -(border_textblock_main.ActualWidth / 2 - 5));
                        }
                        else
                        {
                            Canvas.SetLeft(border_textblock_main, -border_textblock_main.ActualWidth);
                        }

                        image_bar.Height = newVerticalOffset;

                        if (newVerticalOffset == 0)
                        {
                            textblock_main.Text = "0 px";
                        }
                        else
                        {
                            textblock_main.Text = String.Concat((newVerticalOffset).ToString(), " px");
                        }
                    }

                    // PIXEL SIZE USED FOR RIGHT CLICK
                    PixelSize = newVerticalOffset;
                }
                else
                {
                    if (modifyLeftOffset)
                    {
                        Canvas.SetLeft(this, newHorizontalOffset);
                    }
                    else
                    {
                        Canvas.SetRight(this, newHorizontalOffset);
                    }

                    if (modifyTopOffset)
                    {
                        Canvas.SetTop(this, newVerticalOffset);
                    }
                    else
                    {
                        Canvas.SetBottom(this, newVerticalOffset);
                    }
                }
            }
            base.OnPreviewMouseMove(e);
        }

        private static double ResolveOffset(double side1, double side2, out bool useSide1)
        {
            useSide1 = true;
            double result;
            if (Double.IsNaN(side1))
            {
                if (Double.IsNaN(side2))
                {
                    result = 0;
                }
                else
                {
                    result = side2;
                    useSide1 = false;
                }
            }
            else
            {
                result = side1;
            }
            return result;
        }

        private void ButtonImage_ClickButton(object sender, EventArgs e)
        {
            Canvas mainCanvas = (Canvas)Parent;
            mainCanvas.Children.Remove(this);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(PixelSize.ToString());
        }
    }
}