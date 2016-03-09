using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class NeetHelper
{
    public bool Ping(string ip, int port)
    {
        bool b = false;
        IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(point);
            b = true;
        }
        catch
        {
            b = false;
        }
        finally
        {
            socket.Close();
        }
        return b;
    }
}

