using System;
using System.Collections.Generic;
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

namespace Assets.Scripts.Game.BaiTuan
{
    public class BetCtrl : YxView
    {

        public GameObject Select;
        public GameObject ChipParents;
        protected int _chipdepth = 1;
        public GridBetByCoin BetByCoin;
        public GameObject BankerTip;
        public Chip CurrentSelectChip;
        public ChipConfig ChipCfg = new ChipConfig();

        protected Transform ChipArea;


        protected override void OnStart()
        {
            _subGlod = new Queue<int>();
            Select.SetActive(false);
            Init();
        }
        public override void Init()
        {
            _chipdepth = 1;

            InitChipArea();
        }
        public void InitChips()
        {
            var gdata = App.GetGameData<BtwGameData>();
            OnBetClick(BetByCoin.InitChips(gdata.Ante, gdata.AnteRate));
        }
        /// <summary>
        /// 选择筹码
        /// </summary>
        /// <param name="chip"></param>
        public void OnBetClick(Chip chip)
        {
            var v = chip.transform.position;
            v = BetByCoin.transform.InverseTransformPoint(v);
            Select.transform.localPosition = new Vector3(v.x, v.y + 5, v.z);
            CurrentSelectChip = chip;
        }

        protected bool _isAllowBet = true;

        protected readonly List<Transform> _chipAreas = new List<Transform>();

        private void InitChipArea()
        {
            foreach (var area in _chipAreas)
            {
                if (area != null)
                {
                    Destroy(area.gameObject);
                }
            }
            _chipAreas.Clear();
            var areas = ChipCfg.DeskAreas;
            var len = areas.Length;
            for (var i = 0; i < len; i++)
            {
                var go = new GameObject(string.Format("ChipArea_{0}", i));
                var ts = go.transform;
                ts.parent = areas[i].transform;
                ts.localPosition = Vector3.zero;
                ts.localScale = Vector3.one;
                ts.localRotation = Quaternion.identity;
                _chipAreas.Add(ts);
            }
            if (ChipArea != null)
            {
                Destroy(ChipArea.gameObject);
            }
            var temp = new GameObject("ChipArea");
            ChipArea = temp.transform;
            ChipArea.parent = ChipParents.transform;
            ChipArea.localPosition = Vector3.zero;
            ChipArea.localScale = Vector3.one;
            ChipArea.localRotation = Quaternion.identity;
        }

        protected long LastClickTime;
        public virtual void OnDeskClick(Transform ts)
        {

            if (!_isAllowBet) { return; }
            var curClickTime = DateTime.Now.Ticks;
            if (curClickTime - LastClickTime < 0.14f) { return; }
            LastClickTime = curClickTime;
            int index;
            if (!int.TryParse(ts.name, out index) || index < 0) { return; }
            var gdata = App.GetGameData<BtwGameData>();
            if (CurrentSelectChip == null) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if (CouldBet(gold))
            {
                var globadata = App.GetRServer<BtwGameServer>();
                globadata.UserBetOne(index, gold);
                gdata.ThisCanInGold = gdata.ThisCanInGold - gold;
            }
        }

        public bool CouldBet(long gold)
        {
            var gdata = App.GetGameData<BtwGameData>();
            if (!gdata.BeginBet)
            {
                YxMessageTip.Show("下注时间到，不能下注。");
                return false;
            }
            if (gold > gdata.CurrentCanInGold)
            {
                YxMessageTip.Show("团长的金币不够，不能下注。");
                return false;
            }
            if (gold > gdata.ThisCanInGold)
            {
                YxMessageTip.Show("您的金币不够，不能下注。");
                return false;
            }
            if (gdata.BankerPlayer.Info.Seat == gdata.SelfSeat)
            {
                YxMessageTip.Show("您就是团长，不能下注。");
                return false;
            }
            return true;
        }

        public virtual void ShowChip()
        {
            var gdata = App.GetGameData<BtwGameData>();
            if (gdata.CurrentBanker.Seat == gdata.SelfSeat)
            {
                BetByCoin.gameObject.SetActive(false);
                Select.SetActive(false);
                BankerTip.SetActive(true);
                return;
            }
            if (!gdata.BeginBet) return;
            BankerTip.SetActive(false);
            BetByCoin.gameObject.SetActive(true);
            Select.SetActive(true);
            BetByCoin.SetShowBet(gdata.GetPlayerInfo().CoinA);
        }

        public virtual void HideChip()
        {
            BetByCoin.gameObject.SetActive(false);
            Select.SetActive(false);
        }

        protected Queue<int> _subGlod;

        public virtual void Bet(ISFSObject responseData)
        {
            var gold = responseData.GetInt("gold");
            if (gold == 0) return;
            App.GetGameManager<BtwGameManager>().ProgressCtrl.RefreshNum(gold);
            var seat = responseData.GetInt("seat");
            var p = responseData.GetInt("p");
            _subGlod.Enqueue(gold);
            var gdata = App.GetGameData<BtwGameData>();
            if (seat == gdata.SelfSeat)
            {
                SelfMenBet(p, gold);
                return;
            }
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                InstantiateChip(ChipCfg.DeskAreas[p], startPos[0], gold);
                Facade.Instance<MusicManager>().Play("Bet");
            }
        }

        public virtual void SelfBet(int p, int gold)
        {
            var gdata = App.GetGameData<BtwGameData>();
            gdata.SetGameStatus(YxEGameStatus.PlayAndConfine);
            gdata.GetPlayer().Coin -= gold;
            var lpos = GetChipPos(gold);
            lpos = ChipArea.InverseTransformPoint(lpos);
            InstantiateChip(ChipCfg.DeskAreas[p], lpos, gold);
        }

