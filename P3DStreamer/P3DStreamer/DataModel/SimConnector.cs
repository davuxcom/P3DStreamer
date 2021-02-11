using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace P3DStreamer.DataModel
{
    enum DATA_REQUESTS
    {
        GENERIC_REQUEST
    }

    enum DEFINITIONS
    {
        GENERIC,
    }

    enum NOTIFICATION_GROUPS
    {
        GENERIC,
    }


    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class SimLinkAttribute : Attribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SimConnector : NativeWindow
    {
        public event Action<bool> Connected;
        public event Action<GENERIC_DATA> Data;

        private static readonly int WM_USER_SIMCONNECT = 0x402;
        private SimConnect m_simConnect;
        private readonly DispatcherTimer m_refreshTimer = new DispatcherTimer();

        public SimConnector() : base()
        {
            m_refreshTimer.Interval = TimeSpan.FromMilliseconds(500);
            m_refreshTimer.Tick += RefreshTimer_Tick;
            m_refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            m_simConnect?.RequestDataOnSimObjectType(DATA_REQUESTS.GENERIC_REQUEST, DEFINITIONS.GENERIC, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_USER_SIMCONNECT)
            {
                if (m_simConnect != null)
                {
                    try
                    {
                        m_simConnect.ReceiveMessage();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Failed while talking to sim: " + ex);
                        doDisconnected();
                    }
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        public void Connect()
        {
            CreateHandle(new CreateParams());

            ConnectInternal();
        }

        private void ConnectInternal()
        {
            Trace.WriteLine("Connecting to sim...");
            try
            {
                m_simConnect = new SimConnect("Managed Data Request", Handle, (uint)WM_USER_SIMCONNECT, null, 0);
                m_simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(OnSimConnectRecvOpen);
                m_simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnSimConnectRecvQuit);
                m_simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnSimConnectRecvException);
                m_simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnSimConnectRecvSimobjectDataBytype);

                onConnected();
            }
            catch (COMException)
            {
                Trace.WriteLine("Failed to connect to sim");
                doDisconnected();
            }
        }

        void onConnected()
        {
            try
            {
                RegisterStruct<GENERIC_DATA>(DEFINITIONS.GENERIC);
                RegisterEventsEnum<SimEvents>();
            }
            catch (COMException ex)
            {
                Trace.WriteLine("Failed registering: " + ex);
                doDisconnected();
            }
        }

        private void RegisterStruct<TData>(Enum definition)
        {
            foreach (var field in typeof(TData).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var attr = field.GetCustomAttributes<SimLinkAttribute>().First();
                m_simConnect.AddToDataDefinition(definition, attr.Name, attr.Type, attr.Type == null ? SIMCONNECT_DATATYPE.STRING256 : SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            }
            m_simConnect.RegisterDataDefineStruct<TData>(definition);

        }

        private void RegisterEventsEnum<TEnum>()
        {
            foreach (var value in Enum.GetValues(typeof(TEnum)))
            {
                m_simConnect.MapClientEventToSimEvent((Enum)value, Enum.GetName(typeof(TEnum), value).Replace("MOBIFLIGHT_", "MobiFlight."));
            }
        }

        private void OnSimConnectRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Trace.WriteLine("Exception received: " + ((uint)data.dwException));
            doDisconnected();
        }

        private void OnSimConnectRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Trace.WriteLine("Connected to sim");
            Connected(true);
        }

        private void OnSimConnectRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Trace.WriteLine("Sim quit");
            doDisconnected();
        }

        private void OnSimConnectRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            switch ((DATA_REQUESTS)data.dwRequestID)
            {
                case DATA_REQUESTS.GENERIC_REQUEST:
                    Data((GENERIC_DATA)data.dwData[0]);
                    break;
                default:
                    throw new NotImplementedException($"{data.dwRequestID}");
            }
        }

        private void doDisconnected()
        {
            if (m_simConnect != null)
            {
                m_simConnect.Dispose();
                m_simConnect = null;
            }
            Connected(false);

            ReconnectAsync();
        }

        private async void ReconnectAsync()
        {
            await Task.Delay(10 * 1000);
            ConnectInternal();
        }

        public void SendEvent(SimEvents evt, uint evtData)
        {
            m_simConnect.TransmitClientEvent(0U, (Enum)evt, evtData, (Enum)NOTIFICATION_GROUPS.GENERIC, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }
    }
}