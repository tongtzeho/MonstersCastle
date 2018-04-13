# TDFPS Ghost (Small Monster)
# Python 2.7.14

import struct, time, random, math

class ghost:
	def __init__(self, id, bornPoint): # [0, 1, 2] for [Left, Mid, Right]
		self.debug = True
		self.id = id
		self.hp = 150
		self.maxHp = 150
		self.position = [0.0, 0.0, 0.0]
		self.rotationY = 0.0
		self.action = 2 # 1 for walk, 2 for idle, 3 for attack, 4 for die, 5 for bomb
		self.scalarVelocity = 2.0
		self.setWay(bornPoint)
		self.phase = -1
		self.distSqrThreshold = 0.08*0.08
		self.bornCurrTime = 0
		self.bornTotalTime = 1
		self.bornVelocity = 1
		self.attackCurrTime = 0
		self.attackAimTime = 0.5
		self.attackTotalTime = 1.5
		self.attackId = 0
		self.attackMax = 15
		self.dieCurrTime = 0
		self.dieTotalTime = 1
		self.bombCurrTime = 0
		self.bombTotalTime = 1
		
	def setWay(self, bornPoint):
		if bornPoint == 0:
			self.checkPoint = [
				[-30, 50],
				[-20, 10],
				[0, 2]
			]
		elif bornPoint == 1:
			self.checkPoint = [
				[0.2, 50],
				[0.2, 20],
				[0, 2]
			]
		else:
			self.checkPoint = [
				[30, 50],
				[20, 10],
				[0, 2]
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
	
	def update(self, dt):
		if self.phase == len(self.checkPoint) - 1: # bombing
			self.hp = 0
			self.position[0] = self.checkPoint[-1][0]
			self.position[2] = self.checkPoint[-1][1]
			self.position[1] = 0
			self.rotationY = 180.0
			self.action = 5
			if self.bombCurrTime == 0:
				pass # damage to player and gate
			self.bombCurrTime += dt
			if self.bombCurrTime >= self.bombTotalTime:
				return [-1]
			else:
				return [0]
		if self.hp > 0: # alive
			if self.phase == -1: # borning
				if self.bornCurrTime == 0:
					self.position[0] = self.checkPoint[0][0]
					self.position[2] = self.checkPoint[0][1]
					self.position[1] = (self.bornCurrTime - self.bornTotalTime)*self.bornVelocity
					self.rotationY = self.getRotationY(self.checkPoint[0][0], self.checkPoint[0][1], self.checkPoint[1][0], self.checkPoint[1][1])
					self.action = 2
					self.bornCurrTime += dt
					return [0]
				elif self.bornCurrTime < self.bornTotalTime:
					self.position[1] = (self.bornCurrTime - self.bornTotalTime)*self.bornVelocity
					self.bornCurrTime += dt
					return [0]
				else:
					self.position[1] = 0
					self.phase = 0
					self.velocity = self.getVelocity()
					return [0]
			else:
				if self.attackCurrTime == 0: # attack, or walk from checkPoint[phase] to checkPoint[phase+1]
					# if random ... attack, else walk
					self.position[0] += self.velocity[0]*dt
					self.position[2] += self.velocity[1]*dt
					self.position[1] = 0
					self.action = 1
					if (self.position[0]-self.checkPoint[self.phase+1][0])*(self.position[0]-self.checkPoint[self.phase+1][0]) + (self.position[2]-self.checkPoint[self.phase+1][1])*(self.position[2]-self.checkPoint[self.phase+1][1]) <= self.distSqrThreshold:
						self.phase += 1
						if self.phase < len(self.checkPoint) - 1:
							self.velocity = self.getVelocity()
							self.rotationY = self.getRotationY(self.checkPoint[self.phase][0], self.checkPoint[self.phase][1], self.checkPoint[self.phase+1][0], self.checkPoint[self.phase+1][1])
						else:
							self.rotationY = 180.0
					return [0]
				# else: # is attcking
		else: # die
			self.action = 4
			self.dieCurrTime += dt
			if self.dieCurrTime >= self.dieTotalTime:
				return [-1]
			else:
				return [0]

	def handle(self, data):
		id, hp = struct.unpack("=2h", data[:4])
		if id == self.id:
			self.hp = hp
			
	def serialize(self):
		return struct.pack("=3h4fh", self.id, self.hp, self.maxHp, self.position[0], self.position[1], self.position[2], self.rotationY, self.action)
		
	def log(self):
		print [self.id, self.hp], self.position, [self.phase, self.action]