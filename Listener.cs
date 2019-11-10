using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Listener 
{
    private List<TcpClient> listConnectedClients = new List<TcpClient>(new TcpClient[0]);
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    private string clientMessage;


    public void ListenForIncommingRequests()
    {
        //tcpListener = new TcpListener(IPAddress.Any, 8052);
        Int32 port = 9090;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        tcpListener = new TcpListener(localAddr, port);
        tcpListener.Start();

        ThreadPool.QueueUserWorkItem(this.ListenerWorker, null);
    }


    private void ListenerWorker(object token)
    {
        while (tcpListener != null)
        {
            connectedTcpClient = tcpListener.AcceptTcpClient();
            Console.WriteLine("Hello I'm listening ...");

            listConnectedClients.Add(connectedTcpClient);
            
            ThreadPool.QueueUserWorkItem(this.HandleClientWorker, connectedTcpClient);
        }
    }


    private void HandleClientWorker(object token)
    {
        Byte[] bytes = new Byte[1024];
        using (var client = token as TcpClient)
        using (var stream = client.GetStream())
        {
            Console.WriteLine("New client connected");
            int length;

            // Read incomming stream into byte arrary.                      
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incommingData = new byte[length];
                Array.Copy(bytes, 0, incommingData, 0, length);

                // Convert byte array to string message.                          
                clientMessage = Encoding.ASCII.GetString(incommingData);
                Console.WriteLine("Message from client is: " + clientMessage);
            }

            if (connectedTcpClient == null)
            {
                return;
            }

            //remove from list of connected client the item that sent a message
            listConnectedClients.Remove(client);
        }
    }


    private void SendMessage(object token, string msg)
    {
        if (connectedTcpClient == null)
        {
            Console.WriteLine("Problem: connectedTCPClient null");
            return;
        }
        var client = token as TcpClient;
        {
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    // Get a stream object for writing.    
                    // Convert string message to byte array.              
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(msg);

                    // Write byte array to socketConnection stream.            
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Console.WriteLine("Server sent his message - should be received by client");
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
                return;
            }
        }
    }


    public void Start()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();

    }


    // Update is called once per frame
    void Update()
    {}


    public void SendMessageToClient()
    {
        if (listConnectedClients != null)
        {
            Console.WriteLine("Length " + listConnectedClients.Count);
        }

        foreach (TcpClient item in listConnectedClients)
        {
            SendMessage(item, "Hello, I'll send you back this message: " + clientMessage);
        }
    }
}