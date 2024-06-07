using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class MatchManager : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    private List<string> _roomList = new List<string>();

    private UdpClient udpClient;
    private IPEndPoint serverEndpoint;
    private IPEndPoint peerEndpoint;
    private bool isMatched = false;

    void Start()
    {
        udpClient = new UdpClient();
        serverEndpoint = new IPEndPoint(IPAddress.Parse("3.35.235.2"), 5000); // Matchmaking server IP and port
    }

    public void Host()
    {
        string roomId = RandomRoomId();

        string message = $"HOST:{roomId}:{GetLocalIp()}";
        byte[] data = Encoding.ASCII.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndpoint);
        networkManager.StartHost();
    }
    private string GetLocalIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;
    }
    public void Join(TMPro.TMP_InputField roomId)
    {
        RequestMatch(roomId.text);
    }

    private string RandomRoomId()
    {
        string roomId = "";
        for (int i = 0; i < 4; i++)
        {
            roomId += (char)('A' + Random.Range(0, 25));
        }
        return roomId;
    }
    private void RequestMatch(string roomId)
    {
        string message = $"MATCH:{roomId}:{roomId}";
        byte[] data = Encoding.ASCII.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndpoint);
        reqsting = true;

    }
    private void RequestRoomList()
    {
        string message = $"LIST";
        byte[] data = Encoding.ASCII.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndpoint);
        reqsting = true;

    }
    private bool reqsting=false;
    private IPEndPoint ipe = null;
    float time = 0f;
    private void Update()
    {
        if (reqsting)
        {
            if (time > 3f)
                reqsting=false;
            time += Time.unscaledDeltaTime;
            if (udpClient.Available > 0)
            {
                IPEndPoint remoteEndpoint = null;
                byte[] receivedBytes = udpClient.Receive(ref remoteEndpoint);
                string receivedMessage = Encoding.ASCII.GetString(receivedBytes);
                Debug.Log(receivedMessage);
                ipe = HandleServerMessage(receivedMessage);
                if(ipe != null)
                {
                    networkManager.networkAddress = ipe.Address.ToString();
                    networkManager.StartClient();
                    reqsting = false;
                }
            }
        }
        else
        {
            time = 0f;
        }
    }

    private IPEndPoint HandleServerMessage(string message)
    {
        if (message.StartsWith("PEER"))
        {
            string[] peerInfo = message.Split(':');
            IPAddress peerAddress = IPAddress.Parse(peerInfo[1]);
            int peerPort = int.Parse(peerInfo[2]);

            peerEndpoint = new IPEndPoint(peerAddress, peerPort);
            isMatched = true;

            Debug.Log($"Matched with peer at {peerEndpoint}");
            return peerEndpoint;
        }
        else if(message.StartsWith("LIST"))
        {
            string[] strings = message.Split(":")[1].Split(',');
            foreach (string s in strings)
            {
                if(string.IsNullOrWhiteSpace(s)) continue;
                _roomList.Add(s);
            }
        }
        return null;
    }

    public IEnumerator GetRoomList(System.Action<List<string>> callback )
    {
        RequestRoomList();
        while(reqsting == true)
        {
            yield return null;
        }
        callback?.Invoke( _roomList );
        yield break;
    }
    //private void OnReceive(System.IAsyncResult result)
    //{
    //    try
    //    {
    //        UdpClient udpClient = (UdpClient)result.AsyncState;
    //        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    //        byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEndPoint);
    //        string receivedMessage = Encoding.ASCII.GetString(receivedBytes);
    //        Debug.Log("Received: " + receivedMessage);

    //        if (receivedMessage.StartsWith("PEER"))
    //        {
    //            string[] peerInfo = receivedMessage.Split(':');
    //            string peerAddress = peerInfo[1];
    //            int peerPort = int.Parse(peerInfo[2]);

    //            IPEndPoint peerEndPoint = new IPEndPoint(IPAddress.Parse(peerAddress), peerPort);
    //            Debug.Log($"Peer endpoint:{peerEndPoint.Address}:{peerEndPoint.Port}");
    //            // Now you can use the peerEndPoint for hole punching
    //        }

    //        udpClient.BeginReceive(new System.AsyncCallback(OnReceive), udpClient);
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError(e.ToString());
    //    }
    //}

}
