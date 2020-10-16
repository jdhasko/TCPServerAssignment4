using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using MyClassLibary;
using Newtonsoft.Json;

namespace TCPServer
{
    class TcpService
    {
        private TcpClient clientSocket;

        public TcpService(TcpClient newClientSocket)
        {
            this.clientSocket = newClientSocket;
        }

        private static List<Book> books = new List<Book>()
        {
            new Book("Witcher", "Andrzej Sapkowski", 489, "9780316029186"),
            new Book("Call Me By Your Name", "André Aciman", 256, "1298364542832"),
            new Book("The Bible", "God", 456, "1111111111111")
        };

        public void Start()
        {
            Stream stream = clientSocket.GetStream();
            StreamWriter streamWriter = new StreamWriter(stream) { AutoFlush = true };
            StreamReader streamReader = new StreamReader(stream);

            string clientMessage = streamReader.ReadLine();
            string response;

            while (clientMessage != "stop")
            {
                string requestType = clientMessage.Split(" ")[0];
                string requestContent = clientMessage.Split(" ")[1];

                switch (requestType)
                {
                    case "GET":
                        response = GetBook(requestContent);
                        break;
                    case "GETALL":
                        response = GetBooks();
                        break;
                    case "SAVE":
                        SaveBook(JsonConvert.DeserializeObject<Book>(requestContent));
                        response = "Successfully saved.";
                        break;
                    default:
                        response = "Unknown command.";
                        break;
                }
                streamWriter.WriteLine(response);
                Console.WriteLine( clientSocket.Client.RemoteEndPoint + " - " + requestType +" " + requestContent);
                Console.WriteLine(response);
                clientMessage = streamReader.ReadLine();
            }


            stream.Close();
            clientSocket.Close();
        }



        private string GetBook(string isbn13)
        {
            return JsonConvert.SerializeObject(books.Find(x => x.Isbn13 == isbn13));
        }

        private string GetBooks()
        {
            return JsonConvert.SerializeObject(books);
        }

        private void SaveBook(Book newBook)
        {
            books.Add(newBook);
        }
    }
}
