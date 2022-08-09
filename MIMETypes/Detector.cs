using MimeDetective;
using MimeDetective.Engine;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MIMETypes
{
	public static class Detector
	{
		[DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
		private static extern int FindMimeFromData(IntPtr pBC,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
			int cbSize,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
			int dwMimeFlags,
			out IntPtr ppwzMimeOut,
			int dwReserved);

		public static string Detect(byte[] content) => detect(content);

		public static string Detect(string filePath)
		{
			using (FileStream stream = File.OpenRead(filePath))
			{
				byte[] content = new byte[1024];
				stream.Read(content, 0, content.Length);
				return detect(content);
			}
		}

		
	}
}
