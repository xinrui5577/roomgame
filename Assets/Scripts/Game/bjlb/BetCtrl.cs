using System;
using Assets.Scripts.Common.components;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using Random = System.Random;

namespace Assets.Scripts.Game.bjlb
{
    public class BetCtrl : YxView
    {

        public GameObject Select;
        /// <summary>
        /// 筹码区域
        /// </summary>
        public GameObject ChipParents;
        public ResetChip AllBet;

        public Chip CurrentSelectChip;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public ChipConfig ChipCfg = new ChipConfig();

        public virtual void OnBetClick(Chip chip)
        {
            Select.transform.position = chip.transform.position;
            CurrentSelectChip = chip;
        }

        protected override void OnStart()
        {
            Select.SetActive(false);
            Init();
        }

        public virtual void OnDeskClick(UIWidget widget)
        {
            int index;
            if (!int.TryParse(widget.name, out index) || index < 0) { return; }
            var gdata = App.GetGameData<BjlGameData>();
            if (CurrentSelectChip == null) { return; }
            if (!gdata.BeginBet) { return; }
            if (App.GameKey.Equals("bjlb") && gdata.BankSeat == gdata.SelfSeat) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if (gold > gdata.GetPlayerInfo().CoinA)
            {
                YxMessageBox.Show(new YxMessageBoxData { Msg = "金币不够，无法下注." });
                return;
            }
            if (BankerHasFullGold(index, gold))
            {
                var gserver = App.GetRServer<BjlGameServer>();
                gserver.UserBet(index, gold);
            }
            else
            {
                YxMessageBox.Show(new YxMessageBoxData { Msg = "庄家金币不够，无法继续下注." });
            }
            //Facade.Instance<MusicManager>().Play("Bet");
            //var lpos = CurrentSelectChip.transform.position;
            //lpos = ChipArea.InverseTransformPoint(lpos);
            //InstantiateChip(widget, lpos, gold);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">下的位置</param>
        /// <param name="gold">下的筹码</param>
        /// <returns></returns>
        private bool BankerHasFullGold(int index, int gold)
        {
            var gdata = App.GetGameData<BjlGameData>();
            var banker = gdata.CurrentBanker;
            if (banker == null)
            {
                return true;
            }
            var bankerInfo = banker.Info;
            if (bankerInfo == null)
                return true;
            var zNumber = gdata.ZNumber;
            switch (index)
            {
                case 0:
                    return gdata.BankSeat == -1 || zNumber[0] + gold - zNumber[1] - zNumber[2] < bankerInfo.CoinA;
                case 1:
                    return gdata.BankSeat == -1 || zNumber[1] + gold - zNumber[0] - zNumber[2] < bankerInfo.CoinA;
                case 2:
                    var all = (int)((zNumber[1] + zNumber[0] + bankerInfo.CoinA) / 8);
                    return gdata.BankSeat == -1 || zNumber[2] + gold < all;
            }
            return false;
        }

        public void ShowChip()
        {
            var gdata = App.GetGameData<BjlGameData>();
            if (!gdata.BeginBet) return;
            if (gdata.GetPlayerInfo<BjlUserInfo>().Forbiden)
            {
                App.GetGameManager<BjlGameManager>().ShowTip();
                return;
            }
            AllBet.gameObject.SetActive(true);
            Select.SetActive(true);

        }

        public void HideChip()
        {
            AllBet.gameObject.SetActive(false);
            Select.SetActive(false);
        }

        private Random _random = new Random();

        public virtual void Bet(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BjlGameData>();
            var seat = responseData.GetInt("seat");
            var gold = responseData.GetInt("gold");
            var p = responseData.GetInt("p");
            if (seat == gdata.SelfSeat)
            {
                SelfBet(p, gold);
                return;
            }
            //其他人
            Facade.Instance<MusicManager>().Play("Bet");
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                var random = _random.Next(0, 100) % len;
                InstantiateChip(ChipCfg.DeskAreas[p], startPos[random], gold);
            }
        }

        /// <summary>
        /// 本局自己最大下注额
        /// </summary>
        protected int SelfMaxBet;

        protected int MaxRoundBet;

        public void SelfBet(int p, int gold)
        {
            var gdata = App.GetGameData<BjlGameData>();
            gdata.SetGameStatus(YxEGameStatus.PlayAndConfine);
            gdata.IsGaming = true;
            gdata.GetPlayer().Coin -= gold;
            var lpos = GetChipPos(gold);
            lpos = ChipArea.InverseTransformPoint(lpos);
            InstantiateChip(ChipCfg.DeskAreas[p], lpos, gold);
            PlayAmazedSoud(gold, ref SelfMaxBet);
        }


