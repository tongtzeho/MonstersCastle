# Monsters Castle Prop Pool (Bullets/Medicines)
# Python 2.7.14

import struct

class props:
	def __init__(self, num):
		self.pool = []
		self.resetPosition = (0.0, -50.0, 0.0)
		self.thresholdY = -40.0
		for i in range(num):
			self.pool.append(self.resetPosition)
	
	def handle(self, data):
		numProp = struct.unpack("=h", data[:2])[0]
		offset = 2
		for i in range(numProp):
			self.pool[i] = struct.unpack("=3f", data[offset:offset+12])
			offset += 12
		for i in range(numProp, len(self.pool)):
			self.pool[i] = self.resetPosition
	
	def serialize(self):
		result = ""
		numProp = 0
		for prop in self.pool:
			if prop[1] > self.thresholdY:
				numProp += 1
				result += struct.pack("=3f", prop[0], prop[1], prop[2])
		return struct.pack("=h", numProp) + result
