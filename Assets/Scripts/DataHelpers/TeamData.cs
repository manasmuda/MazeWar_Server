using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

public class TeamData
{
    private static readonly System.Random _random = new System.Random();

    public string team;
    public Dictionary<string, ClientData> clientsData;

    public int storedCoins;

    public TeamData(string team)
    {
        this.team = team;
        this.storedCoins = 0;
        clientsData = new Dictionary<string, ClientData> { };
    }


    public ClientData AddNewClient(TcpClient tcpClient)
    {
        string newId = RandomString(10);
        //Debug.Log("Adding new cliernt");
        //string newId = "123";
        ClientData clientData = new ClientData(newId,tcpClient);
        clientData.team = team;
        clientsData.Add(newId, clientData);
        return clientData;
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
