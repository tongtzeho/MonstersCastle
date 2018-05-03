# Monsters Castle Prop Pool (Bullets/Medicines)
# Python 2.7.14

import struct

class props:
	def __init__(self):
		self.pool = []
	
	def handle(self, data):
		self.pool = []
		numProp = struct.unpack("=h", data[:2])[0]
		offset = 4
		for i in range(numProp):
			self.pool.append(struct.unpack("=h3f", data[offset:offset+14]))
			offset += 14
	
	def serialize(self):
		result = ""
		numProp = 0
		for prop in self.pool:
			numProp += 1
			result += struct.pack("=h3f", prop[0], prop[1], prop[2], prop[3])
		return struct.pack("=hh", numProp, 14) + result