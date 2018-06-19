using ChangeCode.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace ChangeCode.Ctrl
{
    public class ChangeCtrl
    {
        static private ChangeCtrl _instance;

        public static ChangeCtrl Instance
        {
            get { 
                if(_instance == null)
                {
                    _instance = new ChangeCtrl();
                }
                return ChangeCtrl._instance; 
            }
        }

        private DataModel _model;

        public DataModel model
        {
            get { 
                if(_model == null)
                {
                    _model = DataModel.Instance;
                }
                return _model; 
            }
        }

        public void ChangeData()
        {
            bool canChange = CheckCanChange();
            if(canChange)
            {
                Change();
            }
        }

        private bool CheckCanChange()
        {
            string path = model.path;
            if(string.IsNullOrEmpty(path))
            {
                SendError("请先选择路径");
                return false;
            }
            if(FileManager.Instance.IsDirectoryExists(path)==false)
            {
                SendError("选择的路径不存在");
                return false;
            }
            string exName = model.exName;
            if(string.IsNullOrEmpty(exName))
            {
                SendError("请先输入扩展名");
                return false;
            }
            return true;
        }

        private void Change()
        {
            try
            {
                string path = model.path;
                string exName = model.exName;
                CodeType ot = model.orgType;
                CodeType ct = model.codeType;
                List<string> paths = FileManager.Instance.GetAllFiles(path, exName);
                Encoding orgCode = GetCode(ot);
                Encoding code = GetCode(ct);
                for (int i = 0; i < paths.Count;i++ )
                {
                    ChangeOne(paths[i], orgCode,code);
                }
                SendComplete(paths.Count);
            }
            catch(Exception e)
            {
                SendError(e.Message);
            }
        }

        private void ChangeOne(string path, Encoding orgCode, Encoding code)
        {
            string str = FileManager.Instance.ReadFileTextByCode(path, orgCode);
            
            FileManager.Instance.SaveFileByCode(path, str, code);
        }

        private Encoding GetCode(CodeType ct)
        {
            if (ct == CodeType.ANSI)
            {
                return Encoding.Default;
            }
            else if (ct == CodeType.Unicode)
            {
                return Encoding.Unicode;
            }
            else if (ct == CodeType.UTF8)
            {
                return Encoding.UTF8;
            }
            else if(ct == CodeType.UTF8_noBom)
            {
                UTF8Encoding code = new UTF8Encoding(false);
                return code;
            }
            return Encoding.Default;
        }

        private void SendError(string content)
        {
            MsgDispatcher.SendMessage(GlobalEventType.CHANGE_RESULT, -1, content);
        }

        private void SendComplete(int num)
        {
            MsgDispatcher.SendMessage(GlobalEventType.CHANGE_RESULT, 0, num.ToString());
        }
    }
}
