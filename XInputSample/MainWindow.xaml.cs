// XInputSample:XInputPowerOffControlerのサンプル

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;

// XInputAPIラッパー
using XInputAPI;
using XControllerTool.Models;

namespace XControllerTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var model = (MainWindowModel)DataContext;
            

            model.ControllersList.Value = new []{
                (XController)ct0.DataContext,
                (XController)ct1.DataContext,
                (XController)ct2.DataContext,
                (XController)ct3.DataContext };
        }
    }
}
