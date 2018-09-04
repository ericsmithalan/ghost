using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GhostV2.Controls
{
    /// <summary>
    /// Interaction logic for ButtonImage.xaml
    /// </summary>
    public partial class ButtonSelect : CheckBox
    {
        private String ImageDirectory = "../images/";
        private String _unSelectedInactiveImageFileName;
        private String _unSelctedActiveImageFileName;
        private String _unSelctedHoverImageFileName;

        private String _selectedInactiveImageFileName;
        private String _selctedActiveImageFileName;
        private String _selctedHoverImageFileName;

        public event EventHandler ClickButton;

        public String SelectedInactiveImageFileName
        {
            get { return _selectedInactiveImageFileName; }
            set { _selectedInactiveImageFileName = String.Concat(this.ImageDirectory, value); }
        }

        public String SelectedActiveImageFileName
        {
            get { return _selctedActiveImageFileName; }
            set { _selctedActiveImageFileName = String.Concat(this.ImageDirectory, value); ; }
        }

        public String SelectedHoverImageFileName
        {
            get { return _unSelctedHoverImageFileName; }
            set { _selctedHoverImageFileName = String.Concat(this.ImageDirectory, value); ; }
        }

        public String UnSelectedInactiveImageFileName
        {
            get { return _unSelectedInactiveImageFileName; }
            set { _unSelectedInactiveImageFileName = String.Concat(this.ImageDirectory, value); }
        }

        public String UnSelectedActiveImageFileName
        {
            get { return _unSelctedActiveImageFileName; }
            set { _unSelctedActiveImageFileName = String.Concat(this.ImageDirectory, value); ; }
        }

        public String UnSelectedHoverImageFileName
        {
            get { return _unSelctedHoverImageFileName; }
            set { _unSelctedHoverImageFileName = String.Concat(this.ImageDirectory, value); ; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public ButtonSelect()
        {
            this.IsChecked = false;
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if ((bool)this.IsChecked)
                this.IsChecked = false;
            else
                this.IsChecked = true;

            MessageBox.Show(this.IsChecked.ToString());
            OnClickButton(new EventArgs());
            base.OnMouseDown(e);
        }

        protected virtual void OnClickButton(EventArgs e)
        {
            this.ClickButton(this, e);
        }
    }
}