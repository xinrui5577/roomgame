using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.pdk
{
    public class OutLogTest : MonoBehaviour
    {
        private static OutLogTest _instance;

        public UILabel LogText;

        void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            //在这里做一个Log的监听
            Application.logMessageReceived += HandleLog;


        }


        private List<string> _logtxtList = new List<string>();
        void HandleLog(string logString, string stackTrace, LogType type)
        {

            if (type != LogType.Error && type != LogType.Exception) return;

            _logtxtList.Add(type.ToString() + ":" + logString + "  trace: " + stackTrace);
            if (_logtxtList.Count > 3)
            {
                _logtxtList.RemoveAt(0);
            }
            LogText.text = "";
            foreach (var str in _logtxtList)
            {
                LogText.text += str + "\n\n";
            }


        }



    }
}
