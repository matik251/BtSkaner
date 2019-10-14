using Network;
using Network.Bluetooth;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class MainWindowVM : BindableBase
    {
        #region Properties

        private string _text = "";
        public string Text
        {
            get => _text;
            set
            {
                SetProperty(ref _text, value);
            }
        }
        public DelegateCommand ScanCommand { get; private set; }
        public DelegateCommand ConnectCommand { get; private set; }

        public List<DeviceInfo> DeviceList = new List<DeviceInfo>();
        public ObservableCollection<DeviceInfo> _deviceListObservable = new ObservableCollection<DeviceInfo>();
        public ObservableCollection<DeviceInfo> DeviceListObservable
        {
            get =>_deviceListObservable;
            set
            {
                SetProperty(ref _deviceListObservable, value);
            }
        }
        private DeviceInfo _selectedDevice;
        public DeviceInfo SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                SetProperty(ref _selectedDevice, value);
                ConnectCommand.CanExecute();
            }
        }
        
        #endregion


        public MainWindowVM()
        {
            ScanCommand = new DelegateCommand(Submit, CanSubmit);
            ConnectCommand = new DelegateCommand(Connect, CanConnect);
        }

        async void Submit()
        {
            List<DeviceInfo> temp = new List<DeviceInfo>();
            try
            {
                await Task.Run(() =>
                temp = ScanForDevices());
                DeviceListObservable = new ObservableCollection<DeviceInfo>(temp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool CanSubmit()
        {
            return true;
        }

        async void Connect()
        {
            List<DeviceInfo> temp = new List<DeviceInfo>();
            try
            {
                await Task.Run(() =>
                ConnectToDevice(SelectedDevice));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool CanConnect()
        {
            return true;
        }

        public List<DeviceInfo> ScanForDevices()
        {
            Text = "Skanowanie...";
            DeviceList.Clear();
            try
            {
                var list = ConnectionFactory.GetBluetoothDevices();
                Text = "";
                foreach (var device in list)
                {
                    Text += "Device: " + device.DeviceName 
                        + " sign_strengh:" + device.SignalStrength
                        + " known:" + device.IsKnown 
                        + " LastSeen" + device.LastSeen 
                        + "\n";

                    DeviceList.Add(device);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return DeviceList;
        }

        public async void ConnectToDevice(DeviceInfo device)
        {
            //var temp = ConnectionFactory.CreateBluetoothConnectionAsync(device);
            //MessageBox.Show(temp.Status.ToString()+"\n");
            Tuple<ConnectionResult, BluetoothConnection> x = await ConnectionFactory.CreateBluetoothConnectionAsync(device);
            do
            {
                x = await ConnectionFactory.CreateBluetoothConnectionAsync(device);

                if (x.Item1 == ConnectionResult.Connected)
                {
                    x.Item2.KeepAlive = true;

                    MessageBox.Show("Sukces ");
                }
                else
                {
                    //MessageBox.Show("Błąd połączenia ");
                }
            } while (x.Item1 != ConnectionResult.Connected);
            
        }    
    }
}
