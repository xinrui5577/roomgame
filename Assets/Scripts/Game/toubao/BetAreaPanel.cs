using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using Assets.Scripts.Common.components;
using YxFramwork.View;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = System.Random;
using System;
using System.Collections.Generic;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.toubao
{
    public class BetAreaPanel : YxView
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

        protected int[] _meBet;
        protected int[] _allBet;
        private List<GameObject> labelList;


        public void CloseWinAnim()
        {
            Transform parents = transform.FindChild("Parents");
            foreach (Transform trans in parents)
            {
                var temp = trans.GetComponent<BetAreaItem>();
                if (temp != null)
                    temp.StopAnimate();
            }
        }

        public void OnBetClick(Chip chip)
        {
            Select.transform.position = chip.transform.position;
            CurrentSelectChip = chip;
        }

        protected override void OnStart()
        {
            Select.SetActive(false);
            _meBet = new int[ChipCfg.DeskAreas.Length];
            _allBet = new int[ChipCfg.DeskAreas.Length];
            labelList = new List<GameObject>();
            Init();
        }

        public virtual void OnDeskClick(UISprite widget, int rate)
        {
            string index = widget.name;
            var gdata = App.GetGameData<GlobalData>();
            if (CurrentSelectChip == null) { return; }
            if (!gdata.BeginBet) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if (gold > gdata.GetPlayerInfo().CoinA)
            {
                YxMessageBox.Show(new YxMessageBoxData { Msg = "金币不够，无法下注." });
                //Debug.LogError("金币不够，无法下注.");
                return;
            }
            if (BankerHasFullGold(rate, gold))
            {
                var gserver = App.GetRServer<TouBaoGameServer>();
                gserver.UserBet(index, gold);
            }
            else
            {
                YxMessageBox.Show(new YxMessageBoxData { Msg = "庄家金币不够，无法继续下注." });
                //Debug.LogError("庄家金币不够，无法继续下注.");
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">下的位置</param>
        /// <param name="gold">下的筹码</param>
        /// <returns></returns>
        private bool BankerHasFullGold(int index, int gold)
        {
            var banker = App.GetGameManager<TouBaoGameManager>().BankerManager.Banker;
            if (banker.Info.Seat == -1)
            {
                return true;
            }
            var canBet = (int)banker.Info.CoinA / index;
            if (gold < canBet)
            {
                return true;
            }
            return false;
        }

        public void ShowChip()
        {
            var gdata = App.GetGameData<GlobalData>();
            if (!gdata.BeginBet) return;
            AllBet.gameObject.SetActive(true);
            Select.SetActive(true);
        }

        public void HideChip()
        {
            AllBet.gameObject.SetActive(false);
            Select.SetActive(false);

        }
        /// <summary>
        /// 显示下注的筹码
        /// </summary>
        /// <param name="responseData"></param>
        public virtual void Bet(ISFSObject responseData)
        {
            var gdata = App.GetGameData<GlobalData>();
            var seat = responseData.GetInt("seat");
            var gold = responseData.GetInt("gold");
            var p = responseData.GetUtfString("p");
            var num = GetChipNum(p);
            if (num == -1) return;
            Facade.Instance<MusicManager>().Play("Bet");
            if (seat == gdata.SelfSeat)
            {
                App.GetGameManager<TouBaoGameManager>().CanQuit = false;
                gdata.IsGaming = true;
                gdata.GetPlayerInfo().CoinA -= gold;
                var lpos = CurrentSelectChip.transform.position;
                lpos = _chipArea.InverseTransformPoint(lpos);
                InstantiateChip(ChipCfg.DeskAreas[num], lpos, gold);
                return;
            }
            //其他人
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;

            if (len > 0)
            {
                var random = UnityEngine.Random.Range(0, 100) % len;
                InstantiateChip(ChipCfg.DeskAreas[num], startPos[random], gold);
            }
        }
        /// <summary>
        /// 显示自己和全部下注数量
        /// </summary>
        /// <param name="data"></param>
        public void ShowRefreshNum(ISFSObject data)
        {
            var gold = data.GetInt("gold");
            var p = data.GetUtfString("p");
            var num = GetChipNum(p);
            var gdata = App.GetGameData<GlobalData>();
            var seat = data.GetInt("seat");
            if (seat == gdata.SelfSeat)
            {
                _meBet[num] += gold;
                UserPanel user = App.GameData.GetPlayer() as UserPanel;
                user.AllBet += gold;
                user.OnceBet += gold;
                UILabel _myLabel = ChipCfg.DeskAreas[num].gameObject.GetComponent<BetAreaItem>().MyBet;
                _myLabel.text = YxUtiles.ReduceNumber(_meBet[num]);
                _myLabel.gameObject.SetActive(true);
                labelList.Add(_myLabel.gameObject);
            }
            _allBet[num] += gold;
            UILabel _allLabel = ChipCfg.DeskAreas[num].gameObject.GetComponent<BetAreaItem>().AllBet;
            _allLabel.text = YxUtiles.ReduceNumber(_allBet[num]);
            _allLabel.gameObject.SetActive(true);
            labelList.Add(_allLabel.gameObject);
        }

        public void ReSetRefreshNum()
        {
            for (int i = 0; i < ChipCfg.DeskAreas.Length; i++)
            {
                _meBet[i] = 0;
                _allBet[i] = 0;
            }
            while (labelList.Count != 0)
            {
                labelList[0].SetActive(false);
                labelList.RemoveAt(0);
            }
            labelList.Clear();
        }

        protected int GetChipNum(string str)
        {
            for (int i = 0; i < ChipCfg.DeskAreas.Length; i++)
            {
                if (ChipCfg.DeskAreas[i].name == str)
                {
                    return i;
                }
            }
            return -1;
        }

        private int _chipdepth = 1;
        /// <summary>
        /// 庄家是否有足够的筹码
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="localPos"></param>
        /// <param name="gold"></param>
        /// <param name="needAnimo"></param>
        public void InstantiateChip(UISprite widget, Vector3 localPos, int gold, bool needAnimo = true)
        {
            var chip = Instantiate(ChipCfg.ChipPerfab);
            var chipTs = chip.transform;
            chipTs.parent = _chipArea;
            chipTs.localPosition = localPos;
            chipTs.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            _chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = App.GetGameData<GlobalData>().AnteRate.IndexOf(gold),
                Depth = _chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            if (!needAnimo) { return; }
            var sp = chip.GetComponent<SpringPosition>();
            sp.target = GetClipPos(widget);
            sp.enabled = true;
        }
        /// <summary>
        /// 获得下注区域
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public Vector3 GetClipPos(UISprite widget)
        {
            var v = Vector3.zero;
            var r = new Random();
            var w = widget.width / 10;
            var h = widget.height / 10;
            var i2 = r.Next(-w, w);
            var i3 = r.Next(-h, h);
            var ts = widget.transform;
            if (ts.rotation == Quaternion.identity)
            {
                v.x = ts.localPosition.x + i2;
                v.y = ts.localPosition.y + i3;
            }
            else
            {
                v.x = ts.localPosition.x + i3;
                v.y = ts.localPosition.y + i2;
            }
            return v;
        }

        public void InitChips()
        {
            var gdata = App.GetGameData<GlobalData>();
            OnBetClick(AllBet.InitChips(gdata.Ante, gdata.AnteRate));
        }

        private Transform _chipArea;
        public override void Init()
        {
            _chipdepth = 1;
            InitChipArea();
        }

        private void InitChipArea()
        {
            if (_chipArea != null)
            {
                Destroy(_chipArea.gameObject);
            }
            var go = new GameObject("ChipArea");
            _chipArea = go.transform;
            _chipArea.parent = ChipParents.transform;
            _chipArea.localPosition = Vector3.zero;
            _chipArea.localScale = Vector3.one;
            _chipArea.localRotation = Quaternion.identity;
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

        [Serializable]
        public class ChipConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public Chip ChipPerfab;
            public Vector3[] StartPos = new Vector3[0];
            public UISprite[] DeskAreas;
        }
    }
}
