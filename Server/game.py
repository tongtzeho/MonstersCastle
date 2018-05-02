# Monsters Castle Game (As Client Thread)
# Python 2.7.14

import socket, threading, random, struct, time
import msg, character, brute, ghost, props

class game(threading.Thread): # run as a game monitor client
	def __init__(self, username, address, port, scene):
		threading.Thread.__init__(self)
		self.recog = random.randint(1, 100000000) # a number recognized the game monitor
		self.isStop = False
		self.gameLock = threading.Lock()
		self.conn = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.conn.connect((address, port))
		self.conn.setblocking(False)
		self.username = username
		self.scene = scene
		self.initGame()
		
	def initGame(self):
		self.gameLock.acquire()
		self.character = character.character()
		self.brute = brute.brute(self.scene)
		self.ghosts = {}
		self.ghostId = 1
		self.ghostMax = 2000
		self.balls = {}
		self.submachineBullets = props.props(12)
		self.sniperBullets = props.props(12)
		self.medicines = props.props(5)
		self.gameTime = 0
		self.gameResult = 0 # 0 for playing, 1 for win, 2 for lose
		self.level = 0
		self.maxLevel = 5
		self.gateMaxHp = 8000
		self.gateHp = self.gateMaxHp
		self.gameLock.release()
	
	def run(self): # override
		self.gameClock = time.time()
		while not self.isStop:
			currTime = time.time()
			deltaTime = currTime - self.gameClock
			if deltaTime > 0.0166666666667: # 60FPS on server
				self.update(deltaTime)
				sendData = msg.encode(self.serialize())
				try:
					self.conn.send(sendData)
				except:
					print "game.py send data exception"
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
	
	def update(self, deltaTime):
		self.gameLock.acquire()
		if self.gameResult == 0:
			try:
				self.gameTime += deltaTime
				damageByBalls = self.updateBalls(deltaTime)
				damageByBrute = self.brute.update(deltaTime, self.character)
				if (self.level == 0 and self.gameTime >= 5) or (self.level >= 1 and self.level <= self.maxLevel - 1 and self.isBruteDead(10, deltaTime)):
					self.brute.reborn()
					self.level += 1
				elif self.level == self.maxLevel and self.brute.isAlive == 0:
					self.level = self.maxLevel+1 # ghost will not born
				if self.gateHp <= 0:
					self.gameResult = 1 # TODO: change to -1
				elif self.level == self.maxLevel+1 and len(self.ghosts) == 0:
					self.gameResult = 1
				damageByGhosts = self.updateGhosts(deltaTime)
				damageToCharacter = int(damageByBalls[0]+damageByBrute[0]+damageByGhosts[0])
				if damageToCharacter > 0:
					self.character.hp -= damageToCharacter
					print "Character Hurt {%d}" % damageToCharacter
				damageToGate = int(damageByBalls[1]+damageByBrute[1]+damageByGhosts[1])
				if damageToGate > 0:
					self.gateHp -= damageToGate
					if self.gateHp < 0:
						self.gateHp = 0
					print "Gate Hurt {%d}, Left {%d}" % (damageToGate, self.gateHp)
				self.character.update(deltaTime)
			except:
				print ("game.py update error")
		self.gameLock.release()
	
	def updateBalls(self, deltaTime):
		damage = [0, 0]
		delList = []
		for k, v in self.balls.items():
			ret = v.update(deltaTime, self.character)
			if ret[0] == -1:
				delList.append(k)
			damage[0] += ret[1]
			damage[1] += ret[2]
		for k in delList:
			self.balls.pop(k)
		return damage
	
	def updateGhosts(self, deltaTime):
		if self.gameTime >= 2.3*self.ghostId and self.gameTime-deltaTime < 2.3*self.ghostId and self.ghostId <= self.ghostMax and self.level <= self.maxLevel:
			self.ghosts[self.ghostId] = ghost.ghost(self.ghostId, self.scene)
			self.ghostId += 1
		damage = [0, 0]
		delList = []
		for k, v in self.ghosts.items():
			ret = v.update(deltaTime, self.character)
			if ret[0] == -1:
				delList.append(k)
			damage[0] += ret[1]
			damage[1] += ret[2]
			if ret[3] != None: # new ball
				self.balls[ret[3].id] = ret[3]
		for k in delList:
			self.ghosts.pop(k)
		return damage
		
	def serialize(self):
		head = struct.pack("=4s2i16s", "^^^@", self.recog, len(self.username), self.username)
		self.gameLock.acquire()
		gameCurrStatus = struct.pack("=hhhh", self.gameResult, self.level, self.gateHp, self.gateMaxHp)
		if self.gameResult != 0:
			self.gameLock.release()
			return head + gameCurrStatus
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
		submachineBulletsResult = self.submachineBullets.serialize()
		sniperBulletsResult = self.sniperBullets.serialize()
		medicinesResult = self.medicines.serialize()
		if len(self.balls) == 0:
			ballsResult = struct.pack("=hh", 0, 0)
		else:
			ballsResult = struct.pack("=h", len(self.balls))
			byteBall = 0
			for b in self.balls.values():
				if byteBall == 0:
					s = b.serialize()
					byteBall = len(s)
					ballsResult += struct.pack("=h", byteBall)+s
				else:
					ballsResult += b.serialize()
		self.gameLock.release()
		return head + gameCurrStatus + characterResult + bruteResult + ghostsResult + submachineBulletsResult + sniperBulletsResult + medicinesResult + ballsResult
		
	def handle(self, data):
		ret = 0
		try:
			gameResult, command = struct.unpack("=hh", data[:4])
			if gameResult == self.gameResult:
				if gameResult == 0: # game is playing
					if command == 0:
						offset = 4
						characterDataLen = struct.unpack("=h", data[offset:offset+2])[0]
						characterData = data[offset+2:offset+2+characterDataLen]
						offset += 2+characterDataLen
						bruteDataLen = struct.unpack("=h", data[offset:offset+2])[0]
						bruteData = data[offset+2:offset+2+bruteDataLen]
						offset += 2+bruteDataLen
						ghostsDataSize, ghostsDataByte = struct.unpack("=hh", data[offset:offset+4])
						ghostsData = data[offset+4:offset+4+ghostsDataSize*ghostsDataByte]
						offset += 4+ghostsDataSize*ghostsDataByte
						submachineBulletsNum = struct.unpack("=h", data[offset:offset+2])[0]
						submachineBulletsData = data[offset:offset+2+12*submachineBulletsNum]
						offset += 2+12*submachineBulletsNum
						sniperBulletsNum = struct.unpack("=h", data[offset:offset+2])[0]
						sniperBulletsData = data[offset:offset+2+12*sniperBulletsNum]
						offset += 2+12*sniperBulletsNum
						medicinesNum = struct.unpack("=h", data[offset:offset+2])[0]
						medicinesData = data[offset:offset+2+12*medicinesNum]
						offset += 2+12*medicinesNum
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
							self.submachineBullets.handle(submachineBulletsData)
							self.sniperBullets.handle(sniperBulletsData)
							self.medicines.handle(medicinesData)
						except:
							print ("game.py handle error")
							pass
						self.gameLock.release()
				else: # win or lose
					ret = command
		except:
			print ("unpack data error")
		return ret