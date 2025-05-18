using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Windows;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class ResultWindow02 : ResultWindow
    {
        public NguiLabelAdapter ResultMe;
        public NguiLabelAdapter ResultBanker;
        public NguiLabelAdapter SelfBet;
        public NguiLabelAdapter TotalBet;
        public GameObject Item;
        [HideInInspector] public List<GameObject> RankList;
        public UIGrid WinParent;
        public UIGrid LostParent;
        public int MaxRankCount = 4;
        public GameObject[] EffectObjs;
        public UILabel TimerLabel;
        [SerializeField] private int _closeTime;
        private int _timer;

        protected override void OnFreshView()
        {
            Reset();
            var win = 0;
            _timer = _closeTime;
            RefreshCountDown();
            InvokeRepeating("RefreshCountDown", 0.1f, 1);

            var sfsdata = GetData<ISFSObject>();
            if (sfsdata == null) return;

            if (sfsdata.ContainsKey("selfBet"))
            {
                var selfBet = YxUtiles.ReduceNumber(sfsdata.GetInt("selfBet"));
                SelfBet.Text(selfBet);
            }

            if (sfsdata.ContainsKey("allBet"))
            {
                var allBet = YxUtiles.ReduceNumber(sfsdata.GetInt("allBet"));
                TotalBet.Text(allBet);
            }
            if (sfsdata.ContainsKey("win") && ResultMe != null)
            {
                win = sfsdata.GetInt("win");
                ResultMe.Text(YxUtiles.ReduceNumber(win));
            }
            if (sfsdata.ContainsKey("bwin") && ResultBanker != null)
            {
                var bwin = YxUtiles.ReduceNumber(sfsdata.GetLong("bwin"));
                ResultBanker.Text(bwin);
            }
            //设置排行榜信息
            if (sfsdata.ContainsKey("pwins"))
            {
                string[] rankArray = sfsdata.GetUtfStringArray("pwins");
                for (int i = 0; i < MaxRankCount; i++)
                {
                    CreateRankItem(rankArray, i);
                }
                WinParent.repositionNow = true;
                WinParent.Reposition();
                LostParent.repositionNow = true;
                LostParent.Reposition();
            }

            SetTitel(win);
        }

        private void RefreshCountDown()
        {
            if (_timer < 0)
            {
                OnClickCloseBtn();
            }
            TimerLabel.text = (_timer--).ToString();
        }

        void SetTitel(int resultMe)
        {
            var bResult = App.GetGameManager<BtwGameManager>().ResultListCtrl.ResultNum;
            switch (bResult)
            {
                case 3:
                    EffectObjs[4].SetActive(true);
                    break;
                case 0:
                    EffectObjs[3].SetActive(true);
                    break;
            }
            if (bResult == 3 || bResult == 0)
                return;

            if (resultMe > 0)
            {
                EffectObjs[0].SetActive(true);
                Facade.Instance<MusicManager>().Play("Win");
            }
            else if (resultMe == 0)
            {
                EffectObjs[1].SetActive(true);
                Facade.Instance<MusicManager>().Play("draw");
            }
            else
            {
                EffectObjs[2].SetActive(true);
                Facade.Instance<MusicManager>().Play("Lose");
            }
        }


        public void ShowResultView(ISFSObject response)
        {
            Reset();
            ShowWithData(response);
        }



        private void CreateRankItem(string[] rankArray, int index)
        {
            if (rankArray == null || rankArray.Length <= index)
            {
                CreatDefultRankItem(index, WinParent.transform);
                CreatDefultRankItem(index, LostParent.transform);
                return;
            }

            CreateWinItem(rankArray, index);
            CreateLostItem(rankArray, index);
        }

        private void CreateLostItem(string[] rankArray, int index)
        {
            int count = rankArray.Length - 1;
            var rankInfo = rankArray[count - index].Split(',');
            int gold = 0;
            var item = CreatDefultRankItem(index, LostParent.transform);
            if (int.TryParse(rankInfo[1], out gold) && gold < 0)
            {
                var cs = item.GetComponent<ResultRankItem>();
                cs.InitItem(rankInfo[0], gold, index + 1);
            }

        }

        private void CreateWinItem(string[] rankArray, int index)
        {
            var rankInfo = rankArray[index].Split(',');
            int gold = 0;
            var item = CreatDefultRankItem(index, WinParent.transform);
            if (int.TryParse(rankInfo[1], out gold) && gold > 0)
            {
                var cs = item.GetComponent<ResultRankItem>();
                cs.InitItem(rankInfo[0], gold, index + 1);
            }

        }

        private GameObject CreatDefultRankItem(int index, Transform parent)
        {
            //初始化物体
            var rankItem = Instantiate(Item);
            var rankItemTran = rankItem.transform;
            var rankItemCS = rankItemTran.GetComponent<ResultRankItem>();
            rankItemCS.InitItem("无", 0, index + 1);
            rankItemTran.parent = parent;
            rankItemTran.localScale = Vector3.one;
            rankItem.SetActive(true);

            RankList.Add(rankItem);

            return rankItem;
        }


        public void OnClickCloseBtn()
        {
            CancelInvoke();
            foreach (var effect in EffectObjs)
            {
                effect.SetActive(false);
            }
            Hide();
        }


        void Reset()
        {
            WinParent.transform.DestroyChildren();
            LostParent.transform.DestroyChildren();

            foreach (var item in EffectObjs)
            {
                item.SetActive(false);
            }
        }
    }
}