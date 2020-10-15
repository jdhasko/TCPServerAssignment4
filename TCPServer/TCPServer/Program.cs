using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Provide IP address: ");
            string ipInput = Console.ReadLine();
            IPAddress ip = IPAddress.Parse(ipInput);
            TcpListener serverSocket = new TcpListener(ip,4646);

            serverSocket.Start();
            Console.WriteLine("The server is up and running!");

            while (true)
            {

                TcpClient clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("New client connected: " + clientSocket.Client.RemoteEndPoint.ToString());
                TcpService service = new TcpService(clientSocket);

                Thread thread = new Thread(service.Start);
                thread.Start();

            }
            serverSocket.Stop();
        }
    }
}
