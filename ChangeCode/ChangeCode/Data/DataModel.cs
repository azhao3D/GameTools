using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeCode.Data
{
    public class DataModel
    {
        static private DataModel _instance;

        public static DataModel Instance
        {
            get { 
                if(_instance==null)
                {
                    _instance = new DataModel();
                }
                return DataModel._instance; 
            }
        }

        private string _path = "";

        public string path
        {
            get { return _path; }
            set { _path = value; }
        }

        private string _exName = "";

        public string exName
        {
            get { return _exName; }
            set { _exName = value; }
        }

        private CodeType _codeType = CodeType.ANSI;

        public CodeType codeType
        {
            get { return _codeType; }
            set { _codeType = value; }
        }

        private CodeType _orgType = CodeType.ANSI;

        public CodeType orgType
        {
            get { return _orgType; }
            set { _orgType = value; }
        }
    }
}
