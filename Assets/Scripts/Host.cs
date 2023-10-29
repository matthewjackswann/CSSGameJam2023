﻿// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License.
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Host : MonoBehaviour {
	/// <summary>
	/// TCPListener to listen for incomming TCP connection
	/// requests.
	/// </summary>
	private TcpListener tcpListener;
	/// <summary>
	/// Background thread for TcpServer workload.
	/// </summary>
	private Thread tcpListenerThread;
	/// <summary>
	/// Create handle to connected tcp client.
	/// </summary>
	private TcpClient connectedTcpClient;

	// Use this for initialization
	void Start () {
		// Start TcpServer background thread
		tcpListenerThread = new Thread (ListenForIncommingRequests)
		{
			IsBackground = true
		};
		tcpListenerThread.Start();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			SendMessage();
		}
	}

	/// <summary>
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests
	/// </summary>
	private void ListenForIncommingRequests () {
		try {
			// Create listener on localhost port 8052.
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8052);
			tcpListener.Start();
			Debug.Log("Server is listening");
			byte[] bytes = new byte[1024];
			while (true) {
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) {
					// Get a stream object for reading
					using (NetworkStream stream = connectedTcpClient.GetStream()) {
						int length;
						// Read incoming stream into byte array.
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incomingData = new byte[length];
							Array.Copy(bytes, 0, incomingData, 0, length);
							// Convert byte array to string message.
							Message clientMessage = Message.Deserialise(incomingData);
							Debug.Log("client message received as: " + clientMessage.data);
						}
					}
				}
			}
		}
		catch (SocketException socketException) {
			Debug.Log("SocketException " + socketException);
		}
	}
	/// <summary>
	/// Send message to client using socket connection.
	/// </summary>
	private void SendMessage() {
		if (connectedTcpClient == null) {
			return;
		}

		try {
			// Get a stream object for writing.
			NetworkStream stream = connectedTcpClient.GetStream();
			if (!stream.CanWrite) return;
			// Convert string message to byte array.
			byte[] serverMessage = new Message().Serialise();
			// Write byte array to socketConnection stream.
			stream.Write(serverMessage, 0, serverMessage.Length);
			Debug.Log("Server sent his message - should be received by client");
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
}