        protected void SelfMenBet(int p, int allGold)
        {
            var gdata = App.GetGameData<BtwGameData>();
            var anteRate = gdata.AnteRate;
            int count = anteRate.Count;

            //处理筹码值不是正好是配置筹码的情况
            var goldList = GetGoldList(allGold);
            foreach (var item in goldList)
            {
                SelfBet(p, item);
            }
        }

        protected Vector3 GetChipPos(int gold)
        {
            var gdata = App.GetGameData<BtwGameData>();
            var anteRate = gdata.AnteRate;
            for (int i = 0; i < anteRate.Count; i++)
            {
                if (anteRate[i] == gold)
                {
                    return BetByCoin.Grid.transform.GetChild(i).position;
                }
            }
            return BetByCoin.Grid.transform.GetChild(0).position;
        }

        public List<int> GetGoldList(int gold)
        {
            var goldList = new List<int>();
            var anteRate = new List<int>(App.GetGameData<BtwGameData>().AnteRate);
            anteRate.Sort((a, b) => a - b);
            int fristIndex = anteRate.Count - 1;

            for (int i = fristIndex; i >= 0; i--)
            {
                var val = anteRate[i];
                if (gold >= val)
                {
                    gold -= val;
                    goldList.Add(val);
                    i++;
                }
            }

            return goldList;
        }

        protected int MaxGroupBet;

        public virtual void GroupBet(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            var sfsArray = responseData.GetSFSArray("coin");
            var startPos = ChipCfg.StartPos;
            Facade.Instance<MusicManager>().Play("groupbet");
            var gdata = App.GameData;
            var selfSeat = gdata.GetPlayerInfo().Seat;

            int maxGold = 0;
            // ReSharper disable once TooWideLocalVariableScope
            foreach (ISFSObject item in sfsArray)
            {
                int p = item.GetInt("p");
                int gold = item.GetInt("gold");
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;

                if (gold >= maxGold)
                {
                    maxGold = gold;
                }

                int len = startPos.Length;
                if (len > 0)
                {
                    InstantiateChip(ChipCfg.DeskAreas[p], startPos[0], gold);
                }
            }

            PlayAmazedSoud(maxGold, ref MaxGroupBet);

        }

        protected void PlayAmazedSoud(int gold, ref int maxBet)
        {
            var anteRate = App.GameData.AnteRate;

            if (gold > maxBet || anteRate[anteRate.Count - 1] == gold)
            {
                maxBet = gold;
                Facade.Instance<MusicManager>().Play("amazed");
            }
        }

        /// <summary>
        /// 创建筹码
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="localPos"></param>
        /// <param name="gold"></param>
        /// <param name="needAnimo"></param>
        protected virtual void InstantiateChip(Transform ts, Vector3 localPos, int gold, bool needAnimo = true)
        {
            var gdata = App.GetGameData<BtwGameData>();
            var chip = Instantiate(ChipCfg.ChipPerfab);
            var chipTs = chip.transform;
            chipTs.parent = ts;
            chipTs.localPosition = localPos;
            chipTs.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            _chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = gdata.AnteRate.IndexOf(gold),
                Depth = _chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            gdata.CurrentChipList.Add(chip.gameObject);
            if (!needAnimo) { return; }
            var sp = chip.GetComponent<SpringPosition>();
            sp.target = GetClipPos(ts);
            sp.onFinished = SubGlod;
            sp.enabled = true;
        }

        public Vector3 GetClipPos(Transform ts)
        {
            var v = Vector3.zero;
            var r = new Random();
            var w = (int)ChipCfg.ChipAreas.x / 2;
            var h = (int)ChipCfg.ChipAreas.y / 2;
            var i2 = r.Next(-w, w);
            var i3 = r.Next(-h, h);
            v.x = i2;
            v.y = i3;
            return v;
        }
        public virtual void ThrowChips(int gold, int p, bool isBanker)
        {

        }

        protected virtual void SubGlod()
        {
            var gMgr = App.GetGameManager<BtwGameManager>();
            gMgr.ProgressCtrl.RefreshNum(_subGlod.Dequeue());
            gMgr.RefreshUserInfo();
        }

        public virtual void Reset()
        {
            _chipdepth = 1;
            ShowChip();
            Init();
        }

        protected void OnDrawGizmos()
        {
            var arr = ChipCfg.StartPos;
            if (arr == null) return;
            var len = arr.Length;
            for (var i = 0; i < len; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(ChipCfg.StartPos[i]), 0.1f);
            }
        }

        protected void InstantiateChipWithWaitTime(Transform ts, Vector3 localPos, int gold, float waitTime)
        {
            var chip = Instantiate(ChipCfg.ChipPerfab);
            var chipTs = chip.transform;
            chipTs.parent = ts;
            chipTs.localPosition = localPos;
            chipTs.localScale = Vector3.one * 0.5f;
            _chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<BtwGameData>().AnteRate.IndexOf(gold),
                Depth = _chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);

            var cs = chip.GetComponent<SpringPosition>();
            cs.target = GetClipPos(ts);
            cs.onFinished = SubGlod;
            cs.enabled = true;

        }

        [Serializable]
        public class ChipConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public Chip ChipPerfab;
            public Vector3[] StartPos = new Vector3[0];
            public Transform[] DeskAreas;
            public Vector2 ChipAreas = new Vector2(100, 100);
        }
    }
}
