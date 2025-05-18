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
using System.Collections.Generic;

namespace Assets.Scripts.Game.brnn
{
    public class BetCtrl : YxView
    {
        public GameObject Select;
        public GameObject ChipParents;
       
        public ResetChip AllBet;
        public Chip CurrentSelectChip;

        public ChipConfig ChipCfg = new ChipConfig();

        protected int Chipdepth = 1;
        protected Transform ChipArea;


        protected Pool<Chip>[] BetPoolArray;

        public virtual void OnBetClick(Chip chip)
        {
            Select.transform.position = chip.transform.position;
            CurrentSelectChip = chip;
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            Select.SetActive(false);
            Init();
        }

        public override void Init()
        {
            Chipdepth = 1;
            InitPool();
            InitChipArea();
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        protected virtual void InitPool()
        {
            var len = ChipCfg.DeskAreas.Length;
            var prefab = ChipCfg.ChipPerfab;
            BetPoolArray = new Pool<Chip>[len];
            for (int i = 0; i < len; i++)
            {
                BetPoolArray[i] = new Pool<Chip>(prefab, 50, 0, transform,false);
            }
        }


        private void InitChipArea()
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

        public virtual void OnDeskClick(UIWidget widget)
        {
            int index;
            if (!int.TryParse(widget.name, out index) || index < 0) { return; }
            var gdata = App.GetGameData<BrnnGameData>();
            if (CurrentSelectChip == null) { return; }
            if (!gdata.BeginBet) { return; }
            if (App.GameKey.Equals("brnn") && gdata.IsBanker) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if(CouldBet(gold))
            {
                var gserver = App.GetRServer<BrnnGameServer>();
                gserver.UserBet(index, gold);
            }
        }

        public void PlayerBet(int index, int gold)
        {
            if (CouldBet(gold))
            {
                var gdata = App.GetGameData<BrnnGameData>();
                gdata.ThisCanInGold = gdata.ThisCanInGold - gold;
            }
        }


        public bool CouldBet(long gold)
        {
            var gdata = App.GetGameData<BrnnGameData>();
            if (!gdata.BeginBet)
            {
                YxMessageTip.Show("下注时间到，不能下注。");
                return false;
            }
            if (gold > gdata.ThisCanInGold)
            {
                YxMessageTip.Show(string.Format("你最多只能下注您所有金币的1/{0}。", gdata.MaxNiuRate));
                return false;
            }
            if (gold > gdata.CurrentCanInGold)
            {
                YxMessageTip.Show("庄家的金币不够，不能下注。");
                return false;
            }
            if (gdata.CurrentBanker.Info == null || gdata.CurrentBanker.Info.Seat == -1)
            {
                YxMessageTip.Show("当前没有庄，不能下注。");
                return false;
            }
            return true;
        }


        public virtual void ShowChip()
        {
            var gdata = App.GetGameData<BrnnGameData>();
            if (gdata.IsBanker)
            {
                AllBet.gameObject.SetActive(false);
                Select.SetActive(false);
                return;
            }
            if (!gdata.BeginBet) return;
            AllBet.gameObject.SetActive(true);
            Select.SetActive(true);

        }
        public virtual void HideChip()
        {
            AllBet.gameObject.SetActive(false);
            Select.SetActive(false);
        }

        public virtual void Bet(ISFSObject responseData)
        {
            int p = responseData.GetInt("p");
            int gold = responseData.GetInt("gold");
            int seat = responseData.GetInt("seat");
            var gdata = App.GetGameData<BrnnGameData>();
            if (seat == gdata.SelfSeat)
            {
                SelfMenBet(p, gold);
                return;
            }
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                var random = UnityEngine.Random.Range(0, 100) % len;
                InstantiateChip(p, startPos[random], gold);
                Facade.Instance<MusicManager>().Play("Bet");
            }
        }

        public virtual void SelfBet(int p, int gold)
        {
            var gdata = App.GetGameData<BrnnGameData>();
            gdata.SetGameStatus(YxEGameStatus.PlayAndConfine);
            gdata.GetPlayer().Coin -= gold;
            var lpos = GetChipPos(gold);
            lpos = ChipArea.InverseTransformPoint(lpos);
            InstantiateChip(p, lpos, gold);
        }


        protected void SelfMenBet(int p, int allGold)
        {
            //处理筹码值不是正好是配置筹码的情况
            var goldList = GetGoldList(allGold);
            foreach (var item in goldList)
            {
                SelfBet(p, item);
            }
        }

        protected int MaxGroupBet;

        public List<int> GetGoldList(int gold)
        {
            var goldList = new List<int>();
            var anteRate = new List<int>(App.GetGameData<BrnnGameData>().AnteRate);
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
            Random random = new Random();
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
                    int startIndex = random.Next(0, 100)%len;
                    InstantiateChip(p, startPos[startIndex], gold);
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


        protected  Vector3 GetChipPos(int gold)
        {
            var gdata = App.GetGameData<BrnnGameData>();
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


        public void ResetChip()
        {
            var gdata = App.GetGameData<BrnnGameData>();
            OnBetClick(AllBet.SetChip(gdata.Ante, gdata.AnteRate));
        }

        public virtual void InstantiateChip(int p, Vector3 localPos, int gold,bool isBanker = false)
        {
            var widget = ChipCfg.DeskAreas[p];
            var chip = GetOneChip(p);
            var chipTs = chip.transform;
            chipTs.parent = ChipArea;
            chipTs.localPosition = localPos;
            chipTs.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<BrnnGameData>().AnteRate.IndexOf(gold),
                Depth = Chipdepth
            };
            
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            
            var sp = chip.GetComponent<SpringPosition>();
            //sp.target = GetClipPos(widget);
            sp.target = GetClipPos(widget);
            sp.enabled = true;
        }

        public virtual Chip GetOneChip(int p)
        {
            return BetPoolArray[p].GetOne();
        }

        

        protected Random R = new Random();
        public virtual Vector3 GetClipPos(UIWidget widget)
        {
            var v = Vector3.zero;
            var w = widget.width / 2;
            var h = widget.height / 2;
            var i2 = R.Next(-w, w);
            var i3 = R.Next(-h, h);
            var ts = widget.transform;
            v.x = ts.localPosition.x + i2;
            v.y = ts.localPosition.y + i3;
            return v;
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

        public virtual void ThrowChips(int gold, int p, bool isBanker)
        {

        }

        public virtual void Reset()
        {
            Chipdepth = 0;
            ShowChip();
            Init();
            foreach (var pool in BetPoolArray)
            {
                pool.HideAll();
            }
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
