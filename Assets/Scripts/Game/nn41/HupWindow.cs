using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sfs2X.Entities.Data;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.nn41
{
    public class HupWindow : MonoBehaviour
    {
        /// <summary>
        /// 内容显示
        /// </summary>
        public UILabel Content;
        /// <summary>
        /// 投票显示标签
        /// </summary>
        public UILabel TitleLabel;
        /// <summary>
        /// 倒计时显示
        /// </summary>
        public UILabel CountDown;
        /// <summary>
        /// 右侧按键
        /// </summary>
        public GameObject RightBtn;
        /// <summary>
        /// 左侧按键
        /// </summary>
        public GameObject LeftBtn;
        /// <summary>
        /// 中间按键
        /// </summary>
        public GameObject MiddleBtn;
        /// <summary>
        /// 显示的控制开关
        /// </summary>
        public GameObject ShowParent;
        [HideInInspector]
        public int CurTime;// 倒计时当前时间
        /// <summary>
        /// 是否暂停倒计时
        /// </summary>
        private bool _isStopCountDown;

        public void OnCLickLeftBtn()
        {
            App.GetRServer<NnGameServer>().StartHandsUp(3);
        }

        public void OnClickRightBtn()
        {
            App.GetRServer<NnGameServer>().StartHandsUp(-1);
        }

        /// <summary>
        /// 当前玩家ID
        /// </summary>
        private int _localId;

        public void OnClickMiddleBtn()
        {
            Close();
        }
        private List<HupData> _hups;
        /// <summary>
        /// 投票结束标识
        /// </summary>
        bool _hupEnd;
        /// <summary>
        /// 显示投票结果内容
        /// </summary>
        public void ShowHandUp(HupData data, int countDown)
        {
            var gdata = App.GetGameData<NnGameData>();
            gdata.IsInHandsUp = true;
            switch (data.Operation)
            {
                case -1:
                    _hupEnd = true;
                    break;
                case 2:
                    ShowParent.SetActive(true);
                    _localId = int.Parse(gdata.GetPlayerInfo().UserId);
                    _hups = new List<HupData>();
                    MiddleBtn.SetActive(false);
                    CurTime = countDown;
                    StartCoroutine(CuntDownTime());
                    _hupEnd = false;
                    break;
                case 3:
                    break;
            }
            _hups.Add(data);
            DealHups();
        }

        private void DealHups()
        {
            var gdata = App.GetGameData<NnGameData>();
            TitleLabel.text = "";
            Content.text = "";

            TitleLabel.text = _hups[0].Id.Equals(_localId) ? "[581e1e]您正在申请解散游戏，请等待其他玩家操作！[-]\n" : string.Format("[581e1e]玩家{0}申请解散游戏,您同意解散游戏么?[-]\n", _hups[0].Name);
            foreach (var userInfo in gdata.UserInfoDict)
            {
                var info = (NnUserInfo)userInfo.Value;

                int hupIndex = _hups.FindIndex(item => item.Id.Equals(int.Parse(info.UserId)));

                if (hupIndex == 0)
                {
                    continue;
                }
                Content.text += string.Format("[581e1e]玩家{0}{1}[-]\n", info.NickM, hupIndex > 0 ? _hups[hupIndex].Operation == -1 ? "[c22525]拒绝[-]" : "[159026]同意[-]" : "正在选择");
            }
            bool haveHup = _hups.Exists(item => item.Id.Equals(_localId));

            RightBtn.SetActive(!haveHup);
            LeftBtn.SetActive(!haveHup);
            if (_hupEnd)
            {
                HandupEnd();
            }
        }

        /// <summary>
        /// 投票结束
        /// </summary>
        public void HandupEnd()
        {
            RightBtn.SetActive(false);
            LeftBtn.SetActive(false);
            MiddleBtn.SetActive(true);
            _isStopCountDown = true;
            StopCoroutine(CuntDownTime());
            HupData lastData = _hups.Last();
            //            YxDebug.Log("最后投票的玩家是：" + lastData.Id);
            //            YxDebug.Log("当前玩家的ID是：" + _localId);
            TitleLabel.text = string.Format("[581e1e]{0}拒绝解散游戏,投票结束![-]\n", lastData.Id == _localId ? "您" : string.Format("玩家{0}", lastData.Name));
            //            YxDebug.Log("显示的title是：" + TitleLabel.text);
            CountDown.text = "";
            _hups.Clear();
            App.GetGameData<NnGameData>().IsInHandsUp = false;
        }

        public void Close()
        {
            DirectClose();
        }

        public Action OnWindowClose;
        public void DirectClose()
        {
            ShowParent.SetActive(false);
        }


        public HupWindow(GameObject showParent)
        {
            ShowParent = showParent;
        }

        /// <summary>
        /// 倒计时协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CuntDownTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (_isStopCountDown)
                {
                    CountDown.text = "";
                    _isStopCountDown = false;
                    yield break;
                }
                CurTime--;
                if (CurTime < 0)
                {
                    CurTime = 0;
                    CountDown.text = string.Format("倒计时{0}秒", CurTime.ToString(CultureInfo.InvariantCulture));
                    RightBtn.SetActive(false);
                    LeftBtn.SetActive(false);
                    MiddleBtn.SetActive(true);
                    yield break;
                }
                CountDown.text = string.Format("倒计时{0}秒", CurTime.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
    /// <summary>
    /// 投票数据
    /// </summary>
    public class HupData
    {
        public int Id;
        public string Name;
        public int Operation; //-1:拒绝 2.发起 3同意。

        public void Parse(ISFSObject data)
        {

        }
    }
}
