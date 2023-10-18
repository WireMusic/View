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

            while (*cstr != '0')
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
    }
}
