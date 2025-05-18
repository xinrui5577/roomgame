using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjl3d
{
    public class CountDownUI : MonoBehaviour// G  11  15
    { 
        public Sprite[] Numbers;

        private int _timecount;
        // private Text TimeCountDownText;

        private Transform _szApplyRankerEff;
        private Transform _xzApplyRankerEff;

        private Transform _szApplyRankerBtn;
        private Transform _xzApplyRankerBtn;

        public Image SiImage;
        public Image GeImage;

        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            _szApplyRankerEff = transform.FindChild("Particle System_sz");
            if (_szApplyRankerEff == null)
                YxDebug.LogError("No Such Object");//没有该物体 

            _xzApplyRankerEff = transform.FindChild("Particle System_xz");
            if (_xzApplyRankerEff == null)
                YxDebug.LogError("No Such Object");//没有该物体 

            _szApplyRankerBtn = transform.FindChild("shangzhuang");
            if (_szApplyRankerBtn == null)
                YxDebug.LogError("No Such Object");//没有该物体 

            _xzApplyRankerBtn = transform.FindChild("xiazhuang");
            if (_xzApplyRankerBtn == null)
                YxDebug.LogError("No Such Object");//没有该物体 


            Transform tf = transform.FindChild("NumBg/SiImg");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) SiImage = tf.GetComponent<Image>();
            if (SiImage == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("NumBg/GeImg");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) GeImage = tf.GetComponent<Image>();
            if (GeImage == null)
                YxDebug.LogError("No Such  Component");//没有该组件
        }

        /// <summary>
        /// 下注
        /// </summary>
        public void NoticeXiaZhuFun()
        {
            YxDebug.Log("下注");
            var gameCfg = App.GetGameData<Bjl3DGameData>().GameConfig;
            var gameMgr = App.GetGameManager<Bjl3DGameManager>();
            _timecount = gameCfg.XiaZhuTime;//下注时间 15
            gameCfg.IsXiaZhuTime = true;//是否是下注时间
            gameMgr.TheCameraMgr.CameraMoveByPath(0);
            gameMgr.TheUerInfoCountDownLuziUI.HideUIFun();
            gameMgr.TheBetMoneyUI.BetMoneyArea();
        }
        /// <summary>
        /// 发牌
        /// </summary>
        public void SendCardFun()
        {
            YxDebug.Log("send card...");
            var gameCfg = App.GetGameData<Bjl3DGameData>().GameConfig;
            var gameMgr = App.GetGameManager<Bjl3DGameManager>();
            _timecount = gameCfg.KaiPaiTime;
            gameCfg.IsXiaZhuTime = false;
            gameMgr.TheCameraMgr.CameraMoveByPath(1);
            gameMgr.TheUerInfoCountDownLuziUI.HideUIFun(false);
            gameMgr.TheBetMoneyUI.BetMoneyQingKongInfo();
            StartCoroutine("SendCardMoveCameraToDo", 3f);
        }

        IEnumerator SendCardMoveCameraToDo(float s)
        {
            yield return new WaitForSeconds(s);
            App.GetGameManager<Bjl3DGameManager>().ThePlanScene.QingKongChouma();
        }
        /// <summary>
        /// 中奖区域（赢得）
        /// </summary>
        public void ShowWinAreasFun()
        {
            var gameCfg = App.GetGameData<Bjl3DGameData>().GameConfig;
            var gameMgr = App.GetGameManager<Bjl3DGameManager>();
            _timecount = gameCfg.ShowWinTime;
            gameCfg.IsXiaZhuTime = false;
            gameMgr.TheCameraMgr.CameraMoveByPath(4); 
            ShowArea();
            Facade.Instance<MusicManager>().Play("win");
            gameMgr.TheUerInfoCountDownLuziUI.HideUIFun();
        }
        /// <summary>
        /// 打开下拉菜单
        /// </summary>
        public void GameResultFun()
        {
            YxDebug.Log("jie suans...","CountDownUI");
            var gameCfg = App.GetGameData<Bjl3DGameData>().GameConfig;
            _timecount = gameCfg.XiaZhuTime;
            gameCfg.IsXiaZhuTime = false;
            App.GetGameManager<Bjl3DGameManager>().TheUerInfoCountDownLuziUI.ShowUIFun();//打开下拉菜单
            gameCfg.XFapaiSpeedflag = 0;
        }


        /// <summary>
        /// 中奖显示区域
        /// </summary>
        void ShowArea()
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            var betJiesuan = gdata.BetJiesuan;
            var len = betJiesuan.Length;
            var winAreaEffs = App.GetGameManager<Bjl3DGameManager>().TheGameScene.WinAreaEffs;
            for (var i = 0; i < len; i++)
            {
                if (betJiesuan[i] != 0)
                {
                    winAreaEffs[i].gameObject.SetActive(true);
                }
            }
        }

        private float _time;
        protected void Update()
        {
            _time += Time.deltaTime;
            if (_time >= 1.0f)
            {
                if (_timecount > 0)
                    _timecount = _timecount - 1;
                GetTimeCountNumberToImg(_timecount);
                if (App.GetGameData<Bjl3DGameData>().GameConfig.IsXiaZhuTime && _timecount < 4)
                {
                    Facade.Instance<MusicManager>().Play("timeout");
                }
                _time = 0f;
            }
        }
        /// <summary>
        /// 倒计时
        /// </summary>
        /// <param name="count"></param>
        public void GetTimeCountNumberToImg(int count)
        {
            var shiN = count / 10;
            SiImage.sprite = Numbers[shiN];
            var geN = count % 10;
            GeImage.sprite = Numbers[geN];
        }

        private bool _isApply;
        //上下庄申请
        public void ApplyToRankerBtn()
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            var gameUI = App.GetGameManager<Bjl3DGameManager>().TheGameUI;
            var selfInfo = gdata.GetPlayerInfo();
            if (selfInfo.CoinA < gdata.BankLimit)
            {
                var showInfo = string.Format("您至少需要 {0}才能上庄",YxUtiles.ReduceNumber(gdata.BankLimit));
                gameUI.NoteText_Show(showInfo);
                return;
            }
            if (selfInfo.Seat== gdata.CurrentBanker.Seat)
            {
                gameUI.NoteText_Show("这把游戏结束后自动下庄！！！");
                ShowS_X_Image(_isApply);
                App.GetRServer<Bjl3DGameServer>().ApplyQuit();//向服务区器发送下庄请求
                return;
            }
            Facade.Instance<MusicManager>().Play("UpAndDownRanker"); 
            if (App.GetGameManager<Bjl3DGameManager>().TheWaitForRankerListUI.IsApplyRankerOrXiaRanker)//true 可以上庄  flase 不可以上庄
            {

                if (_szApplyRankerEff.gameObject.activeSelf)
                    _szApplyRankerEff.gameObject.SetActive(false);
                _szApplyRankerEff.gameObject.SetActive(true);
                App.GetRServer<Bjl3DGameServer>().ApplyBanker();//向服务区器发送上庄请求
                _isApply = false;
            }
            else
            {
                if (_xzApplyRankerEff.gameObject.activeSelf)
                    _xzApplyRankerEff.gameObject.SetActive(false);
                _xzApplyRankerEff.gameObject.SetActive(true);
                App.GetRServer<Bjl3DGameServer>().ApplyQuit();//向服务区器发送下庄请求
                _isApply = true;
            }
            ShowS_X_Image(_isApply);
        }
        /// <summary>
        /// 上下庄图片显示
        /// </summary>
        /// <param name="isApply"></param>
        public void ShowS_X_Image(bool isApply)
        {
            if (isApply)
            {
                App.GetGameManager<Bjl3DGameManager>().TheWaitForRankerListUI.IsApplyRankerOrXiaRanker = true;
                _xzApplyRankerBtn.gameObject.SetActive(false);
                if (_szApplyRankerBtn.gameObject.activeSelf)
                    _szApplyRankerBtn.gameObject.SetActive(false);
                _szApplyRankerBtn.gameObject.SetActive(true);
            }
            else
            {
                App.GetGameManager<Bjl3DGameManager>().TheWaitForRankerListUI.IsApplyRankerOrXiaRanker = false;
                _szApplyRankerBtn.gameObject.SetActive(false);
                if (_xzApplyRankerBtn.gameObject.activeSelf)
                    _xzApplyRankerBtn.gameObject.SetActive(false);
                _xzApplyRankerBtn.gameObject.SetActive(true);
            }
        }


    }
}