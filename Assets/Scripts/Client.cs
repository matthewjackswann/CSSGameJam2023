// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License.
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour {
	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	// Use this for initialization
	void Start () {
		ConnectToTcpServer();
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			SendMessage();
		}
	}
	/// <summary>
	/// Setup socket connection.
	/// </summary>
	private void ConnectToTcpServer() {
		try {
			clientReceiveThread = new Thread(ListenForData)
			{
				IsBackground = true
			};
			clientReceiveThread.Start();
		}
		catch (Exception e) {
			Debug.Log("On client connect exception " + e);
		}
	}
	/// <summary>
	/// Runs in background clientReceiveThread; Listens for incomming data.
	/// </summary>
	private void ListenForData() {
		try {
			socketConnection = new TcpClient("localhost", 8052);
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
	private void SendMessage() {
		if (socketConnection == null) {
			return;
		}
		try {
			// Get a stream object for writing.
			NetworkStream stream = socketConnection.GetStream();
			if (!stream.CanWrite) return;
			// Convert string message to byte array.
			byte[] clientMessageAsByteArray = new Message().Serialise();
			// Write byte array to socketConnection stream
			stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
			Debug.Log("Client sent his message - should be received by server");
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
}