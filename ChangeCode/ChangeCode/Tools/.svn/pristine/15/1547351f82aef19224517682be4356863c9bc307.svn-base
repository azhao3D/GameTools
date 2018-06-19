using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class MouldHelper
    {
        static public string GetMould(string mouldName)
        {
            string mouldPath = FileManager.Instance.GetMouldPath(mouldName);

            string mouldStr = FileManager.Instance.ReadFileText(mouldPath);
            if (string.IsNullOrEmpty(mouldStr))
            {
                return "";
            }
            return mouldStr;
        }
    }
}
