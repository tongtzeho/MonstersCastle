using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Message {
	
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
