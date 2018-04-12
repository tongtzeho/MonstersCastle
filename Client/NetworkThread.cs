using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkThread : MonoBehaviour {

	private byte[] sendHead = new byte[4];
	private byte[] recvData = new byte[4096];
	private string address = "127.0.0.1";
	private int port = 9121;
	private Socket clientSocket;
	public Login login; // assigned in editor
	public Game game; // assigned in editor

	private class Message {
		public int length;
		public int count; // current count
		public byte[] content;
		public Message(int len) {
			length = len;
			count = 0;
			content = new byte[len];
		}
		public void Append(byte[] newContent, int beginIndex) {
			for (int i = beginIndex; i < newContent.Length; ++i) {
				if (count + i - beginIndex >= length) {
					count = length;
					return;
				}
				content [count + i - beginIndex] = newContent [i];
			}
			count += newContent.Length - beginIndex;
		}
	}

	void Start () {
		IPAddress ip = IPAddress.Parse(address);
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		clientSocket.Connect(new IPEndPoint(ip, port));
		Thread recvThread = new Thread(new ThreadStart(Receive));
		recvThread.Name = "Receiver";
		recvThread.IsBackground = true;
		recvThread.Start ();
	}

	void EncodeAndSend(byte[] sendData) {
		sendHead [0] = 0xed; // encode, same as msg.py
		sendHead [1] = 0xcb;
		sendHead [2] = Convert.ToByte (sendData.Length / 256);
		sendHead [3] = Convert.ToByte (sendData.Length % 256);
		clientSocket.Send (sendHead);
		clientSocket.Send (sendData);
	}

	// only called by Login.cs
	public void SendString(string sendString) {
		byte[] sendData = Encoding.ASCII.GetBytes (sendString);
		EncodeAndSend (sendData);
	}

	void Update () {
		if (game.IsStart () && game.IsInitialized ()) {
			byte[] sendData = game.GetCurrentGameStatus ();
			EncodeAndSend (sendData);
		}
	}

	void ProcessMessage(Message msg) {
		if (msg.length == 4) { // login or register result
			String decodeString = Encoding.ASCII.GetString (msg.content);
			login.GetResultFromServer (decodeString);
		} else { // game state from server
			if (game.IsStart ()) {
				game.AppendGameStatusFromServer(msg.content);
			}
		}
	}

	void Receive() {
		List<Message> messageQueue = new List<Message>();
		List<byte> tail = new List<byte> ();
		while (true) {
			int recvLen = clientSocket.Receive (recvData);

			// same as msg.py enqueue
			int pos = 0;
			while (pos < recvLen) {
				if (messageQueue.Count == 0 || messageQueue [messageQueue.Count - 1].length == messageQueue [messageQueue.Count - 1].count) {
					for (int i = 0; i < recvLen - pos; ++i) {
						tail.Add (recvData [i + pos]);
					}
					if (tail.Count >= 4 && tail [0] == '\xed' && tail [1] == '\xcb') {
						int length = Convert.ToInt32 (tail [2]) * 256 + Convert.ToInt32 (tail [3]);
						messageQueue.Add (new Message (length));
						messageQueue [messageQueue.Count - 1].Append (tail.ToArray (), 4);
						pos += 4 + length;
						tail.Clear ();
					} else {
						if (!(tail.Count == 0 || (tail.Count == 1 && tail [0] == '\xed') || (tail.Count >= 2 && tail.Count <= 4 && tail [0] == '\xed' && tail [1] == '\xcb'))) {
							tail.Clear ();
						}
						break;
					}
				} else {
					int addLen = messageQueue [messageQueue.Count - 1].length - messageQueue [messageQueue.Count - 1].count;
					messageQueue [messageQueue.Count - 1].Append (recvData, 0);
					pos += addLen;
					tail.Clear ();
				}
			}

			for (int i = 0; i < messageQueue.Count; ++i) {
				if (messageQueue [i].length == messageQueue [i].count) {
					ProcessMessage (messageQueue [i]);
				} else {
					break;
				}
			}

			if (messageQueue.Count > 0) {
				if (messageQueue [messageQueue.Count - 1].length == messageQueue [messageQueue.Count - 1].count) {
					messageQueue.Clear ();
				} else {
					Message lastMsg = messageQueue [messageQueue.Count - 1];
					messageQueue.Clear ();
					messageQueue.Add (lastMsg);
				}
			}
		}
	}

	void OnApplicationQuit() {
		clientSocket.Close ();
	}
}
