using UnityEngine;

public class Serializer {

	public static void ToBytes(float x, byte[] result, ref int offset) {
		unsafe {
			byte* p = (byte*)(&x);
			result [offset] = p [0];
			result [offset + 1] = p [1];
			result [offset + 2] = p [2];
			result [offset + 3] = p [3];
			offset += 4;
		}
	}

	public static void ToBytes(short x, byte[] result, ref int offset) {
		unsafe {
			byte* p = (byte*)(&x);
			result [offset] = p [0];
			result [offset + 1] = p [1];
			offset += 2;
		}
	}

	public static void ToBytes(Vector3 v, byte[] result, ref int offset) {
		unsafe {
			byte* p = (byte*)(&v);
			result [offset] = p [0];
			result [offset + 1] = p [1];
			result [offset + 2] = p [2];
			result [offset + 3] = p [3];
			result [offset + 4] = p [4];
			result [offset + 5] = p [5];
			result [offset + 6] = p [6];
			result [offset + 7] = p [7];
			result [offset + 8] = p [8];
			result [offset + 9] = p [9];
			result [offset + 10] = p [10];
			result [offset + 11] = p [11];
			offset += 12;
		}
	}
}
