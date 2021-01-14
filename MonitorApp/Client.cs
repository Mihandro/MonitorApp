using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorApp
{
    public struct Received
    {
        public IPEndPoint Sender;
        public string Message;
    }

    abstract class UdpBase
    {
        protected UdpClient Client;

        protected UdpBase()
        {
            Client = new UdpClient();
        }

        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            return new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }
    }

    class Client : UdpBase
    {
        private Client() { }

        public static Client ConnectTo(string hostname, int port)
        {
            var connection = new Client();
            connection.Client.Connect(hostname, port);
            return connection;
        }

        public void Send(string message)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
        }

    }

    public class Logger
    {
        public Logger()
        {

        }

        public void Start()
        {
            var client = Client.ConnectTo("127.0.0.1", 32123);

            //wait for reply messages from server and send them to console 


            while (true)
            {
                if(Monitoring.GetIdleTime() > 5000)
                client.Send(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                Thread.Sleep(1000);
            };
        }
    }

}
