# Monsters Castle Magic Ball Launched by Ghost
# Python 2.7.14

import struct, math, random

class ball:
	def __init__(self, id, start, aim):
		self.debug = False
		self.id = id
		diffuse = 2.0
		direction = [
			aim[0]+random.random()*diffuse*2-diffuse-start[0],
			aim[1]+random.random()*diffuse*2-diffuse-start[1],
			aim[2]+random.random()*diffuse*2-diffuse-start[2],
		]
		scalarVelocity = 20.0
		directionSqr = 1.0 / math.sqrt(direction[0]*direction[0]+direction[1]*direction[1]+direction[2]*direction[2])
		self.velocity = [
			direction[0]*scalarVelocity*directionSqr,
			direction[1]*scalarVelocity*directionSqr,
			direction[2]*scalarVelocity*directionSqr
		]
		self.position = start
		self.time = 0
		self.endTime = 4.0
		
	def update(self, dt, character):
		self.time += dt
		# TODO
		if self.time >= self.endTime:
			print "Ball<%d> Bomb" % self.id
			return [-1, 0, 0]
		else:
			return [0, 0, 0]
			
	def serialize(self):
		return struct.pack("=h6f", self.id, self.position[0], self.position[1], self.position[2], self.velocity[0], self.velocity[1], self.velocity[2])