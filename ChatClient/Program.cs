using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatClient
{
    static string userName;
    static TcpClient client;
    static NetworkStream stream;

    static void Main()
    {
        Console.Write("Введите имя: ");
        userName = Console.ReadLine();

        client = new TcpClient();
        try
        {
            client.Connect("127.0.0.1", 8888);
            stream = client.GetStream();

            byte[] nameData = Encoding.Unicode.GetBytes(userName);
            stream.Write(nameData, 0, nameData.Length);

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
            SendMessage();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }

    static void SendMessage()
    {
        while (true)
        {
            Console.Write("Введите сообщение: ");
            string message = Console.ReadLine();
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }

    static void ReceiveMessage()
    {
        while (true)
        {
            try
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);

                Console.WriteLine(builder.ToString());
            }
            catch
            {
                Console.WriteLine("Соединение прервано!");
                Disconnect();
                break;
            }
        }
    }

    static void Disconnect()
    {
        stream?.Close();
        client?.Close();
        Environment.Exit(0);
    }
}