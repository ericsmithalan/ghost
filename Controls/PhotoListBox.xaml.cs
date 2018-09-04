using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Ghost.Controls
{
    /// <summary>
    /// Interaction logic for PhotoListBox.xaml
    /// </summary>
    public partial class PhotoListBox : UserControl
    {
        public event EventHandler PhotoSelect;

        private BackgroundWorker BackgroundLoader;
        private String _directory;
        private String _photoName;
        private bool _isExpanded;
        private PhotoCollection Photos;
        private Photo _SelectedPhoto;
        private ListBoxItem SelectedItem;
        private ScrollViewer PhotoScroller;

        private delegate void BackgroundDelegate();

        public Photo SelectedPhoto
        {
            get { return _SelectedPhoto; }
            set { _SelectedPhoto = value; }
        }

        public String Directory
        {
            get { return _directory; }
            set { _directory = value; }
        }

        public String PhotoName
        {
            get { return _photoName; }
            set { _photoName = value; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        public PhotoListBox()
        {
            InitializeComponent();
        }

        public void LoadPhotoDirectory()
        {
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            BackgroundLoader = new BackgroundWorker();
            BackgroundLoader.WorkerSupportsCancellation = true;
            BackgroundLoader.DoWork += new DoWorkEventHandler(BackgroundLoader_DoWork);
            BackgroundLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundLoader_RunWorkerCompleted);
            BackgroundLoader.RunWorkerAsync();
        }

        public void OpenFileViewer(object sender, EventArgs args)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.Multiselect = false;
                ofd.Filter = "Data Sources (*.jpg, *.gif, *.png)|*.jpg*;*.gif;*.png";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadingMessage.Visibility = Visibility.Visible;
                    Directory = System.IO.Path.GetDirectoryName(ofd.FileName);
                    PhotoName = System.IO.Path.GetFileName(ofd.FileName);
                    LoadPhotoDirectory();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void LoadDirectoryPhotos()
        {
            try
            {
                BackgroundDelegate del1 = delegate ()
                {
                    Photos.Path = Directory;
                };
                Dispatcher.BeginInvoke(DispatcherPriority.Send, del1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //EVENT HANDLERS

        #region Event Handlers

        private void ListItemMouseOver(object sender, RoutedEventArgs args)
        {
            ListBoxItem item = (ListBoxItem)sender;
            Photo photo = (Photo)item.Content;
            PhotoPreview.UpdatePreviewPhoto(photo);
        }

        private void ListItemMouseMove(object sender, MouseEventArgs args)
        {
            PhotoPreview.PositionPhotoPreview(args);
        }

        private void ListItemMouseOut(object sender, MouseEventArgs args)
        {
            PhotoPreview.Visibility = Visibility.Hidden;
        }

        private void BackgroundLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            try
            {
                SetSelectPhotoByName(PhotoName);
                ScrollSelectedPhotoIntoView();
                LoadingMessage.Visibility = Visibility.Hidden;
                BackgroundLoader.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void BackgroundLoader_DoWork(object sender, DoWorkEventArgs args)
        {
            try
            {
                if (BackgroundLoader.IsBusy)
                {
                    BackgroundLoader.CancelAsync();
                }

                System.Threading.Thread.Sleep(10);
                LoadDirectoryPhotos();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Photos = (PhotoCollection)(Resources["Photos"] as ObjectDataProvider).Data;
            PhotoScroller = (ScrollViewer)PhotosListBox.Template.FindName("PhotoScrollViewer", PhotosListBox);
        }

        public void UnselectSelected()
        {
            Photo photo;
            ListBoxItem item;
            for (int i = 0; i < PhotosListBox.Items.Count; i++)
            {
                photo = (Photo)PhotosListBox.Items[i];
                item = (ListBoxItem)(PhotosListBox.ItemContainerGenerator.ContainerFromItem(photo));
                item.IsSelected = false;
            }
        }

        private void ListItemSelected(object sender, RoutedEventArgs args)
        {
            SelectedItem = (ListBoxItem)sender;
            Photo photo = (Photo)SelectedItem.Content;
            PhotoName = System.IO.Path.GetFileName(photo.Source);
            SelectedPhoto = photo;
            OnPhotoSelect(new EventArgs());

            if (IsExpanded)
            {
                CollapsePhotoList();
            }
        }

        private void SetSelectPhotoByName(string name)
        {
            Photo photo;

            for (int i = 0; i < PhotosListBox.Items.Count; i++)
            {
                photo = (Photo)PhotosListBox.Items[i];
                SelectedItem = (ListBoxItem)(PhotosListBox.ItemContainerGenerator.ContainerFromItem(photo));
                if (System.IO.Path.GetFileName(photo.Source) == name)
                {
                    SelectedItem.IsSelected = true;
                    return;
                }
            }
        }

        private void ScrollSelectedPhotoIntoView()
        {
            if (SelectedItem != null)
            {
                SelectedItem.BringIntoView();
                PhotosListBox.ScrollIntoView(SelectedItem);
            }
        }

        private void CollapsePhotoList()
        {
            IsExpanded = false;
            Storyboard sb = (Storyboard)FindResource("CollapseStoryboard");
            sb.Begin(this);
        }

        private void ExpandPhotoList()
        {
            IsExpanded = true;
            Storyboard sb = (Storyboard)FindResource("ExpandStoryboard");
            sb.Begin(this);
        }

        #endregion Event Handlers

        protected virtual void OnPhotoSelect(EventArgs e)
        {
            PhotoSelect(this, e);
        }

        private void ToggleViewButton_ClickButton(object sender, EventArgs e)
        {
            if (IsExpanded)
            {
                CollapsePhotoList();
            }
            else
            {
                ExpandPhotoList();
            }
        }

        private void ExpandStoryboard_Completed(object sender, EventArgs e)
        {
            PhotoScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            ScrollSelectedPhotoIntoView();
        }

        private void CollapseStoryboard_Completed(object sender, EventArgs e)
        {
            PhotoScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            ScrollSelectedPhotoIntoView();
        }
    }
}