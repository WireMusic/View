using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stage.Utils
{
    internal static class StringHelper
    {
        public static unsafe int StrLen(byte* cstr)
        {
            int count = 0;

            while (*cstr != Encoding.UTF8.GetBytes("\0")[0])
            {
                cstr++;
                count++;
            }

            for (int i = 0; i < count; i++)
            {
                cstr--;
            }

            return count;
        }

        public static unsafe byte* CreateString(string str)
        {
            byte[] cstr = Encoding.UTF8.GetBytes(str);
            byte* bytePtr;
            fixed (byte* ptr = cstr)
            {
                bytePtr = ptr;
            }

            return bytePtr;
        }
    }
}
