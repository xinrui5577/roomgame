using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.components;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;
using Random = System.Random;

namespace Assets.Scripts.Game.car
{
    public class CarBetView : YxView
    {
        public EventObject EventObj;
        public List<string> BetPosName = new List<string>();
        public List<UILabel> BetGoldList = new List<UILabel>();
        public List<UILabel> BetSelfList = new List<UILabel>();
        public UIButton ContinuePressureBtn;
        public string ContinuePressureBtnActive;
        public string ContinuePressureBtnUnActive;

        public GameObject Select;
        /// <summary>
        /// 筹码区域
        /// </summary>
        public GameObject ChipParents;
        public CarResetChip AllBet;


        public Chip CurrentSelectChip;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public ChipConfig ChipCfg = new ChipConfig();

        private Dictionary<int, int> _betGold = new Dictionary<int, int>();
        private Dictionary<int, int> _selfGold = new Dictionary<int, int>();

        private CarGameData _gdata
        {
            get { return App.GetGameData<CarGameData>(); }
        }

        private CarGameManager _gmanager
        {
            get { return App.GetGameManager<CarGameManager>(); }
        }

        private int[] _historyGolds = new int[8] ;
        private int[] _currentGolds = new int[8];
        private bool _lastRoundBet;
        private bool _currentRoundBet;
        private Transform _bankChipParent;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
            }
        }


        public void OnReceive(EventData data)
        {
            switch (data.Name)
            {
                case "Init":
                    InitChips();
                    InitContinuePressure();
                    break;
                case "Show":
                    ShowChip();
                    Init();
                    break;
                case "Hide":
                    HideChip();
                    break;
                case "Bet":
                    Bet((ISFSObject)data.Data);
                    break;
                case "BetClick":
                    OnBetClick((Chip)data.Data);
                    break;
                case "GroupBet":
                    GroupBet((ISFSObject)data.Data);
                    break;
                case "ShowWin":
                    StartCoroutine(ShowUserWin((ResultData)data.Data));
                    break;
            }
        }



        private void FreshCurrentUserBet(int pos, int gold)
        {
            if (_selfGold.ContainsKey(pos))
            {
                _selfGold[pos] += gold;
            }
            else
            {
                _selfGold.Add(pos, gold);
            }
            BetSelfList[pos].text = YxUtiles.GetShowNumberForm(_selfGold[pos], 0, "0.#");
        }

        public void OnBetClick(Chip chip)
        {
            for (int i = 0; i < AllBet.transform.childCount; i++)
            {
                var item = AllBet.transform.GetChild(i).localPosition;
                if (Math.Abs(item.y - 15) < 1)
                {
                    AllBet.transform.GetChild(i).localPosition = new Vector3(item.x, 0, item.z);
                }
            }
            Select.transform.position = chip.transform.position;
            var vec = chip.transform.localPosition;
            chip.transform.localPosition=new Vector3(vec.x,15,vec.z); 
            CurrentSelectChip = chip;
        }

        protected override void OnStart()
        {
            Select.SetActive(false);
            Init();
        }

        public virtual void OnDeskClick(Transform widget)
        {
            string index = widget.name;
            if (CurrentSelectChip == null) { return; }
            if (!_gdata.BeginBet) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if (gold > _gdata.GetPlayerInfo().CoinA)
            {
                YxMessageBox.Show(new YxMessageBoxData { Msg = "金币不够，无法下注." });
                return;
            }

            _currentRoundBet = true;

            ISFSObject obj = new SFSObject();
            obj.PutUtfString("p", index);
            obj.PutInt("gold", gold);
            EventObj.SendEvent("GameServerEvent", "Bet", obj);
        }

        private void InitContinuePressure(bool isClick = false)
        {
            if (ContinuePressureBtn)
            {
                if (isClick)
                {
                    ContinuePressureBtn.GetComponent<BoxCollider>().enabled = true;
                    ContinuePressureBtn.normalSprite = ContinuePressureBtnActive;
                }
                else
                {
                    ContinuePressureBtn.GetComponent<BoxCollider>().enabled = false;
                    ContinuePressureBtn.normalSprite = ContinuePressureBtnUnActive;
                }

            }
        }

        public void OnContinuePressure()
        {
            _currentRoundBet = true;
            EventObj.SendEvent("GameServerEvent", "ContinuteBet", _historyGolds);
        }

        public void ShowChip()
        {
            if (!_gdata.BeginBet) return;

            for (int i = 0; i < AllBet.transform.childCount; i++)
            {
                var item = AllBet.transform.GetChild(i);
                item.GetComponentInChildren<UISprite>().color = new Color(1, 1, 1,1);
                item.GetComponentInChildren<BoxCollider>().enabled = true;
            }

            var curSelect = AllBet.transform.GetChild(0).transform;
            curSelect.localPosition=new Vector3(curSelect.localPosition.x,15,0);  
            Select.SetActive(true);

        }
        public void HideChip()
        {
            for (int i = 0; i < AllBet.transform.childCount; i++)
            {
                var item = AllBet.transform.GetChild(i);
                item.GetComponentInChildren<UISprite>().color=new Color(1,1,1,160/255f);
                item.GetComponentInChildren<BoxCollider>().enabled = false;
                if (Math.Abs(item.localPosition.y - 15) < 1)
                {
                    AllBet.transform.GetChild(i).localPosition = new Vector3(item.localPosition.x, 0, item.localPosition.z);
                }
            }
            Select.SetActive(false);

            _betGold.Clear();
            _selfGold.Clear();
        }

        public void GroupBet(ISFSObject responseData)
        {

            if (responseData.ContainsKey("coin"))
            {
                var groupData = responseData.GetSFSArray("coin");
                for (int i = 0; i < groupData.Count; i++)
                {
                    var seat = groupData.GetSFSObject(i).GetInt("seat");
                    if (seat != _gdata.SelfSeat)
                    {
                        Bet(groupData.GetSFSObject(i));
                    }
                }
            }
        }

        public virtual void Bet(ISFSObject responseData)
        {
            var seat = responseData.GetInt("seat");
            var gold = responseData.GetInt("gold");

            var p = responseData.GetUtfString("p");
            var index = FindBetP(p);

            if (_betGold.ContainsKey(index))
            {
                _betGold[index] += gold;
            }
            else
            {
                _betGold.Add(index, gold);
            }
            BetGoldList[index].text = YxUtiles.GetShowNumberForm(_betGold[index], 2, "0.#");

            Facade.Instance<MusicManager>().Play("bet");

            if (seat == _gdata.SelfSeat)
            {
                _gdata.GetPlayer().Coin -= gold;
                _currentGolds[FindBetP(p)] += gold;
                FreshCurrentUserBet(index, gold);
                App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                //                return;
            }
            //其他人
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;

            if (len > 0)
            {
                for (int i = 0; i < _gmanager.CurrentTableCount; i++)
                {
                    if (_gdata.GoldRank[i] == seat)
                    {
                        if (i == 0)
                        {
                            _gmanager.SpecialPlayers[i].ShowStarMove(index);
                        }
                        _gmanager.SpecialPlayers[i].Coin -= gold;
                        if (seat != _gdata.SelfSeat)
                        {
                            var parent = _gmanager.SpecialPlayers[i].HeadPortrait.transform.GetComponentInParent<TweenPosition>();
                            if (parent == null) return;
                            if (parent.onFinished != null)
                            {
                                parent.onFinished.Clear();
                            }
                            parent.PlayForward();
                            parent.AddOnFinished(() =>
                            {
                                parent.PlayReverse();
                            });
                        }
                        break;
                    }
                }

                if (!_gdata.AnteRate.Contains(gold))
                {
                    var count = _gdata.AnteRate.Count - 1;
                    while (count >= 0)
                    {
                        if (gold >= _gdata.AnteRate[count])
                        {
                            if (seat != _gdata.SelfSeat)
                            {
                                InstantiateChip(ChipCfg.DeskAreas[index], startPos[0].localPosition, _gdata.AnteRate[count]);
                            }
                            else
                            {
                                var lpos = CurrentSelectChip.transform.position;
                                lpos = _chipArea.InverseTransformPoint(lpos);
                                InstantiateChip(ChipCfg.DeskAreas[index], lpos, _gdata.AnteRate[count]);
                            }

                            gold -= _gdata.AnteRate[count];
                        }
                        else
                        {
                            count--;
                        }
                    }
                }
                else
                {
                    if (seat != _gdata.SelfSeat)
                    {
                        InstantiateChip(ChipCfg.DeskAreas[index], startPos[0].localPosition, gold);
                    }
                    else
                    {
                        var lpos = CurrentSelectChip.transform.position;
                        lpos = _chipArea.InverseTransformPoint(lpos);
                        InstantiateChip(ChipCfg.DeskAreas[index], lpos, gold);
                    }
                }
            }
        }

        private int FindBetP(string posName)
        {
            for (int i = 0; i < BetPosName.Count; i++)
            {
                if (posName == BetPosName[i])
                {
                    return i;
                }
            }
            return -1;
        }
        private int _chipdepth = 1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="localPos"></param>
        /// <param name="gold"></param>
        /// <param name="needAnimo"></param>
        public void InstantiateChip(UIWidget widget, Vector3 localPos, int gold, bool needAnimo = true)
        {
            var chip = Instantiate(ChipCfg.ChipPerfab);
            chip.name = widget.name;
            var chipTs = chip.transform;
            chipTs.parent = _chipArea;
            chipTs.localPosition = localPos;
            chipTs.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            _chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = _gdata.AnteRate.IndexOf(gold),
                Depth = _chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            if (!needAnimo) { return; }

            var to = GetClipPos(widget);
            _allVector3S.Add(to);

            TweenPosition.Begin(chip.gameObject, _gdata.UnitTime * 3f, to);
            var sp = chip.GetComponent<TweenPosition>();
            Random r = new Random(GetRandomSeed());
            var time = r.Next(1, 5);
            sp.delay = time * _gdata.UnitTime;
            sp.AddOnFinished(() =>
            {
                TweenScale.Begin(sp.gameObject, _gdata.UnitTime * 3, new Vector3(0.5f, 0.5f, 0.5f));
                sp.GetComponent<TweenScale>().delay = _gdata.UnitTime * 2;
            });
        }

        int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public Vector3 GetClipPos(UIWidget widget)
        {
            var v = Vector3.zero;
            Random r = new Random(GetRandomSeed());
            var w = widget.width / 4;
            var h = widget.height / 4;

            var i2 = r.Next(-w, w);
            var i3 = r.Next(-h, h);
            var ts = widget.transform;
            v.x = ts.localPosition.x + i2;
            v.y = ts.localPosition.y + i3;
            return v;
        }

        public void InitChips()
        {
            OnBetClick(AllBet.InitChips(_gdata.Ante, _gdata.AnteRate, this, "OnBetClick"));
        }
        public Transform _chipArea;

        public override void Init()
        {
            _chipdepth = 1;
            foreach (var betGold in BetGoldList)
            {
                betGold.text = "";
            }
            foreach (var selfBet in BetSelfList)
            {
                selfBet.text = "";
            }
            InitChipArea();
            _allVector3S.Clear();
            _lastRoundBet = _currentRoundBet;
            if (!_lastRoundBet)
            {
                InitContinuePressure();
                _historyGolds = new int[8];
            }
            else
            {
                _historyGolds = _currentGolds;
                _currentGolds = new int[8] ;
                InitContinuePressure(true);
            }
            _currentRoundBet = false;
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

            if (_bankChipParent != null)
            {
                Destroy(_bankChipParent.gameObject);
            }
            var go1 = new GameObject("ss").transform;
            _bankChipParent = go1.transform;
            _bankChipParent.parent = ChipParents.transform;
            _bankChipParent.localPosition = Vector3.zero;
            _bankChipParent.localScale = Vector3.one;
            _bankChipParent.localRotation = Quaternion.identity;
        }

        private readonly List<Vector3> _allVector3S = new List<Vector3>();

        IEnumerator ShowUserWin(ResultData resultData)
        {
            if (resultData.Bpg != null)
            {
                var index = 0;
                while (_chipArea.childCount > 0)
                {
                    var item = _chipArea.transform.GetChild(index);
                    if (!_chipArea.GetChild(index).name.Equals(resultData.Car.ToString()))
                    {
                        if (item.GetComponent<TweenScale>() != null)
                        {
                            item.GetComponent<TweenScale>().PlayReverse();
                        }
                        var vec = item.localPosition;

                        var itemTween = item.GetComponent<TweenPosition>();
                        itemTween.ResetToBeginning();
                        itemTween.transform.localPosition = vec;
                        itemTween.delay = _gdata.UnitTime * 2;

                        itemTween.duration = _gdata.UnitTime * 3;
                        itemTween.from = vec;
                        itemTween.to = ChipCfg.StartPos[1].localPosition;
                        itemTween.enabled = true;
                        item.transform.parent = _bankChipParent;

                        index = 0;
                        itemTween.AddOnFinished(() =>
                        {
                            itemTween.transform.parent.gameObject.SetActive(false);
                        });
                    }
                    else
                    {
                        if (index == _chipArea.childCount - 1)
                        {
                            break;
                        }
                        else
                        {
                            index++;
                        }
                    }
                }

                yield return new WaitForSeconds(1f);

                for (int i = 0; i < resultData.Bpg.Length; i++)
                {
                    if (resultData.Bpg[i] < 0)
                    {
                        resultData.Bpg[i] = -resultData.Bpg[i];
                        var anteCount = _gdata.AnteRate.Count - 1;
                        while (anteCount >= 0)
                        {
                            if (resultData.Bpg[i] >= _gdata.AnteRate[anteCount])
                            {
                                InstantiateChip(ChipCfg.DeskAreas[i], ChipCfg.StartPos[1].localPosition, _gdata.AnteRate[anteCount]);
                                resultData.Bpg[i] -= _gdata.AnteRate[anteCount];
                            }
                            else
                            {
                                anteCount--;
                            }
                        }
                    }
                }

                yield return new WaitForSeconds(1f);

                ChipMoveBack(-1,-1, resultData);
            }
        }


        public void ChipMoveBack(int pos = 0, int num = -1,ResultData resultData=null)
        {

            Facade.Instance<MusicManager>().Play("win_bet");

            for (int i = 0; i < _chipArea.transform.childCount; i++)
            {
                if (_chipArea.transform.GetChild(i).GetComponent<TweenScale>() != null)
                {
                    _chipArea.transform.GetChild(i).GetComponent<TweenScale>().PlayReverse();
                }
                var item = _chipArea.transform.GetChild(i).GetComponent<TweenPosition>();

                var vec = item.transform.localPosition;
                item.ResetToBeginning();
                item.transform.localPosition = vec;
                item.delay = _gdata.UnitTime * 2;

                item.duration = _gdata.UnitTime * 7;
                item.from = vec;
                item.@from = item.transform.localPosition;
                item.to = ChipCfg.StartPos[pos + 1].localPosition;
                item.enabled = true;

                item.AddOnFinished(() =>
                {
                    item.gameObject.SetActive(false);
                  
                });
            }
            EventObj.SendEvent("ResultViewEvent", "Result", resultData);
        }


        [Serializable]
        public class ChipConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public Chip ChipPerfab;
            public Transform[] StartPos;
            public UIWidget[] DeskAreas;
        }
    }

}
