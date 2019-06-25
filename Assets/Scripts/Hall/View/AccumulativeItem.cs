using System;
using System.Collections.Generic;
using Assets.Scripts.Hall.View.TaskWindows;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 满局任务状态条
    /// </summary>
    public class AccumulativeItem : MonoBehaviour
    {
        /// <summary>
        /// 显示的标题
        /// </summary>
        public UILabel ShowTitle;
        /// <summary>
        /// 显示的任务内容
        /// </summary>
        public UILabel ShowInfo;
        /// <summary>
        /// 按钮组
        /// </summary>
        public GameObject[] Btns;
        /// <summary>
        /// 奖励Prefab
        /// </summary>
        public RewardItem PrefabItem;
        /// <summary>
        /// 完成回调
        /// </summary>
        public Action FinishCallBack;
        /// <summary>
        /// 数量文本
        /// </summary>
        public UILabel NumberLabel;
        /// <summary>
        /// 奖励列表
        /// </summary>
        public UIGrid RewardGrid;

        private AccumulativeData _saveData;

        public bool Init(KeyValuePair<string,object> data)
        {
            AccumulativeData saveData=new AccumulativeData(data);
            _saveData = saveData;
            ShowTitle.text = saveData.GameName;
            ShowInfo.text = saveData.ShowText;
            name = saveData.TaskName;
            int lenth = Btns.Length;
            bool finish = saveData.State.Equals(AccumulativeData.FinishState);
            for (int i = 0; i < lenth; i++)
            {
                Btns[i].SetActive(i.Equals(Math.Abs(saveData.State)));
            }
            NumberLabel.text = string.Format("{0}/{1}", saveData.GetNum,saveData.NeedNum);
            while (RewardGrid.transform.childCount>0)
            {
                DestroyImmediate(RewardGrid.transform.GetChild(0).gameObject);
            }
            foreach (KeyValuePair<string,object> rewardItem in saveData.Rewards)
            {
                GameObject obj=NGUITools.AddChild(RewardGrid.gameObject,PrefabItem.gameObject);
                if (obj)
                {
                    RewardItem item = obj.GetComponent<RewardItem>();
                    item.Init(rewardItem.Key,int.Parse(rewardItem.Value.ToString()));
                }
            }
            UIEventListener.Get(Btns[1]).onClick = OnClickGetBtn;
            return finish;
        }

        public void OnClickGetBtn(GameObject obj)
        {
            
            List<string> task=new List<string>();
            task.Add(_saveData.TaskName);
            TaskAccumulativeView.OnClickGetRewardBtn(task);
        }

    }

        /// <summary>
        /// 累计数据
        /// </summary>
        public class AccumulativeData
        {
            /// <summary>
            /// 奖励完成状态值-1
            /// </summary>
            public static int FinishState = -1;
            /// <summary>
            /// 任务名称
            /// </summary>
            public string TaskName;
            /// <summary>
            /// 需要达成的次数
            /// </summary>
            public string NeedNum;
            /// <summary>
            /// 目前的累计次数
            /// </summary>
            public string GetNum;
            /// <summary>
            /// 累计游戏的GameKey
            /// </summary>
            public string Gamekey;
            /// <summary>
            /// 游戏名称
            /// </summary>
            public string GameName;
            /// <summary>
            /// 显示内容
            /// </summary>
            public string ShowText;
            /// <summary>
            /// 任务状态
            /// </summary>
            public int State;
            #region keys
            /// <summary>
            /// 房间参数
            /// </summary>
            private string _keyArgs = "args";
            /// <summary>
            /// 奖励
            /// </summary>
            private string _keyReward = "reward";
            /// <summary>
            /// 显示内容文本
            /// </summary>
            private string _keyShowText = "txt";
            /// <summary>
            /// 显示游戏内容
            /// </summary>
            private string _keyGameName = "name";
            /// <summary>
            /// 需要的数量
            /// </summary>
            private string _keyNeedNum= "num";
            /// <summary>
            /// Gamekey
            /// </summary>
            private string _keyGameKey = "game_key";
            /// <summary>
            /// 任务完成类型 0未完成  -1已完成未领取，-2已领取
            /// </summary>
            private string _keyTaskFinishType = "stat_s";
            /// <summary>
            /// 已完成的次数
            /// </summary>
            private string _keyGetNum = "num_f";
            #endregion
            /// <summary>
            /// 奖励物品
            /// </summary>
            public  Dictionary<string, object> Rewards;
            public AccumulativeData(KeyValuePair<string,object> data)
            {
                TaskName = data.Key;
                Dictionary<string, object> param = (Dictionary<string, object>) data.Value;
                ShowText = param[_keyShowText].ToString();
                Dictionary<string, object> args = (Dictionary<string, object>)param[_keyArgs];
                Rewards =(Dictionary<string, object>)param[_keyReward];
                GameName = param[_keyGameName].ToString();
                #region args
                NeedNum = args[_keyNeedNum].ToString();
                Gamekey = args[_keyGameKey].ToString();
                State = int.Parse(args[_keyTaskFinishType].ToString());
                GetNum = args[_keyGetNum].ToString();
                #endregion
            }
        }
}