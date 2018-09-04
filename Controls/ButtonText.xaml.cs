using System;
using System.Windows;
using System.Windows.Controls;

namespace Ghost.Controls
{
    /// <summary>
    /// Interaction logic for ButtonImage.xaml
    /// </summary>
    public partial class ButtonText : UserControl
    {
        private String ImageDirectory = "../images/";
        private bool IsSelected = false;
        private String _text;

        private String _inactiveImageFileName;
        private String _activeImageFileName;
        private String _hoverImageFileName;

        public event EventHandler ClickButton;

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

        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public ButtonText()
        {
            InitializeComponent();
        }

        protected virtual void OnClickButton(EventArgs e)
        {
            ClickButton(this, e);
        }

        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            OnClickButton(new EventArgs());
        }

        private void MainSelectButtonControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}