using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.server;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.CommonCode
{
    public class DdzYxMessageBox : MonoBehaviour
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
        /// 卷轴动画
        /// </summary>
        public TweenWidth ScrollAnim;
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

        public void OnCLickLeftBtn()
        {
            App.GetRServer<SanPianGameServer>().StartHandsUp(3);
        }

        public void OnClickRightBtn()
        {
            App.GetRServer<SanPianGameServer>().StartHandsUp(-1);
        }

        /// <summary>
        /// 当前玩家ID
        /// </summary>
        private int localID;
        void OnEnable()
        {
            localID = App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Id;
        }

        public void OnClickMiddleBtn()
        {
            Close();
        }
        private List<HupData> hups;
        /// <summary>
        /// 投票结束标识
        /// </summary>
        bool hupEnd;
        /// <summary>
        /// 显示投票结果内容
        /// </summary>
        public void ShowHandUp(HupData data, int countDown)
        {
            App.GetRServer<SanPianGameServer>().IsInHandsUp = true;
            switch (data.Operation)
            {
                case -1:
                    hupEnd = true;
                    break;
                case 2:
                    gameObject.SetActive(true);
                    ScrollAnim.PlayForward();
                    hups = new List<HupData>();
                    MiddleBtn.SetActive(false);
                    CurTime = countDown;
                    StartCoroutine(CuntDownTime());
                    hupEnd = false;
                    break;
                case 3:
                    break;
            }
            hups.Add(data);
            DealHups();
        }

        private void DealHups()
        {     
            YxDebug.Log("当前的hups数量是:"+hups.Count);
            TitleLabel.text = "";
            Content.text = "";
            foreach (var hup in hups)
            {
                YxDebug.Log(hup.ID);
            }
            if (hups[0].ID.Equals(localID))
            {
                TitleLabel.text = "[581e1e]您正在申请解散游戏，请等待其他玩家操作！[-]\n";
            }
            else
            {
                TitleLabel.text = string.Format("[581e1e]玩家{0}申请解散游戏,您同意解散游戏么?[-]\n", hups[0].Name);
            }
            bool haveHup = false;
            for (int i = 0, max = App.GetGameData<SanPianGameData>().MaxPeopleNum; i < max; i++)
            {
                UserInfo info = App.GetGameManager<SanPianGameManager>().PlayerArr[i].userInfo;
                if (info != null)
                {
                    YxDebug.Log(string.Format("当前玩家{0}的名称是：{1}",i,info.Name));
                    int hupIndex = hups.FindIndex(item => item.ID.Equals(info.Id));
                    YxDebug.Log("hupIndex"+hupIndex);
                    if (hupIndex==0)
                    {
                        continue;
                    }
                    Content.text += string.Format("[581e1e]玩家{0}{1}[-]\n", info.Name, hupIndex>0? hups[hupIndex].Operation == -1 ? "[c22525]拒绝[-]" : "[159026]同意[-]" : "正在选择");                 
                }
            }
            haveHup = hups.Exists(item => item.ID.Equals(localID));
            //button
            RightBtn.SetActive(!haveHup);
            LeftBtn.SetActive(!haveHup);

            //End
            if (hupEnd)
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
            HupData lastData = hups.Last();
            YxDebug.Log("最后投票的玩家是："+lastData.ID);
            YxDebug.Log("当前玩家的ID是：" + localID);
            TitleLabel.text = string.Format("[581e1e]{0}拒绝解散游戏,投票结束![-]\n",lastData.ID==localID?"您":string.Format("玩家{0}",lastData.Name));
            YxDebug.Log("显示的title是："+ TitleLabel.text );
            CountDown.text = "";
            hups.Clear();
            App.GetRServer<SanPianGameServer>().IsInHandsUp =false;
        }

        private EventDelegate _del;
        /// <summary>
        /// 关闭卷轴并删除对象
        /// </summary>
        public void Close()
        {
            _del = new EventDelegate(DirectClose);
            ScrollAnim.AddOnFinished(_del);
            ScrollAnim.PlayReverse();
        }

        /// <summary>
        /// 关闭响应
        /// </summary>
        public Action OnWindowClose;
        /// <summary>
        /// 直接关闭
        /// </summary>
        public void DirectClose()
        {
            ScrollAnim.RemoveOnFinished(_del);
            if (OnWindowClose!=null)
            {
                OnWindowClose();
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 倒计时当前时间
        /// </summary>
        public int CurTime;
        /// <summary>
        /// 是否暂停倒计时
        /// </summary>
        private bool _isStopCountDown;
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
}
