using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using Random = UnityEngine.Random;
using System.Collections;

namespace Assets.Scripts.Game.fruit
{
    public class LightItemCtrl : MonoBehaviour
    {
        public enum ItemAnimState
        {
            Sleep = 0,
            Start,
            Run,
            End,
            FinalEnd,
            Casino,//比大小状态
            GoodLuck,//goodLuck状态，大三元，小三元等可能情况
            RunchooInit,//跑火车初始化
            RunchooShining,//炮火车闪烁
            Runchoo,//炮火车
            Fresh,
        }
        public GameObject[] LightItems;

        private ItemAnimState _itemAnimState;
        public ItemAnimState AnimState
        {
            get { return _itemAnimState; }
        }

        private int _itemPosI;

        private const float TimeLength = 1f;

        private float _timeScale;
        [SerializeField]private float _speedRate;
        private float _runTime;

        private GameObject _startBtn;
        //public GameObject CasinoText;
        public GameObject CasinoImgLeft;
        public GameObject CasinoImgRight;
        private readonly SlotInfo _slotInfo = SlotInfo.GetSlotInfo();

        //比倍彩金按钮
        public GameObject HalfBtn;//减半按钮
        public GameObject DoublBtn;//加倍按钮
        public GameObject ClearBtn;//清空按钮

        //运行比大小按钮
        private GameObject _btnBig;
        private GameObject _btnSmall;

        public GameObject LightItemBack;
        public GameObject BoxShiningPrefab;

        //box闪烁gameobjects
        public GameObject[] BoxShiningGobs;
         
        private readonly float[] _boxPosList = 
        {
            -273.8336f,229.8998f,
            -182.5889f,229.8998f,
            -91.34474f,229.8998f,
            -0.09997559f,229.8998f,
            91.14417f,229.8998f,
            182.3889f,229.8998f,
            273.6337f,229.8998f,
            364.8778f,229.8998f,

            364.8778f,128.4333f,
            364.8778f,26.59975f,
            364.8778f,-75.23315f,

            364.8778f,-177.6001f,
            273.6324f,-177.6001f,
            182.3876f,-177.6001f,
            91.14352f ,-177.6001f,
            -0.1012421f, -177.6001f,
            -91.34537f, -177.6001f,
            -182.5901f, -177.6001f,
            -273.8343f,-177.6001f,
            -365.079f,-177.6001f,

            -365.079f,-75.23315f,
            -365.079f,26.59975f,
            -365.079f,128.4333f,
            -365.079f,229.8998f
        };

        //事件句柄
        public event EventHandler eve_Win;
        public event EventHandler eve_Lose;
        public event EventHandler eve_shakeBtnFrame;


       protected void Awake()
       {
           Facade.EventCenter.AddEventListener<FruitEventType, UiTeventArgs>(FruitEventType.ChipUpdate, ChangeitemAnimState);
           Facade.EventCenter.AddEventListener<FruitEventType,object>(FruitEventType.CasinoResult, OnGetCasinoResult);
        }

