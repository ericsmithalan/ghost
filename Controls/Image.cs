using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GhostV2.Controls
{
    internal class GhostImage : System.Windows.Controls.Image
    {
        private Point _pixelOffset;

        public GhostImage()
        {
            this.LayoutUpdated += new EventHandler(GhostImage_LayoutUpdated);
            this.SourceUpdated += new EventHandler<System.Windows.Data.DataTransferEventArgs>(GhostImage_SourceUpdated);
            this.SnapsToDevicePixels = true;
        }

        private void GhostImage_SourceUpdated(object sender, EventArgs args)
        {
            InvalidateMeasure();
            InvalidateVisual();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            BitmapSource bitmapSource = (BitmapSource)this.Source;
            if (bitmapSource != null)
            {
                _pixelOffset = GetPixelOffset();

                // Render the bitmap offset by the needed amount to align to pixels.
                dc.DrawImage(bitmapSource, new Rect(_pixelOffset, DesiredSize));
            }

            base.OnRender(dc);
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            Size measureSize = new Size();

            BitmapSource bitmapSource = (BitmapSource)this.Source;
            if (bitmapSource != null)
            {
                PresentationSource ps = PresentationSource.FromVisual(this);
                if (ps != null)
                {
                    Matrix fromDevice = ps.CompositionTarget.TransformFromDevice;

                    Vector pixelSize = new Vector(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
                    Vector measureSizeV = fromDevice.Transform(pixelSize);
                    measureSize = new Size(measureSizeV.X, measureSizeV.Y);
                }
            }
            return base.MeasureOverride(constraint);
        }

        private Point GetPixelOffset()
        {
            Point pixelOffset = new Point();

            PresentationSource ps = PresentationSource.FromVisual(this);
            if (ps != null)
            {
                Visual rootVisual = ps.RootVisual;

                // Transform (0,0) from this element up to pixels.
                pixelOffset = this.TransformToAncestor(rootVisual).Transform(pixelOffset);
                pixelOffset = ApplyVisualTransform(pixelOffset, rootVisual, false);
                pixelOffset = ps.CompositionTarget.TransformToDevice.Transform(pixelOffset);

                // Round the origin to the nearest whole pixel.
                pixelOffset.X = Math.Round(pixelOffset.X);
                pixelOffset.Y = Math.Round(pixelOffset.Y);

                // Transform the whole-pixel back to this element.
                pixelOffset = ps.CompositionTarget.TransformFromDevice.Transform(pixelOffset);
                pixelOffset = ApplyVisualTransform(pixelOffset, rootVisual, true);
                pixelOffset = rootVisual.TransformToDescendant(this).Transform(pixelOffset);
            }

            return pixelOffset;
        }

        private void GhostImage_LayoutUpdated(object sender, EventArgs e)
        {
            Point pixelOffset = GetPixelOffset();
            if (!AreClose(pixelOffset, _pixelOffset))
            {
                InvalidateVisual();
            }
        }

        private bool AreClose(Point point1, Point point2)
        {
            return AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y);
        }

        private bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double delta = value1 - value2;
            return ((delta < 1.53E-06) && (delta > -1.53E-06));
        }

        private Matrix GetVisualTransform(Visual v)
        {
            if (v != null)
            {
                Matrix m = Matrix.Identity;

                Transform transform = VisualTreeHelper.GetTransform(v);
                if (transform != null)
                {
                    Matrix cm = transform.Value;
                    m = Matrix.Multiply(m, cm);
                }

                Vector offset = VisualTreeHelper.GetOffset(v);
                m.Translate(offset.X, offset.Y);

                return m;
            }

            return Matrix.Identity;
        }

        private Point TryApplyVisualTransform(Point point, Visual v, bool inverse, bool throwOnError, out bool success)
        {
            success = true;
            if (v != null)
            {
                Matrix visualTransform = GetVisualTransform(v);
                if (inverse)
                {
                    if (!throwOnError && !visualTransform.HasInverse)
                    {
                        success = false;
                        return new Point(0, 0);
                    }
                    visualTransform.Invert();
                }
                point = visualTransform.Transform(point);
            }
            return point;
        }

        private Point ApplyVisualTransform(Point point, Visual v, bool inverse)
        {
            bool success = true;
            return TryApplyVisualTransform(point, v, inverse, true, out success);
        }
    }
}