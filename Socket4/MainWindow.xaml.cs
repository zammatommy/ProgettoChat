using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;  //
using System.Net;        //
using System.Windows.Threading; //
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Socket4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket = null;
        DispatcherTimer dTimer = null;
        public MainWindow()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //Abbiamo crato la classe socket
            IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endpoint = new IPEndPoint(local_address.MapToIPv4(), 65000); //perchè la cosa funzionerà se i firewall sono sbloccati, possibilità di far comunicare due macchine
            //Socket ha bisogno di due endpoint

            socket.Bind(local_endpoint);    //consente di unire la socket al endpoint locale, posso iniziare a trasmettere qualcosa

            dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(aggiornamento_dTimer);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250); // Si parla di 250 millisecondi
            dTimer.Start(); //avvia il timer

            InitializeComponent();
        }

        private void BtnInvia_Click(object sender, RoutedEventArgs e)
        {
            IPAddress remote_address = IPAddress.Parse(txtIndirizzo.Text);     //Prendo l'ip da wpf
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, int.Parse(txtPorta.Text)); //Prendo la porta da wpf
            Byte[] Messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);    //Trasformo il messaggio in Byte
            socket.SendTo(Messaggio, remote_endpoint);
            //Metodo per mandare il messaggio

        }

        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int nBytes = 0;  //se maggiore di zero posso leggere qualcosa, sennò esce dal metodo
            if ((nBytes = socket.Available) > 0)
            {
                //Ricezione dei caratteri in Attesa
                byte[] buffer = new byte[nBytes];
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);
                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();

                string message = Encoding.UTF8.GetString(buffer, 0, nBytes);
                lst.Items.Add(
                    from + ": " + message
                    );
            }

        }
    }
}
