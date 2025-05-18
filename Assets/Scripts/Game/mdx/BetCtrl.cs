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

namespace Assets.Scripts.Game.mdx
{
    public class BetCtrl : YxView
    {

        public GameObject Select;
        /// <summary>
        /// 筹码区域
        /// </summary>
        public GameObject ChipParents;
        public ResetChip AllBet;

        public GameObject WinBankerView;
       

        public Chip CurrentSelectChip;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public ChipConfig ChipCfg = new ChipConfig();

        protected Pool<Chip>[] BetPoolArray;

        private int _selfBetSide = -1;
     

        public void OnBetClick(Chip chip)
        {
            Select.transform.position = chip.transform.position;
            CurrentSelectChip = chip;
        }


        public void OnClickFastBetBtn(Chip chip)
        {
            YxMessageBox.Show(new YxMessageBoxData
            {
                Msg = App.GetGameManager<MdxGameManager>().TipStringFormat.ChoiseFastBet ,
                BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                Listener = (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        OnBetClick(chip);
                    }
                }
            });
        }


        protected override void OnAwake()
        {
            base.OnAwake();
            Init();
            if (BetPoolArray == null)
            {
                var prefab = ChipCfg.ChipPerfab;
                int len = ChipCfg.DeskAreas.Length;
                BetPoolArray = new Pool<Chip>[len];
                for (int i = 0; i < len; i++)
                {
                    BetPoolArray[i] = new Pool<Chip>(prefab, 100, 0, transform, false);
                }
            }
        }

        public void ProcessUsersInfo(ISFSArray usersInfo)
        {
            foreach (ISFSObject user in usersInfo)
            {
                if (!user.ContainsKey("betGolds")) return;
                var selfSeat = App.GameData.SelfSeat;
                int seat = user.GetInt("seat");
                

                var betArray = user.GetIntArray("betGolds");
                int len = betArray.Length;
                
                for (int i = 0; i < len; i++)
                {
                    var betV = betArray[i];
                    if (betV == 0) continue;
                    if (seat == selfSeat)
                    {
                        SelfBet(i, betV);
                    }
                    else
                    {
                        GameInfoBet(i, betV);
                    }
                }
            }
        }

        private void GameInfoBet(int p, int betV)
        {
            var gdata = App.GetGameData<MdxGameData>();
            var anteRate = gdata.AnteRate;
            int count = anteRate.Count;
            int betCount = 0;
            for (int i = count - 1; i >= 0 && betCount <= 5;)
            {
                int chipVal = anteRate[i];
                if (betV >= chipVal)
                {
                    OthersBet(p, anteRate[i],false);
                    betV -= chipVal;
                    betCount++;
                }
                else
                {
                    i--;
                }
            }
        }

