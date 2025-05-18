using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class BottomPourTable : MonoBehaviour
    {
        /// <summary>
        /// 本局下注的金币数量
        /// </summary>
        public UILabel[] RoundPourLabels;
        /// <summary>
        /// 本局单线下注的金币数量
        /// </summary>
        public UILabel[] LinePourLabels;
        /// <summary>
        /// 彩池中的金币数量
        /// </summary>
        public UILabel[] AllScoreLabels;
        /// <summary>
        /// 本局得了多少金币
        /// </summary>
        public UILabel[] RoundScoreLabels;

        public int AnteTimes//给服务器发送单线下注多少倍
        {
            get { return _ante / _initialAnte; }
        }

        private int _initialAnte;//从服务器接收到单线下注的初始值
        public int InitialAnte
        {
            get { return _initialAnte; }
        }
        private int _ante;//实际单线下注的金币数量,在从服务器接收数据是把它赋值为单线下注的初始值

        public int Ante
        {
            get { return _ante; }
        }
        private int _lineNum;//固定值多少条线

        public int LineNum
        {
            get { return _lineNum; }
        }
        //显示下注界面的初始值,这个不应该在Start中而是在服务器发消息的时候调用一下

        //初始化下注区显示数据
        public virtual void InitData(EventData data)
        {
            if (data.data2 == null)
                return;
            _initialAnte = (int)data.data1;
            _lineNum = (int)data.data2;
            _ante = _initialAnte;
            string initial =YxUtiles.GetShowNumber(_initialAnte).ToString();
            string allAnte = YxUtiles.GetShowNumber((_initialAnte * _lineNum)).ToString();
            int temp0 = allAnte.Length;
            int temp1 = initial.Length;
            ChangePourByValue(temp1, initial, LinePourLabels);
            ChangePourByValue(temp0, allAnte, RoundPourLabels);
        }

        // 修改本局得分的显示
        public virtual void ChangeRoundScore()
        {
            int _roundSceore = App.GetGameData<OverallData>().Response.GetJackpotGold;
            string roundStr = YxUtiles.GetShowNumber(_roundSceore).ToString();
            int length = roundStr.Length;
            ChangePourByValue(length, roundStr, RoundScoreLabels);
        }

        public void ClearRoundScore()
        {
            for (int i = 0; i < RoundScoreLabels.Length; i++)
            {
                RoundScoreLabels[i].text = "0";
            }
        }

        //更新彩池分数
        public virtual void ChangeAllScore(EventData data)
        {
            if (data.data1 == null)
                return;
            long allSceore = (long)data.data1;
            string sceoreStr = YxUtiles.GetShowNumber(allSceore).ToString();
            int length = sceoreStr.Length;
            ChangePourByValue(length, sceoreStr, AllScoreLabels);
        }

        /// <summary>
        /// 点击加号和减号显示单线下注金额与总下注金额
        /// </summary>
        /// <param name="num">0为+  1为-</param>
        public virtual void ChangePourVauleByAdd(string num)
        {
            var gdata = App.GetGameData<OverallData>();
            switch (num)
            {
                case "0":
                    _ante += _initialAnte;
                    long temp = _ante * _lineNum;
                    if (temp > gdata.UserGold)
                    {
                        _ante -= _initialAnte;
                        return;
                    }
                    break;
                case "1":
                    if (_ante == _initialAnte)
                        return;
                    _ante -= _initialAnte;
                    break;
            }
            string anteStr = YxUtiles.GetShowNumber(_ante).ToString();
            string allAnte = YxUtiles.GetShowNumber(_ante * _lineNum).ToString();
            int length0 = anteStr.Length;
            int length1 = allAnte.Length;
            ChangePourByValue(length0, anteStr, LinePourLabels);
            ChangePourByValue(length1, allAnte, RoundPourLabels);
        }
        /// <summary>
        /// 修改各个得分的值
        /// </summary>
        /// <param name="length">分数值得字符长度</param>
        /// <param name="str">分数值的字符串</param>
        protected virtual void ChangePourByValue(int length, string str, UILabel[] targetLabels)
        {
            for (int i = 0; i < targetLabels.Length; i++)
            {
                if (i < length)
                    targetLabels[length - 1 - i].text = str[i].ToString();
                else
                    targetLabels[i].text = "0";
            }
        }
    }
}