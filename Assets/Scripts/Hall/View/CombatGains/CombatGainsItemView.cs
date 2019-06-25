using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Manager;
using fastJSON;

namespace Assets.Scripts.Hall.View.CombatGains
{
    public class CombatGainsItemView : YxView
    {
        /// <summary>
        /// 序列号
        /// </summary>
        public UILabel SerialLabel;
        /// <summary>
        /// 结果
        /// </summary>
        public UISprite Result;
        /// <summary>
        /// 结果文本
        /// </summary>
        public UILabel ResultLabel;
        /// <summary>
        /// 房间id
        /// </summary>
        public UILabel RoomIdLabel;
        /// <summary>
        /// 日期
        /// </summary>
        public UILabel DateLabel; 
        /// <summary>
        /// 
        /// </summary>
        public InfoData[] InfoDatas;

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var dict = Data as IDictionary;
            if (dict == null) return;
            name = dict.Contains("room_id") ? dict["room_id"].ToString() : "";

            //截取后六位
            if (name.Length > 6) name = name.Substring(name.Length-6, 6);

            if (SerialLabel != null) SerialLabel.text = string.IsNullOrEmpty(Id) ? "-" : Id;
            if (RoomIdLabel != null) RoomIdLabel.text = name;
            if (ResultLabel !=null && dict.Contains("score_n"))
            {
                long score;
                long.TryParse(dict["score_n"].ToString(),out score);
                ResultLabel.text = score>0?"赢":score==0?"平":"输";
            }

            if (DateLabel != null) DateLabel.text = dict.Contains("create_dt") ? dict["create_dt"].ToString() : "----/--/-- --:--:--";
            if (!dict.Contains("info_h")) return;
            var info = dict["info_h"];
            if (info == null) return;
            var infoList = info is string? (List<object>)JSON.Instance.Parse(info.ToString()) :info as List<object>;
            if (infoList == null) return; 
            var count = InfoDatas.Length;
            var len = Mathf.Min(count, infoList.Count);
            var i = 0;
            for (; i < len; i++)
            {
                var idata = InfoDatas[i];
                var obj = infoList[i];
                var iDict = obj as Dictionary<string, object>;
                if(iDict==null)continue;
                if (iDict.ContainsKey("gold"))
                {
                    var goldL = idata.GoldLabel;
                    if (goldL != null)
                    {
                        goldL.text = iDict["gold"].ToString();
                        goldL.gameObject.SetActive(true);
                    }
                }
                if (iDict.ContainsKey("name"))
                {
                    var usnL = idata.UserNameLabel;
                    if (usnL != null)
                    {
                        usnL.text = iDict["name"].ToString();
                        usnL.gameObject.SetActive(true);
                    }
                }
            }
            for (; i < count; i++)
            {
                var idata = InfoDatas[i];
                var goldL = idata.GoldLabel;
                if (goldL != null)
                {
                    goldL.gameObject.SetActive(false);
                }
                var usnL = idata.UserNameLabel;
                if (usnL != null)
                {
                    usnL.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 点击CombatGainsItemView
        /// </summary>
        public void OnItemClick()
        {
            if (Data == null) return;
            var dict = Data as IDictionary;
            if (dict == null) return;
            var win = YxWindowManager.OpenWindow("DefCombatGainsDetailWindow",true);
            if (win == null) return;
            var replayWin = win as CombatGainsDetailWindow;
            if (replayWin == null) return;
            replayWin.UpdateView(name);
        }
    }

    [Serializable]
    public class InfoData
    {
        public UILabel UserNameLabel;
        public UILabel GoldLabel;
    }
}