        protected Vector3 GetChipPos(int gold)
        {
            var gdata = App.GetGameData<BjlGameData>();
            var anteRate = gdata.AnteRate;
            for (int i = 0; i < anteRate.Count; i++)
            {
                if (anteRate[i] == gold)
                {
                    return AllBet.Grid.transform.GetChild(i).position;
                }
            }
            return AllBet.Grid.transform.GetChild(0).position;
        }

        protected void PlayAmazedSoud(int gold, ref int maxBet)
        {
            var anteRate = App.GameData.AnteRate;

            if (anteRate.Count <= 0) return;
            if (gold > maxBet || anteRate[anteRate.Count - 1] == gold)
            {
                maxBet = gold;
                Facade.Instance<MusicManager>().Play("amazed");
            }
        }

        public virtual void ThrowChips(int gold, int p, bool bankerBet)
        {
        }

        protected int Chipdepth = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="localPos"></param>
        /// <param name="gold"></param>
        /// <param name="isBanker"></param>
        /// <param name="needAnimo"></param>
        public virtual void InstantiateChip(UIWidget widget, Vector3 localPos, int gold, bool isBanker = false, bool needAnimo = true)
        {
            var chip = GetOneChip(isBanker);
            var chipTs = chip.transform;
            chipTs.parent = ChipArea;
            chipTs.localPosition = localPos;
            chipTs.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<BjlGameData>().AnteRate.IndexOf(gold),
                Depth = Chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            if (!needAnimo) { return; }
            var sp = chip.GetComponent<SpringPosition>();
            sp.target = GetClipPos(widget);
            
            sp.enabled = true;
        }

        protected virtual Chip GetOneChip(bool isBnaker = false)
        {
            return Instantiate(ChipCfg.ChipPerfab);
        }

        public virtual Vector3 GetClipPos(UIWidget widget)
        {
            var v = Vector3.zero;
            var w = widget.width / 2;
            var h = widget.height / 2;
            var i2 = _random.Next(-w, w);
            var i3 = _random.Next(-h, h);
            var ts = widget.transform;
            v.x = ts.localPosition.x + i2;
            v.y = ts.localPosition.y + i3;
            return v;
        }

        public void InitChips()
        {
            var gdata = App.GetGameData<BjlGameData>();
            OnBetClick(AllBet.InitChips(gdata.Ante, gdata.AnteRate));
        }

        protected Transform ChipArea;
        public override void Init()
        {
            Chipdepth = 1;
            InitChipArea();
        }

        protected virtual void InitChipArea()
        {
            if (ChipArea != null)
            {
                Destroy(ChipArea.gameObject);
            }
            var go = new GameObject("ChipArea");
            ChipArea = go.transform;
            ChipArea.parent = ChipParents.transform;
            ChipArea.localPosition = Vector3.zero;
            ChipArea.localScale = Vector3.one;
            ChipArea.localRotation = Quaternion.identity;
        }

        public virtual void GroupBet(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            var sfsArray = responseData.GetSFSArray("coin");
            var startPos = ChipCfg.StartPos;
            Facade.Instance<MusicManager>().Play("Bet");
            var gdata = App.GameData;
            var selfSeat = gdata.GetPlayerInfo().Seat;

            int count = sfsArray.Count;
            for (int i = 0; i < count; i++)
            {
                ISFSObject item = sfsArray.GetSFSObject(i);
                int p = item.GetInt("p");
                int gold = item.GetInt("gold");
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;

                int len = startPos.Length;
                if (len > 0)
                {
                    int startIndex = _random.Next(0, 100) % len;
                    InstantiateChip(ChipCfg.DeskAreas[p], startPos[startIndex], gold);
                }
            }
        }

        public virtual void Reset()
        {
            Chipdepth = 0;
            ShowChip();
            Init();
        }

        protected virtual void OnDrawGizmos()
        {
            var arr = ChipCfg.StartPos;
            if (arr == null) return;
            var len = arr.Length;
            for (var i = 0; i < len; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(ChipCfg.StartPos[i]), 0.1f);
            }
        }

        [Serializable]
        public class ChipConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public Chip ChipPerfab;
            public Vector3[] StartPos = new Vector3[0];
            public UIWidget[] DeskAreas;
        }
    }


}
