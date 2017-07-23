using System;
using System.Runtime.InteropServices;



namespace XInputAPI
{
    #region WINAPI宣言
    internal static class NativeMethods
    {


        //LoadLibraty：DLLモジュールをマップ
        [DllImport("kernel32", EntryPoint = "LoadLibrary", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);
        //FreeLibrary：DLLモジュールの参照カウントを1つ減らす
        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);
        //GetProcAddress：関数名を指定して関数のアドレスを取得
        [DllImport("kernel32", EntryPoint = "GetProcAddress", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true,
            BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string lpProcName);
        //GetProcAddress：序数を指定して関数のアドレスを取得
        [DllImport("kernel32", EntryPoint = "GetProcAddress", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true,
            BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr GetProcAddressFromOrdinalNum(IntPtr hModule, IntPtr Num);

    }
    #endregion

    /// <summary>
    /// XInputAPI呼び出し用.netラッパー
    /// XInputDLLをLoadLibraryを使用して動的に読み込む
    /// </summary>  
    public static class XInput
    {
        /*
        // XInputDLLが確実に存在する場合は以下のように静的にDllImportするのみでも可
        [DllImport("XInput1_3.dll",EntryPoint = "XInputEnable")]
        public static extern void XInputEnable(bool enable);
        [DllImport("XInput1_3.dll", EntryPoint = "XInputGetState")]
        public static extern uint XInputGetState(uint dwUserIndex, ref XINPUT_STATE pState);
        .
        .
        //序数のみでエクスポートされている関数の場合
        [DllImport("XInput1_3.dll", EntryPoint = "#103")]       //EntryPointに#[OrdinalNumber]を指定する
        public static extern uint XInputPowerOffController(uint dwUserIndex);
        */


        #region XInput用構造体の宣言

        //XInput用構造体の宣言
        [StructLayout(LayoutKind.Explicit)]
        public struct XINPUT_GAMEPAD
        {
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0)]
            public ushort wButtons;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(2)]
            public byte bLeftTrigger;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(3)]
            public byte bRightTrigger;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(4)]
            public short sThumbLX;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(6)]
            public short sThumbLY;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(8)]
            public short sThumbRX;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(10)]
            public short sThumbRY;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "XINPUT_GAMEPAD")]
        [StructLayout(LayoutKind.Explicit)]
        public struct XINPUT_STATE
        {
            [FieldOffset(0)]
            public UInt32 dwPacketNumber;
            [FieldOffset(4)]
            public XINPUT_GAMEPAD XINPUT_GAMEPAD;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct XINPUT_VIBRATION
        {
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0)]
            public ushort wLeftMotorSpeed;
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(2)]
            public ushort wRightMotorSpeed;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct XINPUT_BATTERY_INFORMATION
        {
            [FieldOffset(0)]
            public byte BatteryType;
            [FieldOffset(1)]
            /* BATTERY_LEVEL_EMPTY = 0x00,
             * BATTERY_LEVEL_LOW = 0x01,
             * BATTERY_LEVEL_MEDIUM = 0x02,
             * BATTERY_LEVEL_FULL = 0x03 
             */
            public byte BatteryLevel;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct XINPUT_KEYSTROKE
        {
            [FieldOffset(0)]
            public ushort VirtualKey;
            [FieldOffset(2)]
            public char Unicode;
            [FieldOffset(4)]
            public ushort Flags;
            [FieldOffset(6)]
            public byte UserIndex;
            [FieldOffset(7)]
            public byte HidCode;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Gamepad")]
        [StructLayout(LayoutKind.Explicit)]
        public struct XINPUT_CAPABILITIES
        {
            [FieldOffset(0)]
            public byte Type;
            [FieldOffset(1)]
            public byte SubType;
            [FieldOffset(2)]
            public ushort Flags;
            [FieldOffset(4)]
            public XINPUT_GAMEPAD Gamepad;
            [FieldOffset(16)]
            public XINPUT_VIBRATION Vibration;
        }

        #endregion

        #region 定数の定義

        /// <summary>
        /// エラーコードの定義
        /// </summary>
        public enum ErrCode : uint
        {
            ERROR_SUCCESS = 0,
            ERROR_DEVICE_NOT_CONNECTED = 1167
        }
        #endregion

        public const byte XINPUT_FLAG_GAMEPAD = 1;

        #region 関数Delegateの定義

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate void XInputEnable(bool enable);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint XInputGetState(uint dwUserIndex, ref XINPUT_STATE pState);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint XInputSetState(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint XInputGetBatteryInformation(uint dwUserIndex, byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint XInputGetKeystroke(uint dwUserIndex, uint dwReserved, ref XINPUT_KEYSTROKE pKeystroke);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint XInputGetCapabilities(uint dwUserIndex, uint dwFlags, ref XINPUT_CAPABILITIES pCapabilities);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint XInputGetStateEx(uint dwUserIndex, ref XINPUT_STATE pControllerData);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint XInputPowerOffController(uint dwUserIndex);

        #endregion

        #region 関数ポインタの宣言

        private static IntPtr hModule = IntPtr.Zero;
        private static IntPtr XInputEnable_FuncAdd = IntPtr.Zero;
        private static IntPtr XInputGetState_FuncAdd = IntPtr.Zero;
        private static IntPtr XInputSetState_FuncAdd = IntPtr.Zero;
        private static IntPtr XInputGetBatteryInfomation_FuncAdd = IntPtr.Zero;
        private static IntPtr XInputGetKeystroke_FuncAdd = IntPtr.Zero;
        private static IntPtr XInputGetCapabilities_FuncAdd = IntPtr.Zero;
        private static IntPtr XInputGetStateEx_FuncAdd = IntPtr.Zero;
        private static IntPtr XInputPowerOffController_FuncAdd = IntPtr.Zero;
        private static XInputEnable funcXInputEnable;
        private static XInputGetState funcXInputGetState;
        private static XInputSetState funcXInputSetState;
        private static XInputGetBatteryInformation funcXInputGetBatteryInformation;
        private static XInputGetKeystroke funcXInputGetKeystroke;
        private static XInputGetCapabilities funcXInputGetCapabilities;
        private static XInputGetStateEx func_XInputGetStateEx;
        private static XInputPowerOffController func_XInputPowerOffController;

        #endregion

        #region プロパティ定義

        private static bool _IsEnable = false;
        public static bool IsEnable
        {
            get { return _IsEnable; }
        }

        private static string _DllName = "";
        public static String DllName
        {
            get { return _DllName; }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static XInput()
        {
            //DLL読み込み
            Load("XInput1_4.dll");          //XInput1_4はWindows8以降に含まれる
            //XInput1_4がなけばXInput1_3を読み込む
            if (!_IsEnable) Load("XInput1_3.dll");              //XInput1_3はDirectXランタイムに含まれる

        }

        /// <summary>
        /// DLLを動的にロードし、関数ポインタを格納する。
        /// </summary>
        /// <param name="DllName">ロードするDLL名</param>
        private static void Load(string DllName)
        {
            _IsEnable = false;
            //DLLを読み込み
            hModule = NativeMethods.LoadLibrary(DllName);
            //失敗したら終了
            if (hModule == IntPtr.Zero)
            {
                return;
            }

            //関数アドレスの取得
            XInputEnable_FuncAdd = NativeMethods.GetProcAddress(hModule, "XInputEnable");
            XInputGetState_FuncAdd = NativeMethods.GetProcAddress(hModule, "XInputGetState");
            XInputSetState_FuncAdd = NativeMethods.GetProcAddress(hModule, "XInputSetState");
            XInputGetBatteryInfomation_FuncAdd = NativeMethods.GetProcAddress(hModule, "XInputGetBatteryInformation");
            XInputGetKeystroke_FuncAdd = NativeMethods.GetProcAddress(hModule, "XInputGetKeystroke");
            XInputGetCapabilities_FuncAdd = NativeMethods.GetProcAddress(hModule, "XInputGetCapabilities");
            XInputGetStateEx_FuncAdd = NativeMethods.GetProcAddressFromOrdinalNum(hModule, (IntPtr)100);                      //非公開関数：XInputGetStateEx
            XInputPowerOffController_FuncAdd = NativeMethods.GetProcAddressFromOrdinalNum(hModule, (IntPtr)103);              //非公開関数：XInputPowerOffController OrdinalNumber=103

            //関数アドレスからDelegateを取得
            funcXInputEnable = (XInputEnable)Marshal.GetDelegateForFunctionPointer(XInputEnable_FuncAdd, typeof(XInputEnable));
            funcXInputGetState = (XInputGetState)Marshal.GetDelegateForFunctionPointer(XInputGetState_FuncAdd, typeof(XInputGetState));
            funcXInputSetState = (XInputSetState)Marshal.GetDelegateForFunctionPointer(XInputSetState_FuncAdd, typeof(XInputSetState));
            funcXInputGetBatteryInformation = (XInputGetBatteryInformation)Marshal.GetDelegateForFunctionPointer(XInputGetBatteryInfomation_FuncAdd, typeof(XInputGetBatteryInformation));
            funcXInputGetKeystroke = (XInputGetKeystroke)Marshal.GetDelegateForFunctionPointer(XInputGetKeystroke_FuncAdd, typeof(XInputGetKeystroke));
            funcXInputGetCapabilities = (XInputGetCapabilities)Marshal.GetDelegateForFunctionPointer(XInputGetCapabilities_FuncAdd, typeof(XInputGetCapabilities));
            func_XInputGetStateEx = (XInputGetStateEx)Marshal.GetDelegateForFunctionPointer(XInputGetStateEx_FuncAdd, typeof(XInputGetStateEx));
            func_XInputPowerOffController = (XInputPowerOffController)Marshal.GetDelegateForFunctionPointer(XInputPowerOffController_FuncAdd, typeof(XInputPowerOffController));

            _DllName = DllName;
            _IsEnable = true;
        }

        //以下XInputAPI関数 ***************************************************************************************************************

        /// <summary>
        /// XInputEnable
        /// https://msdn.microsoft.com/ja-jp/library/bb174825(v=vs.85).aspx
        /// </summary>
        /// <param name="bEnable"></param>
        public static void Enable(bool bEnable)
        {
            funcXInputEnable(bEnable);
        }

        /// <summary>
        /// XInputetState
        /// https://msdn.microsoft.com/ja-jp/library/bb174829(v=vs.85).aspx
        /// </summary>
        public static uint GetState(uint dwUserIndex, ref XINPUT_STATE pState)
        {
            return (funcXInputGetState(dwUserIndex, ref pState));
        }

        /// <summary>
        /// XInputSetState
        /// https://msdn.microsoft.com/ja-jp/library/bb174830(v=vs.85).aspx
        /// <summary>
        public static uint SetState(uint dwUserIndex, ref XINPUT_VIBRATION pVibration)
        {
            return funcXInputSetState(dwUserIndex, ref pVibration);
        }

        /// <summary>
        /// XInputGetBatteryInfomation
        /// https://msdn.microsoft.com/ja-jp/library/windows/desktop/microsoft.directx_sdk.reference.xinputgetbatteryinformation.aspx
        /// <summary>
        public static uint GetBatteryInformation(uint dwUserIndex, byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation)
        {
            return funcXInputGetBatteryInformation(dwUserIndex, devType, ref pBatteryInformation);
        }

        /// <summary>
        /// XInputGetKeystroke
        /// https://msdn.microsoft.com/ja-jp/library/bb174828(v=vs.85).aspx
        /// <summary>
        public static uint GetKeystroke(uint dwUserIndex, uint dwReserved, ref XINPUT_KEYSTROKE pKeystroke)
        {
            return funcXInputGetKeystroke(dwUserIndex, dwReserved, ref pKeystroke);
        }

        /// <summary>
        /// XInputGetCapabilities
        /// https://msdn.microsoft.com/ja-jp/library/bb174826(v=vs.85).aspx
        /// <summary>
        public static uint GetCapabilities(uint dwUserIndex, uint dwFlags, ref XINPUT_CAPABILITIES pCapabilities)
        {
            return funcXInputGetCapabilities(dwUserIndex, dwFlags, ref pCapabilities);
        }

        /// <summary>
        /// XInputGetStateEX OrdinalNo=#100
        /// 非公開関数：ガイドボタン状態もふくむGetState
        /// <summary>
        public static uint GetStateEx(uint dwUserIndex, ref XINPUT_STATE pState)
        {
            return func_XInputGetStateEx(dwUserIndex, ref pState);
        }

        /// <summary>
        /// XInputPowerOffController
        /// 非公開関数：ワイヤレスコントローラーの電源OFF
        /// </summary>
        /// <param name="dwUserIndex">コントローラーNo(0~3)</param>
        /// <returns></returns>        
        public static uint PowerOffController(uint dwUserIndex)
        {
            return func_XInputPowerOffController(dwUserIndex);
        }


    }

}