// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License.
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour {
	private TcpClient socketConnection;
	private Thread clientReceiveThread;

	[SerializeReference] private TMP_InputField hostIPText;

	private String hostIP;

	private bool pendingConnection = false;
	// Use this for initialization
	public void TryConnect()
	{
		hostIP = hostIPText.text;
		ConnectToTcpServer();
	}

	private void Update()
	{
		if (!pendingConnection) return;
		pendingConnection = false;
		DontDestroyOnLoad(this);
		SceneManager.LoadScene("Scenes/ClientGame", LoadSceneMode.Single);

	}

	/// <summary>
	/// Setup socket connection.
	/// </summary>
	private void ConnectToTcpServer() {
		try {
			clientReceiveThread?.Abort();
			clientReceiveThread = new Thread(ListenForData)
			{
				IsBackground = true
			};
			clientReceiveThread.Start();
		}
		catch (Exception e) {
			Debug.Log("On client connect exception " + e);
			clientReceiveThread?.Abort();
		}
	}
	/// <summary>
	/// Runs in background clientReceiveThread; Listens for incomming data.
	/// </summary>
	private void ListenForData() {
		try {
			// socketConnection = new TcpClient(IPAddress.Parse(hostIP), 8052);
			socketConnection = new TcpClient();
			socketConnection.Connect(IPAddress.Parse(hostIP), 8052);
			SendMessage(new Message(Message.MessageType.ConnectionAck));
			byte[] bytes = new byte[1024];
			while (true)
			{
				// Get a stream object for reading
				using NetworkStream stream = socketConnection.GetStream();
				int length;
				// Read incoming stream into byte array.
				while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
					var incomingData = new byte[length];
					Array.Copy(bytes, 0, incomingData, 0, length);
					// Convert byte array to string message.
					Message serverMessage = Message.Deserialise(incomingData);

					if (serverMessage.type == Message.MessageType.ConnectionAck)
					{
						pendingConnection = true;
					}

					Debug.Log("server message received as: " + serverMessage.data);
				}
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
	/// <summary>
	/// Send message to server using socket connection.
	/// </summary>
	private void SendMessage(Message m) {
		if (socketConnection == null) {
			return;
		}
		try {
			// Get a stream object for writing.
			NetworkStream stream = socketConnection.GetStream();
			if (!stream.CanWrite) return;
			// Convert string message to byte array.
			byte[] clientMessageAsByteArray = m.Serialise();
			// Write byte array to socketConnection stream
			stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
			Debug.Log("Client sent his message - should be received by server");
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
}