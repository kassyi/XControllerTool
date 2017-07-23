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
using XControllerTool.Models;

namespace XControllerTool
{
    /// <summary>
    /// ControllerStatus.xaml の相互作用ロジック
    /// </summary>
    public partial class ControllerStatusView : UserControl
    {
        /// <summary>
        /// xamlからnoを設定できるようにするための依存プロパティ。0-3
        /// </summary>
        public static readonly DependencyProperty ControllerNoProperty =
            DependencyProperty.Register("ControllerNo",
                typeof(uint),
                typeof(ControllerStatusView),
                new PropertyMetadata(0u,
                    new PropertyChangedCallback((d, e) =>
                    {
                        //内部modelと同期
                        var instance = d as ControllerStatusView;
                        var ctr = instance.DataContext as XController;
                        ctr.ControllerNo.Value = (uint)e.NewValue;
                    })));

        public uint ControllerNo
        {
            get { return (uint)GetValue(ControllerNoProperty); }
            set { SetValue(ControllerNoProperty, value); }
        }



        public ControllerStatusView()
        {
            InitializeComponent();

            var vm = (XController)DataContext;
            Loaded += (_, __) => vm.UpdateStatusCommand.Execute();
            //コントローラの状態を取得した後に、viewに反映させる。IsEnabledはxamlからいじれない
            vm.IsConnected.PropertyChanged += (_, __) => { IsEnabled = vm.IsConnected.Value; };
            IsEnabled = vm.IsConnected.Value;
        }
    }
}
