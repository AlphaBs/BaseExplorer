using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BaseExplorer.UI
{
    /// <summary>
    /// ListControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ListControl : UserControl
    {
        public ListControl(bool isdir, string name, string displayname, bool isenc)
        {
            this.IsDir = isdir;
            this.ItemName = name;
            this.DisplayName = displayname;
            this.IsEnc = isenc;

            InitializeComponent();

            lbIsDir.Content = isdir ? "DIR" : "FILE";
            lbItemName.Content = name;
            if (name != displayname)
                lbDisplayName.Content = displayname;
            lbIsEnc.Content = isenc ? "ENC" : "";
        }

        public bool IsDir { get; private set; }
        public string ItemName { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsEnc { get; private set; }

        bool _isChecked = false;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                    Background = new SolidColorBrush(Colors.LightYellow);
                else
                    Background = new SolidColorBrush(Colors.White);

                _isChecked = value;
            }
        }
    }
}
