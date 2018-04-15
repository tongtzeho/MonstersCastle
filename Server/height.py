import struct

class height:
	def __init__(self, infile):
		with open(infile, "rb") as fin:
			data = fin.read()
		self.minX, self.maxX, self.numX, self.minZ, self.maxZ, self.numZ = struct.unpack("=ffiffi", data[:24])
		offset = 24
		self.dx = self.numX / (self.maxX-self.minX)
		self.dz = self.numZ / (self.maxZ-self.minZ)
		self.height = []
		for i in range(self.numX):
			h = []
			for j in range(self.numZ):
				h.append(struct.unpack("=f", data[offset:offset+4])[0])
				offset += 4
			self.height.append(h)
			
	def getHeight(self, x, z):
		if x <= self.minX or x >= self.maxX or z <= self.minZ or z >= self.maxZ:
			return 0.0
		else:
			indexX = int((x-self.minX)*self.dx)
			indexZ = int((z-self.minZ)*self.dz)
			return self.height[indexX][indexZ]