# Monsters Castle Magic Ball Launched by Ghost
# Python 2.7.14

import struct, math, random

class ball:
	def __init__(self, id, start, aim, scene):
		self.debug = False
		self.id = id
		diffuse = [1.2, 0.75, 1.2]
		direction = [
			aim[0]+random.random()*diffuse[0]*2-diffuse[0]-start[0],
			aim[1]+random.random()*diffuse[1]*2-diffuse[1]-start[1],
			aim[2]+random.random()*diffuse[2]*2-diffuse[2]-start[2],
		]
		scalarVelocity = 20.0
		directionSqr = 1.0 / math.sqrt(direction[0]*direction[0]+direction[1]*direction[1]+direction[2]*direction[2])
		self.velocity = [
			direction[0]*scalarVelocity*directionSqr,
			direction[1]*scalarVelocity*directionSqr,
			direction[2]*scalarVelocity*directionSqr
		]
		self.position = start
		self.scene = scene
		self.time = 0
		self.endTime = 4.6
		self.radius = 0.32
		self.attack = 8
		self.gateBottom = [-0.06, 3, -2.6]
		self.gateTop = [-0.06, 6, -2.6]
		self.gateRadius = 4.0
		
	def update(self, dt, character):
		self.time += dt
		if self.time >= self.endTime:
			print "Ball<%d> Bomb (Timeout)" % self.id
			return [-1, 0, 0]
		else:
			self.nextPosition = [self.position[0]+self.velocity[0]*dt, self.position[1]+self.velocity[1]*dt, self.position[2]+self.velocity[2]*dt]
			if character.isAlive and self.scene.getSegmentDistanceSquare(self.position, self.nextPosition, character.getBodyBottom(), character.getBodyTop()) <= (self.radius + character.radius)*(self.radius + character.radius):
				if self.scene.getSegmentDistanceSquare(self.position, self.nextPosition, self.gateBottom, self.gateTop) <= (self.radius + self.gateRadius)*(self.radius + self.gateRadius):
					print "Ball<%d> Bomb (Hit Character and Gate)" % self.id
					return [-1, self.attack, self.attack]
				else:
					print "Ball<%d> Bomb (Hit Character)" % self.id
					return [-1, self.attack, 0]
			elif self.scene.getSegmentDistanceSquare(self.position, self.nextPosition, self.gateBottom, self.gateTop) <= (self.radius + self.gateRadius)*(self.radius + self.gateRadius):
				print "Ball<%d> Bomb (Hit Gate)" % self.id
				return [-1, 0, self.attack]
			elif self.scene.capsuleCollideDetection(self.position, self.nextPosition, self.radius):
				print "Ball<%d> Bomb (Hit Environment)" % self.id
				return [-1, 0, 0]
			else:
				self.position = self.nextPosition
				return [0, 0, 0]
			
	def serialize(self):
		return struct.pack("=h6f", self.id, self.position[0], self.position[1], self.position[2], self.velocity[0], self.velocity[1], self.velocity[2])