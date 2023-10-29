using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Message
{
    public String data = "This is my message";
    
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
}