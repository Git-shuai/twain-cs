using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TWAINWorkingGroup
{
    public class XkwExtensions
    {
        public static String PathCombine(String path1, String path2, String path3)
        {
            String path = Path.Combine(path1, path2);
            return Path.Combine(path, path3);
        }

        public static IntPtr Add(IntPtr intPtr, int number)
        {
            return new IntPtr(intPtr.ToInt64() + number);
        }
    }
}
