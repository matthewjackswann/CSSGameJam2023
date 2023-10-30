using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Message
{
    public String data = "This is my message";
    public MessageType type;
    public float y;
    public float movX;
    public float movY;
    public Disease d;
    public int red;
    public int blue;
    public int green;

    public Message(MessageType t)
    {
        type = t;
    }

    public Message(float y, Vector2 movement, Disease d)
    {
        type = MessageType.Teleport;
        this.y = y;
        movX = movement.x;
        movY = movement.y;
        this.d = d;
    }

    public Message(MessageType t, Disease disease)
    {
        type = t;
        d = disease;

    }

    public Message(int r, int b, int g)
    {
        type = MessageType.Infection;
        red = r;
        blue = b;
        green = g;
    }

    public Message()
    {
        type = MessageType.Message;
    }

    public byte[] Serialise()
    {
        var bf = new BinaryFormatter();
        using var ms = new MemoryStream();
        bf.Serialize(ms, this);
        return ms.ToArray();
    }

    public static Message Deserialise(byte[] byteArray)
    {
        using var memStream = new MemoryStream();
        var binForm = new BinaryFormatter();
        memStream.Write(byteArray, 0, byteArray.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        var obj = (Message)binForm.Deserialize(memStream);
        return obj;
    }

    public enum MessageType
    {
        ConnectionAck,
        Message,
        Teleport,
        Infection,
        IncrementInfection,
        IncrementResistance,
        IncrementSpeed,
        IncrementSize
    }
}