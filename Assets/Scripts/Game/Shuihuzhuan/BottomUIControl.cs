using com.yxixia.utile.YxDebug;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class BottomUIControl : MonoBehaviour
    {

        public static BottomUIControl instance;

        public Button autoBtn;

        public Button handBtn;
        
        //跳过按钮
        public Button skip;

        //跳过按钮图片
        public Sprite skip_true;

        public Sprite skip_false;
        

        //跳过动画开关
        public bool skip_bool= false;
          
        /// <summary>
        /// 当局所得
        /// </summary>
        public Text winMoneyText;

        /// <summary>
        /// 总押注数
        /// </summary>
        public Text betNumText;

        /// <summary>
        /// 压注倍数
        /// </summary>
        public Text betBaseText;

        /// <summary>
        /// 线数
        /// </summary>
        public Text LineText;

        private int _everyTimeNum = 0;
        private int _iWinMoney = 0;
        private int _mainMoney = 0;
        private bool _kongzhi = false;
        private void Awake()
        {
            instance = this;
            skip_bool = GetSkipAniState();
            SetSkipStype(skip_bool);
        }

        /// <summary>
        /// 游戏开始--向服务器发送数据
        /// </summary>
        public void BeginTurn()
        {
            Facade.Instance<MusicManager>().Play("gundong2");
            var gdata = App.GetGameData<WmarginGameData>();
            if (gdata.GetPlayerInfo().CoinA >= gdata.BetNum)
            {
                //Debug.LogError("---------------向服务器发送数据--------------------");
                App.GetRServer<WmarginGameServer>().SendMyGameStart(App.GetGameData<WmarginGameData>().BetBaseNum); //服务器发送数据
                if (gdata.IsAuto)
                {
                    GameStateUiControl.instance.Isaudt();
                }
                else
                {
                    GameStateUiControl.instance.StartWait(); //点击开始关闭所有
                }      
            }
            else//如果是自动而且自己的金币少于加注钱，那么就把自动关闭
            {
                if (gdata.IsAuto)
                {
                    gdata.IsAuto = false;
                    Auto(false);
                    GameStateUiControl.instance.LostWait();
                }

            }
        }
        /// <summary>
        /// 大小和数据刷新
        /// </summary>
        public void DaxiaoheFun()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, gdata.GetPlayerInfo().CoinA);
            SetWinMoneyText(gdata.iWinMoney);
        }

        protected void SetWinMoneyText(int coin)
        {
            winMoneyText.text = YxUtiles.GetShowNumberToString(coin);
        }

        protected void SetBetNumText(int coin)
        {
            betNumText.text = YxUtiles.GetShowNumberToString(coin);
        }

        protected void SetBetBaseText(int coin)
        {
            betBaseText.text = YxUtiles.GetShowNumberToString(coin);
        }

        /// <summary>
        ///压注总钱数和我的钱数变化
        /// </summary>
        public void ShuaXin()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            var self = gdata.GetPlayerInfo();
            var coin = self.CoinA - gdata.BetNum;
            self.CoinA = coin;
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, coin);
        }

        /// <summary>
        /// 设置自动
        /// </summary>
        public void AutoBtnFun()
        {
            Auto(true);
            App.GetGameData<WmarginGameData>().IsAuto = true;
            if (App.GetGameData<WmarginGameData>().IsAotozhuangtai)//如果在运行动画就不能进行自动
            {
                YxDebug.LogError("执行");
              if (App.GetGameData<WmarginGameData>().ZhuanState == 1) //刚下注
               {
                   BeginTurn();
                }
            
             if (App.GetGameData<WmarginGameData>().ZhuanState == 2) //输了
             {
                BeginTurn();
             }
             if (App.GetGameData<WmarginGameData>().ZhuanState == 3) //赢了
             {
                 if (App.GetGameData<WmarginGameData>().iWinMoney == 0)
                 {
                     BeginTurn();
                 }
                 else { GetWinMoneyBtnFun(); }
                
             }
        }
    }
        /// <summary>
        /// 点击比倍
        /// </summary>
        public void BigSmallBtnFun()
        {

            BigOrSmallControl.Instance.ThisPlaneFun();
            BigOrSmallControl.Instance.bibeiFun();
            WinPanelControl.instance.winPanel.SetActive(false);
          
        }
        /// <summary>
        /// 点击加注
        /// </summary>
        public void AddBetNumFun()
        {
            App.GetGameData<WmarginGameData>().ZhuanState = 1;
            if (App.GetGameData<WmarginGameData>().BetBaseNum < (App.GetGameData<WmarginGameData>().iXiazhushangxian *10))//压注数不能大于9
            {
                App.GetGameData<WmarginGameData>().BetBaseNum = App.GetGameData<WmarginGameData>().BetBaseNum+App.GetGameData<WmarginGameData>().iXiazhushangxian;
            }
            else App.GetGameData<WmarginGameData>().BetBaseNum = App.GetGameData<WmarginGameData>().iXiazhushangxian;//否则押注数是1
            RefreshBetNum();
            if (App.GetGameData<WmarginGameData>().BetBaseNum != 0)//初始化的时候判断
            {
                GameStateUiControl.instance.BeginButton.interactable = true;//如果是不等于0 那就打开开始
                GameStateUiControl.instance.ZiDongButton.interactable = true;//如果是不等于0 那就打开自动
            }
        }
        /// <summary>
        /// 点击得分
        /// </summary>
        public void GetWinMoneyBtnFun()
        {
            Facade.Instance<MusicManager>().Play("defen");
            GameStateUiControl.instance.BiBeiButton.interactable = false;
            _everyTimeNum = 0;
            var gdata = App.GetGameData<WmarginGameData>();
            _iWinMoney = gdata.iWinMoney;
            _mainMoney = (int)gdata.GetPlayerInfo().CoinA;
            if (_iWinMoney >= 10)
            {
                _everyTimeNum = _iWinMoney / 10;
                _kongzhi = true;
            }
            else _kongzhi = false;
            GetMoney();
        }

        public void GetMoney() //得分动画
        {
            if (_iWinMoney >= 10 || _kongzhi)
            {
                _iWinMoney -= _everyTimeNum;
                _mainMoney += _everyTimeNum;
            }
            else
            {
                _mainMoney = _mainMoney + _iWinMoney;
                _iWinMoney = 0;
            }
            RefreshMoney();
            if (_iWinMoney <= 0)
            {
                var gdata = App.GetGameData<WmarginGameData>();
                Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, gdata.GetPlayerInfo().CoinA);
                SetWinMoneyText(0);
                GameStateUiControl.instance.DeFenWait();//显示赢了
                WinPanelControl.instance.winPanel.SetActive(false);
                if (gdata.IsAuto)
                {
                    GameStateUiControl.instance.StopButton.interactable = false;
                    BeginTurn();
                }
                return;
             }
             Invoke("GetMoney",0.06f);
          
        }
        public void RefreshMoney()//显示金钱和当局所得
        {
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, (long)_mainMoney);
            WinPanelControl.instance.SetWinText(_iWinMoney);
            SetWinMoneyText(_iWinMoney);
        }
        /// <summary>
        /// 手动
        /// </summary>
        public void HandBtnFun()
        {
            Auto(false);
            App.GetGameData<WmarginGameData>().IsAuto = false ;
            GameStateUiControl.instance.Esc.interactable = true;
           

        }
        /// <summary>
        /// 自动和手动设置
        /// </summary>
        /// <param name="isAuto"></param>
        public void Auto(bool isAuto)
        {
            autoBtn.gameObject.SetActive(!isAuto);
            handBtn.gameObject.SetActive(isAuto);
        }
        /// <summary>
        /// 刷新倍数
        /// </summary>
        public void RefreshBetNum()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            var i = gdata.BetBaseNum;
            SetBetNumText(i);
            gdata.BetNum = App.GetGameData<WmarginGameData>().BetLineNum * i;//总压注钱数
            SetBetNumText(gdata.BetNum);
        }
        public void SetMoney()
        {
            var gdata = App.GetGameData<WmarginGameData>();
//            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, gdata.GetPlayerInfo().CoinA);
            LineText.text = gdata.BetLineNum.ToString();
            SetBetBaseText(gdata.BetBaseNum);
            SetBetNumText(gdata.BetBaseNum * gdata.BetLineNum);
        }
        /// <summary>
        /// 显示当前所应的钱数大小和
        /// </summary>
        public void Theincome()
        {

            SetWinMoneyText(0);
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, App.GetGameData<WmarginGameData>().GetPlayerInfo().CoinA);
        }
        /// <summary>
        /// 显示当前所应的钱数大小和
        /// </summary>
        public void TheincomeMali()
        {
            SetWinMoneyText(0);
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, App.GetGameData<WmarginGameData>().GetPlayerInfo().CoinA);
        }
        public void Escfun()
        {
            App.GameManager.OnQuitGameClick();
        }
        /// <summary>
        /// 停止按钮
        /// </summary>
        /// 
        
        public void BtnStop()
        {
            if (skip_bool){return;}
            skip_bool = true;
            Invoke("SkipToFalse",2);
        }

        void SkipToFalse()
        {
            skip_bool = false;
        }

        public void SkipAni()
        {
            skip_bool = !skip_bool;
            SetSkipStype(skip_bool);
            SetSkipAniState(skip_bool);
        }

        public void SetSkipStype(bool isSkip)
        {
            skip.GetComponent<Image>().sprite = isSkip ? skip_true : skip_false;
        }

        public void SetSkipAniState(bool isSkip)
        {
            PlayerPrefs.SetInt(string.Format("{0}_skipanistate", App.GameKey),isSkip?1:0);
        }

        public bool GetSkipAniState()
        {
            return PlayerPrefs.GetInt(string.Format("{0}_skipanistate", App.GameKey), 0)>0;
        }
    }
}
