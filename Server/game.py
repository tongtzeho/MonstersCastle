import socket, threading, random, struct, time

class game(threading.Thread): # run as a game monitor client
	def __init__(self, username, address, port):
		threading.Thread.__init__(self)
		self.recog = random.randint(1, 100000000) # a number recognized the game monitor
		self.isStop = False
		self.gameLock = threading.Lock()
		self.conn = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.conn.connect((address, port))
		self.username = username
		self.initGame()
		
	def initGame(self):
		self.gameLock.acquire()
		self.characterPos = (20.0, 5.0, 15.0)
		self.characterRotation = 90.0
		self.gameLock.release()
	
	def run(self): # override
		self.gameTime = time.time()
		while not self.isStop:
			currTime = time.time()
			if currTime - self.gameTime > 0.0333333333333:
				self.update()
				self.conn.send(self.getGameCurrInfo())
				self.gameTime = currTime
		self.conn.close()
		
	def stop(self):
		self.isStop = True
		
	def update(self):
		self.gameLock.acquire()
		# TODO: Update Game By Current Status
		self.gameLock.release()
		
	def getGameCurrInfo(self):
		self.gameLock.acquire()
		headStr = "^^^@"
		result = struct.pack("4sii16sffff", headStr, self.recog, len(self.username), self.username, self.characterPos[0], self.characterPos[1], self.characterPos[2], self.characterRotation)
		self.gameLock.release()
		return result
		
	def handle(self, data):
		self.gameLock.acquire()
		# TODO: Get Game Status From Client
		self.gameLock.release()