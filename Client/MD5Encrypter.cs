using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class MD5Encrypter {

	MD5 md5;

	public MD5Encrypter() {
		md5 = MD5.Create ();
	}

	public string Encrypt(string input) {
		byte[] data = md5.ComputeHash (Encoding.UTF8.GetBytes (input));
		StringBuilder builder = new StringBuilder ();
		for(int i = 0; i < data.Length; ++i)
		{
			builder.Append (data [i].ToString ("x2"));
		}
		return builder.ToString ().ToUpper ();
	}
}
