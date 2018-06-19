using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tools
{
    public class ExcelReader
    {
        const string worksheetSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        const string sharedStringSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        static public List<List<string>> ReadExcelFromFile(string path)
        {
            string exName = StringTools.GetExName(path).ToLower();
            if (exName != "xlsx")
            {
                //扩展名不对
                return null;
            }
            NameTable nt = new NameTable();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            //nsManager.AddNamespace("d", worksheetSchema);
            //nsManager.AddNamespace("s", sharedStringSchema);
            SpreadsheetDocument xlDoc = SpreadsheetDocument.Open(path, false);
            List<sheetNameData> sheetNames = GetSheetNamesByDoc(xlDoc);
            if(sheetNames.Count==0)
            {
                //没有内容
                return null;
            }

            List<List<string>> datas = new List<List<string>>();

            for (int i = 0; i < sheetNames.Count;i++ )
            {
                List<List<string>> subdata = GetSheetContents(xlDoc, sheetNames[i]);
                if(subdata!=null)
                {
                    for(int j = 0;j<subdata.Count;j++)
                    {
                        datas.Add(subdata[j]);
                    }
                }
            }

            return datas;

        }

        static private XmlNode GetSheets(XmlNode root)
        {
            XmlNodeList nodeList = root.ChildNodes;
            foreach(XmlNode node in nodeList)
            {
                if(node.LocalName=="sheets")
                {
                    return node;
                }
            }
            return null;
        }


        static private List<sheetNameData> GetSheetNamesByDoc(SpreadsheetDocument xlDoc)
        {
            XmlDocument rootDoc = new XmlDocument();
            rootDoc.Load(xlDoc.WorkbookPart.GetStream());

            XmlNode root = rootDoc.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;
            //XmlNode workBookPart = doc.SelectSingleNode("/xl/workbook");
            XmlNode sheets = GetSheets(root);
            List<sheetNameData> sheetNames = GetSheetNames(sheets);
            return sheetNames;
        }
        static private List<sheetNameData> GetSheetNames(XmlNode sheets)
        {
            List<sheetNameData> sheetNames = new List<sheetNameData>();
            XmlNodeList sheetList = sheets.ChildNodes;
            foreach(XmlNode sheet in sheetList)
            {
                XmlElement subSheet = (XmlElement)sheet;
                sheetNameData snd = new sheetNameData(subSheet.GetAttribute("name").ToString().ToLower(), subSheet.GetAttribute("r:id").ToString());
                sheetNames.Add(snd);
            }
            return sheetNames;
        }

        static private List<List<string>> GetSheetContents(SpreadsheetDocument xlDoc,sheetNameData nameData)
        {
            XmlDocument sheetDoc = new XmlDocument();
            sheetDoc.Load(xlDoc.WorkbookPart.GetPartById(nameData.rid).GetStream());
            XmlNode sheetSize = null;
            XmlNode sheetData = null;
            XmlNode sheetRoot = null;
            XmlNodeList nodeList = sheetDoc.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if(node.LocalName=="worksheet")
                {
                    sheetRoot = node;
                    break;
                }
            }
            if(sheetRoot==null)
            {
                return null;
            }
            nodeList = sheetRoot.ChildNodes;
            foreach(XmlNode node in nodeList)
            {
                if(node.LocalName=="dimension")
                {
                    sheetSize = node;
                }
                if(node.LocalName =="sheetData")
                {
                    sheetData = node;
                }
            }
            if(sheetSize==null||sheetData==null)
            {
                return null;
            }
            XmlElement sheetSizeElement = (XmlElement)sheetSize;
            string refStr = sheetSizeElement.GetAttribute("ref").ToString();
            if (string.IsNullOrEmpty(refStr))
            {
                return null;
            }
            string sizeStr = StringTools.GetFirstMatch(refStr, "(?<=:)[^:]+");
            if (string.IsNullOrEmpty(sizeStr))
            {
                return null;
            }
            string rowStr = StringTools.GetFirstMatch(sizeStr, "[a-z,A-Z]+");
            int rowSize = GetRealNum(rowStr);
            int lineSize =int.Parse( StringTools.GetFirstMatch(sizeStr,"[0-9]+"));

            NameTable nt = new NameTable();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            nsManager.AddNamespace("d", worksheetSchema);
            nsManager.AddNamespace("s", sharedStringSchema);

            List<List<string>> sData = new List<List<string>>();
            for (int i = 0; i < lineSize; i++)
            {
                List<string> lineData = new List<string>();
                for(int j = 0;j<rowSize;j++)
                {
                    lineData.Add(GetCellValue(sheetDoc,nsManager,nt,xlDoc,j,i+1));
                }
                sData.Add(lineData);
            }
            return sData;
        }

        private static int GetRealNum(string str)
        {
            string[] strs = StringTools.GetAllMatchs(str, "[a-z,A-Z]");
            if(strs==null)
            {
                return 0;
            }
            int sum = 0;
            for(int i = 0;i<strs.Length;i++)
            {
                int v = GetKeyCode(strs[strs.Length - i - 1]) + 26 * i;
                sum += v;
            }
            return sum;
        }

        private static int GetKeyCode(string str)
        {
            int cur = (int)Convert.ToChar(str);
            return cur - 64;
        }

        private static string GetCellValue(XmlDocument sheetDoc, XmlNamespaceManager nsManager, NameTable nt, SpreadsheetDocument xlDoc, int x, int y)
        {
            string addressName = GetAddressName(x, y);

            return GetValue(sheetDoc,nsManager,nt,xlDoc,addressName);
        }

        /// <summary>
        /// 获取表格具体的地址
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static public string GetAddressName(int x, int y)
        {

            int bei = x / 26 - 1;
            int yu = x % 26;
            string s1;
            string s2 = getChar(yu);
            if (bei < 0)
            {
                s1 = "";
            }
            else
            {
                s1 = getChar(bei);
            }
            return s1 + s2 + y;
        }
        static private string getChar(int num)
        {
            char c = (char)(num + 65);
            string s1 = new string(c, 1);
            return s1;
        }

        public static string GetValue(XmlDocument sheetDoc, XmlNamespaceManager nsManager, NameTable nt, SpreadsheetDocument xlDoc, string addressName)
        {
            string cellValue = "";
            XmlNode cellNode = sheetDoc.SelectSingleNode(string.Format("//d:sheetData/d:row/d:c[@r='{0}']", addressName), nsManager);
            if (cellNode != null)
            {
                XmlAttribute typeAttr = cellNode.Attributes["t"];
                string cellType = string.Empty;
                if (typeAttr != null)
                {
                    cellType = typeAttr.Value;
                }
                XmlNode valueNode = cellNode.SelectSingleNode("d:v", nsManager);
                if (valueNode != null)
                {
                    cellValue = valueNode.InnerText;
                }
                if (cellType == "b")
                {
                    if (cellValue == "1")
                    {
                        cellValue = "TRUE";
                    }
                    else
                    {
                        cellValue = "FALSE";
                    }
                }
                else if (cellType == "s")
                {
                    if (xlDoc.WorkbookPart.SharedStringTablePart != null)
                    {
                        XmlDocument stringDoc = new XmlDocument(nt);
                        stringDoc.Load(xlDoc.WorkbookPart.SharedStringTablePart.GetStream());
                        //  Add the string schema to the namespace manager. 
                        nsManager.AddNamespace("s", sharedStringSchema);
                        int requestedString = Convert.ToInt32(cellValue);
                        string strSearch = string.Format("//s:sst/s:si[{0}]", requestedString + 1);
                        XmlNode stringNode = stringDoc.SelectSingleNode(strSearch, nsManager);
                        if (stringNode != null)
                        {
                            cellValue = stringNode.InnerText;
                        }
                    }
                }
            }
            return cellValue;

        }

    }


    
    public class sheetNameData
    {
        public string name;
        public string rid;
        public sheetNameData(string n,string r)
        {
            name = n;
            rid = r;
        }
    }
}
