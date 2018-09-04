using System;
using System.Windows.Controls;

namespace Ghost.Controls
{
    /// <summary>
    /// Displays opacity control
    /// </summary>
    public partial class OpacityEditor : UserControl
    {
        /// <summary>
        /// Bubbles up click to MainWindow
        /// </summary>
        public event EventHandler OpacityDownClicked;

        /// <summary>
        /// Bubbles up click to MainWindow
        /// </summary>
        public event EventHandler OpacityUpClicked;

        /// <summary>
        /// Current opacity value
        /// </summary>
        public double OpacityValue = 100;

        /// <summary>
        /// Initialize Control
        /// </summary>
        public OpacityEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fires when up image is clicked
        /// </summary>
        protected virtual void OnOpacityUpClicked(EventArgs e)
        {
            OpacityUpClicked(this, e);
        }

        /// <summary>
        /// Fires when down image is clicked
        /// </summary>
        protected virtual void OnOpacityDownClicked(EventArgs e)
        {
            OpacityDownClicked(this, e);
        }

        /// <summary>
        /// Updates opacity value in label
        /// </summary>
        public void UpdateOpacityLabel(double opacity)
        {
            text_opacity.Text = String.Format("{0}%", opacity.ToString());
        }

        /// <summary>
        /// fires when up image is clicked.
        /// Sets opacity value in label and fires OnOpacityUpClicked
        /// </summary>
        private void Image_Up_ClickButton(object sender, EventArgs e)
        {
            if (OpacityValue < 100)
            {
                OpacityValue += 10;
                UpdateOpacityLabel(OpacityValue);
                OnOpacityUpClicked(new System.EventArgs());
            }
        }

        /// <summary>
        /// fires when down image is clicked.
        /// Sets opacity value in label and fires OnOpacityDownClicked
        /// </summary>
        private void Image_down_ClickButton(object sender, EventArgs e)
        {
            if (OpacityValue > 0)
            {
                OpacityValue -= 10;
                UpdateOpacityLabel(OpacityValue);
                OnOpacityDownClicked(new System.EventArgs());
            }
        }
    }
}