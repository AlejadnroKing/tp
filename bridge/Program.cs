using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpListener server1;
    static int entra = 8030;
    static int sale = 3300;
    static string ipsale = "190.195.130.24";
    static string entrada = ""; 

    static void Main(string[] args)
    {

        Console.WriteLine("IP default destino: " + ipsale + " entre la Ip destino ");
        entrada = Console.ReadLine();
        if (entrada != "")
        {
            ipsale = entrada;
            Console.WriteLine("Nueva IP destino: " + ipsale );
        }
        Console.WriteLine("Puerto default destino: " + sale + " entre el puerto destino ");
        entrada = Console.ReadLine();
        if (entrada != "")
        {
            sale = Int32.Parse(entrada);
            Console.WriteLine("Nuevo puerto destino: " + sale);

        }
        Console.WriteLine("Puerto default escucha: " + entra + " entre puerto escucha ");
        entrada = Console.ReadLine();
        if (entrada != "")
        {
            entra = Int32.Parse(entrada);
            Console.WriteLine("Nuevo puerto entrada: " + entra);
        }

        Console.WriteLine("puerto entrada: " + entra.ToString());
        Console.WriteLine("puerto salida:  " + sale.ToString());
        Console.WriteLine("IP salida:      " + ipsale);



        // Configurar el servidor en el primer puerto

        server1 = new TcpListener(IPAddress.Any, entra);
        server1.Start();

        // Iniciar un hilo para manejar conexiones en el primer puerto
        Thread serverThread1 = new Thread(ServerThread1);
        serverThread1.Start();

        Console.ReadLine(); // Mantener la aplicación abierta
    }

    static void ServerThread1()
    {
        while (true)
        {
            TcpClient client1 = server1.AcceptTcpClient();
            TcpClient client2 = new TcpClient();

            // Conectar el segundo cliente al segundo servidor (otra dirección IP)
            try
            {
              // Cambia esto por la dirección IP del servidor 2
                client2.Connect(ipsale, sale); // Conectar al puerto 54321 del servidor 2
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar al servidor 2: " + ex.Message);
                continue;
            }

            // Iniciar hilos para la comunicación bidireccional entre los clientes
            Thread clientThread1 = new Thread(() => HandleClient(client1, client2, "Rcvr: "));
            clientThread1.Start();

            Thread clientThread2 = new Thread(() => HandleClient(client2, client1, "Srv: "));
            clientThread2.Start();
        }
    }
    static int[] GetAsciiValues(string input)
    {
        int[] asciiValues = new int[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            asciiValues[i] = (int)input[i];
        }
        return asciiValues;
    }

    static void HandleClient(TcpClient sourceClient, TcpClient targetClient, string direction)
    {
        try
        {
            using (NetworkStream sourceStream = sourceClient.GetStream())
            using (NetworkStream targetStream = targetClient.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    targetStream.Write(buffer, 0, bytesRead);
                    string m = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"{direction}: Datos enviados: {m} : Valores ASCII: {string.Join(", ", GetAsciiValues(m))}  ");
                    
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            sourceClient.Close();
            targetClient.Close();
        }
    

    }
}