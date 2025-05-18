using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Tbs
{
    public class HupMgr : MonoBehaviour
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
        [SerializeField]
        private readonly GameObject _showParent;

        public void OnCLickLeftBtn()
        {
            App.GetRServer<TbsRemoteController>().StartHandsUp(3);
        }

        public void OnClickRightBtn()
        {
            App.GetRServer<TbsRemoteController>().StartHandsUp(-1);
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
            switch (data.Operation)
            {
                case -1:
                    _hupEnd = true;
                    break;
                case 2:
                    _showParent.SetActive(true);
                    _localId = int.Parse(App.GetGameData<TbsGameData>().GetPlayerInfo().UserId);
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
            YxDebug.Log("当前的hups数量是:" + _hups.Count);
            TitleLabel.text = "";
            Content.text = "";
            foreach (var hup in _hups)
            {
                YxDebug.Log(hup.Id);
            }
            var gdata = App.GetGameData<TbsGameData>();
            TitleLabel.text = _hups[0].Id.Equals(_localId) ? "您正在申请解散游戏，请等待其他玩家操作！" : string.Format("玩家{0}申请解散游戏,您同意解散游戏么?", _hups[0].Name);
            for (int i = 0, max = gdata.PlayerList.Length; i < max; i++)
            {
                if (gdata.GetPlayerInfo(i, true) != null)
                {
                    YxDebug.Log(string.Format("当前玩家{0}的名称是：{1}", i, gdata.GetPlayerInfo(i, true).NickM));
                    int hupIndex = _hups.FindIndex(item => item.Id.Equals(gdata.GetPlayerInfo(i, true).Id));
                    YxDebug.Log("hupIndex" + hupIndex);
                    if (hupIndex == 0)
                    {
                        continue;
                    }
                    Content.text += string.Format("[C2B6FF]玩家{0}{1}[-]\n", gdata.GetPlayerInfo(i, true).NickM, hupIndex > 0 ? _hups[hupIndex].Operation == -1 ? "[c22525]拒绝[-]" : "[159026]同意[-]" : "正在选择");
                }
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
            YxDebug.Log("最后投票的玩家是：" + lastData.Id);
            YxDebug.Log("当前玩家的ID是：" + _localId);
            TitleLabel.text = string.Format("[581e1e]{0}拒绝解散游戏,投票结束![-]\n", lastData.Id == _localId ? "您" : string.Format("玩家{0}", lastData.Name));
            YxDebug.Log("显示的title是：" + TitleLabel.text);
            CountDown.text = "";
            _hups.Clear();
        }

        public void Close()
        {
            DirectClose();
        }

        public Action OnWindowClose;
        public void DirectClose()
        {
            _showParent.SetActive(false);
        }
        [HideInInspector]
        public int CurTime;
        /// <summary>
        /// 是否暂停倒计时
        /// </summary>
        private bool _isStopCountDown;

        public HupMgr(GameObject showParent)
        {
            _showParent = showParent;
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
                    CountDown.text = CurTime.ToString(CultureInfo.InvariantCulture);
                    RightBtn.SetActive(false);
                    LeftBtn.SetActive(false);
                    MiddleBtn.SetActive(true);
                    yield break;
                }
                CountDown.text = CurTime.ToString(CultureInfo.InvariantCulture);
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
