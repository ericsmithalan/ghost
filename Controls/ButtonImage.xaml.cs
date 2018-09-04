using System;
using System.Windows;
using System.Windows.Controls;

namespace Ghost.Controls
{
    /// <summary>
    /// Interaction logic for ButtonImage.xaml
    /// </summary>
    public partial class ButtonImage : UserControl
    {
        private String _inactiveImageFileName;
        private String _activeImageFileName;
        private String _hoverImageFileName;
        private String ImageDirectory = "../images/";

        public event EventHandler ClickButton;

        protected virtual void OnClickButton(EventArgs e)
        {
            ClickButton(this, e);
        }

        public String InactiveImageFileName
        {
            get { return _inactiveImageFileName; }
            set { _inactiveImageFileName = String.Concat(ImageDirectory, value); }
        }

        public String ActiveImageFileName
        {
            get { return _activeImageFileName; }
            set { _activeImageFileName = String.Concat(ImageDirectory, value); ; }
        }

        public String HoverImageFileName
        {
            get { return _hoverImageFileName; }
            set { _hoverImageFileName = String.Concat(ImageDirectory, value); ; }
        }

        public ButtonImage()
        {
            InitializeComponent();
        }

        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            OnClickButton(new EventArgs());
        }
    }
}