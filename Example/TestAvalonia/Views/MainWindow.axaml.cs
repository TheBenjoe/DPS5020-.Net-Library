using Avalonia.Controls;
using Avalonia.Interactivity;
//using Meadow.Modbus;
//using System.Device.Gpio;


using TestAvalonia.ViewModels;



namespace TestAvalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

           DataContext = new MainWindowViewModel();
            

        }

        private void Grid_SourceA_Visible(object sender, RoutedEventArgs args)
        {
            Grid_SourceASet.IsVisible = true;
            Grid_SourceBSet.IsVisible = false;


            Grid_SourceA.IsVisible = true;
            Grid_SourceB.IsVisible = false;
            set_splitbutton_source.Content = "Source A";
            get_splitbutton_source.Content = "Source A";
        }

        private void Grid_SourceB_Visible(object sender, RoutedEventArgs args)
        {
            Grid_SourceASet.IsVisible = false;
            Grid_SourceBSet.IsVisible = true;

            Grid_SourceA.IsVisible = false;
            Grid_SourceB.IsVisible = true;
            set_splitbutton_source.Content = "Source B";
            get_splitbutton_source.Content = "Source B";
        }

        private void OnClick_Read(object sender, RoutedEventArgs args) {

      /*      if (Modbus.Clients[0].ModbusInterface.IsOpen == false) {
                Modbus.Clients[0].ModbusInterface.Open();
            }
            Label_SourceA_USet.Text=Modbus.Clients[0].U_set.ToString();
            Label_SourceA_ISet.Text=Modbus.Clients[0].I_set.ToString();
            Label_SourceA_UOut.Text=Modbus.Clients[0].Uout.ToString();
            Label_SourceA_IOut.Text=Modbus.Clients[0].Iout.ToString();
            Label_SourceA_Power.Text=Modbus.Clients[0].Power.ToString();
            Label_SourceA_UIn.Text=Modbus.Clients[0].Uin.ToString();
            Label_SourceA_Lock.Text=Modbus.Clients[0].Lock.ToString();
            Label_SourceA_Protect.Text=Modbus.Clients[0].Protect.ToString();
        */   

         //   SourceA.Text = Modbus.Clients[0].ReadRegisterData(2).ToString();
            
            //SourceA.Text = Modbus.Clients[0].ModbusInterface.BytesToRead.ToString();
            /*
            for (int i = 0; i < received.Length; i++)
            {
                received[i] = Convert.ToByte(Modbus.Clients[0].ModbusInterface.ReadByte());
            }



            int value = received[3] << 8 | received[4];
            SourceA.Text = value.ToString();
            Thread.Sleep(4000);
          
            Modbus.Close();

            Thread.Sleep(1000);
            //serialPort.Close();
            Environment.Exit(0);*/
        }
        //old rasp config: dtoverlay=vc4-kms-v3d,int_pin=24,addr=0x48
        private void OnClick_Set(object sender, RoutedEventArgs args)
        {
        
            /*Modbus.Clients[0].U_set = float.Parse(Textbox_SourceA_Uset.Text);
            Thread.Sleep(1000);
            Modbus.Clients[0].I_set = float.Parse(Textbox_SourceA_Iset.Text);
            Thread.Sleep(1000);
            Modbus.Clients[0].Lock = int.Parse(Textbox_SourceA_Lock.Text);


            Thread.Sleep(1000);
            */
           // refreshTextblock();


            //Modbus.Clients[0].U_set=2;
            //Modbus.Clients[0].WriteSingleRegister(0, 2);
            
            
                        //var receive = serialPort.ReadExisting();
            /*   SerialPortShim serialPortshim = new(serialPort);
               ModbusRtuClient modbusClient= new ModbusRtuClient(serialPortshim);
               modbusClient.Connect();
               //modbusClient.
              */
            
            //Thread.Sleep(1000);
            //serialPort.Close();
            //Environment.Exit(0);
        }
    }
}