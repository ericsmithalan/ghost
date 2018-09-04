using System;
using System.Windows;
using System.Windows.Controls;

namespace Ghost.Controls
{
    /// <summary>
    /// Shows information at the top of the window
    /// </summary>
    public partial class ToolBarTop : UserControl
    {
        public bool IsMaximizedWorkSpace = false;

        public event EventHandler MaximizeWorkSpaceClicked;

        public event EventHandler MinimizedWindowClicked;

        public Grid MainUIGrid;
        protected String _selectedFilePath;

        public String SelectedFilePath
        {
            get { return _selectedFilePath; }
            set { _selectedFilePath = value; }
        }

        protected virtual void OnMinimizedWindowClicked(EventArgs e)
        {
            MinimizedWindowClicked(this, e);
        }

        protected virtual void OnMaximizeWorkSpaceClicked(EventArgs e)
        {
            MaximizeWorkSpaceClicked(this, e);
        }

        public ToolBarTop()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            RenderFullTitle(SelectedFilePath);
            base.OnApplyTemplate();
        }

        public void RenderFullTitle(String path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                label_filePath.Content = String.Format("Ghost: {0}", path);
            }
            else
            {
                label_filePath.Content = "Ghost";
            }
        }

        private void image_minize_ClickButton(object sender, EventArgs e)
        {
            OnMinimizedWindowClicked(new EventArgs());
        }

        private void image_close_ClickButton(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void image_maximize_screen_Click(object sender, RoutedEventArgs e)
        {
            if (IsMaximizedWorkSpace)
            {
                IsMaximizedWorkSpace = false;
                label_filePath.Margin = new Thickness(25, 0, 0, 0);
            }
            else
            {
                IsMaximizedWorkSpace = true;
                label_filePath.Margin = new Thickness(0, 0, 0, 0);
            }
            OnMaximizeWorkSpaceClicked(new EventArgs());
        }
    }
}