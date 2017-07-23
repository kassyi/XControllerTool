using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using XInputAPI;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace XControllerTool.Models
{
    public class XController
    {
        public ReactiveProperty<uint> ControllerNo { get; } = new ReactiveProperty<uint>();
        public ReactiveProperty<bool> IsConnected { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<byte> BatteryLevel { get; } = new ReactiveProperty<byte>();
        public ReactiveProperty<string> Stauts { get; } = new ReactiveProperty<string>();

        public ReactiveCommand PowerOffControllerCommand { get; }
        public ReactiveCommand UpdateStatusCommand { get; }

        public XController()
        {
            //各コマンドはXInput.IsEnable==trueの場合のみ使用できる
            PowerOffControllerCommand = new[] {
                IsConnected,
                Observable.Return(XInput.IsEnable)//固定値
            }.CombineLatestValuesAreAllTrue()
            .ToReactiveCommand();
            PowerOffControllerCommand.Subscribe(_ => PowerOffController());

            UpdateStatusCommand = Observable.Return(XInput.IsEnable)
                .ToReactiveCommand();
            UpdateStatusCommand.Subscribe(_ => UpdateStatus());
        }

        void UpdateStatus()
        {

            var capabilities = new XInput.XINPUT_CAPABILITIES();

            var conneced = XInput.GetCapabilities(ControllerNo.Value, XInput.XINPUT_FLAG_GAMEPAD, ref capabilities);
            if (conneced == (uint)XInput.ErrCode.ERROR_SUCCESS)
            {
                //capabilities.Typeから値を取得すると1になって、バッテリー情報が取れない。ので決め打ち。ゲームパッドしか使えない仕様。仕様です。
                const byte BATTERY_DEVTYPE_GAMEPAD = 0;
                var battryInfo = new XInput.XINPUT_BATTERY_INFORMATION();
                var sucsess = XInput.GetBatteryInformation(ControllerNo.Value, BATTERY_DEVTYPE_GAMEPAD, ref battryInfo);

                BatteryLevel.Value = battryInfo.BatteryLevel;
                IsConnected.Value = true;
            }
            else if (conneced == (uint)XInput.ErrCode.ERROR_DEVICE_NOT_CONNECTED)
            {
                BatteryLevel.Value = 0;
                IsConnected.Value = false;
            }
            Stauts.Value = $"Controller {ControllerNo.Value + 1}: {(IsConnected.Value ? "Connect" : "Disconnect")}";
        }

        void PowerOffController()
        {
            uint Res = XInput.PowerOffController(ControllerNo.Value);

            if (Res == (uint)XInput.ErrCode.ERROR_DEVICE_NOT_CONNECTED)
            {
                Console.WriteLine($"DEVICE {ControllerNo.Value.ToString()}* NOT CONNETECTED.");
            }
            else
            {
                BatteryLevel.Value = 0;
                IsConnected.Value = false;
                Stauts.Value = $"Controller {ControllerNo.Value + 1}: {(IsConnected.Value ? "Connect" : "Disconnect")}";
            }
        }
    }
}
