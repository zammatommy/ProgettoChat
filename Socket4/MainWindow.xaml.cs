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

    //Andremo a realizzare una semplice chat peer to peer(No versione client e server, devono solo mandare e ricevere)

    {
        Socket socket = null; //Socket= canale di comunicazione
        DispatcherTimer dTimer = null;
        public MainWindow()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //Abbiamo creato la classe socket
            IPAddress local_address = IPAddress.Any; //Ipaddres = Classe per creare il tuo indirizzo ip
            IPEndPoint local_endpoint = new IPEndPoint(local_address.MapToIPv4(), 65000 ); //perchè la cosa funzionerà se i firewall sono sbloccati, possibilità di far comunicare due macchine(65.000 porta mittente)
            //Socket ha bisogno di due endpoint

            socket.Bind(local_endpoint);    //consente di unire la socket al endpoint locale, posso iniziare a trasmettere qualcosa

            
            dTimer = new DispatcherTimer(); //per poterlo fare ce una problematica, c'e una programmazione concorrente,abbiamo cosi un thread che ascolta e uno che gestisce l'invio,dispatcherTimer = un thread ascolta costantemente, nel nostro caso abbiamo scelto 250 millisecondi, ma si può mettere il valore che si vuole a seconda dell'esigenze del mittente. 
            dTimer.Tick += new EventHandler(aggiornamento_dTimer); // Quando passa dTimer.Interval tempo, chiama il metodo aggiornamento_dTimer
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250); // Si parla di 250 millisecondi
            dTimer.Start(); //avvia il timer

            InitializeComponent();
        }

        private void BtnInvia_Click(object sender, RoutedEventArgs e) //Bottone di invio
        {
            IPAddress remote_address = IPAddress.Parse(txtIndirizzo.Text);     //Prendo l'ip da wpf, Destinatario
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, int.Parse(txtPorta.Text)); //Prendo la porta da wpf a scelta
            Byte[] Messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);    //Trasformo  il messaggio che ho in un array in Byte
            socket.SendTo(Messaggio, remote_endpoint);  //Metodo per inviare il messaggio


        }

        private void aggiornamento_dTimer(object sender, EventArgs e) //Metodo per ricevere il messaggio
        {
            int nBytes = 0;  //se maggiore di zero posso leggere qualcosa, sennò esce dal metodo.
            if ((nBytes = socket.Available) > 0) // il > 0 non è obbligatorio
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