        // Use this for initialization
       protected void Start()
        {
            //事件绑定
            eve_Win += DisPlayDxResult.getInstance().onBdxWin;
            eve_Lose += DisPlayDxResult.getInstance().onBdxLose;
            eve_Win += NewLightsBlink.getInstance().onWinLights;
            eve_Lose += NewLightsBlink.getInstance().onLoseLights;
            eve_run += NewLightsBlink.getInstance().onRunLights;
            eve_End += NewLightsBlink.getInstance().onEndLights;
            eve_shakeBtnFrame += NewLightsBlink.getInstance().CtrlBtnFrameShake;

            _itemAnimState = ItemAnimState.Sleep;
            foreach (var item in LightItems)
            {
                item.SetActive(false);
            }
            //for (int i = 0; i < LightItems.Length; i++)
            //{
            //    LightItems[i].SetActive(false);

            //}
            LightItems[0].GetComponent<Image>().rectTransform.localPosition = new Vector3(_boxPosList[0], _boxPosList[1]);
            _startBtn = GameObject.Find("startButton");
            //绑定开始按钮
            EventTriggerListener.Get(_startBtn).OnClick = OnStartClick;

            //绑定加倍减半按钮
            EventTriggerListener.Get(HalfBtn).OnClick = OnHalfBtnClick;
            EventTriggerListener.Get(DoublBtn).OnClick = OnDoubleBtnClick;
            EventTriggerListener.Get(ClearBtn).OnClick = OnClearBtnClick;

            _btnBig = GameObject.Find("btn_big");
            _btnSmall = GameObject.Find("btn_Small");
            //比大小按钮
            EventTriggerListener.Get(_btnBig).OnClick = OnBtnBigClick;
            EventTriggerListener.Get(_btnSmall).OnClick = OnBtnSmallClick;
            //slotInfo.GameState = itemAnimState;//获得游戏当前的状态
            InitGoodLuckAnimBoxes();

        }

        private void InitGoodLuckAnimBoxes()
        {
            BoxShiningGobs = new GameObject[_boxPosList.Length / 2];
            var j = 1;
            for (var i = 0; i < _boxPosList.Length - 1; i += 2)
            {

                GameObject gobitem = Instantiate(BoxShiningPrefab);
                gobitem.transform.SetParent(LightItemBack.transform);
                var trans = gobitem.GetComponent<Transform>();
                trans.localPosition = new Vector2(_boxPosList[i], _boxPosList[i + 1]);
                trans.localScale = new Vector3(1, 1, 1);
                BoxShiningGobs[j] = gobitem;
                gobitem.SetActive(false);
                if (j < BoxShiningGobs.Length - 1)
                {
                    j++;
                }
                else
                {
                    j = 0;
                }
            }
        }

        bool admitShow;  //控制showOneTime
        IEnumerator showOneTime()
        {
            if (admitShow)
            {
                admitShow = false;

                foreach (var t in BoxShiningGobs)
                {
                    t.SetActive(true);
                }

                yield return new WaitForSeconds(2);

                foreach (var t in BoxShiningGobs)
                {
                    t.SetActive(false);
                }
            }
        }

        int goodLuckIdx;
        //出现n个选中水果的动画图标
        private void GoodLuckAnimMultiShow()
        {
            if (_runTime > 2f)
            {
                itemAnim_run();
            }

            if (_runTime > 7 && (_itemPosI / 2 == SlotInfo.GoodLuckPoints[goodLuckIdx]) && goodLuckIdx < SlotInfo.GoodLuckPoints.Count)
            {  
                List<int> itemIlist = SlotInfo.GoodLuckPoints;

                //Debug.Log("box_i:" + itemIlist[goodLuckIdx]);
                BoxShiningGobs[itemIlist[goodLuckIdx]].SetActive(true);

                //视觉上取消最后一个框，不与跑马灯所在的框重合
                if (goodLuckIdx < SlotInfo.GoodLuckPoints.Count - 1)
                    BoxShiningGobs[itemIlist[goodLuckIdx]].GetComponentInChildren<Image>().enabled = true;

                //BoxShiningGobs[itemIlist[goodLuckIdx]].GetComponent<BoxShining>().TimeRate = 0.1f;
                //BoxShiningGobs[itemIlist[goodLuckIdx]].GetComponent<BoxShining>().ChangeBoxColor(new Color(1f, 1f, 0));

                goodLuckIdx++;

                if (goodLuckIdx < SlotInfo.GoodLuckPoints.Count)
                {
                    _runTime = 0;

                    //隐藏尾迹
                    for (int i = 1; i < LightItems.Length; i++)
                    {
                        LightItems[i].SetActive(false);
                    }

                    return;
                }
                else
                {
                    goodLuckIdx = 0;
                    _itemAnimState = ItemAnimState.Fresh;
                    eve_shakeBtnFrame(this, EventArgs.Empty);
                }
               
                //隐藏尾迹
                for (int i = 1; i < LightItems.Length; i++)
                {
                    LightItems[i].SetActive(false);
                }

                return;
            }
            _runTime += Time.fixedDeltaTime;
        }


