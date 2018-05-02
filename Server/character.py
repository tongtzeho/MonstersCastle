# Monsters Castle Character
# Python 2.7.14

import struct

class character:
	def __init__(self):
		self.debug = False
		self.reborn()
		self.sniperBulletNum = 5
		self.sniperBulletOwn = 25
		self.submachineBulletNum = 30
		self.submachineBulletOwn = 150
		self.medicineNum = 0
		self.upHpLeft = 0
		self.buffTimeLeft = 0.0
		self.upHpTick = 0.16
		self.upHpOnce = 50 # +50Hp in 8sec
		self.medicineTaking = 0
		self.rebornTime = 7.7
		self.radius = 0.35
		
	def reborn(self):
		self.isAlive = 1
		self.rebornTimeLeft = 0.0
		self.hp = 100
		self.maxHp = 100
		self.resetPosition()
		print "Character Reborn"
		
	def resetPosition(self):
		self.position = [0.0, 3.0, 10.0]
		self.rotationY = 0.0
		
	def getBodyCenter(self):
		return [self.position[0], self.position[1]+1.34, self.position[2]]
	
	def getBodyBottom(self):
		return [self.position[0], self.position[1]+self.radius, self.position[2]]
	
	def getBodyTop(self):
		return [self.position[0], self.position[1]+1.57, self.position[2]]
		
	def die(self):
		self.hp = 0
		self.isAlive = 0
		self.rebornTimeLeft = self.rebornTime
		self.upHpLeft = 0
		self.buffTimeLeft = 0.0
		print "Character Die"
	
	def update(self, dt):
		if self.isAlive == 0: # dead
			self.rebornTimeLeft -= dt
			if self.rebornTimeLeft <= 0:
				self.reborn()
		else: # alive
			if self.hp <= 0 or self.position[1] <= -2:
				self.die()
			else:
				if self.buffTimeLeft != 0.0:
					self.buffTimeLeft -= dt
					currUpHpLeft = int(self.buffTimeLeft/self.upHpTick)
					if currUpHpLeft < self.upHpLeft:
						self.hp += self.upHpLeft-currUpHpLeft
						if self.hp > self.maxHp:
							self.hp = self.maxHp
						self.upHpLeft = currUpHpLeft
						if self.upHpLeft == 0:
							self.buffTimeLeft = 0.0
				if self.medicineTaking > 0:
					self.medicineTaking -= 1
					if self.buffTimeLeft == 0.0:
						self.buffTimeLeft = self.upHpTick*(self.upHpOnce+1)-0.0001
						self.upHpLeft = self.upHpOnce
					else:
						self.buffTimeLeft += self.upHpTick*self.upHpOnce
						self.upHpLeft += self.upHpOnce
		if self.debug:
			self.log()
			
	def handle(self, data):
		if self.isAlive:
			self.position[0], self.position[1], self.position[2], self.rotationY = struct.unpack("=4f", data[:16])
		self.prevMedicineNum = self.medicineNum
		self.sniperBulletNum, self.sniperBulletOwn, self.submachineBulletNum, self.submachineBulletOwn, self.medicineNum = struct.unpack("=5h", data[16:26])
		if self.medicineNum < self.prevMedicineNum:
			self.medicineTaking += self.prevMedicineNum - self.medicineNum
	
	def serialize(self):
		return struct.pack("=hf2h4f6h", self.isAlive, self.rebornTimeLeft, self.hp, self.maxHp, self.position[0], self.position[1], self.position[2], self.rotationY, self.sniperBulletNum, self.sniperBulletOwn, self.submachineBulletNum, self.submachineBulletOwn, self.medicineNum, self.upHpLeft)
	
	def log(self):
		print [self.isAlive, self.rebornTimeLeft, self.hp], self.position, [self.sniperBulletNum, self.sniperBulletOwn], [self.submachineBulletNum, self.submachineBulletOwn], self.medicineNum, self.upHpLeft
			