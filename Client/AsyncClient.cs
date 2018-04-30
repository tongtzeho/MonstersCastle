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

	private byte[] sendData = new byte[64];
	private byte[] recvData = new byte[8192];
	private string address = "127.0.0.1";
	private int port = 9121;
	private Socket clientSocket;

	private Message[] messageQueue = new Message[512];
	private int queueBegin = 0;
	private int queueEnd = 0;

	private byte[] tail = new byte[2048];
	private int tailCurrLen = 0;

	void Awake () {
		sendData [0] = 0xed; // encode, same as msg.py
		sendData [1] = 0xcb;
		for (int i = 0; i < messageQueue.Length; ++i) {
			messageQueue [i] = new Message (2048);
		}
		IPAddress ip = IPAddress.Parse(address);
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		clientSocket.BeginConnect(ip, port, asyncResult => {
			clientSocket.EndConnect(asyncResult);
			AsyncReceive();
		}, null);
	}

	private void AsyncSend(byte[] sendBuf, int size) {
		clientSocket.BeginSend (sendBuf, 0, size, SocketFlags.None, asyncResult => {
			clientSocket.EndSend(asyncResult);
		}, null);
	}

	// only called by Login.cs
	public void SendString(string sendString) {
		byte[] stringBytes = Encoding.ASCII.GetBytes (sendString);
		sendData [2] = Convert.ToByte (stringBytes.Length >> 8);
		sendData [3] = Convert.ToByte (stringBytes.Length & 0xFF);
		for (int i = 0; i < stringBytes.Length && i + 4 < sendData.Length; ++i) {
			sendData [i + 4] = stringBytes [i];
		}
		AsyncSend (sendData, stringBytes.Length + 4);
	}

	// only called by Game.cs
	public void SendCommand(short gameResult, short command) {
		sendData [2] = 0;
		sendData [3] = 4;
		int offset = 4;
		Serializer.ToBytes (gameResult, sendData, ref offset);
		Serializer.ToBytes (command, sendData, ref offset);
		AsyncSend (sendData, 8);
	}

	void Update () {
		if (game.IsStart () && game.IsInitialized () && !game.IsGameOver()) {
			int dataSize;
			byte[] sendBuf = game.GetCurrentGameStatus (out dataSize);
			sendBuf [2] = Convert.ToByte ((dataSize - 4) >> 8);
			sendBuf [3] = Convert.ToByte ((dataSize - 4) & 0xFF);
			AsyncSend (sendBuf, dataSize);
		}
	}

	void ProcessMessage(int messageIndex) {
		if (messageQueue[messageIndex].length == 4) { // login or register result
			String decodeString = Encoding.ASCII.GetString (messageQueue[messageIndex].content);
			login.GetResultFromServer (decodeString);
		} else { // game state from server
			if (game.IsStart () && !game.IsGameOver()) {
				game.AppendGameStatusFromServer (messageIndex);
			}
		}
	}

	// only called by Game.cs
	public byte[] GetMessageContent(int index) {
		return messageQueue [index].content;
	}

	public int GetMessageCount() { // including incompleted message
		if (queueEnd >= queueBegin) {
			return queueEnd - queueBegin;
		} else {
			return queueEnd + messageQueue.Length - queueBegin;
		}
	}

	public int GetLastMessageIndex() { // including incompleted message
		if (queueEnd == 0) {
			return messageQueue.Length - 1;
		} else {
			return queueEnd - 1;
		}
	}

	private Message Enqueue(int length) {
		messageQueue [queueEnd].Reset (length);
		Message result = messageQueue [queueEnd];
		++queueEnd;
		if (queueEnd == messageQueue.Length) {
			queueEnd = 0;
		}
		return result;
	}

	private void AsyncReceive() {
		clientSocket.BeginReceive (recvData, 0, recvData.Length, SocketFlags.None, asyncResult => {
			int recvLen = clientSocket.EndReceive(asyncResult);

			int pos = 0;
			while (pos < recvLen) {

				if (GetMessageCount() == 0 || messageQueue [GetLastMessageIndex()].length == messageQueue [GetLastMessageIndex()].count) {
					for (int i = 0; i < recvLen - pos; ++i) {
						tail[tailCurrLen] = recvData [i + pos];
						++tailCurrLen;
					}
					if (tailCurrLen >= 4 && tail [0] == '\xed' && tail [1] == '\xcb') {
						int length = Convert.ToInt32 (tail [2]) * 256 + Convert.ToInt32 (tail [3]);
						Message newMsg = Enqueue(length);
						newMsg.Append(tail, 4, 4 + length);
						pos += 4 + length;
						tailCurrLen = 0;
					} else {
						if (!(tailCurrLen == 0 || (tailCurrLen == 1 && tail [0] == '\xed') || (tailCurrLen >= 2 && tailCurrLen <= 4 && tail [0] == '\xed' && tail [1] == '\xcb'))) {
							tailCurrLen = 0;
						}
						break;
					}
				} else {
					int addLen = messageQueue [GetLastMessageIndex()].length - messageQueue [GetLastMessageIndex()].count;
					messageQueue [GetLastMessageIndex()].Append (recvData, 0, recvLen);
					pos += addLen;
					tailCurrLen = 0;
				}
			}

			while (queueBegin != queueEnd) {
				if (messageQueue[queueBegin].length == messageQueue[queueBegin].count) {
					ProcessMessage(queueBegin);
					++queueBegin;
					if (queueBegin == messageQueue.Length) {
						queueBegin = 0;
					}
				} else {
					break;
				}
			}

			AsyncReceive();
		}, null);
	}

	void OnApplicationQuit() {
		clientSocket.Close ();
	}
}
