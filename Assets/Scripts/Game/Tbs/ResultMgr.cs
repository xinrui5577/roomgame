using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Tbs
{
    /// <summary>
    /// 结算面板 单例类
    /// </summary>
    public class ResultMgr : MonoBehaviour
    {

        public GameObject Bg;
        /// <summary>
        /// 赢家标题
        /// </summary>
        public GameObject WinTitle;
        /// <summary>
        /// 败家标题
        /// </summary>
        public GameObject LoseTitle;
        /// <summary>
        /// 显示输赢钱数 按庄初天末来
        /// </summary>
        public GameObject[] Items;
        /// <summary>
        /// 提示自己的背景
        /// </summary>
        public GameObject SelfBg;
        /// <summary>
        /// 结算时显示点数
        /// </summary>
        public UILabel DiceNumLabel;

        /// <summary>
        /// 点数
        /// </summary>
        private int _diceNum;
        /// <summary>
        /// 首赔玩家座位号
        /// </summary>
        private int _sp;
        /// <summary>
        /// 设置结算数据
        /// </summary>
        public void SetData(int diceN,int sp)
        {
            _diceNum = diceN;
            _sp = sp;
        }

        /// <summary>
        /// 打开结算界面
        /// </summary>
        /// <param name="golds"></param>
        /// <param name="ttgold"></param>
        /// <param name="dians"></param>
        /// <param name="colset"></param>
        public void OpenResult(string[] golds, string[] ttgold, int[] dians, float colset = 1.0f)
        {
            Bg.SetActive(true);

            DiceNumLabel.text = _diceNum + "点";

            WinTitle.SetActive(false);
            LoseTitle.SetActive(true);
            var gmanager = App.GetGameManager<TbsGameManager>();
            var gdata = App.GetGameData<TbsGameData>();
            int bankerseat = gdata.BankerSeat;
            int j = 0;
            for (int i = bankerseat; i < bankerseat + gdata.PlayerList.Length; i++)
            {
                int index = i % gdata.PlayerList.Length;
                Items[j].transform.FindChild("Coin").GetComponent<UILabel>().text = YxUtiles.ReduceNumber(int.Parse(golds[index]));
                Items[j].transform.FindChild("sp").gameObject.SetActive(index == _sp);

                int gold = int.Parse(golds[index]);

                bool isWin = false;

                if (gold > 0)
                {
                    isWin = true;
                }
                else if (gold < 0)
                {
                    isWin = false;
                }
                else
                {
                    if (index == gdata.BankerSeat)
                    {
                        int bankerD = dians[index];
                        for (int k = 0; k < dians.Length; k++)
                        {
                            if (k == index) continue;
                            if (bankerD < dians[k]) continue;
                            isWin = true;
                            break;
                        }
                    }
                    else
                    {
                        int bankerD = dians[gdata.BankerSeat];
                        int selfD = dians[index];
                        if (bankerD < selfD)
                        {
                            isWin = true;
                        }
                    }
                }

                //输赢显示
                if (isWin)
                {
                    Items[j].transform.FindChild("Fruit").GetComponent<UISprite>().spriteName = "txt_win";
                    if (index == App.GameData.SelfSeat)
                    {
                        Facade.Instance<MusicManager>().Play("win");
                        WinTitle.SetActive(true);
                        LoseTitle.SetActive(false);
                    }
                }
                else
                {
                    if (index == App.GameData.SelfSeat)
                    {
                        Facade.Instance<MusicManager>().Play("lose");
                    }
                    Items[j].transform.FindChild("Fruit").GetComponent<UISprite>().spriteName = "txt_lose";
                }

                if (gdata.GetPlayerInfo(index,true)!= null && index != gdata.BankerSeat)
                {
                    gdata.GetPlayerInfo(index,true).CoinA = long.Parse(ttgold[index]);
                }
                else if (index == gdata.BankerSeat)
                {
                    gdata.GuoGold += gold;
                    if (gdata.GuoGold < 0)
                    {
                        gdata.Boold += gdata.GuoGold;
                        gdata.GuoGold = 0;
                    }

                    if (gdata.GetPlayerInfo(index,true) != null)
                    {
                        gdata.GetPlayerInfo(index, true).CoinA = long.Parse(ttgold[index]);
                    }
                    gmanager.RefreshLabel();
                }

                if (index == App.GameData.SelfSeat)
                {
                    SelfBg.transform.localPosition = Items[j].transform.localPosition;
                }

                j++;
            }
            Invoke("CloseResult", colset);
        }
        /// <summary>
        /// 关闭结算界面
        /// </summary>
        public void CloseResult()
        {
            Bg.SetActive(false);
            CancelInvoke("CloseResult");
            var gmanager = App.GetGameManager<TbsGameManager>();
            var gdata = App.GetGameData<TbsGameData>();
            gmanager.BankerBtnMgr.SetBtn(BankerBtn.ChangeBanker);
            App.GetRServer<TbsRemoteController>().ReadyGame();
            gmanager.Reset();
            for (int i = 0; i < gdata.PlayerList.Length; i++)
            {
                if (gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.LeftPoker != null)
                {
                    gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.LeftPoker.SetActive(false);
                    gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.LeftPoker = null;
                }

                if (gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.RightPoker != null)
                {
                    gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.RightPoker.SetActive(false);
                    gdata.GetPlayer<TbsPlayer>(i).UserBetPoker.RightPoker = null;
                }

            }
            App.GetGameManager<TbsGameManager>().TimeMgr.OpenTime();
        }
    }
}
