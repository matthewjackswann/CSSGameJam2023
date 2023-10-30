// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License.
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	private string myIP = "error";
	private bool connectionPending = false;

	[SerializeReference] private TextMeshProUGUI ipDisplay;

	[SerializeField] private GameObject person;
	private readonly List<Message> toSpawn = new();

	[SerializeField] public int red = 1;
	[SerializeField] public int blue = 1;
	[SerializeField] public int green = 100;
	
	
	[SerializeField] private int money = 0;
	[SerializeReference] private SkillTreeRunner skilltree;

	
	
	public void Infection(int r, int g, int b)
	{
		if (r > 0)
		{
			money += r;
		}
		red += r;
		green += g;
		blue += b;
		SendMessage(new Message(r, b, g));
	}
	
	public void IncrementInfectionProbability(Disease incomingDisease)
	{
		skilltree.IncrementInfectionProbability(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementInfection, incomingDisease));
	}

	public void IncrementResistanceProbability(Disease incomingDisease)
	{
		skilltree.IncrementResistanceProbability(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementResistance, incomingDisease));
	}

	public void IncreaseSize(Disease incomingDisease)
	{
		skilltree.IncreaseSize(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementSize, incomingDisease));
	}

	public void IncreaseSpeed(Disease incomingDisease)
	{
		skilltree.IncreaseSpeed(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementSpeed, incomingDisease));
	}
	

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		foreach (var ipAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
		{
			if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				myIP = ipAddress.ToString();
			}
		}
		ipDisplay.text = myIP;
		// Start TcpServer background thread
		tcpListenerThread?.Abort();
		tcpListenerThread = new Thread(ListenForIncommingRequests)
		{
			IsBackground = true
		};
		tcpListenerThread.Start();
		skilltree = FindObjectOfType<SkillTreeRunner>();
	}

	private void Update()
	{
		if (connectionPending)
		{
			connectionPending = false;
			SendMessage(new Message(Message.MessageType.ConnectionAck));
			SceneManager.LoadScene("Scenes/HostGame", LoadSceneMode.Single);
		}

		if (toSpawn.Count > 0)
		{
			Debug.Log("To spawn: " + toSpawn.Count);
			foreach (Message p in toSpawn)
			{
				GameObject spawned = Instantiate(person);
				Contaminate c = spawned.GetComponent<Contaminate>();
				c.disease = p.d;
				c.movement = new Vector2(p.movX, p.movY);
				spawned.transform.position = new Vector3(49, p.y, -1);
			}
			toSpawn.Clear();
		}
	}

	/// <summary>
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests
	/// </summary>
	private void ListenForIncommingRequests () {
		try {
			// Create listener on localhost port 8052.
			tcpListener = new TcpListener(IPAddress.Any, 8053);
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
							switch (clientMessage.type)
							{
								case Message.MessageType.ConnectionAck:
									connectionPending = true;
									break;
								case Message.MessageType.Teleport:
									toSpawn.Add(clientMessage);
									Debug.Log("New person");
									break;
								case Message.MessageType.Infection:
									if (clientMessage.red > 0)
									{
										money += red;
									}
									red += clientMessage.red;
									blue += clientMessage.blue;
									green += clientMessage.green;
									break;
								case Message.MessageType.IncrementInfection:
									skilltree.IncrementInfectionProbability(clientMessage.d);
									break;
								case Message.MessageType.IncrementResistance:
									skilltree.IncrementResistanceProbability(clientMessage.d);
									break;
								case Message.MessageType.IncrementSize:
									skilltree.IncreaseSize(clientMessage.d);
									break;
								case Message.MessageType.IncrementSpeed:
									skilltree.IncreaseSpeed(clientMessage.d);
									break;
							}
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
	public void SendMessage(Message m) {
		if (connectedTcpClient == null) {
			return;
		}

		try {
			// Get a stream object for writing.
			NetworkStream stream = connectedTcpClient.GetStream();
			if (!stream.CanWrite) return;
			// Convert string message to byte array.
			byte[] serverMessage = m.Serialise();
			// Write byte array to socketConnection stream.
			stream.Write(serverMessage, 0, serverMessage.Length);
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
}