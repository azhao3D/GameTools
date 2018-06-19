using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzhaoJson
{
    public class JsonData
    {
        private string _type = "";

        public string Type
        {
            get { 
                return _type; 
            }

        }
        private int _count = 0;

        public int Count
        {
            get { return _count; }
        }
        private object content;
        private Dictionary<object, JsonData> contentDict;
        private List<string> keys = new List<string>();

        public JsonData()
        {
            contentDict = new Dictionary<object, JsonData>();
        }

        public void SetType(string t)
        {
            _type = t;
        }

        public void SetValue(object obj)
        {
            content = obj;
            _type = GetTypeByObj(obj);
        }

        public void SetValue(JsonData[] objs)
        {

            _type = DataType.ARRAY;
            _count = 0;
            if(objs!=null&&objs.Length>0)
            {
                _count = objs.Length;
                for(int i = 0;i<objs.Length;i++)
                {
                    //contents.Add(objs[i]);
                    SetValue(i, objs[i]);
                }
            }            
        }

        public void SetValue(List<JsonData> objs)
        {
            _count = 0;
            _type = DataType.ARRAY;
            if(objs!=null&&objs.Count>0)
            {
                _count = objs.Count;
                for (int i = 0; i < objs.Count; i++)
                {
                    SetValue(i,objs[i]);
                }
            }


        }

        public void SetValue(object key,JsonData value)
        {
            if(contentDict.ContainsKey(key))
            {
                contentDict[key] = value;
            }
            else
            {
                contentDict.Add(key, value);
            }
            string ks = key.ToString();
            if(keys.IndexOf(ks)<0)
            {
                keys.Add(ks);
            }
        }



        /// <summary>
        /// 判断传入内容的类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string GetTypeByObj(object obj)
        {
            Type type = obj.GetType();

            return JsonTools.GetDataType(type);
        }

        public JsonData Get(object id)
        {
            if(contentDict.ContainsKey(id))
            {
                return contentDict[id];
            }
            else
            {
                return null;
            }
        }

        private void Set(string id,JsonData obj)
        {
            if(contentDict.ContainsKey(id))
            {
                contentDict[id] = obj;
            }
            else
            {
                contentDict.Add(id, obj);
            }
        }

        public string ToString()
        {
            if(content!=null)
            {
                return content.ToString();
            }
            else
            {
                return "";
            }
        }

        public List<string> GetKeys()
        {
            if(_type!=DataType.OBJECT)
            {
                return null;
            }
            return keys;
        }

        public List<JsonData> GetList()
        {
            if(_type!=DataType.ARRAY)
            {
                return null;
            }
            List<JsonData> list = new List<JsonData>();
            for(int i = 0;i<_count;i++)
            {
                if(contentDict.ContainsKey(i))
                {
                    list.Add(contentDict[i]);
                }      
            }
            return list;
        }


    }
}
