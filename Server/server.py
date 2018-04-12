# TDFPS Socket Server
# Python 2.7.14

import socket, select, os, time, json, struct
import game, msg

CONNECTION_LIST = [] # Read sockets
CONNECTION_USERS = {} # Socket-Username (if not logined, username is None)
CONNECTION_MSGQUEUE = {} # Socket-MsgQueue [[len, str], tail]
USERS_CONNECTION = {} # Username-Socket (logined user)
PLAYER_GAME = {} # Username-Game
RECV_BUFFER = 4096
ADDRESS = 'localhost'
PORT = 9121

def loadUserDatabase(jsonFile): # load user information from json
	if not os.path.isfile(jsonFile):
		return {}
	with open(jsonFile, "r") as f:
		temp = json.load(f)
		users = {}
		for k, v in temp.items():
			users[k] = v # Username-Password
		return users
		
def dumpUserDatabase(jsonFile): # dump user information to json
	with open(jsonFile, "w") as f:
		json.dump(USER_DATABASE, f)

def handleGameMonitorData(sock, data):
	recog, usernameLen = struct.unpack("ii", data[4:12])
	username = struct.unpack("16s", data[12:28])[0][:usernameLen]
	if username in USERS_CONNECTION and username in PLAYER_GAME and PLAYER_GAME[username].recog == recog:
		sendMsgToSock(USERS_CONNECTION[username], data[28:])
	
def createNewGame(username):
	PLAYER_GAME[username] = game.game(username, ADDRESS, PORT)
	PLAYER_GAME[username].start() # start a client thread as game monitor
	print ("'%s' create new game" % username)

def fetchUsernamePassword(data):
	wordArr = data.split(' ')
	return wordArr[1], wordArr[2]

def login(sock, username, password):
	if username in USER_DATABASE:
		if password == USER_DATABASE[username]:
			if not username in USERS_CONNECTION:
				CONNECTION_USERS[sock] = username
				USERS_CONNECTION[username] = sock
				print ("'%s' login" % username)
				return 0
			else:
				return 3
		else:
			return 2
	else:
		return 1
	
def handleSignIn(sock, data):
	username, password = fetchUsernamePassword(data)
	ret = login(sock, username, password)
	if ret == 0:
		if not username in PLAYER_GAME:
			createNewGame(username)
		sendMsgToSock(sock, "$si0")
	else:
		sendMsgToSock(sock, "$si"+str(ret))
	
def register(username, password):
	if username in USER_DATABASE:
		return False
	else:
		USER_DATABASE[username] = password
		print ("'%s' register" % username)
		dumpUserDatabase("user.json")
		return True
	
def handleSignUp(sock, data):
	username, password = fetchUsernamePassword(data)
	if register(username, password):
		login(sock, username, password) # login after register
		if not username in PLAYER_GAME:
			createNewGame(username)
		sendMsgToSock(sock, "$si0")
	else:
		sendMsgToSock(sock, "$su1")
	
def sendMsgToSock(sock, message): # send msg to client socket
	try:
		sock.send(msg.encode(message))
	except:
		pass

def handleClientData(sock, data): # handle data received from client (including game monitor thread)
	connectionUsername = CONNECTION_USERS[sock]
	if data.startswith("^^^@"): # game monitor
		handleGameMonitorData(sock, data)
	elif connectionUsername == None:
		if data.startswith("$su "): # Sign up : $su username password
			handleSignUp(sock, data)					
		elif data.startswith("$si "): # Sign in : $si username password
			handleSignIn(sock, data)
	else:
		if connectionUsername in PLAYER_GAME:
			PLAYER_GAME[connectionUsername].handle(data)
	
def logout(username):
	if username != None and username in USERS_CONNECTION:
		print ("User '%s' quit" % username)
		USERS_CONNECTION.pop(username)
	
if __name__ == "__main__":
	USER_DATABASE = loadUserDatabase("user.json")
	gameSocketServer = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	gameSocketServer.setblocking(False)
	gameSocketServer.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
	gameSocketServer.bind((ADDRESS, PORT))
	gameSocketServer.listen(100)
	CONNECTION_LIST.append(gameSocketServer)
	print ("TDFPS server starting on port %d" % PORT)
	
	while True:
		readSockets, writeSockets, errSockets = select.select(CONNECTION_LIST, [], [])
		for sock in readSockets:
		
			# new connection
			if sock == gameSocketServer:
				newSock, addr = gameSocketServer.accept()
				newSock.setblocking(False)
				CONNECTION_LIST.append(newSock)
				CONNECTION_USERS[newSock] = None
				CONNECTION_MSGQUEUE[newSock] = [[], ""]
				print ("Client (%s, %s) connected" % addr)
			
			# message from existing client
			else:
				try:
					data = sock.recv(RECV_BUFFER)
					if data:
						CONNECTION_MSGQUEUE[sock][1] = msg.enqueue(CONNECTION_MSGQUEUE[sock][0], data, CONNECTION_MSGQUEUE[sock][1])
						for m in CONNECTION_MSGQUEUE[sock][0]:
							if m[0] == len(m[1]):
								handleClientData(sock, m[1])
							else:
								break
						if len(CONNECTION_MSGQUEUE[sock][0]):
							if CONNECTION_MSGQUEUE[sock][0][-1][0] == len(CONNECTION_MSGQUEUE[sock][0][-1][1]):
								CONNECTION_MSGQUEUE[sock][0] = []
							else:
								CONNECTION_MSGQUEUE[sock][0] = [CONNECTION_MSGQUEUE[sock][0][-1]]
				except:
					logout(CONNECTION_USERS[sock])
					try:
						print ("Close a socket")
						sock.close()
					except:
						pass
					CONNECTION_LIST.remove(sock)
					CONNECTION_USERS.pop(sock)
					CONNECTION_MSGQUEUE.pop(sock)
					continue
	
	gameSocketServer.close()