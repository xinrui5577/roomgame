using UnityEngine;
using Sfs2X.Entities.Data;
using Assets.Scripts.Common.Adapters;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Manager;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.brnn.brnn_skin02
{

    public class ResultWindow02 : ResultWindow
    {

        public NguiLabelAdapter SelfBet;

        public NguiLabelAdapter TotalBet;

        public GameObject ItemPrefab;

        [HideInInspector]
        public List<GameObject> RankList;

        public UIGrid WinParent;

        public UIGrid LostParent;

        public int MaxRankCount = 4;

        public GameObject[] EffectObjs;

        public UILabel TimerLabel;

        [SerializeField]
        private int _closeTime;

        private int _timer;




        protected override void OnFreshView()
        {
            Reset();
            _timer = _closeTime;
            RefreshCountDown();
            InvokeRepeating("RefreshCountDown", 0.1f, 1);
            base.OnFreshView();

            var sfsdata = GetData<ISFSObject>();
            if (sfsdata == null) return;

            if (sfsdata.ContainsKey("selfBet"))
            {
                SelfBet.Text(sfsdata.GetInt("selfBet"));
            }

            if (sfsdata.ContainsKey("allBet"))
            {
                TotalBet.Text(sfsdata.GetInt("allBet"));
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

            SetTitel();
        }

        private void RefreshCountDown()
        {
            if (_timer < 0)
            {
                OnClickCloseBtn();
            }
            TimerLabel.text = (_timer--).ToString();
        }

        void SetTitel()
        {
            var bResult = App.GetGameManager<BrnnGameManager02>().CardsCtrl.Result;
            if (bResult == null || bResult.Length <= 0)
                return;

            bool tongchi = true;
            bool tongpei = true;

            for (int i = 1; i < bResult.Length; i++)
			{
                tongchi &= !bResult[i];
                tongpei &= bResult[i];
            }
            EffectObjs[3].SetActive(tongchi);
            EffectObjs[4].SetActive(tongpei);

            if (tongchi || tongpei)
                return;

            int resultMe = App.GetGameData<BrnnGameData>().ResultUserTotal;
            if(resultMe > 0)
            {
                EffectObjs[0].SetActive(true);
                EffectObjs[1].SetActive(false);
                EffectObjs[2].SetActive(false);
                Facade.Instance<MusicManager>().Play("win");
            }
            else if (resultMe == 0)
            {
                EffectObjs[0].SetActive(false);
                EffectObjs[1].SetActive(true);
                EffectObjs[2].SetActive(false);
                Facade.Instance<MusicManager>().Play("draw");
            }
            else
            {
                EffectObjs[0].SetActive(false);
                EffectObjs[1].SetActive(false);
                EffectObjs[2].SetActive(true);
                Facade.Instance<MusicManager>().Play("lose");
            }
            
           

        }


        public override void ShowResultView(ISFSObject response)
        {
            Reset();
            ShowWithData(response);
        }



        private void CreateRankItem(string[] rankArray,int index)
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

        private void CreateLostItem(string[] rankArray,int index)
        {
            int count = rankArray.Length - 1;
            var rankInfo = rankArray[count - index].Split(',');
            int gold = 0;
            var item = CreatDefultRankItem(index, LostParent.transform);
            if(int.TryParse(rankInfo[1],out gold) && gold < 0)
            {
                var cs = item.GetComponent<ResultRankItem02>();
                cs.InitItem(rankInfo[0], gold, index + 1);
            }
           
        }

        private void CreateWinItem(string[] rankArray,int index)
        {
            var rankInfo = rankArray[index].Split(',');
            int gold = 0;
            var item = CreatDefultRankItem(index, WinParent.transform);
            if (int.TryParse(rankInfo[1], out gold) && gold > 0)
            {
                var cs = item.GetComponent<ResultRankItem02>();
                cs.InitItem(rankInfo[0], gold, index + 1);
            }
          
        }

        private GameObject CreatDefultRankItem(int index,Transform parent)
        {
            //初始化物体
            var rankItem = Instantiate(ItemPrefab);
            var rankItemTran = rankItem.transform;
            var rankItemCS = rankItemTran.GetComponent<ResultRankItem02>();
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