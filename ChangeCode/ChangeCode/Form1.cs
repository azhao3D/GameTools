using ChangeCode.Ctrl;
using ChangeCode.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeCode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private DataModel _model;

        public DataModel model
        {
            get { 
                if(_model==null)
                {
                    _model = DataModel.Instance;
                }
                return _model; 
            }
        }

        private ChangeCtrl _ctrl;

        public ChangeCtrl ctrl
        {
            get { 
                if(_ctrl==null)
                {
                    _ctrl = ChangeCtrl.Instance;
                }
                return _ctrl; 
            }
        }

        private Dictionary<int, CodeType> types;

        #region 界面响应
        private void selectBtn_Click(object sender, EventArgs e)
        {
            SelectPath();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeFile();
        }

        private void exNameTxt_TextChanged(object sender, EventArgs e)
        {
            model.exName = exNameTxt.Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(types.ContainsKey(comboBox1.SelectedIndex))
            {
                model.codeType = types[comboBox1.SelectedIndex];
            }
            else
            {
                model.codeType = CodeType.ANSI;
            }
            
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (types.ContainsKey(comboBox2.SelectedIndex))
            {
                model.orgType = types[comboBox2.SelectedIndex];
            }
            else
            {
                model.orgType = CodeType.ANSI;
            }
        }

        #endregion

        #region 实现

        private void Init()
        {
            MsgDispatcher.AddEventListener(GlobalEventType.CHANGE_RESULT, ChangeResult);
            InitCombo();
            ReflashView();
        }
        private void SelectPath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog()==DialogResult.OK)
            {
                model.path = fbd.SelectedPath;
                ReflashView();
            }
        }

        private void ChangeFile()
        {
            ctrl.ChangeData();
        }

        private void ChangeResult(object[] parms)
        {
            int c = (int)parms[0];
            string errorCode = parms[1].ToString();
            if(c==0)
            {
                ShowTips("转换成功:"+errorCode+"个");
            }
            else
            {
                
                ShowTips("转换失败：" + errorCode);
            }
        }

        private void ReflashView()
        {
            pathTxt.Text = model.path;
            exNameTxt.Text = model.exName;
            comboBox1.SelectedIndex = GetTypeIndex(model.codeType);
            comboBox2.SelectedIndex = GetTypeIndex(model.orgType);
        }

        private int GetTypeIndex(CodeType ct)
        {
            int ind = -1;
            foreach(KeyValuePair<int,CodeType>item in types)
            {
                if(item.Value == ct)
                {
                    ind = item.Key;
                    break;
                }
            }
            if(ind>=0)
            {
                return ind;
            }
            else
            {
                return 0;
            }
        }

        private void InitCombo()
        {
            types = new Dictionary<int, CodeType>();
            comboBox1.Items.Clear();
            string str;
            int ind = 0;
            foreach(CodeType ct in Enum.GetValues(typeof(CodeType)))
            {
                str = ct.ToString();
                types.Add(ind, ct);
                comboBox1.Items.Add(str);
                comboBox2.Items.Add(str);
                ind++;
            }
        }

        #endregion

        #region 显示
        private void ShowTips(string content)
        {
            MessageBox.Show(content);
        }
        #endregion





    }
}
