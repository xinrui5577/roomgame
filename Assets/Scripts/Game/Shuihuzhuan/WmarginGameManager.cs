using System;
using System.Collections;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class WmarginGameManager : YxGameManager
    {
        /// <summary>
        /// 总钱数
        /// </summary>
        public Text MyMoneyText;

        /// <summary>
        /// 当局所得
        /// </summary>
        public Text WinMoneyText;

        /// <summary>
        /// 总押注数
        /// </summary>
        public Text BetNumText;

        /// <summary>
        /// 压注倍数
        /// </summary>
        public Text BetBaseText;

        /// <summary>
        /// 线数
        /// </summary>
        public Text LineText;

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.Instance<MusicManager>().ChangeBackSound(0);
            Facade.EventCenter.AddEventListeners<EWmarginEventType, long>(EWmarginEventType.RefreshTotalMoney, OnReshTotalMoney);
        }

        private void OnReshTotalMoney(long obj)
        {
            if (MyMoneyText == null) { return; }
            MyMoneyText.text = YxUtiles.GetShowNumberToString(obj);
        }

        public void LotteryJudge()
        {
            StartCoroutine(ShowAwardEffect());
            GameStateUiControl.instance.StartWait();
        }

        public void GamePlay()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            var iLineImgid = gdata.iLineImgid;
            for (var y = 0; y < 9; y++)
            {
                var imgId = iLineImgid[y];
                if (imgId < 3) { continue;}
                gdata.changeState = true;
                gdata.m_LineType[y] = 1;
                for (var i = 0; i < imgId; i++)
                {
                    gdata.m_LineType[y] = 1;
                    gdata.m_ResultArray[y, i] = 1;
                }
            }
            var w = 9;
            for (var q = 0; q < 9; q++)
            {
                gdata.changeState = true;
                var imgId = iLineImgid[w];
                if (imgId >= 3 && w < 18)
                {
                    gdata.m_LineType[q] = 1;
                    for (var e = 4; e >= (5 - imgId); e--)
                    {
                        gdata.m_ResultArray[q, e] = 1;
                    }
                }
                w++;
            }
        }
        //显示开奖动画
        public IEnumerator ShowAwardEffect()
        {
            GamePlay();
            var gdata = App.GetGameData<WmarginGameData>();
            var lineTypes = gdata.m_LineType;
            var results = gdata.m_ResultArray;
            var types = gdata.m_TypeArray;
            var typeImgIds = gdata.iTypeImgid;
            var linetypeCont = lineTypes.Length;
            var _curType = 0;
            for (var i = 0; i < linetypeCont; i++)
            {
                if (lineTypes[i] != 1) { continue;}
                for (var j = 0; j < 5; j++)
                {
                    if (results[i, j] != 1) { continue;}
                    var type = types[i,j];
                    var resultImage = TurnControl.instance.resultImages[type];
                    gdata.m_ShowSecAnimate[type] = 1;//显示动画的位置
                    resultImage.gameObject.SetActive(false);//关闭组建
                    var typeId = typeImgIds[type];
                    if(typeId!=0) _curType = typeId;
                     var aniStr = typeId + "_" + "0";
                    resultImage.GetComponent<Animator>().enabled = true;
                    resultImage.gameObject.SetActive(true);
                    if (!BottomUIControl.instance.skip_bool)
                    {
                        resultImage.GetComponent<Animator>().Play(aniStr);
                    }
                }
                if (!BottomUIControl.instance.skip_bool)
                {
                    var clip = PlayEffectSound(_curType);
                    var waitTime = clip == null ? 4f : clip.length;
                    yield return new WaitForSeconds(waitTime);
                }
                else
                {
                    yield return new WaitForSeconds(0.01f);
                }
            }
            for (var i = 0; i < 15; i++)
            {
                if (gdata.m_ShowSecAnimate[i] != 1) { continue;}
                var imgId = typeImgIds[i];
                var resultImage = TurnControl.instance.resultImages[i];
                resultImage.gameObject.SetActive(false);
                resultImage.sprite = TurnControl.instance.cardSprites[imgId];
                var aniStr = imgId + "_" + "1";
                resultImage.GetComponent<Animator>().enabled = true;
                resultImage.gameObject.SetActive(true);
                resultImage.GetComponent<Animator>().Play(aniStr);
            }
            gdata.IsAotozhuangtai = true;
            Facade.Instance<MusicManager>().Play("winsound");
            if (gdata.iWinMoney > 0)//当前所得钱数是否大于0
            {
                if (!BottomUIControl.instance.skip_bool)
                {
                    yield return new WaitForSeconds(4f);
                }
                else
                {
                    yield return new WaitForSeconds(0.4f);
                }

                if (!gdata.isMary) //不是小玛丽
                {
                    WinPanelControl.instance.ShowWinPanel();//打开动画
                }
                ChangeToBigSmall();
                ClearData();//清空数据
            }
        }

        private AudioClip PlayEffectSound(int curType)
        {
            var lineTypeSound = App.GetGameData<WmarginGameData>().LineTypeSoundName;
            if (curType < 0 || curType >= lineTypeSound.Length) { return null;}
            var effectName = lineTypeSound[curType];
            return Facade.Instance<MusicManager>().Play(effectName);
        }


        public void ClearData()
        {
            for (int i = 0; i < 9; i++)
            {
                App.GetGameData<WmarginGameData>().m_LineType[i] = 0;
            }
            for (int o = 0; o < 15; o++)
            {
                App.GetGameData<WmarginGameData>().m_ShowSecAnimate[o] = 0;

            }
            for (int y = 0; y < 9; y++)
            {
                for (int j = 0; j < 5; j++)
                {
                    App.GetGameData<WmarginGameData>().m_ResultArray[y, j] = 0;
                } //逐元素赋值。
            }
            YxDebug.LogError("-----------清空存储数据-----------");
        }
        public void ChangeToBigSmall()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            gdata.changeState = false;
            if (gdata.isMary) //是小玛丽
            {
                gdata.IsAuto = false;
                GameStateUiControl.instance.ChangeToWait();
                Invoke("Mali", 3f);
            }
            else
            {
                ii();
            }
        }

        public void ii()
        {
            if (App.GetGameData<WmarginGameData>().IsAuto) //是自动
            {
                Invoke("IsAutoFun", 2f);
            }
            else
            {
                GameStateUiControl.instance.WinWait(); //赢了

            }
        }

        public void IsAutoFun()
        {
            GameStateUiControl.instance.Isaudt();
            BottomUIControl.instance.GetWinMoneyBtnFun();

        }

        public void Mali()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            App.GetRServer<WmarginGameServer>().SendMaLi();//服务器发送数据
            BottomUIControl.instance.Auto(false);
            LittleMaryControl.Instance.OpenMaryPanel();
            //            gdata.MaliWinMony = gdata.MaliWinMony + gdata.iWinMoney;
            LittleMaryControl.Instance.StartMali();
            gdata.IsAuto = false;

        }
        /// <summary>
        /// 输了刷新
        /// </summary>
        public void LostShuaxinFun()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, gdata.GetPlayerInfo().CoinA);
            SetWinMoney(gdata.iWinMoney);
        }
        /// <summary>
        /// 赢了刷新
        /// </summary>
        public void YingShuaxinFun()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            var self = gdata.GetPlayerInfo();
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, self.CoinA - gdata.iWinMoney);
        }
  
        public void Theincome()
        {
            SetWinMoney(App.GetGameData<WmarginGameData>().iWinMoney);
        }

        protected void SetWinMoney(int coin)
        {
            WinMoneyText.text = YxUtiles.GetShowNumberToString(coin);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        { 
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int type, ISFSObject gameinfo)
        { 
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<WmarginGameData>();
            if (response.ContainsKey("mali"))//判断是否有马力
            {
                gdata.isMary = response.GetInt("mali") > 0;
            }
            switch (type)
            {
                case 1://运行水浒传
                    Array.Copy(response.GetIntArray("lines"), 0, gdata.iLineImgid, 0, 18);
                    Array.Copy(response.GetIntArray("images"), 0, gdata.iTypeImgid, 0, 15);
                    if (response.ContainsKey(RequestKey.KeyUser))
                    {
                        var userObj = response.GetSFSObject(RequestKey.KeyUser);
                        var self = gdata.GetPlayerInfo();
                        self.Parse(userObj);
                        if (self.WinCount > 0)
                        {
                            gdata.iWinMoney = self.WinCount;
                            if (gdata.isMary)
                            {
                                gdata.MaliWinMony = self.WinCount;
                            }
                            YingShuaxinFun();
                        }
                        else
                        {
                            gdata.iWinMoney = 0;
                            LostShuaxinFun();
                        }
                    }
                    ShowResult.instance.SetResultSprite();
                    gdata.IsAotozhuangtai = false;//按钮的状态
                    TurnControl.instance.GameResultFun();
                    break;
                case 2:
                    gdata.iDice1 = 0;
                    gdata.iDice2 = 0;
                    gdata.iDice1 = response.GetInt("dice1");
                    gdata.iDice2 = response.GetInt("dice2");
                    gdata.iWinMoney = 0;
                    if (response.ContainsKey(RequestKey.KeyUser))
                    {
                        var userObj = response.GetSFSObject(RequestKey.KeyUser);
                        var self = gdata.GetPlayerInfo();
                        self.Parse(userObj);
                        gdata.iWinMoney = userObj.GetInt("bwin");
                    }
                    BigOrSmallControl.Instance.GameBiBeiResultFun();
                    YxDebug.LogError("服务器=》------大小和----------");
                    break;
                case 3: //玛丽
                    Array.Copy(response.GetIntArray("items"), 0, gdata.iMaliImage, 0, 4);
                    if (response.ContainsKey(RequestKey.KeyUser))
                    {
                        var userObj = response.GetSFSObject(RequestKey.KeyUser);
                        var self = gdata.GetPlayerInfo();
                        self.Parse(userObj);
                        gdata.MaliWinMony += userObj.GetInt("mali");
                    }
                    gdata.iMaliZhuanImage = response.GetInt("idx");//转的图片
                    gdata.iMaliGames = response.GetInt("mali");//马力次数 
                    LittleMaryControl.Instance.MaryResultFun();
                    //                YxDebug.LogError(App.GetGameData<GlobalData>().MaliWinMony);
                    YxDebug.LogError("服务器=》-------玛丽----------");
                    break;
            }
        }
    }
} 