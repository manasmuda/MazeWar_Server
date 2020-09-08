using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class TeamData
{
    private static readonly System.Random _random = new System.Random();

    public string team;
    private Dictionary<IPEndPoint, string> endPointsPlayerId;
    private Dictionary<string, ClientData> clientsData;

    private int storedCoins;

    public TeamData(string team)
    {
        this.team = team;
        this.storedCoins = 0;
    }


    public string AddNewClient()
    {
        string newId = RandomString(10);
        ClientData clientData = new ClientData(newId);
        clientsData.Add(newId, clientData);
        return newId;
    }

    public void SetEndPoints(IPEndPoint endPoint,string id)
    {
        endPointsPlayerId.Add(endPoint, id);
    }

    public static string RandomString(int size)
    {
        var builder = new StringBuilder(size);

        char offset = 'A';
        const int lettersOffset = 26; // A...Z or a..z: length=26  

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return builder.ToString();
    }
}
