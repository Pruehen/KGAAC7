using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;

public class NATTraversal : MonoBehaviour
{
    public string matchmakerIP = "15.165.18.111";
    public int matchmakerPort = 5000;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private int listenPort = 7777;

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(matchmakerIP), matchmakerPort);

        StartCoroutine(RegisterWithMatchmaker());
    }

    IEnumerator RegisterWithMatchmaker()
    {
        yield return new WaitForSeconds(1);

        byte[] data = new byte[] { };
        udpClient.Send(data, data.Length, remoteEndPoint);

        StartCoroutine(WaitForMatchInfo());
    }

    IEnumerator WaitForMatchInfo()
    {
        while (true)
        {
            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log(message);
            }
            yield return null;
        }
    }

    void HandleMatchInfo(string message)
    {
        string[] parts = message.Split(':');
        string player1IP = parts[0];
        int player1Port = int.Parse(parts[1]);
        string player2IP = parts[2];
        int player2Port = int.Parse(parts[3]);

        if (remoteEndPoint.Address.ToString() == player1IP && listenPort == player1Port)
        {
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(player2IP), player2Port);
        }
        else
        {
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(player1IP), player1Port);
        }

        StartCoroutine(HolePunch());
    }

    IEnumerator HolePunch()
    {
        while (true)
        {
            byte[] data = Encoding.UTF8.GetBytes("hello");
            udpClient.Send(data, data.Length, remoteEndPoint);

            yield return new WaitForSeconds(1);
        }
    }
}