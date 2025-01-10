using Avalonia.Media;
using System.ComponentModel;
using System.Threading;

namespace TestAvalonia.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
      
        ModbusSourceInterface Modbus;
        public Source_ViewModel SourceA { get; set; }
        public Source_ViewModel SourceB { get; set; }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public void refreshValues() {
      
            SourceA.refreshValues();
            SourceB.refreshValues();
        }

        public MainWindowViewModel() {
            Modbus = new ModbusSourceInterface();
            Modbus.newClient(1);
            Modbus.newClient(2);

            SourceA = new(Modbus.Clients[0]);
            SourceB = new(Modbus.Clients[1]);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Source_ViewModel : INotifyPropertyChanged
    {
        private ModbusSourceClient Client;

        public string uout = "";
        public string Uout
        {
            get{ 
            
                return uout;
            } 
            set
            {
                uout= value;
                OnPropertyChanged(nameof(Uout));
            }
        }

        public string iout = "";
        public string Iout
        {
            get {

                return iout;
            } set
            {
                iout= value;
                OnPropertyChanged(nameof(Iout));
            }
        }

        public string power;
        public string Power
        {
            get { 
            return power;   
            } set
            {
                power= value;
                OnPropertyChanged(nameof(Power));
            }
        }

        public string uin = "";
        public string Uin
        {
            get { 
            return uin;
            
            } set
            {
                uin= value;
                OnPropertyChanged(nameof(Uin));
            }
        }

        public string protect = "";
        public string Protect
        {
            get
            {
                return protect;
            }
            set
            {
                protect = value;
                OnPropertyChanged(nameof(Protect));
            }
        }

        private string uset = "0";
        public string Uset
        {
            get
            {
                return uset;
            }
            set
            {
                uset = value;
                Client.U_set = float.Parse(uset);

                OnPropertyChanged(nameof(Uset));
            }
        }

        private string iset = "0";
        public string Iset
        {
            get
            {
                return iset;
            }
            set
            {
                iset = value;
                Client.I_set = float.Parse(iset);

                OnPropertyChanged(nameof(Iset));
            }
        }

        private string _lock = "0";
        public string Lock
        {
            get
            {
                return _lock;
            }
            set
            {
                _lock = value;
                Client.I_set = float.Parse(_lock);

                OnPropertyChanged(nameof(Lock));
            }
        }

        public void refreshValues()
        {
            Uset = Client.U_set.ToString("0.00");
            Iset = Client.I_set.ToString("0.00");

            Uout = Client.Uout.ToString("0.00");
            Iout =  Client.Iout.ToString("0.00");
            Power = Client.Power.ToString("0.00");
            Uin = Client.Uin.ToString("0.00");
            Protect = Client.Protect.ToString();
            
        }

        public Source_ViewModel(ModbusSourceClient client) {
            Client = client;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
