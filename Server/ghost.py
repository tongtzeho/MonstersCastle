# Monsters Castle Ghost (Small Monster)
# Python 2.7.14

import struct, time, random, math
import ball

class ghost:
	def __init__(self, id, bornPoint, height): # [0, 1, 2] for [Left, Mid, Right]
		self.debug = False
		self.height = height
		self.id = id
		self.hp = 150
		self.maxHp = 150
		self.position = [0.0, 0.0, 0.0]
		self.rotationY = 0.0
		self.action = 2 # 1 for walk, 2 for idle, 3 for attack, 4 for die, 5 for bomb
		self.scalarVelocity = 2.0
		self.setWay(bornPoint)
		self.phase = -1
		self.bornCurrTime = 0
		self.bornTotalTime = 1
		self.bornVelocity = 1
		self.attackCurrTime = 0
		self.attackAimTime = 0.35
		self.attackTotalTime = 1.5
		self.attackId = 0
		self.attackMax = 15
		self.generateAttackCD()
		self.dieCurrTime = 0
		self.dieTotalTime = 1
		self.bombCurrTime = 0
		self.bombTotalTime = 1
		print "Ghost(%d) Born" % self.id
	
	def generateAttackCD(self):
		self.attackCD = random.random()*3+2
	
	def setWay(self, bornPoint):
		if bornPoint == 0:
			if random.random() < 0.5:
				self.checkPoint = [
					[-19.5, 46], [-21.2, 43], [-21.2, 17], [-17.4, 13.2], [-12.5, 13.2], [-8.5, 6.8], [-2.5, 1.8]
				]
			else:
				self.checkPoint = [
					[-19.5, 46], [-17.8, 43], [-17.8, 16], [-17.1, 14.7], [-9, 14.7], [-1.5, 1.8]
				]
		elif bornPoint == 1:
			if random.random() < 0.5:
				self.checkPoint = [
					[-6, 49], [-5, 16], [-1, 1.8]
				]
			else:
				self.checkPoint = [
					[-6, 49], [-2, 43], [-0.5, 1.8]
				]
		else:
			if random.random() < 0.5:
				self.checkPoint = [
					[14.6, 54], [16, 40], [15, 17], [9, 7], [2.5, 1.8]
				]
			else:
				self.checkPoint = [
					[14.6, 54], [13.5, 46], [9, 34], [7, 18], [1.7, 1.8]
				]
	
	def getRotationY(self, x0, z0, x1, z1): # from [x0, z0] to [x1, z1]
		dx = x1 - x0
		dz = z1 - z0
		l = 1.0/math.sqrt(dx*dx + dz*dz)
		dx, dz = dx*l, dz*l
		if dz > 1:
			dz = 1
		elif dz < -1:
			dz = -1
		if dx >= 0:
			return math.acos(dz)*57.29577951308232
		else:
			return math.acos(dz)*-57.29577951308232
	
	def getVelocity(self): # calculate velocity(vector) from checkPoint[phase] to checkPoint[phase+1]
		v = [self.checkPoint[self.phase+1][0]-self.checkPoint[self.phase][0], self.checkPoint[self.phase+1][1]-self.checkPoint[self.phase][1]]
		vLen = 1.0/math.sqrt(v[0]*v[0] + v[1]*v[1])
		return [v[0]*vLen*self.scalarVelocity, v[1]*vLen*self.scalarVelocity]
	
	def attack(self, dt, character):
		ret = None
		if character.isAlive:
			aim = character.position
		else:
			aim = [-0.06, 4.67, 0.711]
		if self.attackCurrTime > self.attackTotalTime: # an attack end
			self.action = 2
			self.rotationY = self.getRotationY(self.checkPoint[self.phase][0], self.checkPoint[self.phase][1], self.checkPoint[self.phase+1][0], self.checkPoint[self.phase+1][1])
			self.attackCurrTime = 0
			self.generateAttackCD()
		else:
			if self.attackCurrTime > self.attackAimTime:
				if self.attackCurrTime - dt <= self.attackAimTime: # push a ball out
					self.rotationY = self.getRotationY(self.position[0], self.position[2], aim[0], aim[2])
					ballId = self.id*self.attackMax + self.attackId
					theta = self.rotationY*0.0174532925199 # pi/180
					c = math.cos(theta)
					s = math.sin(theta)
					startPoint = [
						0.62*c+0.35*s+self.position[0],
						2.1+self.position[1],
						-0.62*s+0.35*c+self.position[2]
					]
					ret = ball.ball(ballId, startPoint, aim)
					print "Ghost(%d) Attack<%d>" % (self.id, ballId)
					self.attackId += 1
				self.action = 2 # wait for animation end
			else: # aiming
				if self.attackCurrTime == 0: # animation begin
					self.action = 3
				else: # animation already begun
					self.action = 2
				self.rotationY = self.getRotationY(self.position[0], self.position[2], aim[0], aim[2])
			self.attackCurrTime += dt
		return ret
	
	def bomb(self, character):
		if character.isAlive:
			dx = self.position[0] - character.position[0]
			dz = self.position[2] - character.position[2]
			distSqr = dx*dx+ dz*dz
			if distSqr >= 36:
				return 0
			elif distSqr <= 1:
				return 25
			else:
				return 5 * (6-math.sqrt(distSqr))
		return 0
	
	def update(self, dt, character):
		if self.phase == len(self.checkPoint) - 1: # bombing
			self.hp = 0
			self.position[0] = self.checkPoint[-1][0]
			self.position[2] = self.checkPoint[-1][1]
			self.position[1] = self.height.getHeight(self.position[0], self.position[2])
			self.rotationY = 180.0
			self.action = 5
			damageToCharacter = 0
			if self.bombCurrTime == 0:
				damageToCharacter = self.bomb(character)
				print "Ghost(%d) Bomb" % self.id
			self.bombCurrTime += dt
			if self.bombCurrTime >= self.bombTotalTime:
				return [-1, damageToCharacter, 0, None]
			else:
				return [0, damageToCharacter, 0, None]
		if self.hp > 0: # alive
			if self.phase == -1: # borning
				if self.bornCurrTime == 0:
					self.position[0] = self.checkPoint[0][0]
					self.position[2] = self.checkPoint[0][1]
					self.position[1] = self.height.getHeight(self.position[0], self.position[2])+(self.bornCurrTime - self.bornTotalTime)*self.bornVelocity
					self.rotationY = self.getRotationY(self.checkPoint[0][0], self.checkPoint[0][1], self.checkPoint[1][0], self.checkPoint[1][1])
					self.action = 2
					self.bornCurrTime += dt
				elif self.bornCurrTime < self.bornTotalTime:
					self.position[1] = self.height.getHeight(self.position[0], self.position[2])+(self.bornCurrTime - self.bornTotalTime)*self.bornVelocity
					self.bornCurrTime += dt
				else:
					self.position[1] = self.height.getHeight(self.position[0], self.position[2])
					self.phase = 0
					self.velocity = self.getVelocity()
				return [0, 0, 0, None]
			else:
				newBall = None
				if self.attackCurrTime == 0: # attack, or walk from checkPoint[phase] to checkPoint[phase+1]
					if self.attackId < self.attackMax and self.attackCD <= 0:
						newBall = self.attack(dt, character)
					else:
						oldDistSqr = (self.position[0]-self.checkPoint[self.phase+1][0])*(self.position[0]-self.checkPoint[self.phase+1][0]) + (self.position[2]-self.checkPoint[self.phase+1][1])*(self.position[2]-self.checkPoint[self.phase+1][1])
						self.position[0] += self.velocity[0]*dt
						self.position[2] += self.velocity[1]*dt
						self.position[1] = self.height.getHeight(self.position[0], self.position[2])
						newDistSqr = (self.position[0]-self.checkPoint[self.phase+1][0])*(self.position[0]-self.checkPoint[self.phase+1][0]) + (self.position[2]-self.checkPoint[self.phase+1][1])*(self.position[2]-self.checkPoint[self.phase+1][1])
						self.action = 1
						self.attackCD -= dt
						if oldDistSqr < newDistSqr: # already arrived at checkPoint[phase+1]
							self.phase += 1
							if self.phase < len(self.checkPoint) - 1:
								self.velocity = self.getVelocity()
								self.rotationY = self.getRotationY(self.checkPoint[self.phase][0], self.checkPoint[self.phase][1], self.checkPoint[self.phase+1][0], self.checkPoint[self.phase+1][1])
							else:
								self.rotationY = 180.0
				else:
					newBall = self.attack(dt, character)
				return [0, 0, 0, newBall]
		else: # die
			self.action = 4
			self.dieCurrTime += dt
			if self.dieCurrTime >= self.dieTotalTime:
				print "Ghost(%d) Die" % self.id
				return [-1, 0, 0, None]
			else:
				return [0, 0, 0, None]

	def handle(self, data):
		id, hp = struct.unpack("=2h", data[:4])
		if id == self.id:
			self.hp = hp
			
	def serialize(self):
		return struct.pack("=3h4fh", self.id, self.hp, self.maxHp, self.position[0], self.position[1], self.position[2], self.rotationY, self.action)
		
	def log(self):
		print [self.id, self.hp], self.position, [self.phase, self.action]