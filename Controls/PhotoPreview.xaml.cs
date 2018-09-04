using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ghost.Controls
{
    /// <summary>
    /// Interaction logic for PhotoPreview.xaml
    /// </summary>
    public partial class PhotoPreview : UserControl
    {
        private Photo _photo;
        private BackgroundWorker ImagePreviewBackgroundWorker = new BackgroundWorker();

        private delegate void BackgroundDelegate();

        public Photo PreviewPhoto
        {
            get { return _photo; }
            set { _photo = value; }
        }

        private void InitializeBackgroundWorker()
        {
            ImagePreviewBackgroundWorker = new BackgroundWorker();
            ImagePreviewBackgroundWorker.WorkerSupportsCancellation = true;
            ImagePreviewBackgroundWorker.DoWork += new DoWorkEventHandler(ImagePreviewBackgroundWorker_DoWork);
        }

        public void UpdatePreviewPhoto(Photo photo)
        {
            Visibility = Visibility.Visible;
            InitializeBackgroundWorker();
            PreviewPhoto = photo;
            if (ImagePreviewBackgroundWorker.IsBusy)
            {
                ImagePreviewBackgroundWorker.CancelAsync();
            }

            ImagePreviewBackgroundWorker.RunWorkerAsync();
        }

        public PhotoPreview()
        {
            InitializeComponent();
        }

        public void PositionPhotoPreview(MouseEventArgs e)
        {
            Canvas.SetTop(this, e.GetPosition(null).Y);
            Canvas.SetLeft(this, e.GetPosition(null).X - 70);
        }

        private void ImagePreviewBackgroundWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            try
            {
                BackgroundDelegate del1 = delegate ()
                {
                    BitmapImage bm = new BitmapImage();
                    bm.BeginInit();
                    bm.DecodePixelWidth = 270;
                    bm.CacheOption = BitmapCacheOption.None;
                    bm.UriSource = new Uri(PreviewPhoto.Source);
                    bm.EndInit();
                    bm.Freeze();

                    image_main.Source = bm;
                    text_fileName.Text = System.IO.Path.GetFileName(PreviewPhoto.Source);
                };
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, del1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}