        private void OnBtnBigClick(GameObject gob)
        {
            var gdata = App.GameData;
            var player = gdata.GetPlayer();
            var winCoin = player.WinCoin;
            if (_itemAnimState != ItemAnimState.Sleep) return;
            Facade.Instance<MusicManager>().Play("button");
            _casinoRunEnd = null; 
            if (_itemAnimState != ItemAnimState.Sleep || winCoin <= 0) return;
            _itemAnimState = ItemAnimState.Casino;
            _timeScale = TimeLength;
            _speedRate = 100f;
            _runTime = 0;
            App.GetRServer<FruitGameServer>().SendBiDxPlay((int)winCoin,1);
            InvokeRepeating("PlayDiuDiu",0,0.3f);
        }
        private void OnBtnSmallClick(GameObject gob)
        {
            if (_itemAnimState != ItemAnimState.Sleep) return;
            var gdata = App.GameData;
            var player = gdata.GetPlayer();
            var winCoin = player.WinCoin;
            Facade.Instance<MusicManager>().Play("button");
            _casinoRunEnd = null;
            if (_itemAnimState != ItemAnimState.Sleep || winCoin <= 0)
            {
                return;
            }
            _itemAnimState = ItemAnimState.Casino;
            _timeScale = TimeLength;
            _speedRate = 100f;
            _runTime = 0;
            App.GetRServer<FruitGameServer>().SendBiDxPlay((int)winCoin, 0);
            InvokeRepeating("PlayDiuDiu", 0, 0.3f);

        }

        public void PlayDiuDiu()
        {
            Facade.Instance<MusicManager>().Play("diudiu");
        }


        //减半按钮
        private void OnHalfBtnClick(GameObject gob)
        {
            if (_itemAnimState != ItemAnimState.Sleep) return;
            var gdata = App.GameData;
            var player = gdata.GetPlayer();
            Facade.Instance<MusicManager>().Play("button");
            var winMoney = player.WinCoin;
            if (winMoney <= 1) return;
            var halfwinmoney = winMoney / 2;//获得赢钱的数
            if (halfwinmoney <= 0) return;
            player.Coin += halfwinmoney; 

            //减半赢钱数
            var curWinmoney = winMoney - halfwinmoney;
            player.WinCoin = curWinmoney; 
        }
        //加倍按钮
        private void OnDoubleBtnClick(GameObject gob)
        {
            if (_itemAnimState != ItemAnimState.Sleep) return;
            var gdata = App.GameData;
            var player = gdata.GetPlayer();
            Facade.Instance<MusicManager>().Play("button");
            var winMoney = player.WinCoin;
            if (winMoney <= 0)
                return;

            if (winMoney <= player.Coin)
            {
                player.Coin -= winMoney;
                //加倍赢钱数赢钱数
                var curWinmoney = winMoney * 2;
                player.WinCoin = curWinmoney; 
            }
        }

        private void OnClearBtnClick(GameObject gob)
        {
            Facade.EventCenter.DispatchEvent<string, object>("AnimStateFresh");
        }


