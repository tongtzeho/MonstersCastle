# TDFPS Game (As Client Thread)
# Python 2.7.14

import socket, threading, random, struct, time
import msg, character

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
		self.character = character.character()
		self.gameLock.release()
	
	def run(self): # override
		self.gameTime = time.time()
		while not self.isStop:
			currTime = time.time()
			deltaTime = currTime - self.gameTime
			if deltaTime > 0.0166666666667:
				self.update(deltaTime)
				self.conn.send(msg.encode(self.getGameCurrInfo()))
				self.gameTime = currTime
		self.conn.close()
		
	def stop(self):
		self.isStop = True
		
	def update(self, deltaTime):
		self.gameLock.acquire()
		self.character.update(deltaTime)
		self.gameLock.release()
		
	def getGameCurrInfo(self):
		head = struct.pack("=4s2i16s", "^^^@", self.recog, len(self.username), self.username)
		self.gameLock.acquire()
		characterResult = self.character.serialize()
		characterResult = struct.pack("=h", len(characterResult))+characterResult
		self.gameLock.release()
		return head + characterResult
		
	def handle(self, data):
		characterDataLen = struct.unpack("=h", data[:2])[0]
		characterData = data[2:2+characterDataLen]
		self.gameLock.acquire()
		try:
			self.character.handle(characterData)
		except:
			print ("game.py handle error")
			pass
		self.gameLock.release()