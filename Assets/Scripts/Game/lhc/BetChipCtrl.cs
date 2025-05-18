using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.lhc
{
    public class BetChipCtrl : MonoBehaviour
    {
        public ChipItem ChipItem;
        public UIGrid ChipGrid;
        public GameObject SelectObj;
        public GameObject CurrentBetPos;



        public void CreatChipBtn(List<int> bets)
        {
            for (int i = 0; i < bets.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(ChipItem, ChipGrid.transform);
                item.GetComponent<UISprite>().spriteName = "chip_" + i;
                item.ChipValueLabel.text = YxUtiles.GetShowNumber(bets[i]).ToString();
                item.name = bets[i].ToString();
                //                item.Pos = i;
            }

            ChipGrid.repositionNow = true;
            //            SelectObj.transform.localPosition = ChipGrid.transform.GetChild(0).localPosition;
            App.GetGameData<LhcGameData>().CurrentBet = int.Parse(ChipGrid.transform.GetChild(0).name);
        }

        public void OnChipBtn(ChipItem obj)
        {
            SelectObj.transform.localPosition = obj.transform.localPosition;
            App.GetGameData<LhcGameData>().CurrentBet = int.Parse(obj.name);
        }

        public UILabel LoseAnteDes;

        public UIGrid BetGrid;
        public BetRegionItem BetRegionItem;
        public List<BetRegionItem> AllBetRegionItem = new List<BetRegionItem>();

        private int _index;

        private UIGrid _partGrid;

        private string _itemName = "";

        public void ChangeBank()
        {
            for (int i = 0; i < AllBetRegionItem.Count; i++)
            {
                while (AllBetRegionItem[i].transform.childCount > 1)
                {
                    DestroyImmediate(AllBetRegionItem[i].transform.GetChild(1).gameObject);
                }
            }
            InitBetData();
        }

        private void DeleteAllBetRegionItem()
        {
            AllBetRegionItem.Clear();
            if (_partGrid != null)
            {
                DestroyImmediate(_partGrid.gameObject);
            }
        }

        public void InitBetData()
        {
            DeleteAllBetRegionItem();

            var gdata = App.GetGameData<LhcGameData>();
            LoseAnteDes.text = gdata.LoseAnteDes;
            var index = 0;
            _partGrid = Instantiate(BetGrid);
            _partGrid.transform.parent = BetGrid.transform;
            _partGrid.transform.localPosition = Vector3.zero;
            _partGrid.transform.localScale = Vector3.one;
            for (int i = 1; i < 67; i++)
            {
                var item = YxWindowUtils.CreateItem(BetRegionItem, _partGrid.transform);
                AllBetRegionItem.Add(item);

                foreach (var value in gdata.BetPosColors)
                {
                    if (value.Value == i)
                    {
                        item.ShowValueBg.spriteName = value.Color;
                        item.ShowValue.text = value.Pos;
                        continue;
                    }
                }

                item.name = i.ToString();

                item.OtherBetLabel.text = 0.ToString();
            }

            _partGrid.repositionNow = true;
            //刷新界面信息
            InitBetPanel(-1, -1, -1, true);
        }
        /// <summary>
        /// 初始化下注界面
        /// </summary>
        public void InitBetPanel(int p = -1, int seat = -1, int gold = -1, bool isInit = false)
        {
            var gdata = App.GetGameData<LhcGameData>();

            for (int i = 0; i < AllBetRegionItem.Count; i++)
            {
                var item = AllBetRegionItem[i];
                if (isInit)
                {
                    if (gdata.BetDic != null)
                    {
                        if (gdata.BetDic.ContainsKey((i+1).ToString()))
                        {
                            for (int j = 0; j < gdata.AnteRate.Count; j++)
                            {
                                var betdic = gdata.BetDic[(i + 1).ToString()] as Dictionary<string, object>;
                                if (betdic != null)
                                {
                                    if (betdic.ContainsKey(gdata.AnteRate[j].ToString()))
                                    {
                                        var count = int.Parse(betdic[gdata.AnteRate[j].ToString()].ToString());
                                        for (int k = 0; k < count; k++)
                                        {
                                            CreatChip(i+1, seat, gdata.AnteRate[j]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                item.AllowBetLabel.text = YxUtiles.GetShowNumber(gdata.BetAllows[i]).ToString();
                if (seat == gdata.SelfSeat && p == i+1 )
                {
                    item.SelfBetLabel.text = YxUtiles.GetShowNumber(gdata.SelfBets[i]).ToString();
                }
                else
                {
                    item.SelfBetLabel.text = YxUtiles.GetShowNumber(gdata.SelfBets[i]).ToString();

                }
                if (gdata.AllBets.ContainsKey(i + 1))
                {
                    var otherBetValue = gdata.AllBets[i + 1] - gdata.SelfBets[i];
                    var betValue = otherBetValue - YxUtiles.RecoverShowNumber(int.Parse(item.OtherBetLabel.text)) ;
                    if (betValue > 0)
                    {
                        p = i+1;
                        gold =(int)betValue;
                    }
                    item.OtherBetLabel.text = YxUtiles.GetShowNumber(otherBetValue).ToString();
                }
                else
                {
                    item.OtherBetLabel.text = 0.ToString();
                }
            }
            if (isInit) return;
            if (p == -1 && seat == -1 && gold == -1) return;
            CreatChip(p, seat, gold);
        }
        public void OnClickBox(BetRegionItem obj)
        {
            if (!_itemName.Equals(obj.name))
            {
                _itemName = obj.name;
                CurrentBetPos.transform.localPosition = obj.transform.localPosition;
                return;
            }
            var gdata = App.GetGameData<LhcGameData>();
            if (gdata.CurrentBanker.Info.Seat == gdata.SelfSeat)
            {
                YxMessageBox.Show("您是庄家不可以下注！！！");
                return;
            }

            if (gdata.GetPlayerInfo().CoinA - gdata.CurrentBet < 0)
            {
                YxMessageBox.Show("您金币不够充值后继续游戏！！！");
                return;
            }
            var str = string.Format("您在{0}位置下注{1}金币", obj.ShowValue.text,YxUtiles.GetShowNumber(gdata.CurrentBet));
            YxMessageBox.Show(new YxMessageBoxData()
            {
                Msg = str,
                Listener = (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnMiddle)
                    {
                        App.GetRServer<LhcGameServer>().SendBetReq(gdata.CurrentBet, int.Parse(obj.name));
                    }
                }
            });
        }

        public void CreatChip(int p, int seat, int gold)
        {
            var gdata = App.GetGameData<LhcGameData>();

            if (seat == gdata.SelfSeat)
            {
                gdata.GetPlayer().Coin -= gold;
            }
            _index += 2;
            var item = Instantiate(ChipItem);
            item.transform.parent = AllBetRegionItem[p-1].transform;
            item.GetComponent<BoxCollider>().enabled = false;
            item.FreshDep(_index);
            item.gameObject.SetActive(true);
            var x = Random.Range(AllBetRegionItem[p-1].BetPos.localPosition.x - 30,
                AllBetRegionItem[p-1].BetPos.localPosition.x + 30);
            var y = Random.Range(AllBetRegionItem[p-1].BetPos.localPosition.y - 55,
                AllBetRegionItem[p-1].BetPos.localPosition.y + 5);

            item.transform.localPosition = new Vector3(x, y, 0);
            item.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            item.name = gold.ToString();
            item.ChipValueLabel.text = YxUtiles.GetShowNumber(gold).ToString();

            item.GetComponent<UISprite>().spriteName = "chip_" + GetChipSpriteName(gold);
            Facade.Instance<MusicManager>().Play("Bet");

        }

        private int GetChipSpriteName(int gold)
        {
            var anteRate = App.GetGameData<LhcGameData>().AnteRate;
            var val = 0;
            for (int i = 0; i < anteRate.Count; i++)
            {
                if (gold == anteRate[i])
                {
                    val = i;
                }
            }
            return val;
        }
    }
}