        private void OnStartClick(GameObject gob)
        {
            if (_itemAnimState != ItemAnimState.Sleep) return;
            var gdata = App.GetGameData<FruitGameData>();
            var player = gdata.GetPlayer();
            Facade.Instance<MusicManager>().Play("button"); 
            if (player.WinCoin > 0)
            {
                App.GetRServer<FruitGameServer>().SendRestart();
                return;
            }
            FruitSlotInfoCtrl.HasClearFruitList = false;

            var dict = new Dictionary<FruitType, int>();
            var fruitSlotDic = SlotInfo.FruitSlotList;
            int useMoney = 0;
            
            foreach (var fruitkey in fruitSlotDic.Keys)
            {

                useMoney += fruitSlotDic[fruitkey] * gdata.Ante;
                if (useMoney > player.Coin)
                {
                    YxMessageBox.Show("您的金币不足！！！");
                    return;
                }
                dict[fruitkey] = fruitSlotDic[fruitkey];
            }

            if (useMoney <= 0)
            {
                return;
            }
            _itemAnimState = ItemAnimState.Start;
            //点击开始按钮后，不允许押注了
            _slotInfo.SlotEnable = false;
            player.Coin -= useMoney;
            player.WinCoin = 0;
            //执行押注play逻辑
            App.GetRServer<FruitGameServer>().SendYazhuPlay(dict);

        }

        //灯在转的时候发事件
        public event EventHandler eve_run;
        public event EventHandler eve_End;

        // FixedUpdate is called once per time
        protected void FixedUpdate()
        {
            switch (_itemAnimState)
            {
                case ItemAnimState.Sleep:
                    {
                        _slotInfo.SlotEnable = true;
                        break;
                    }
                case ItemAnimState.Start:
                    {
                        //发事件=>控制下新灯泡
                        eve_run(this, EventArgs.Empty);

                        //隐藏goodluck闪烁box
                        foreach (var t in BoxShiningGobs)
                        {
                            //t.GetComponent<BoxShining>().ResetBox();
                            t.SetActive(false);
                            t.GetComponentInChildren<Image>().enabled = false;
                        }

                        _slotInfo.SlotEnable = false;

                        _itemAnimState = ItemAnimState.Run;
                        _timeScale = TimeLength;
                        _speedRate = 1f;
                        LightItems[0].SetActive(true);
                        _runTime = 0;
                        //CasinoText.GetComponent<Text>().text = "00";//清零上次比大小的数值显示  --last version
                        this.numToImage(0);
                        break;
                    }
                case ItemAnimState.Run:
                    {
                        _slotInfo.SlotEnable = false;
                        itemAnim_run();
                        break;
                    }
                case ItemAnimState.End:
                    {
                        _slotInfo.SlotEnable = false;
                        itemAnim_end(SlotInfo.LotteryPoint);//里面参数设置指定中奖的水果
                        break;
                    }
                case ItemAnimState.FinalEnd:
                    {
                        //发事件=>控制下新灯泡
                        eve_End(this, EventArgs.Empty);

                        itemAnim_FinalEnd(SlotInfo.LotteryPoint);
                        break;
                    }
                //运行比大小的动画状态
                case ItemAnimState.Casino:
                    {
                        CasinoRun();
                        break;
                    }
                case ItemAnimState.GoodLuck:
                    {
                        StartCoroutine(showOneTime());
                        GoodLuckAnimMultiShow();
                        break;
                    }
                //炮火车准备
                case ItemAnimState.RunchooInit:
                    {

                        _itemAnimState = ItemAnimState.RunchooShining;
                        _runTime = 0;
                        foreach (var t in BoxShiningGobs)
                        {
                            t.SetActive(true);
                        }
                        PlayTrainSound();
                        break;
                    }
                //炮火车闪烁
                case ItemAnimState.RunchooShining:
                    {

                        _runTime += Time.fixedDeltaTime;
                        if (_runTime > 1.5f)
                        {
                            _runTime = 0;
                            _speedRate = 60;
                            _itemAnimState = ItemAnimState.Runchoo;
                            foreach (var t in BoxShiningGobs)
                            {
                                t.GetComponentInChildren<Image>().enabled = false;
                                t.SetActive(false);
                            }
                        }


                        break;
                    }

                //跑火车
                case ItemAnimState.Runchoo:
                    {

                        itemAnim_run();

                        RunChooAnimEnd();


                        break;
                    }
                case ItemAnimState.Fresh:
                    {
                        var gdata = App.GameData;
                        var player = gdata.GetPlayer();
                        var userInfo = player.GetInfo<YxBaseGameUserInfo>();
                        player.Coin = userInfo.CoinA;
                        player.WinCoin = userInfo.WinCoin;
                        _itemAnimState = ItemAnimState.Sleep;
                        Facade.EventCenter.DispatchEvent<string,object>("AnimStateFresh"); 
                        break;
                    }
            }


        }

