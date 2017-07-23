using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using XInputAPI;

namespace XControllerTool.Models
{
    public class MainWindowModel
    {
        public ReactiveProperty<IEnumerable<XController>> ControllersList { get; } = new ReactiveProperty<IEnumerable<XController>>();

        public ReactiveCommand UpdateAllControllerStatusCommand { get; } = new ReactiveCommand();
        public ReactiveCommand PowerOffAllControllerAndExitCommad { get; }

        public MainWindowModel()
        {
            UpdateAllControllerStatusCommand.Subscribe(_ =>
            {
                foreach (var controller in ControllersList.Value)
                {
                    controller.UpdateStatusCommand.Execute();
                }
            });

            PowerOffAllControllerAndExitCommad = Observable.Return(XInput.IsEnable)
                .ToReactiveCommand();
            PowerOffAllControllerAndExitCommad.Subscribe(_ =>
            {
                foreach (var controller in ControllersList.Value)
                {
                    controller.PowerOffControllerCommand.Execute();
                }
                Environment.Exit(0);
            });
        }

    }
}
