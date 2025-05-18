using System;
using System.Globalization;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Tbs
{
    public class TimeMgr : MonoBehaviour
    {
        /*********间隔时间提醒*********/
        public GameObject NextTimeFather;

        public UILabel NtContent;

        public UILabel NtNum;
        /*********骰子点数提醒*********/
        public GameObject DiceNumFather;

        public UILabel DnContent;

        public UILabel DnNum;

        /// <summary>
        /// 提示
        /// </summary>
        public UILabel Tips;


        // Update is called once per frame
        protected void Update()
        {
            CountDownUpdate();
        }

        #region 骰子显示

        private Action _diceAction;
        /// <summary>
        /// 显示骰子点数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="door"></param>
        /// <param name="delayed"></param>
        /// <param name="dType"></param>
        /// <param name="ac"></param>
        public void PlayDiceNum(int num, string door, float delayed, DiceType dType = DiceType.Deal, Action ac = null)
        {
            DiceNumFather.gameObject.SetActive(true);
            DnNum.text = num + "点";

            string info = "";
            switch (dType)
            {
                case DiceType.Deal:
                    info = "本局从" + door + "开始发牌";
                    break;
                case DiceType.Result:
                    info = "本局从" + door + "开始结算";
                    break;
                default:
                    YxDebug.Log("不存在的掷骰子类型!");
                    break;
            }

            DnContent.text = info;
            DnContent.MakePixelPerfect();
            _diceAction = ac;

            Invoke("FinishDice", delayed);
        }

        protected void FinishDice()
        {
            DiceNumFather.gameObject.SetActive(false);
            if (_diceAction != null)
            {
                _diceAction();
            }
        }

        #endregion

        #region 倒计时
        /// <summary>
        /// 本地开始时间
        /// </summary>
        private float _localStartTime;
        /// <summary>
        /// 本地结束时间
        /// </summary>
        private float _localEndTime;
        /// <summary>
        /// 是否开始倒计时
        /// </summary>
        private bool _isCountDown;

        private float _countTime;

        public int CdTime;

        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void BeginCountDown(int cd, string content, bool isShow = true, long st = 0, long ct = 0)
        {
            if (App.GetGameData<TbsGameData>().IsNobody)
            {
                return;
            }

            CdTime = cd;
            _localStartTime = Time.time;
            _localEndTime = _localStartTime + CdTime;
            _localEndTime -= (ct - st) / 1000;
            NtContent.text = content;
            NtContent.MakePixelPerfect();
            _countTime = 0f;
            float surplus = _localEndTime - _localStartTime;
            if (surplus <= 0)
            {
                YxDebug.Log("倒计时服务器已走完");
                return;
            }

            surplus = (float)Math.Round(surplus);
            CdTime = (int)surplus;
            NtNum.text = CdTime.ToString(CultureInfo.InvariantCulture);
            NextTimeFather.SetActive(isShow);
            _isCountDown = true;
        }

        public void CountDownUpdate()
        {
            if (_isCountDown)
            {
                _countTime += Time.deltaTime;
                if (_countTime >= 1f)
                {
                    _countTime = 0f;
                    CdTime--;
                    if (CdTime <= 0)
                    {
                        StopCountTime();
                        return;
                    }
                    NtNum.text = CdTime.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public void OpenTime()
        {
            if (_isCountDown)
            {
                NextTimeFather.SetActive(true);
            }
        }

        public void CloseTime()
        {
            NextTimeFather.SetActive(false);
        }

        public void StopCountTime()
        {
            _isCountDown = false;
            NextTimeFather.SetActive(false);
        }

        #endregion

        /// <summary>
        /// 显示提示
        /// </summary>
        public void ShowTips(string content)
        {
            Tips.gameObject.SetActive(true);
            Tips.text = content;
        }
        /// <summary>
        /// 关闭提示
        /// </summary>
        public void CloseTips()
        {
            Tips.gameObject.SetActive(false);
        }

    }
}
