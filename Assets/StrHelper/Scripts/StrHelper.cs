using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace CStrHelper
{
    public class StrHelper
    {
        private static StrHelper _instance;
        private static object _locker = new object();
        public static StrHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new StrHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private StrHelper()
        {

        }
        private Dictionary<string, string> _strDic = new Dictionary<string, string>();
        private const string _config = "CStrHelperConfig.ini";
        /// <summary>
        /// 加载数据
        /// </summary>
        public void LoadStr()
        {
            string path = string.Format(@"{0}\StrHelper\{1}", Application.streamingAssetsPath, _config);
            string[] strs = File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 0; i < strs.Length; i++)
            {
                string str = strs[i].Trim();
                if (!string.IsNullOrEmpty(str))
                {
                    int signIndex = str.IndexOf(':');
                    string key = str.Substring(0, signIndex).Trim();
                    string value = str.Substring(signIndex + 1).Trim();
                    _strDic[key] = value;
                }
            }
        }
        /// <summary>
        /// 通过key得到str值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetStrByKey(string key)
        {
            if (_strDic != null && _strDic.ContainsKey(key))
            {
                return _strDic[key];
            }
            else
            {
                return null;
            }
        }
    }
}
