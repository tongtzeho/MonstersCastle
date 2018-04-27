using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncClient : MonoBehaviour {

	public Login login; // assigned in editor
	public Game game; // assigned in editor

	private byte[] recvData = new byte[8192];
	private string address = "127.0.0.1";
	private int port = 9121;
	private Socket clientSocket;
	private List<Message> messageQueue = new List<Message>();
	private List<byte> tail = new List<byte> ();

	void Awake () {
		IPAddress ip = IPAddress.Parse(address);
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		clientSocket.BeginConnect(ip, port, asyncResult => {
			clientSocket.EndConnect(asyncResult);
			AsyncReceive();
		}, null);
	}

	private void AsyncSend(byte[] sendData) {
		byte[] sendBuf = new byte[4 + sendData.Length];
		sendBuf [0] = 0xed; // encode, same as msg.py
		sendBuf [1] = 0xcb;
		sendBuf [2] = Convert.ToByte (sendData.Length / 256);
		sendBuf [3] = Convert.ToByte (sendData.Length % 256);
		sendData.CopyTo (sendBuf, 4);
		clientSocket.BeginSend (sendBuf, 0, sendBuf.Length, SocketFlags.None, asyncResult => {
			clientSocket.EndSend(asyncResult);
		}, null);
	}

	// only called by Login.cs
	public void SendString(string sendString) {
		byte[] sendData = Encoding.ASCII.GetBytes (sendString);
		AsyncSend (sendData);
	}

	// only called by Game.cs
	public void SendCommand(short gameResult, short command) {
		List<byte> result = new List<byte> ();
		result.AddRange (BitConverter.GetBytes (gameResult));
		result.AddRange (BitConverter.GetBytes (command));
		AsyncSend (result.ToArray ());
	}

	void Update () {
		if (game.IsStart () && game.IsInitialized () && !game.IsGameOver()) {
			byte[] sendData = game.GetCurrentGameStatus ();
			AsyncSend (sendData);
		}
	}

	void ProcessMessage(Message msg) {
		if (msg.length == 4) { // login or register result
			String decodeString = Encoding.ASCII.GetString (msg.content);
			login.GetResultFromServer (decodeString);
		} else { // game state from server
			if (game.IsStart () && !game.IsGameOver()) {
				game.AppendGameStatusFromServer(msg.content);
			}
		}
	}

	private void AsyncReceive() {
		clientSocket.BeginReceive (recvData, 0, recvData.Length, SocketFlags.None, asyncResult => {
			int recvLen = clientSocket.EndReceive(asyncResult);

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

			AsyncReceive();
		}, null);
	}

	void OnApplicationQuit() {
		clientSocket.Close ();
	}
}
