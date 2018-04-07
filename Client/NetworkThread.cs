using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkThread : MonoBehaviour {

	private byte[] recvData = new byte[4096];
	private string address = "127.0.0.1";
	private int port = 1921;
	private Socket clientSocket;
	public Login login; // assigned in editor
	public Game game; // assigned in editor

	void Start () {
		IPAddress ip = IPAddress.Parse(address);
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		clientSocket.Connect(new IPEndPoint(ip, port));
		Thread recv_thread = new Thread(new ThreadStart(Receive));
		recv_thread.Name = "Receiver";
		recv_thread.Start ();
	}

	// only called by Login.cs
	public void Send(string sendData) {
		clientSocket.Send (Encoding.ASCII.GetBytes (sendData));
	}

	void Update () {
		// TODO: Get Game Status From Game.cs And Send To Server
	}

	void Receive() {
		while (true) {
			int recvLen = clientSocket.Receive (recvData);
			if (recvLen == 4) { // login or register result
				String decode = Encoding.ASCII.GetString (recvData);
				login.GetResultFromServer (decode);
			} else { // game state from server
				game.AppendGameStatusFromServer(recvData);
			}
		}
	}

	void OnApplicationQuit() {
		clientSocket.Close ();
	}
}