        private void RunChooAnimEnd()
        {
            if (_runTime > 1.5f && (_itemPosI / 2 == SlotInfo.GoodLuckPoints[0]))
            {
                foreach (var t in BoxShiningGobs)
                {
                    t.SetActive(false);
                }
                List<int> itemIlist = SlotInfo.GoodLuckPoints;
                foreach (int boxI in itemIlist)
                {
                    Debug.Log("box_i:" + boxI);
                    BoxShiningGobs[boxI].SetActive(true);
                    BoxShiningGobs[boxI].GetComponentInChildren<Image>().enabled = true;
                    //BoxShiningGobs[boxI].GetComponent<BoxShining>().TimeRate = 0.1f;
                    //BoxShiningGobs[boxI].GetComponent<BoxShining>().ChangeBoxColor(new Color(1f, 1f, 0));
                }
                _itemAnimState = ItemAnimState.Fresh;
                eve_shakeBtnFrame(this, EventArgs.Empty);
                //隐藏尾迹
                for (int i = 1; i < LightItems.Length; i++)
                {
                    LightItems[i].SetActive(false);
                }

                return;
            }
            _runTime += Time.fixedDeltaTime;
        }


        private int _randType = -1;
        private Action _playWinSound;


        public void PlayBswinSound()
        {
            Facade.Instance<MusicManager>().Play("bswin");
        }

        public void PlayW2Sound()
        {
            Facade.Instance<MusicManager>().Play("w2");
        }

        public void PlayTrainSound()
        {
            Facade.Instance<MusicManager>().Play("train");
        }

        public void PlayChiDiao()
        {
            Facade.Instance<MusicManager>().Play("chidiao");
        }

        private void ChangeitemAnimState(UiTeventArgs e)
        {
            if (e == null) return;
            if (e.AnimState != ItemAnimState.End) return; 
            _randType = e.RandType;//是否出现的goodluck类型；跑火车或者大四喜

            if (!e.HasWin)
            {
                _playWinSound = null;
            }
            else
            {
                if (_randType == -1)
                {
                    _playWinSound = PlayBswinSound;
                }
                else
                {
                    _playWinSound = PlayW2Sound;
                }
            }

            Invoke("PlayAnimstateEnd", 3f);
        }

        protected void PlayAnimstateEnd()
        {
            _itemAnimState = ItemAnimState.End;
            if (_speedRate > 50)
            {
                _speedRate = 50;
            }
            for (int i = 1; i < LightItems.Length; i++)
            {
                LightItems[i].SetActive(false);
            } 
        }

        private void itemAnim_run()
        {
            _runTime += Time.fixedDeltaTime;
            if (_timeScale > 0)
            {
                _timeScale -= Time.fixedDeltaTime * _speedRate;

                if (_speedRate < 10)
                {
                    _speedRate += Time.fixedDeltaTime * 10 > .1f ? Time.fixedDeltaTime * 10 : .1f;
                }
                else if (_speedRate < 100)
                {
                    _speedRate += 5;
                }
                    
            }
            else
            {
                int posi = _itemPosI;

                if (_speedRate < 20)
                {
                    SetLightItemPos(ref posi, 0);
                }
                else if (_speedRate < 40)
                {
                    SetLightItemPos(ref posi, 0);
                    SetLightItemPos(ref posi, 1);

                }
                else if (_speedRate < 60)
                {
                    SetLightItemPos(ref posi, 0);
                    SetLightItemPos(ref posi, 1);
                    SetLightItemPos(ref posi, 2);
                }
                else
                {
                    SetLightItemPos(ref posi, 0);
                    SetLightItemPos(ref posi, 1);
                    SetLightItemPos(ref posi, 2);
                    SetLightItemPos(ref posi, 3);
                }


                if (_itemPosI < _boxPosList.Length - 2)
                {
                    _itemPosI += 2;
                }
                else
                {
                    _itemPosI = 0;
                }

                PlayDingSds();
                _timeScale = TimeLength;
            }
        }

