using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace CStrHelper
{
    public class StrHelperWindow : EditorWindow
    {
        private Dictionary<string, string> _strDic = new Dictionary<string, string>();
        private const string _config = "CStrHelperConfig.ini";
        private string _strDefPath = @"E:\StrHelper\Assets\StrHelper\Scripts\StrDef.cs";

        #region About Window
        private string _addKeyStr;
        private string _addValueStr;
        #endregion
        public void Init()
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
        private void OnGUI()
        {
            GUILayout.Label("Key          Value");
            foreach (string keyStr in _strDic.Keys)
            {
                StringBuilder sb = new StringBuilder();
                int sbLength = 14 - keyStr.Length;
                for (int i = 0; i < sbLength; i++)
                {
                    sb.Append(" ");
                }
                GUILayout.Label(string.Format("{0}{1}{2}", keyStr, sb.ToString(), _strDic[keyStr]));
            }
            _addKeyStr = EditorGUILayout.TextField("AddKey", _addKeyStr);
            _addValueStr = EditorGUILayout.TextField("AddValue", _addValueStr);
            if (GUILayout.Button("Add",GUILayout.Width(200)))
            {
                OnAddButtonClick();
            }
            if (GUILayout.Button("OK", GUILayout.Width(200)))
            {
                OnSubmitButtonClick();
            }
        }

        [MenuItem("Tools/StrHelper")]
        private static void OpenWindow()
        {
            Rect rect = new Rect(0, 0, 500, 500);
            StrHelperWindow window = (StrHelperWindow)EditorWindow.GetWindowWithRect(typeof(StrHelperWindow), rect, true, "StrHelper");
            window.Init();
            window.Show();
        }
        private void OnSubmitButtonClick()
        {
            SaveConfig();
            ChangeStrDef();
            this.Close();
        }
        private void OnAddButtonClick()
        {
            _addKeyStr = _addKeyStr.Trim();
            if(_strDic.ContainsKey(_addKeyStr))
            {
                this.ShowNotification(new GUIContent("The key exists"));
            }
            else
            {
                _strDic.Add(_addKeyStr, _addValueStr);
            }
            _addKeyStr = string.Empty;
            _addValueStr = string.Empty;
        }
        private void SaveConfig()
        {
            string[] strs = new string[_strDic.Count];
            if (_strDic != null)
            {
                int i = 0;
                foreach (string keyStr in _strDic.Keys)
                {
                    strs[i++] = string.Format("{0}:{1}", keyStr, _strDic[keyStr]);
                }
            }
            string path = string.Format(@"{0}\StrHelper\{1}", Application.streamingAssetsPath, _config);
            File.WriteAllLines(path, strs, Encoding.UTF8);
        }
        private void ChangeStrDef()
        {
            string[] originStrs = File.ReadAllLines(_strDefPath);
            int startIndex = 0;
            int endIndex = 0;
            int outStartIndex = 0;
            for (int i = 0; i < originStrs.Length; i++)
            {
                if (originStrs[i].Trim() == "//Start")
                {
                    startIndex = i;
                    outStartIndex = originStrs[i].IndexOf('/');
                }
                if (originStrs[i].Trim() == "//End")
                {
                    endIndex = i;
                }
            }
            List<string> outStrList = new List<string>();
            for (int i = 0; i <= startIndex; i++)
            {
                outStrList.Add(originStrs[i]);
            }
            foreach (string strKey in _strDic.Keys)
            {
                string outStrLine = string.Format("        public static string {0} = StrHelper.Instance.GetStrByKey(\"{0}\");", strKey);
                outStrList.Add(outStrLine);
            }
            for (int i = endIndex; i < originStrs.Length; i++)
            {
                outStrList.Add(originStrs[i]);
            }
            File.WriteAllLines(_strDefPath, outStrList.ToArray());
        }
    }
}
