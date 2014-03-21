using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Echelon.Core
{
    public sealed class NetworkBrowser
    {
        [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern int NetServerEnum(
            string ServerNane, // must be null
            int dwLevel,
            ref IntPtr pBuf,
            int dwPrefMaxLen,
            out int dwEntriesRead,
            out int dwTotalEntries,
            int dwServerType,
            string domain, // null for login domain
            out int dwResumeHandle
        );
        
        [DllImport("Netapi32", SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern int NetApiBufferFree(IntPtr pBuf);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct ServerInfo
        {
            public readonly int PlatformId;
            [MarshalAs(UnmanagedType.LPWStr)] 
            public readonly string Name;
        }
        
        public NetworkBrowser()
        {
        }

        public IEnumerable<ServerInfo> GetNetworkComputers()
        {
            //local fields
            var networkComputers = new List<ServerInfo>();
            const int MAX_PREFERRED_LENGTH = -1;
            int SV_TYPE_WORKSTATION = 1;
            int SV_TYPE_SERVER = 2;
            IntPtr buffer = IntPtr.Zero;
            IntPtr tmpBuffer = IntPtr.Zero;
            int entriesRead = 0;
            int totalEntries = 0;
            int resHandle = 0;
            int sizeofINFO = Marshal.SizeOf(typeof(ServerInfo));

            try
            {
                //call the DllImport : NetServerEnum 
                //with all its required parameters
                //see http://msdn.microsoft.com/library/
                //default.asp?url=/library/en-us/netmgmt/netmgmt/netserverenum.asp
                //for full details of method signature
                var ret = NetServerEnum(null, 100, ref buffer, MAX_PREFERRED_LENGTH, out entriesRead, out totalEntries, SV_TYPE_WORKSTATION | SV_TYPE_SERVER, null, out resHandle);
                //if the returned with a NERR_Success 
                //(C++ term), =0 for C#
                if (ret == 0)
                {
                    //loop through all SV_TYPE_WORKSTATION 
                    //and SV_TYPE_SERVER PC's
                    for (var i = 0; i < totalEntries; i++)
                    {
                        //get pointer to, Pointer to the 
                        //buffer that received the data from
                        //the call to NetServerEnum. 
                        //Must ensure to use correct size of 
                        //STRUCTURE to ensure correct 
                        //location in memory is pointed to
                        tmpBuffer = new IntPtr((int)buffer + (i * sizeofINFO));
                        //Have now got a pointer to the list 
                        //of SV_TYPE_WORKSTATION and 
                        //SV_TYPE_SERVER PC's, which is unmanaged memory
                        //Needs to Marshal data from an 
                        //unmanaged block of memory to a 
                        //managed object, again using 
                        //STRUCTURE to ensure the correct data
                        //is marshalled 
                        var svrInfo = (ServerInfo)Marshal.PtrToStructure(tmpBuffer, typeof(ServerInfo));

                        //add the PC names to the ArrayList
                        networkComputers.Add(svrInfo);
                    }
                }
            }
            finally
            {
                //The NetApiBufferFree function frees 
                //the memory that the 
                //NetApiBufferAllocate function allocates
                NetApiBufferFree(buffer);
            }
            //return entries found
            return networkComputers;

        }
    }
}