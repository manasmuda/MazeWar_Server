using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public enum MessageType
{
    Connect,
    Disconnect,
    Ready,
    Reject,
    PlayerAccepted,
    PlayerLeft,
    GameStarted,
    GameReady,
};


// We will use the same message for all requests so it will include a type and optional float values (used for position and orientation)
[Serializable]
public class SimpleMessage
{
    public SimpleMessage(MessageType type, string message = "")
    {
        this.messageType = type;
        this.message = message;
        this.listData = new List<object> { };
        this.dictData = new Dictionary<string, object> { };
        this.listdictdata = new List<Dictionary<string, object>> { };
    }

    public MessageType messageType { get; set; }
    public string message { get; set; }
    public int clientId { get; set; }
    public int time { get; set; }
    public string playerId { get; set; }
    public List<object> listData { get; set; }
    public Dictionary<string, object> dictData { get; set; }
    public List<Dictionary<string, object>> listdictdata { get; set; }
    public byte[] imageData { get; set; }
    public string playerName { get; set; }
    public int iw { get; set; }
    public int ih { get; set; }

    // As we are using one generic message for simplicity, we always have all possible data here
    // You would likely want to use different classes for different message types

}