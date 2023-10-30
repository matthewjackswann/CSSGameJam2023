// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License.
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using System;
using System.Collections.Generic;
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

	[SerializeField] public int red = 1;
	[SerializeField] public int blue = 1;
	[SerializeField] public int green = 1000;

	[SerializeField] private int money = 0;
	[SerializeReference] public SkillTreeRunner skilltree;


	private String hostIP;

	private bool pendingConnection = false;
	[SerializeField] private GameObject person;
	private readonly List<Message> toSpawn = new();

	public void Start()
	{
		skilltree = FindObjectOfType<SkillTreeRunner>();
	}

	public void Infection(int r, int g, int b)
	{
		if (b > 0)
		{
			money += b;
		}
		red += r;
		green += g;
		blue += b;
		SendMessage(new Message(r, b, g));
	}

	public void IncrementInfectionProbability(Disease incomingDisease)
	{
		//skilltree.IncrementInfectionProbability(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementInfection, incomingDisease));
	}

	public void IncrementResistanceProbability(Disease incomingDisease)
	{
		//skilltree.IncrementResistanceProbability(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementResistance, incomingDisease));
	}

	public void IncreaseSize(Disease incomingDisease)
	{
		//skilltree.IncreaseSize(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementSize, incomingDisease));
	}

	public void IncreaseSpeed(Disease incomingDisease)
	{
		//skilltree.IncreaseSpeed(incomingDisease);
		SendMessage(new Message(Message.MessageType.IncrementSpeed, incomingDisease));
	}


	// Use this for initialization
	public void TryConnect()
	{
		hostIP = hostIPText.text;
		ConnectToTcpServer();
	}

	private void Update()
	{
		if (pendingConnection)
		{
			pendingConnection = false;
			DontDestroyOnLoad(this);
			SceneManager.LoadScene("Scenes/ClientGame", LoadSceneMode.Single);
		}

		lock (toSpawn)
		{
			if (toSpawn.Count > 0)
			{
				foreach (Message p in toSpawn)
				{
					GameObject spawned = Instantiate(person);
					Contaminate c = spawned.GetComponent<Contaminate>();
					c.disease = p.d;
					c.movement = new Vector2(p.movX, p.movY);
					spawned.transform.position = new Vector3(-49, p.y, -1);
				}
				toSpawn.Clear();
			}
		}
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
			socketConnection = new TcpClient();
			socketConnection.Connect(IPAddress.Parse(hostIP), 8053);
			SendMessage(new Message(Message.MessageType.ConnectionAck));
			byte[] bytes = new byte[2048];
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
					try
					{
						Message serverMessage = Message.Deserialise(incomingData);
						switch (serverMessage.type)
						{
							case Message.MessageType.ConnectionAck:
								pendingConnection = true;
								break;
							case Message.MessageType.Teleport:
								lock (toSpawn)
								{
									toSpawn.Add(serverMessage);
								}
								break;
							case Message.MessageType.Infection:
								if (serverMessage.blue > 0)
								{
									money += serverMessage.blue;
								}
								red += serverMessage.red;
								blue += serverMessage.blue;
								green += serverMessage.green;
								break;
							case Message.MessageType.IncrementInfection:
								skilltree.IncrementInfectionProbability2(serverMessage.d);
								break;
							case Message.MessageType.IncrementResistance:
								skilltree.IncrementResistanceProbability2(serverMessage.d);
								break;
							case Message.MessageType.IncrementSize:
								skilltree.IncreaseSize2(serverMessage.d);
								break;
							case Message.MessageType.IncrementSpeed:
								skilltree.IncreaseSpeed2(serverMessage.d);
								break;
						}
					}
					catch (Exception e)
					{
						Debug.Log(e);
					}
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
	public void SendMessage(Message m) {
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