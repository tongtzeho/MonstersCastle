# Monsters Castle Scene Build (Height and Triangle)
# Python 2.7.14

import struct, ctypes

class scene:
	def __init__(self, heightFile, triangleFile, octreeDLLFile):
		self.buildHeight(heightFile)
		self.buildDLL(octreeDLLFile)
		self.buildOctree(triangleFile)
				
	def buildHeight(self, heightFile):
		with open(heightFile, "rb") as fin:
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
				
	def buildDLL(self, octreeDLLFile):
		self.octreeDLL = ctypes.cdll.LoadLibrary(octreeDLLFile)
		self.octreeDLL.buildOctreeFromFile.argtypes = [ctypes.c_char_p, ctypes.c_float, ctypes.c_uint, ctypes.c_float]
		self.octreeDLL.buildOctreeFromFile.restype = ctypes.c_void_p
		self.octreeDLL.capsuleCollideDetection.argtypes = [ctypes.c_void_p, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float]
		self.octreeDLL.capsuleCollideDetection.restype = ctypes.c_bool
		self.octreeDLL.getSegmentDistanceSquare.argtypes = [ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float, ctypes.c_float]
		self.octreeDLL.getSegmentDistanceSquare.restype = ctypes.c_float
		
	def buildOctree(self, triangleFile): # octree<triangle> implemented in octree.dll (2017.7)
		self.octree = self.octreeDLL.buildOctreeFromFile(triangleFile, 1, 8, 1)
			
	def getHeight(self, x, z):
		if x <= self.minX or x >= self.maxX or z <= self.minZ or z >= self.maxZ:
			return 0.0
		else:
			indexX = int((x-self.minX)*self.dx)
			indexZ = int((z-self.minZ)*self.dz)
			return self.height[indexX][indexZ]
			
	def capsuleCollideDetection(self, pos0, pos1, radius): # a sphere with radius moved from pos0(x,y,z) to pos1(x,y,z), return whether collides with a triangle in the scene
		return self.octreeDLL.capsuleCollideDetection(self.octree, pos0[0], pos0[1], pos0[2], pos1[0], pos1[1], pos1[2], radius)
		
	def getSegmentDistanceSquare(self, seg0p0, seg0p1, seg1p0, seg1p1): # 2 segments distance in the space
		return self.octreeDLL.getSegmentDistanceSquare(seg0p0[0], seg0p0[1], seg0p0[2], seg0p1[0], seg0p1[1], seg0p1[2], seg1p0[0], seg1p0[1], seg1p0[2], seg1p1[0], seg1p1[1], seg1p1[2])
