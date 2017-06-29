using System;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{
    // http://www.codeproject.com/Articles/138614/Advanced-Topics-in-PInvoke-String-Marshaling
    public class UTF8Marshaler : ICustomMarshaler
    {
        static UTF8Marshaler static_instance;

        // maybe we could play with WideCharToMultiByte too and avoid Marshal.Copy
        // http://stackoverflow.com/questions/537573/how-to-get-intptr-from-byte-in-c-sharp
        /*
        Byte[] byNewData = null;

        iNewDataLen = NativeMethods.WideCharToMultiByte(NativeMethods.CP_UTF8, 0, cc.ccUserProfilePath, -1, null, 0, IntPtr.Zero, IntPtr.Zero);
        Console.WriteLine("iNewDataLen:" + iNewDataLen + " len:" + cc.ccUserProfilePath.Length + " ulen:" + iNewDataLen);
        byNewData = new Byte[iNewDataLen];
        iNewDataLen = NativeMethods.WideCharToMultiByte(NativeMethods.CP_UTF8, 0, cc.ccUserProfilePath, cc.ccUserProfilePath.Length, byNewData, iNewDataLen, IntPtr.Zero, IntPtr.Zero);

        libcCc.ccUserProfilePath = Marshal.UnsafeAddrOfPinnedArrayElement(byNewData, 0);
     */
        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj == null)
                return IntPtr.Zero;
            if (!(managedObj is string))
                throw new MarshalDirectiveException(
                       "UTF8Marshaler must be used on a string.");

            // not null terminated
            byte[] strbuf = System.Text.Encoding.UTF8.GetBytes((string)managedObj);
            IntPtr buffer = Marshal.AllocHGlobal(strbuf.Length + 1);
            Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

            // write the terminating null
            Marshal.WriteByte(buffer + strbuf.Length, 0);
            return buffer;
        }
        public unsafe object MarshalNativeToManaged(IntPtr pNativeData)
        {
            byte* walk = (byte*)pNativeData;

            // find the end of the string
            while (*walk != 0)
            {
                walk++;
            }
            int length = (int)(walk - (byte*)pNativeData);

            // should not be null terminated
            byte[] strbuf = new byte[length];
            // skip the trailing null
            Marshal.Copy((IntPtr)pNativeData, strbuf, 0, length);
            string data = System.Text.Encoding.UTF8.GetString(strbuf);
            return data;
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public void CleanUpManagedData(object managedObj)
        {
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (static_instance == null)
            {
                return static_instance = new UTF8Marshaler();
            }
            return static_instance;
        }

        [DllImport("kernel32.dll")]
        public static extern int WideCharToMultiByte(uint CodePage, uint dwFlags,
           [MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, int cchWideChar,
           [MarshalAs(UnmanagedType.LPArray)] Byte[] lpMultiByteStr, int cbMultiByte, IntPtr lpDefaultChar,
           IntPtr lpUsedDefaultChar);

        public const uint CP_UTF8 = 65001;
    }
}
