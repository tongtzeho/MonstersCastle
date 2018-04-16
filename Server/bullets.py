# Monsters Castle Bullet Pool
# Python 2.7.14

import struct

class bullets:
	def __init__(self, num):
		self.pool = []
		for i in range(num):
			self.pool.append((0.0, -50.0, 0.0))
	
	def handle(self, data):
		num = struct.unpack("=h", data[:2])[0]
		if num == len(self.pool):
			offset = 2
			for i in range(num):
				self.pool[i] = struct.unpack("=3f", data[offset:offset+12])
				offset += 12
	
	def serialize(self):
		result = struct.pack("=h", len(self.pool))
		for bullet in self.pool:
			result += struct.pack("=3f", bullet[0], bullet[1], bullet[2])
		return result