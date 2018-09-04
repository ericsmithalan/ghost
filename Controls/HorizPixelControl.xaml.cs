using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ghost.Controls
{
    /// <summary>
    /// Interaction logic for HorizPixelControl.xaml
    /// </summary>
    public partial class HorizPixelControl : UserControl
    {
        private double origHorizOffset;
        private double origVertOffset;
        private bool modifyLeftOffset;
        private bool modifyTopOffset;
        private Point origCursorLocation;
        private double PixelSize;
        private bool IsResize = false;

        public HorizPixelControl()
        {
            Canvas.SetTop(this, 40);
            Canvas.SetLeft(this, 80);
            InitializeComponent();
        }

        private void InitilizeMouseDown(MouseButtonEventArgs args)
        {
            origCursorLocation = args.GetPosition(null);
            UIElement control;

            if (IsResize)
            {
                control = image_size;
                Cursor = Cursors.SizeWE;
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
                    if (newHorizontalOffset >= 0)
                    {
                        if (modifyLeftOffset)
                        {
                            Canvas.SetLeft(image_size, newHorizontalOffset);
                        }
                        else
                        {
                            Canvas.SetRight(image_size, newHorizontalOffset);
                        }

                        Canvas.SetLeft(border_textblock, ((grid_main.ActualWidth / 2) - (border_textblock.ActualWidth / 2)));

                        if (newHorizontalOffset > border_textblock.ActualWidth)
                        {
                            Canvas.SetTop(border_textblock, -(border_textblock.ActualHeight / 2 - 5));
                        }
                        else
                        {
                            Canvas.SetTop(border_textblock, -(border_textblock.ActualHeight));
                        }
                        image_middle.Width = newHorizontalOffset;
                        textblock_measure.Text = String.Concat(newHorizontalOffset.ToString(), " px");
                    }
                    PixelSize = newHorizontalOffset;
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