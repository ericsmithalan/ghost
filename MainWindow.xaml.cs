using Ghost.Controls;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ghost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand AK_TopMost = new RoutedCommand();
        public static RoutedCommand AK_OpenClipBoardImage = new RoutedCommand();
        public static RoutedCommand AK_OpenNewFile = new RoutedCommand();

        public static RoutedCommand AK_UpKey = new RoutedCommand();
        public static RoutedCommand AK_DownKey = new RoutedCommand();
        public static RoutedCommand AK_LeftKey = new RoutedCommand();
        public static RoutedCommand AK_RightKey = new RoutedCommand();

        public static RoutedCommand AK_Opacity_1 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_2 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_3 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_4 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_5 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_6 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_7 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_8 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_9 = new RoutedCommand();
        public static RoutedCommand AK_Opacity_0 = new RoutedCommand();
        public static RoutedCommand AK_Quit = new RoutedCommand();
        public static RoutedCommand AK_Minimize = new RoutedCommand();
        public static RoutedCommand AK_Color = new RoutedCommand();
        public static RoutedCommand AK_HRuler = new RoutedCommand();
        public static RoutedCommand AK_VRuler = new RoutedCommand();

        private IntPtr viewerHandle = IntPtr.Zero;
        private IntPtr installedHandle = IntPtr.Zero;

        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;

        [DllImport("user32.dll")]
        private extern static IntPtr SetClipboardViewer(IntPtr hWnd);

        [DllImport("user32.dll")]
        private extern static int ChangeClipboardChain(IntPtr hWnd, IntPtr hWndNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private System.Windows.Media.Color CopyToClipBoardColor;

        public MainWindow()
        {
            InitializeComponent();
            ColorPicker.BitmapSourceControl = image_main.Source;
            InitializeKeyboardEvents();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                installedHandle = hwndSource.Handle;
                viewerHandle = SetClipboardViewer(installedHandle);
                hwndSource.AddHook(new HwndSourceHook(hwndSourceHook));
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            scroll_image.Width = Math.Floor(main_window.ActualWidth - 4);
            scroll_image.Height = Math.Floor(main_window.ActualHeight - 78);
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void main_window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource Source;
            Source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            Source.AddHook(new HwndSourceHook(Window_Proc));
        }

        private IntPtr hwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CHANGECBCHAIN:
                    viewerHandle = lParam;
                    try
                    {
                        if (Clipboard.ContainsData(DataFormats.Bitmap))
                        {
                            image_main.Source = (System.Windows.Interop.InteropBitmap)Clipboard.GetData(DataFormats.Bitmap);
                        }
                    }
                    catch { MessageBox.Show("WM_CHANGECBCHAIN"); }
                    if (viewerHandle != IntPtr.Zero)
                    {
                        SendMessage(viewerHandle, msg, wParam, lParam);
                    }

                    break;

                case WM_DRAWCLIPBOARD:
                    try
                    {
                        if (Clipboard.ContainsData(DataFormats.Bitmap))
                        {
                            image_main.Source = (System.Windows.Interop.InteropBitmap)Clipboard.GetData(DataFormats.Bitmap);
                        }
                    }
                    catch { MessageBox.Show("WM_DRAWCLIPBOARD"); }

                    if (viewerHandle != IntPtr.Zero)
                    {
                        SendMessage(viewerHandle, msg, wParam, lParam);
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        private IntPtr Window_Proc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam, ref bool Handled)
        {
            if (Msg == NativeMethods.CC)
            {
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                }
                Activate();
                Focus();
            }
            return IntPtr.Zero;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ChangeClipboardChain(installedHandle, viewerHandle);
            int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            e.Cancel = error != 0;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            viewerHandle = IntPtr.Zero;
            installedHandle = IntPtr.Zero;
            base.OnClosed(e);
        }

        private void UpdatePhoto()
        {
            try
            {
                image_main.Source = PhotoListBoxControl.SelectedPhoto.Image;
                toolbar_top.RenderFullTitle(String.Concat(PhotoListBoxControl.Directory, @"\", PhotoListBoxControl.PhotoName));
                ColorPicker.BitmapSourceControl = image_main.Source;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void GetImageFromClipBoard()
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    BitmapSource bpSource = Clipboard.GetImage();
                    FormatConvertedBitmap fb = new FormatConvertedBitmap(bpSource, PixelFormats.Bgr32, null, 0);
                    image_main.Source = fb.Source;
                }
                else
                {
                    MessageBox.Show("Click 'Print Scrn' to add image to your clipboard.\n\nClick OK to try again.'");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        internal BitmapSource CreateBitmap(IntPtr handle)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void Clicked_Down_OpacityEditor(object sender, EventArgs e)
        {
            scroll_image.Opacity = Math.Round(opacity_editor.OpacityValue / 100, 1);
            canvas_image.Opacity = Math.Round(opacity_editor.OpacityValue / 100, 1);
        }

        private void Clicked_Up_OpacityEditor(object sender, EventArgs e)
        {
            scroll_image.Opacity = Math.Round(opacity_editor.OpacityValue / 100, 1);
            canvas_image.Opacity = Math.Round(opacity_editor.OpacityValue / 100, 1);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void toolbar_files_Click_PhotoSelected(object sender, EventArgs e)
        {
            UpdatePhoto();
            Keyboard.Focus(this);
        }

        private void buttonSelect_horizontalMeasure_ClickButton(object sender, EventArgs e)
        {
            Ghost.Controls.HorizPixelControl ruler = new Ghost.Controls.HorizPixelControl();
            canvas_image.Children.Add(ruler);
        }

        private void buttonSelect_verticalMeasure_ClickButton(object sender, EventArgs e)
        {
            Ghost.Controls.VertPixelControl ruler = new Ghost.Controls.VertPixelControl();
            canvas_image.Children.Add(ruler);
        }

        private void image_main_MouseMove(object sender, MouseEventArgs e)
        {
            ColorPicker.SetColorInfo(e);
        }

        private void toolbar_top_MaximizeWorkSpaceClicked(object sender, EventArgs e)
        {
            if (toolbar_top.IsMaximizedWorkSpace)
            {
                scroll_image.MinHeight = 576;
                PhotoListBoxControl.Visibility = Visibility.Hidden;
                grid_main.ColumnDefinitions[0].Width = new GridLength(0);
                grid_main.RowDefinitions[1].Height = new GridLength(0);
            }
            else
            {
                scroll_image.MinHeight = 521;
                grid_main.RowDefinitions[1].Height = new GridLength(55);
                grid_main.ColumnDefinitions[0].Width = new GridLength(55);
                PhotoListBoxControl.Visibility = Visibility.Visible;
            }
        }

        private void toolbar_top_MinimizedWindowClicked(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void buttonSelect_topMost_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (!(bool)checkbox.IsChecked)
            {
                Topmost = false;
            }
            else
            {
                Topmost = true;
            }
        }

        private void buttonSelect_eyeDrop_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (!(bool)checkbox.IsChecked)
            {
                ColorPicker.Visibility = Visibility.Hidden;
            }
            else
            {
                ColorPicker.Visibility = Visibility.Visible;
            }
        }

        private void menuItem_copyColor_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)buttonSelect_eyeDrop.IsChecked)
            {
                Clipboard.SetDataObject(CopyToClipBoardColor.ToString().Remove(0, 3));
            }
            else
            {
                Clipboard.SetDataObject(CopyToClipBoardColor.ToString().Remove(0, 3));
            }
        }

        private void menuItem_copyColorRGB_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)buttonSelect_eyeDrop.IsChecked)
            {
                Clipboard.SetDataObject(String.Format("{0},{1},{2}", CopyToClipBoardColor.R, CopyToClipBoardColor.G, CopyToClipBoardColor.B));
            }
            else
            {
                Clipboard.SetDataObject(String.Format("{0},{1},{2}", CopyToClipBoardColor.R, CopyToClipBoardColor.G, CopyToClipBoardColor.B));
            }
        }

        private void image_main_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CopyToClipBoardColor = ColorPicker.PickColor(image_main);
        }

        private void menuItem_copyImage_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource img = image_main.Source;
            Clipboard.SetImage(img);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Uri uri = new Uri("pack://application:,,,/images/img_blank.png", UriKind.Absolute);
            PngBitmapDecoder decoder = new PngBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            image_main.Source = decoder.Frames[0];

            PhotoListBoxControl.UnselectSelected();
        }

        private void InitializeKeyboardEvents()
        {
            CommandBinding cb_paste = new CommandBinding(AK_OpenClipBoardImage, AK_OpenClipBoardImage_execute, AK_OpenClipBoardImage_canExecute);
            CommandBinding cb_open = new CommandBinding(AK_OpenNewFile, AK_OpenNewFile_execute, AK_OpenNewFile_canExecute);
            CommandBinding cb_topMost = new CommandBinding(AK_TopMost, AK_TopMost_execute, AK_TopMost_canExecute);
            CommandBinding cb_keyUp = new CommandBinding(AK_UpKey, AK_UpKey_execute, AK_UpKey_canExecute);
            CommandBinding cb_keyDown = new CommandBinding(AK_DownKey, AK_DownKey_execute, AK_DownKey_canExecute);
            CommandBinding cb_keyLeft = new CommandBinding(AK_LeftKey, AK_LeftKey_execute, AK_LeftKey_canExecute);
            CommandBinding cb_keyRight = new CommandBinding(AK_RightKey, AK_RightKey_execute, AK_RightKey_canExecute);

            CommandBinding cb_opacity_1 = new CommandBinding(AK_Opacity_1, AK_Opacity_1_execute, AK_Opacity_1_canExecute);
            CommandBinding cb_opacity_2 = new CommandBinding(AK_Opacity_2, AK_Opacity_2_execute, AK_Opacity_2_canExecute);
            CommandBinding cb_opacity_3 = new CommandBinding(AK_Opacity_3, AK_Opacity_3_execute, AK_Opacity_3_canExecute);
            CommandBinding cb_opacity_4 = new CommandBinding(AK_Opacity_4, AK_Opacity_4_execute, AK_Opacity_4_canExecute);
            CommandBinding cb_opacity_5 = new CommandBinding(AK_Opacity_5, AK_Opacity_5_execute, AK_Opacity_5_canExecute);
            CommandBinding cb_opacity_6 = new CommandBinding(AK_Opacity_6, AK_Opacity_6_execute, AK_Opacity_6_canExecute);
            CommandBinding cb_opacity_7 = new CommandBinding(AK_Opacity_7, AK_Opacity_7_execute, AK_Opacity_7_canExecute);
            CommandBinding cb_opacity_8 = new CommandBinding(AK_Opacity_8, AK_Opacity_8_execute, AK_Opacity_8_canExecute);
            CommandBinding cb_opacity_9 = new CommandBinding(AK_Opacity_9, AK_Opacity_9_execute, AK_Opacity_9_canExecute);
            CommandBinding cb_opacity_0 = new CommandBinding(AK_Opacity_0, AK_Opacity_0_execute, AK_Opacity_0_canExecute);

            CommandBinding cb_color = new CommandBinding(AK_Color, AK_Color_execute, AK_Color_canExecute);
            CommandBinding cb_hRuler = new CommandBinding(AK_HRuler, AK_HRuler_execute, AK_HRuler_canExecute);
            CommandBinding cb_vRuler = new CommandBinding(AK_VRuler, AK_VRuler_execute, AK_VRuler_canExecute);

            CommandBinding cb_quit = new CommandBinding(AK_Quit, AK_Quit_execute, AK_Quit_canExecute);

            CommandBinding cb_minimize = new CommandBinding(AK_Minimize, AK_Minimize_execute, AK_Minimize_canExecute);

            KeyGesture CKG_Minimize = new KeyGesture(Key.M, ModifierKeys.Control);

            KeyGesture CKG_Color = new KeyGesture(Key.C, ModifierKeys.Control);
            KeyGesture CKG_HRuler = new KeyGesture(Key.H, ModifierKeys.Control);
            KeyGesture CKG_VRuler = new KeyGesture(Key.U, ModifierKeys.Control);

            KeyGesture CKG_Quit = new KeyGesture(Key.Q, ModifierKeys.Control);

            KeyGesture CKG_TopMost = new KeyGesture(Key.T, ModifierKeys.Control);
            KeyGesture CKG_Paste = new KeyGesture(Key.V, ModifierKeys.Control);
            KeyGesture CKG_Open = new KeyGesture(Key.O, ModifierKeys.Control);

            KeyGesture CKG_MoveUp = new KeyGesture(Key.Up);
            KeyGesture CKG_MoveDown = new KeyGesture(Key.Down);
            KeyGesture CKG_MoveLeft = new KeyGesture(Key.Left);
            KeyGesture CKG_MoveRight = new KeyGesture(Key.Right);

            KeyGesture CKG_Opacity_1 = new KeyGesture(Key.NumPad1);
            KeyGesture CKG_Opacity_2 = new KeyGesture(Key.NumPad2);
            KeyGesture CKG_Opacity_3 = new KeyGesture(Key.NumPad3);
            KeyGesture CKG_Opacity_4 = new KeyGesture(Key.NumPad4);
            KeyGesture CKG_Opacity_5 = new KeyGesture(Key.NumPad5);
            KeyGesture CKG_Opacity_6 = new KeyGesture(Key.NumPad6);
            KeyGesture CKG_Opacity_7 = new KeyGesture(Key.NumPad7);
            KeyGesture CKG_Opacity_8 = new KeyGesture(Key.NumPad8);
            KeyGesture CKG_Opacity_9 = new KeyGesture(Key.NumPad9);
            KeyGesture CKG_Opacity_0 = new KeyGesture(Key.NumPad0);

            AK_Color.InputGestures.Add(CKG_Color);
            AK_HRuler.InputGestures.Add(CKG_HRuler);
            AK_VRuler.InputGestures.Add(CKG_VRuler);

            AK_Minimize.InputGestures.Add(CKG_Minimize);
            AK_Quit.InputGestures.Add(CKG_Quit);

            AK_TopMost.InputGestures.Add(CKG_TopMost);
            AK_OpenClipBoardImage.InputGestures.Add(CKG_Paste);
            AK_OpenNewFile.InputGestures.Add(CKG_Open);

            AK_UpKey.InputGestures.Add(CKG_MoveUp);
            AK_DownKey.InputGestures.Add(CKG_MoveDown);
            AK_LeftKey.InputGestures.Add(CKG_MoveLeft);
            AK_RightKey.InputGestures.Add(CKG_MoveRight);

            AK_Opacity_1.InputGestures.Add(CKG_Opacity_1);
            AK_Opacity_2.InputGestures.Add(CKG_Opacity_2);
            AK_Opacity_3.InputGestures.Add(CKG_Opacity_3);
            AK_Opacity_4.InputGestures.Add(CKG_Opacity_4);
            AK_Opacity_5.InputGestures.Add(CKG_Opacity_5);
            AK_Opacity_6.InputGestures.Add(CKG_Opacity_6);
            AK_Opacity_7.InputGestures.Add(CKG_Opacity_7);
            AK_Opacity_8.InputGestures.Add(CKG_Opacity_8);
            AK_Opacity_9.InputGestures.Add(CKG_Opacity_9);
            AK_Opacity_0.InputGestures.Add(CKG_Opacity_0);
        }

        public void AK_Color_execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(bool)buttonSelect_eyeDrop.IsChecked)
            {
                ColorPicker.Visibility = Visibility.Visible;
                buttonSelect_eyeDrop.IsChecked = true;
            }
            else
            {
                ColorPicker.Visibility = Visibility.Hidden;
                buttonSelect_eyeDrop.IsChecked = false;
            }
        }

        public void AK_Color_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_HRuler_execute(object sender, ExecutedRoutedEventArgs e)
        {
            canvas_image.Children.Add(new Ghost.Controls.HorizPixelControl());
        }

        public void AK_HRuler_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_VRuler_execute(object sender, ExecutedRoutedEventArgs e)
        {
            canvas_image.Children.Add(new Ghost.Controls.VertPixelControl());
        }

        public void AK_VRuler_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Minimize_execute(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void AK_Minimize_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Quit_execute(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void AK_Quit_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_1_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .1;
            canvas_image.Opacity = .1;
            opacity_editor.UpdateOpacityLabel(10);
        }

        public void AK_Opacity_1_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_2_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .2;
            canvas_image.Opacity = .2;
            opacity_editor.UpdateOpacityLabel(20);
        }

        public void AK_Opacity_2_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_3_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .3;
            canvas_image.Opacity = .3;
            opacity_editor.UpdateOpacityLabel(30);
        }

        public void AK_Opacity_3_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_4_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .4;
            canvas_image.Opacity = .4;
            opacity_editor.UpdateOpacityLabel(40);
        }

        public void AK_Opacity_4_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_5_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .5;
            canvas_image.Opacity = .5;
            opacity_editor.UpdateOpacityLabel(50);
        }

        public void AK_Opacity_5_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_6_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .6;
            canvas_image.Opacity = .6;
            opacity_editor.UpdateOpacityLabel(60);
        }

        public void AK_Opacity_6_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_7_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .7;
            canvas_image.Opacity = .7;
            opacity_editor.UpdateOpacityLabel(70);
        }

        public void AK_Opacity_7_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_8_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .8;
            canvas_image.Opacity = .8;
            opacity_editor.UpdateOpacityLabel(80);
        }

        public void AK_Opacity_8_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_9_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = .9;
            canvas_image.Opacity = .9;
            opacity_editor.UpdateOpacityLabel(90);
        }

        public void AK_Opacity_9_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_Opacity_0_execute(object sender, ExecutedRoutedEventArgs e)
        {
            scroll_image.Opacity = 1;
            canvas_image.Opacity = 1;
            opacity_editor.UpdateOpacityLabel(100);
        }

        public void AK_Opacity_0_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_RightKey_execute(object sender, ExecutedRoutedEventArgs e)
        {
            Left = Left + 1;
        }

        public void AK_RightKey_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_LeftKey_execute(object sender, ExecutedRoutedEventArgs e)
        {
            Left = Left - 1;
        }

        public void AK_LeftKey_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_DownKey_execute(object sender, ExecutedRoutedEventArgs e)
        {
            Top = Top + 1;
        }

        public void AK_DownKey_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_UpKey_execute(object sender, ExecutedRoutedEventArgs e)
        {
            Top = Top - 1;
        }

        public void AK_UpKey_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_TopMost_execute(object sender, ExecutedRoutedEventArgs e)
        {
            if ((bool)buttonSelect_topMost.IsChecked)
            {
                Topmost = false;
                buttonSelect_topMost.IsChecked = false;
            }
            else
            {
                Topmost = true;
                buttonSelect_topMost.IsChecked = true;
            }
        }

        public void AK_TopMost_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_OpenClipBoardImage_execute(object sender, ExecutedRoutedEventArgs e)
        {
            GetImageFromClipBoard();
        }

        public void AK_OpenClipBoardImage_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AK_OpenNewFile_execute(object sender, ExecutedRoutedEventArgs e)
        {
            PhotoListBoxControl.OpenFileViewer(sender, e);
        }

        public void AK_OpenNewFile_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}