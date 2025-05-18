using Assets.Scripts.Common.components;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = System.Random;

namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class BetCtrl02 : BetCtrl
    {
        [SerializeField]
        public ChipConfig BankerChipCfg;

        public TweenInfo BetTweenInfo;

        public TweenInfo ResultTweenInfo;

        Pool<Chip> _playerBetPool;

        Pool<Chip> _bankerPool;

        Random _ran = new Random();

        public Color[] SelectedBgColors;

        public override void Init()
        {
            base.Init();
            InitAmazedVal();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (_playerBetPool == null)
            {
                var prefab = ChipCfg.ChipPerfab;
                _playerBetPool = new Pool<Chip>(prefab, 150, 0, transform);
            }

            if(_bankerPool == null)
            {
                var prefab = BankerChipCfg.ChipPerfab;
                _bankerPool = new Pool<Chip>(prefab, 30, 0, transform);
            }
        }

        public override void OnBetClick(Chip chip)
        {
            int id = chip.GetData<ChipData>().BgId % SelectedBgColors.Length;
            Select.GetComponentInChildren<UITexture>().color = SelectedBgColors[id];
            base.OnBetClick(chip);
        }

        public override void GroupBet(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            var sfsArray = responseData.GetSFSArray("coin");
            var startPos = ChipCfg.StartPos;
            Facade.Instance<MusicManager>().Play("groupbet");
            var gdata = App.GameData;
            var selfSeat = gdata.GetPlayerInfo().Seat;

            int maxGold = 0;

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

                if (startPos.Length > 0)
                {
                    InstantiateChip(ChipCfg.DeskAreas[p], startPos[0], gold);
                }
            }

            PlayAmazedSoud(maxGold, ref MaxRoundBet);
        }

        /// <summary>
        /// 获取一个筹码
        /// </summary>
        /// <param name="isBanker"></param>
        /// <returns></returns>
        protected override Chip GetOneChip(bool isBanker = false)
        {
            var chip = isBanker ? _bankerPool.GetOne() : GetOneWithChipParentChildCount(_playerBetPool);
            chip.transform.parent = transform;
            return chip;
        }

        /// <summary>
        /// 检测同层级下筹码个数,选择池中的筹码
        /// </summary>
        /// <param name="pool">所选池</param>
        /// <returns></returns>
        protected Chip GetOneWithChipParentChildCount(Pool<Chip> pool)
        {
            var chip = pool.GetOne();
            return CouldGetThisChip(chip) ? chip : GetOneWithChipParentChildCount(pool);
        }

        /// <summary>
        /// 是否可以获取该筹码(该门下筹码少于10个,不能获取)
        /// </summary>
        /// <param name="chip"></param>
        /// <returns></returns>
        protected bool CouldGetThisChip(Chip chip)
        {
            var chipParent = chip.transform.parent;
            return chipParent.childCount > 10;      //该门下筹码少于10个,不能获取
        }


        public override void InstantiateChip(UIWidget widget, Vector3 localPos, int gold, bool isBanker = false, bool needAnimo = true)
        {
            var chip = (Chip02)GetOneChip(isBanker);
            var chipTs = chip.transform;
            chipTs.localPosition = localPos;
            chipTs.parent = widget.transform;
            chipTs.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<BjlGameData>().AnteRate.IndexOf(gold),
                Depth = Chipdepth
            };
            chip.UpdateView(data);
            Vector3 target = GetClipPos(widget);
            chip.gameObject.SetActive(true);
            SetChipAnim(chipTs, target, BetTweenInfo);
        }

        public override Vector3 GetClipPos(UIWidget widget)
        {
            var v = Vector3.zero;
            var w = widget.width / 2;
            var h = widget.height / 2;
            var i2 = _ran.Next(-w, w);
            var i3 = _ran.Next(-h, h);
            v.x = i2;
            v.y = i3;
            return v;
        }


        private void SetChipAnim(Transform chipTs, Vector3 to, TweenInfo tweenInfo, List<EventDelegate> actionList = null)
        {
            var tp = chipTs.GetComponent<TweenPosition>();
            if (tp == null) return;

            Vector3 from = chipTs.localPosition;
            tp.from = from;
            tp.to = to;
            tp.animationCurve = tweenInfo.TweenCurve;
            tp.duration = tweenInfo.Duration;

            tp.onFinished = actionList == null ? tweenInfo.OnFinish : actionList;

            var chip02 = chipTs.GetComponent<Chip02>();
            if (chip02 == null) return;
            chip02.DelayTime = tweenInfo.RandomDelay ? _ran.Next(0, 30) * 0.01f : tweenInfo.Delay;
            chip02.BeginAnim();
        }

        public override void ThrowChips(int gold, int p, bool bankerBet)
        {
            var golds = GetGoldList(gold).ToArray();
            var startPos = bankerBet ? BankerChipCfg.StartPos : ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                Facade.Instance<MusicManager>().Play("Bet");
                for (int i = 0; i < golds.Length; i++)
                {
                    var randomVal = _ran.Next(0, 100);
                    var randomIndex = randomVal % len;
                    InstantiateChip(ChipCfg.DeskAreas[p], startPos[randomIndex], golds[i], bankerBet);
                }
            }
        }

        public List<int> GetGoldList(int gold)
        {
            var goldList = new List<int>();
            var anteRate = new List<int>(App.GetGameData<BjlGameData>().AnteRate);
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


        protected override void InitChipArea()
        {
            if (ChipArea != null)
            {
                _playerBetPool.HideAll();
                return;
            }
            var go = new GameObject("ChipArea");
            ChipArea = go.transform;
            ChipArea.parent = ChipParents.transform;
            ChipArea.localPosition = Vector3.zero;
            ChipArea.localScale = Vector3.one;
            ChipArea.localRotation = Quaternion.identity;
        }

        public void MoveAllBet(int areaIndex, float persent,bool bankerWin)
        {
            var parent = ChipCfg.DeskAreas[areaIndex].transform;
            int childCount = parent.childCount;
            int index = Mathf.RoundToInt(childCount*persent);
            var startPos = bankerWin ? BankerChipCfg.StartPos : ChipCfg.StartPos;
            int len = startPos.Length;
            if (len > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    var target = i >= index ? ChipCfg.StartPos[0] : BankerChipCfg.StartPos[0];
                    Transform item = parent.GetChild(0);
                    item.parent = ChipArea;
                    Chip02 cs = item.GetComponent<Chip02>();
                    List<EventDelegate> de = new List<EventDelegate> { new EventDelegate(cs, "HideChip") };
                    SetChipAnim(item.transform, target, ResultTweenInfo, de);
                }
            }
        }

        protected override void OnDrawGizmos()
        {
            DrowPoint(ChipCfg.StartPos);

            DrowPoint(BankerChipCfg.StartPos);
        }

        void DrowPoint(Vector3[] arr)
        {
            if (arr == null) return;
            var len = arr.Length;
            for (var i = 0; i < len; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(arr[i]), 0.05f);
            }
        }

        public override void Reset()
        {
            base.Reset();
            InitAmazedVal();
        }

        void InitAmazedVal()
        {
            var anteRante = App.GetGameData<BjlGameData>().AnteRate;
            int midVal = (anteRante.Count - 1) / 2;         //确立中间索引
            midVal = anteRante[midVal];                     //确定中间值
            MaxRoundBet = midVal;
            SelfMaxBet = midVal;
        }

    }

    [Serializable]
    public class TweenInfo
    {
        public AnimationCurve TweenCurve;
        public float Delay;
        public float Duration;
        public List<EventDelegate> OnFinish;

        [Tooltip("是否是随机延迟")]
        public bool RandomDelay;
    }

}
