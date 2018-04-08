# Socket Server Message Encode/Decode
# Python 2.7.14

def encode(message): # add header before msg
	return "%s%s%s%s" % ('\xed\xcb', chr(len(message)/256), chr(len(message)%256), message)

def decode(message): # parse header
	if len(message) >= 4 and message[0] == '\xed' and message[1] == '\xcb':
		l = ord(message[2])*256+ord(message[3])
		return [True, l, message[4:]]
	else:
		return [False]
		
def enqueue(msgQueue, message, tail): # parse and add message to msgQueue
	pos = 0
	while pos < len(message):
		if not len(msgQueue) or msgQueue[-1][0] == len(msgQueue[-1][1]): # New message from pos
			decodeResult = decode(tail+message[pos:])
			if decodeResult[0]:
				msgQueue.append([decodeResult[1], decodeResult[2]])
				pos += (4+decodeResult[1])
				tail = ""
			else:
				newTail = tail+message[pos:]
				if len(newTail) == 0 or (len(newTail) == 1 and newTail[0] == '\xed') or (len(newTail) >= 2 and len(newTail) < 4 and newTail[0] == '\xed' and newTail[1] == '\xcb'):
					tail = newTail
				else:
					tail = ""
				break
		else:
			l = msgQueue[-1][0]-len(msgQueue[-1][1])
			msgQueue[-1][1] += message[:l]
			pos += l
			tail = ""
	return tail