using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
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

    //Server
    class UdpListener : UdpBase
    {
        private IPEndPoint _listenOn;

        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 32123))
        {
        }

        public UdpListener(IPEndPoint endpoint)
        {
            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }

        public void Reply(string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);
        }

    }

    //Client
    class UdpUser : UdpBase
    {
        private UdpUser() { }

        public static UdpUser ConnectTo(string hostname, int port)
        {
            var connection = new UdpUser();
            connection.Client.Connect(hostname, port);
            return connection;
        }

        public void Send(string message)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            //create a new server
            var server = new UdpListener();

            //start listening for messages and copy the messages back to the client
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var received = await server.Receive();
                    Console.WriteLine(received.Message);
                    if (received.Message == "quit")
                        break;
                }
            });

            //create a new client
            Console.ReadLine();
        }
    }
}

