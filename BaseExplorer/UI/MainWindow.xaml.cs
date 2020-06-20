using BaseExplorer.Core;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace BaseExplorer.UI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            viewer = new Base64Viewer();
            TempPath = Path.Combine(Path.GetTempPath(), "explorerBaseTemp");
            Directory.CreateDirectory(TempPath);

            InitializeComponent();
        }

        IPreviewer[] Previewer = new IPreviewer[]
            {
                new ImagePreviewer()
            };


        IViewer viewer;
        string TempPath;
        string CurrentPath = "";
        ListControl SelectedControl;
        bool ShowPreview = true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Navigate(Environment.CurrentDirectory);
        }

        void Navigate(string path)
        {
            try
            {
                sideTh.Visibility = Visibility.Collapsed;
                stkList.Children.Clear();
                SelectedControl = null;

                var dirinfo = new DirectoryInfo(path);
                foreach (var item in dirinfo.GetDirectories())
                {
                    AddCtrl(item.Name, true);
                }
                foreach (var item in dirinfo.GetFiles())
                {
                    AddCtrl(item.Name, false);
                }

                CurrentPath = path;
                txtPath.Text = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void AddCtrl(string name, bool isdir)
        {
            var isenc = viewer.CheckIsEncodedName(name, isdir);
            var dname = name;
            if (isenc)
                dname = viewer.GetDecodedName(dname, isdir);
            var ctrl = new ListControl(isdir, name, dname, isenc);
            ctrl.MouseDown += Ctrl_Click;
            ctrl.MouseDoubleClick += Ctrl_DoubleClick;
            stkList.Children.Add(ctrl);
        }

        private void Ctrl_Click(object sender, EventArgs e)
        {
            if (SelectedControl != null)
                SelectedControl.IsChecked = false;

            SelectedControl = (ListControl)sender;
            SelectedControl.IsChecked = true;

            ShowPreviewContent();
        }

        private void ShowPreviewContent()
        {
            if (!ShowPreview)
                return;
            if (SelectedControl == null)
                return;

            if (SelectedControl.IsDir)
                return;

            try
            {
                IPreviewer view = null;
                var ext = Path.GetExtension(SelectedControl.DisplayName);

                foreach (var item in Previewer)
                {
                    foreach (var type in item.SupportExtensions)
                    {
                        if (type == ext)
                        {
                            view = item;
                            break;
                        }
                    }
                }

                if (view == null)
                    sideTh.Visibility = Visibility.Collapsed;
                else
                {
                    var realpath = Path.Combine(CurrentPath, SelectedControl.ItemName);
                    var displaypath = Path.Combine(CurrentPath, SelectedControl.DisplayName);
                    var preview = view.GetPreview(realpath, displaypath);

                    if (preview == null)
                        sideTh.Visibility = Visibility.Collapsed;
                    else
                    {
                        sideTh.Visibility = Visibility.Visible;
                        sideName.Content = preview.Name;
                        sideThumb.Source = preview.PreviewImage;
                        sidePath.Content = SelectedControl.DisplayName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Ctrl_DoubleClick(object sender, EventArgs e)
        {
            SelectedControl = (ListControl)sender;

            var path = Path.Combine(CurrentPath, SelectedControl.ItemName);

            if (SelectedControl.IsDir)
                Navigate(path);
            else
            {
                if (SelectedControl.IsEnc)
                {
                    var orgpath = Path.Combine(CurrentPath, SelectedControl.ItemName);
                    var ext = SelectedControl.DisplayName.Split('.').Last();
                    var newpath = Path.Combine(TempPath, "_." + ext);

                    File.Copy(orgpath, newpath, true);
                    Launch(newpath);
                }
                else
                    Launch(path);
            }
        }

        private void btnNavigate_Click(object sender, RoutedEventArgs e)
        {
            Navigate(txtPath.Text);
        }

        private void btnExplorer_Click(object sender, RoutedEventArgs e)
        {
            Launch(CurrentPath);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            // Open dialog
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedControl == null)
                return;

            if (SelectedControl.IsEnc)
                return;

            try
            {
                var path = Path.Combine(CurrentPath, SelectedControl.ItemName);

                if (SelectedControl.IsDir)
                    EncodeRev(path);
                else
                    viewer.EncodeFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            Navigate(CurrentPath);
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedControl == null)
                return;

            if (!SelectedControl.IsEnc)
                return;

            try
            {
                var path = Path.Combine(CurrentPath, SelectedControl.ItemName);

                if (SelectedControl.IsDir)
                    DecodeRev(path);
                else
                    viewer.DecodeFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            Navigate(CurrentPath);
        }

        void EncodeRev(string path)
        {
            var dirinfo = new DirectoryInfo(path);

            foreach (var item in dirinfo.GetFiles())
            {
                if (viewer.CheckIsEncodedName(item.Name))
                    continue;

                viewer.EncodeFile(path, item.Name);
            }

            foreach (var item in dirinfo.GetDirectories())
            {
                if (viewer.CheckIsEncodedName(item.Name))
                    continue;

                EncodeRev(Path.Combine(path, item.Name));
            }

            viewer.EncodeDir(path);
        }

        void DecodeRev(string path)
        {
            var dirinfo = new DirectoryInfo(path);

            foreach (var item in dirinfo.GetFiles())
            {
                if (!viewer.CheckIsEncodedName(item.Name))
                    continue;

                viewer.DecodeFile(path, item.Name);
            }

            foreach (var item in dirinfo.GetDirectories())
            {
                if (!viewer.CheckIsEncodedName(item.Name))
                    continue;

                DecodeRev(Path.Combine(path, item.Name));
            }

            viewer.DecodeDir(path);
        }

        private void btnParent_Click(object sender, RoutedEventArgs e)
        {
            var d = DivideDir(CurrentPath);
            Navigate(d.Item1);
        }

        Tuple<string, string> DivideDir(string dirpath)
        {
            var dirIndex = dirpath.LastIndexOf(Path.DirectorySeparatorChar);

            var parent = dirpath.Substring(0, dirIndex);
            var name = dirpath.Substring(dirIndex + 1);

            return new Tuple<string, string>(parent, name);
        }

        private void Launch(string p)
        {
            try
            {
                System.Diagnostics.Process.Start(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Directory.GetFiles(TempPath))
            {
                File.Delete(item);
            }

            MessageBox.Show("Success");
        }

        private void cbShowPreview_Checked(object sender, RoutedEventArgs e)
        {
            ShowPreview = true;
            ShowPreviewContent();
        }

        private void cbShowPreview_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowPreview = false;
            sideTh.Visibility = Visibility.Collapsed;
        }
    }
}
