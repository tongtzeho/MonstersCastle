using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Message {
	
	public int length;
	public int count; // current count
	public byte[] content;

	public Message(int reserve) {
		length = 0;
		count = 0;
		content = new byte[reserve];
	}

	public void Reset(int len) {
		length = len;
		count = 0;
	}

	public void Append(byte[] newContent, int beginIndex, int endIndex) {
		if (endIndex > newContent.Length) {
			endIndex = newContent.Length;
		}
		for (int i = beginIndex; i < endIndex; ++i) {
			if (count + i - beginIndex >= length) {
				count = length;
				return;
			}
			content [count + i - beginIndex] = newContent [i];
		}
		count += endIndex - beginIndex;
	}
}
