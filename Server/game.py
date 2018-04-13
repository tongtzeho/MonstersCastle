# TDFPS Game (As Client Thread)
# Python 2.7.14

import socket, threading, random, struct, time
import msg, character, brute, ghost

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
		self.brute = brute.brute()
		self.ghosts = {}
		self.ghostId = 1
		self.ghostMax = 2000
		self.gameTime = 0
		self.level = 0
		self.gameLock.release()
	
	def run(self): # override
		self.gameClock = time.time()
		while not self.isStop:
			currTime = time.time()
			deltaTime = currTime - self.gameClock
			if deltaTime > 0.0166666666667: # 60FPS on server
				self.update(deltaTime)
				self.conn.send(msg.encode(self.serialize()))
				self.gameClock = currTime
		self.conn.close()
		
	def stop(self):
		self.isStop = True
	
	def isBruteDead(self, waitTime, dt):
		if self.brute.isAlive:
			self.waitTime = 0
		else:
			try:
				self.waitTime += dt
			except:
				self.waitTime = 0
			if self.waitTime >= waitTime:
				return True
		return False
		
	def allMonsterReborn(self):
		self.brute.reborn()
	
	def update(self, deltaTime):
		self.gameLock.acquire()
		self.gameTime += deltaTime
		self.character.update(deltaTime)
		self.brute.update(deltaTime)
		if (self.level == 0 and self.gameTime >= 5) or (self.level >= 1 and self.level <= 4 and self.isBruteDead(10, deltaTime)):
			self.brute.reborn()
			self.level += 1
		self.updateGhosts(deltaTime)
		self.gameLock.release()
		
	def updateGhosts(self, deltaTime):
		if self.gameTime >= 2*self.ghostId and self.gameTime-deltaTime < 2*self.ghostId and self.ghostId <= self.ghostMax and self.level < 5:
			bornPoint = int(self.gameTime/2)%3
			self.ghosts[self.ghostId] = ghost.ghost(self.ghostId, bornPoint)
			self.ghostId += 1
		delList = []
		for k, v in self.ghosts.items():
			ret = v.update(deltaTime)
			if ret[0] == -1:
				delList.append(k)
		for k in delList:
			self.ghosts.pop(k)
		
	def serialize(self):
		head = struct.pack("=4s2i16s", "^^^@", self.recog, len(self.username), self.username)
		self.gameLock.acquire()
		characterResult = self.character.serialize()
		characterResult = struct.pack("=h", len(characterResult))+characterResult
		bruteResult = self.brute.serialize()
		bruteResult = struct.pack("=h", len(bruteResult))+bruteResult
		if len(self.ghosts) == 0:
			ghostsResult = struct.pack("=hh", 0, 0)
		else:
			ghostsResult = struct.pack("=h", len(self.ghosts))
			byteGhost = 0
			for g in self.ghosts.values():
				if byteGhost == 0:
					s = g.serialize()
					byteGhost = len(s)
					ghostsResult += struct.pack("=h", byteGhost)+s
				else:
					ghostsResult += g.serialize()
		self.gameLock.release()
		return head + characterResult + bruteResult + ghostsResult
		
	def handle(self, data):
		try:
			offset = 0
			characterDataLen = struct.unpack("=h", data[offset:offset+2])[0]
			characterData = data[offset+2:offset+2+characterDataLen]
			offset += 2+characterDataLen
			bruteDataLen = struct.unpack("=h", data[offset:offset+2])[0]
			bruteData = data[offset+2:offset+2+bruteDataLen]
			offset += 2+bruteDataLen
			ghostsDataSize, ghostsDataByte = struct.unpack("=hh", data[offset:offset+4])
			ghostsData = data[offset+4:offset+4+ghostsDataSize*ghostsDataByte]
			offset += 4
			self.gameLock.acquire()
			try:
				self.character.handle(characterData)
				self.brute.handle(bruteData)
				offset = 0
				for i in range(ghostsDataSize):
					id = struct.unpack("=h", ghostsData[offset:offset+2])[0]
					if id in self.ghosts:
						self.ghosts[id].handle(ghostsData[offset:offset+ghostsDataByte])
					offset += ghostsDataByte
			except:
				print ("game.py handle error")
				pass
			self.gameLock.release()
		except:
			print ("unpack data error")