        public virtual void OnDeskClick(UIWidget widget)
        {
            int index;
            if (!int.TryParse(widget.name, out index) || index < 0) {return; }
            var gdata = App.GetGameData<MdxGameData>();
            var tipFormat = App.GetGameManager<MdxGameManager>().TipStringFormat;
            if(gdata.BankSeat < 0)
            {
                YxMessageTip.Show(tipFormat.NoBank);
                return;
            }
            if (CurrentSelectChip == null) { return; }
            if (!gdata.BeginBet) { return; }
            int selfCoinA = (int) gdata.GetPlayerInfo().CoinA;
            if(selfCoinA <= 0)
            {
                YxMessageTip.Show(tipFormat.NoEnoughGold);
                return;
            }
            var chipData = CurrentSelectChip.GetData<ChipData>() ;
            int gold = chipData == null ? Mathf.Min(gdata.MaxBet[index], selfCoinA) : chipData.Value;
            if (gold <= 0) {return;}

            if (gold > gdata.GetPlayerInfo().CoinA)
            {
                YxMessageTip.Show(tipFormat.NoEnoughGold);
                return;
            }


            if (HasFullGold(index, gold))
            {

                if (_selfBetSide >= 0 && index != _selfBetSide)
                {
                    YxMessageTip.Show(tipFormat.BetOneSide);
                    //YxMessageBox.Show(new YxMessageBoxData { Msg = "游戏只能单边下注." });
                    return;
                }
                var gserver = App.GetRServer<MdxGameServer>();
                gserver.UserBet(index, gold);
            }
            else
            {
                YxMessageTip.Show(tipFormat.BetOutLimit);
                //YxMessageBox.Show(new YxMessageBoxData { Msg = "超出上限，无法下注." });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">下的位置</param>
        /// <param name="gold">下的筹码</param>
        /// <returns></returns>
        private bool HasFullGold(int index, int gold)
        {
            var gdata = App.GetGameData<MdxGameData>();
            var maxBet = gdata.MaxBet[index];
           
            return maxBet >= gold;
        }

        public void ShowChip()
        {
            var gdata = App.GetGameData<MdxGameData>();
            if (!gdata.BeginBet) return;
            if (gdata.GetPlayerInfo<MdxUserInfo>().Forbiden)
            {
                App.GetGameManager<MdxGameManager>().ShowTip();
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

        private readonly Random _random = new Random();

        public virtual void Bet(ISFSObject responseData)
        {
            var gdata = App.GetGameData<MdxGameData>();
            var seat = responseData.GetInt("seat");
            var gold = responseData.GetInt("gold");
            var p = MdxTools.GetP(responseData.GetUtfString("p"));
            if (seat == gdata.SelfSeat)
            {
                SelfBet(p, gold);
                return;
            }
            //其他人
            Facade.Instance<MusicManager>().Play("Bet");
           
            OthersBet(p, gold);
            
        }

        public void OthersBet(int p,int gold,bool needAnim = true)
        {
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;
            if (len > 0)
            {
                var random = _random.Next(0, 100) % len;
                var anteRate = App.GameData.AnteRate;
                int bgId = anteRate.IndexOf(gold);
                if (bgId < 0)
                {
                    int count = anteRate.Count;
                    for (int i = count - 1; i >= 0;)
                    {
                        int rate = anteRate[i];
                        if (gold >= rate)
                        {
                            gold -= rate;
                            InstantiateChip(p, transform.InverseTransformVector(startPos[random].position), rate,i, false, needAnim);
                        }
                        else
                        {
                            i--;
                        }
                    }

                    if (gold > 0)
                    {
                        //少于这个最少的筹码,下注100
                        InstantiateChip(p, transform.InverseTransformVector(startPos[random].position), anteRate[0], 0, false, needAnim);
                    }
                }
                else
                {
                    InstantiateChip(p, transform.InverseTransformVector(startPos[random].position), gold, bgId, false, needAnim);
                }
            }
        }

        

        /// <summary>
        /// 本局自己最大下注额
        /// </summary>
        protected int SelfMaxBet;

        protected int MaxRoundBet;

        public void SelfBet(int p, int gold)
        {
            if (gold <= 0) return;

            var gdata = App.GetGameData<MdxGameData>();
            gdata.SetGameStatus(YxEGameStatus.PlayAndConfine);
            gdata.IsGaming = true;
            gdata.GetPlayer().Coin -= gold;
            var lpos = GetChipPos(gold);
            lpos = ChipArea.InverseTransformPoint(lpos);
            BetChips(p, lpos, gold);

            _selfBetSide = p;
        }

        void BetChips(int p ,Vector3 lpos, int gold)
        {
            var gdata = App.GetGameData<MdxGameData>();
            var anteRate = gdata.AnteRate;
            int anteRateCount = anteRate.Count;
            for (int i = anteRateCount - 1; i >= 0; )
            {
                int ante = anteRate[i];
                if (gold > 0 && ante <= gold)
                {
                    InstantiateChip(p, lpos, ante, i);
                    gold -= ante;
                }
                else
                {
                    i--;
                }
            }
        }


        public void MoveAllBet()
        {
            int childCount = ChipArea.childCount;
            var startPos = ChipCfg.StartPos;
            int len = startPos.Length;
            if (len > 0)
            {
                var target = transform.InverseTransformVector(startPos[0].position);
                for (int i = 0; i < childCount; i++)
                {
                    Transform item = ChipArea.GetChild(i);
                    MdxChip cs = item.GetComponent<MdxChip>();
                    List<EventDelegate> de = new List<EventDelegate> { new EventDelegate(cs, "HideChip") };
                    SetChipAnim(item.transform, target,  de);
                }
            }
        }

        private void SetChipAnim(Transform chipTs, Vector3 to, List<EventDelegate> actionList = null)
        {
            TweenPosition.Begin(chipTs.gameObject, 0.5f, to);

            var ts = chipTs.GetComponent<TweenScale>();
            ts.delay = 0;
            ts.to = Vector3.zero;
            ts.duration = 0.5f;
            ts.ResetToBeginning();
            ts.PlayForward();
            ts.onFinished = actionList;
        }


        protected Vector3 GetChipPos(int gold)
        {
            var gdata = App.GetGameData<MdxGameData>();
            var anteRate = gdata.AnteRate;
            for (int i = 0; i < anteRate.Count; i++)
            {
                if (anteRate[i] == gold)
                {
                    return AllBet.ChipBtnList[i].transform.position;
                }
            }
            return AllBet.ChipBtnList[0].transform.position;
        }


        public virtual void ThrowChips(int gold, int p, bool bankerBet)
        {
        }

        protected int Chipdepth = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="localPos"></param>
        /// <param name="gold"></param>
        /// <param name="isBanker"></param>
        /// <param name="needAnimo"></param>
        public virtual void InstantiateChip(int p, Vector3 localPos, int gold, int bgId, bool isBanker = false, bool needAnimo = true)
        {
            if (bgId < 0)
            {
                return;
            }

            UIWidget widget = ChipCfg.DeskAreas[p];
            var chip = GetOneChip(p,isBanker);
            var chipTs = chip.transform;
            chipTs.parent = ChipArea;
            chipTs.localPosition = localPos;
            chipTs.localScale = Vector3.one;
            Chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                //BgId = App.GetGameData<MdxGameData>().GetChipIndex(gold),
                BgId = bgId,
                Depth = Chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            var pos = GetClipPos(widget);
            
            float delayTime = needAnimo ? 0.3f : 0f;
            TweenPosition.Begin(chip.gameObject, delayTime, pos);
            var ts = chip.GetComponent<TweenScale>();
            ts.delay = delayTime;
            ts.onFinished = null;// new List<EventDelegate>();
            TweenScale.Begin(chip.gameObject, delayTime, Vector3.one * .8f);
        }

        protected virtual Chip GetOneChip(int p ,bool isBnaker = false)
        {
            return BetPoolArray[p].GetOne();
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
            var gdata = App.GetGameData<MdxGameData>();
            OnBetClick(AllBet.InitChips(gdata.Ante, gdata.AnteRate));
        }

        internal Transform ChipArea;
        public override void Init()
        {
            _selfBetSide = -1;
            Chipdepth = 1;
            InitChipArea();
        }

        protected virtual void InitChipArea()
        {
            if (ChipArea != null) return;
            
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
                    int bgId = gdata.AnteRate.IndexOf(gold);
                    if (bgId >= 0)
                        InstantiateChip(p, transform.InverseTransformVector(startPos[startIndex].position), gold, bgId);
                }
            }
        }

        public virtual void Reset()
        {
            _selfBetSide = -1;
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
                Gizmos.DrawSphere(transform.InverseTransformDirection(ChipCfg.StartPos[i].position), 0.01f);
            }
        }

        public void ShowThing(GameObject go)
        {
            go.SetActive(true);
        }

        public void HideThing(GameObject go)
        {
            go.SetActive(false);
        }

        [Serializable]
        public class ChipConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public Chip ChipPerfab;
            public Transform[] StartPos = new Transform[0];
            public UIWidget[] DeskAreas;
        }

        public void ShowBankerMark()
        {
            WinBankerView.SetActive(true);
            ChildBack();
            HideChip();
        }

        void ChildBack()
        {
            WinBankerView.transform.GetChild(0).localPosition = Vector3.zero;
        }

        public void OnRelease()
        {
            ChildBack();
        }

        public void ShowChipBtns()
        {
            ShowChip();
            ChildBack();
            WinBankerView.SetActive(false);
        }
    }


}