        private void PlayDingSds()
        {
            Facade.Instance<MusicManager>().Play("s3");
        }

        // private float timeAccumulate = 0;
        private void itemAnim_end(int fixitemi)
        {

            int distance = Mathf.Abs(fixitemi - (_itemPosI / 2));

            _timeScale -= Time.fixedDeltaTime * _speedRate;

            if (distance > 5 && distance < 12)
            {
                _itemAnimState = ItemAnimState.FinalEnd;

                return;
            }

            if (_timeScale <= 0)
            {
                LightItems[0].SetActive(true);
                LightItems[0].GetComponent<Image>().rectTransform.localPosition = new Vector3(_boxPosList[_itemPosI], _boxPosList[_itemPosI + 1]);



                if (_itemPosI < _boxPosList.Length - 2)
                {
                    _itemPosI += 2;
                }
                else
                {
                    _itemPosI = 0;
                }
                PlayDingSds();
                _timeScale = TimeLength;
            }

        }


        private void itemAnim_FinalEnd(int fixitemi)
        {
            // Debug.Log("fixItemI: " + fixitemi);
            // Debug.Log("itemPos_i / 2: " + itemPos_i / 2);

            float fiexdDeltTime = Time.fixedDeltaTime;
            if (_timeScale > 0)
            {
                //float distance = Mathf.Abs(fixitemi - (_itemPosI / 2)) * 5f;  //--该bug导致：distance可能越来越大从而灯的速度降不下来
                float distance = (fixitemi - (_itemPosI / 2)) * 5f > 0 ? (fixitemi - (_itemPosI / 2)) * 5f
                   : (24 - _itemPosI / 2 + fixitemi) * 5f;

                if (distance <= _speedRate)
                {
                    _speedRate = distance;
                }

                if (_speedRate < 16f)
                    _speedRate = 3f;



                _timeScale -= fiexdDeltTime * _speedRate;
                //Debug.LogError("itemPos_i" + itemPos_i);
                if (_itemPosI / 2 == fixitemi)
                {
                    //test
                    /*                bool test = true;
                                    _randType = 1;
                                    SlotInfo.GoodLuckPoints.Add(1); SlotInfo.GoodLuckPoints.Add(3); SlotInfo.GoodLuckPoints.Add(7);
                                    PlayW2Sound();*/

                    _runTime = 0;
                    if (fixitemi == _slotInfo.LuckPointLeft || fixitemi == _slotInfo.LuckPointRight)
                    {

                        switch (_randType)
                        {
                            case 0:
                                {
                                    admitShow = true;
                                    _itemAnimState = ItemAnimState.GoodLuck;
                                    break;
                                }
                            case 1:
                                {
                                    //炮火车
                                    _itemAnimState = ItemAnimState.RunchooInit;
                                    break;
                                }
                        }

                        if (_playWinSound != null)
                        {
                            _playWinSound();
                        }

                        return;
                    }


                    BoxShiningGobs[fixitemi].SetActive(true);
                    //BoxShiningGobs[fixitemi].GetComponent<BoxShining>().TimeRate = 0.1f;
                    //BoxShiningGobs[fixitemi].GetComponent<BoxShining>().ChangeBoxColor(new Color(1f, 1f, 0));
                    _itemAnimState = ItemAnimState.Fresh;
                    eve_shakeBtnFrame(this, EventArgs.Empty);
                    if (_playWinSound != null)
                    {
                        _playWinSound();
                    }
                }
            }
            else
            {
                LightItems[0].SetActive(true);
                LightItems[0].GetComponent<Image>().rectTransform.localPosition = new Vector3(_boxPosList[_itemPosI], _boxPosList[_itemPosI + 1]);



                if (_itemPosI < _boxPosList.Length - 2)
                {
                    _itemPosI += 2;
                }
                else
                {
                    _itemPosI = 0;
                }
                PlayDingSds();
                _timeScale = TimeLength;
            }

        }



