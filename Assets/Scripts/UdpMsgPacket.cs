using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public enum PacketType
{
    Spawn
};

[Serializable]
public class UdpMsgPacket 
{
   public UdpMsgPacket(PacketType type,string message)
    {
        this.type = type;
        this.message = message;
    }

    public PacketType type { get; set; }
    public string message { get; set; }
}
