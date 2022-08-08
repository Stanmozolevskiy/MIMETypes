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

		private static string detect(byte[] content)
		{
			ImmutableArray<DefinitionMatch> results = (new ContentInspectorBuilder { Definitions = MimeDetective.Definitions.Default.All() })
			   .Build().Inspect(content);
			if (results.Any())
				return results.ByMimeType().OrderBy(o => o.Points).First().MimeType;

			if (System.Diagnostics.Debugger.Launch())
				System.Diagnostics.Debugger.Break();

			IntPtr mimeTypePtr = IntPtr.Zero;
			try
			{
				int result = FindMimeFromData(IntPtr.Zero, null, content, content.Length, null, 0, out mimeTypePtr, 0);
				if (result != 0)
					throw Marshal.GetExceptionForHR(result);

				string mimeType = Marshal.PtrToStringUni(mimeTypePtr);
				Marshal.FreeCoTaskMem(mimeTypePtr);
				return mimeType;
			}
			catch
			{
				if (mimeTypePtr != IntPtr.Zero)
					Marshal.FreeCoTaskMem(mimeTypePtr);
				return "unknown/unknown";
			}
		}
	}
}