        private void SetLightItemPos(ref int posi, int itemi)
        {
            if (LightItems[itemi] == null)
                return;
            LightItems[itemi].SetActive(true);
            LightItems[itemi].GetComponent<Image>().rectTransform.localPosition = new Vector3(_boxPosList[posi], _boxPosList[posi + 1]);
            if (posi - 2 >= 0)
            {
                posi -= 2;
            }
            else
            {
                posi = _boxPosList.Length - 2;
            }

        }

        private void CasinoRun()
        {
            if (_timeScale > 0)
            {
                _timeScale -= Time.fixedDeltaTime * _speedRate;

            }
            else
            {

                int randomNum = Random.Range(1, 14);
                //SetTextNum(CasinoText, randomNum);  //--last version
                this.numToImage(randomNum);
                _timeScale = TimeLength;
            }

            if (_casinoRunEnd != null)
            {
                _casinoRunEnd();
            } 
        }

        private Action _casinoRunEnd;

        private void CasinoRunEnd()
        {
            if (_speedRate > 10)
            {
                _speedRate -= 1.3f;
            }
            else
            {
                _speedRate = 10f;
            }

            _runTime += Time.fixedDeltaTime;
            //随机滚动数字时间到
            if (_runTime >= 3)
            {
                var gdata = App.GameData;
                var player = gdata.GetPlayer();
                _runTime = 0;
                _itemAnimState = ItemAnimState.Fresh;
                //SetTextNum(CasinoText, SlotInfo.CasinoNum);   //--last version
                this.numToImage(SlotInfo.CasinoNum);
                /*                //如果赢得比大小则翻倍奖金
                                if (slotInfo.IsWinTheCasino)
                                {
                                    long curWinMoney = SlotInfo.GetWinMoney();
                                    SlotInfo.SetWinMoney(curWinMoney*2);
                
                                }
                                else//否则奖金清零
                                {
                                    //Debug.Log("奖金清零");
                                    SlotInfo.SetWinMoney(0);
                                }*/
                _casinoRunEnd = null;
                CancelInvoke("PlayDiuDiu");
                if (player.WinCoin > 0)
                {
                    PlayBswinSound();
                    eve_Win(this, EventArgs.Empty);
                    player.Coin = player.Coin;
                }
                else
                {
                    PlayChiDiao();
                    eve_Lose(this, EventArgs.Empty);
                }
            }
        }

        private void OnGetCasinoResult(object obj)
        {
            _casinoRunEnd = CasinoRunEnd;
        }



        /*  last version
        private void SetTextNum(GameObject textGob, long setNum)
        {
            var text = textGob.GetComponent<Text>();
            if (text != null)
            {

                string str = "";
                if (setNum < 10)
                {
                    str = "0";
                }

                CasinoText.GetComponent<Text>().text = str + setNum.ToString(CultureInfo.InvariantCulture);

            }
        }
        */

        //文字图片图集
        public List<Sprite> numImgAtlas; 
        //文字倍转图片
        private void numToImage(int num)
        {
            if (num > 14)
                return;

            if (num.ToString().Length > 1)
            {
                CasinoImgLeft.GetComponent<Image>().sprite = numImgAtlas[1];
                CasinoImgRight.GetComponent<Image>().sprite = numImgAtlas[num % 10];
            }
            else
            {
                CasinoImgLeft.GetComponent<Image>().sprite = numImgAtlas[0];
                CasinoImgRight.GetComponent<Image>().sprite = numImgAtlas[num];
            }

        }
    }
}
