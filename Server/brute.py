# TDFPS Brute (Big Monster)
# Python 2.7.14

import struct, time

class brute:
	def __init__(self):
		self.debug = False
		self.level = 0
		self.maxHp = [300, 600, 900, 1200, 1500]
		self.atk = [100, 200, 300, 400, 500]
		self.isAlive = 0
		self.hp = 0
		self.position = [0.0, 0.0, 0.0]
		self.rotationY = 0.0
		self.action = 2 # 0 for born, 1 for walk, 2 for idle, 3 for attack, 4 for die
		self.rebornPosition = [0.0, 0.0, 50.0]
		self.rebornRotationY = 180.0
		self.velocity = [0.0, 0.0, -4.0]
		self.rebornTime = 1.733
		self.walkTime = 11.0
		
	def reborn(self):
		if self.level >= len(self.maxHp):
			return
		self.isAlive = 1
		self.hp = self.maxHp[self.level]
		self.position[0] = self.rebornPosition[0]
		self.position[1] = self.rebornPosition[1]
		self.position[2] = self.rebornPosition[2]
		self.rotationY = self.rebornRotationY
		self.action = 0
		self.lifeTime = 0
		self.level += 1
		
	def die(self):
		self.isAlive = 0
		self.action = 4
		
	def update(self, dt):
		if self.isAlive:
			if self.hp <= 0:
				self.die()
			else:
				self.lifeTime += dt
				if self.lifeTime <= self.rebornTime:
					self.action = 0
				elif self.lifeTime <= self.rebornTime + self.walkTime:
					self.action = 1
					self.position[0] += self.velocity[0]*dt
					self.position[1] += self.velocity[1]*dt
					self.position[2] += self.velocity[2]*dt
				else:
					self.action = 2
		if self.debug:
			self.log()
			
	def handle(self, data):
		isAlive, level, hp = struct.unpack("=3h", data[:6])
		if isAlive and level == self.level:
			self.hp = hp
			
	def serialize(self):
		return struct.pack("=4h4fh", self.isAlive, self.level, self.hp, self.maxHp[self.level-1], self.position[0], self.position[1], self.position[2], self.rotationY, self.action)
		
	def log(self):
		print [self.isAlive, self.level, self.hp], self.position, [self.action]
