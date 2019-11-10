using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace tcplistener
{
    class Program
    {
        static void Main(string[] args) {
            Listener listener = new Listener();
            listener.Start();
            listener.ListenForIncommingRequests();
            listener.SendMessageToClient();
        }
    }
}