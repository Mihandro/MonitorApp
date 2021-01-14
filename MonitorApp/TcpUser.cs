using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MonitorApp
{
    class TcpUser
    {
        public void Start()
        {
            // адрес и порт сервера, к которому будем подключаться
            int port = 8005; // порт сервера
            string address = "127.0.0.1"; // адрес сервера
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                while (true)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // подключаемся к удаленному хосту
                    socket.Connect(ipPoint);

                
                
                    //Console.Write("Введите сообщение:");
                    //string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    socket.Send(data);

                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    //Console.WriteLine("ответ сервера: " + builder.ToString());
                    Thread.Sleep(1000);
                    // закрываем сокет
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();

        }
    }
}
