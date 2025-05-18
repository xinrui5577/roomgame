using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mx97
{
    public class ResultMgr : MonoBehaviour {

        List<UILabel> mListScore = new List<UILabel>();

        List<GameObject> mListOuter = new List<GameObject>();
        List<GameObject> mListLine = new List<GameObject>();
        List<GameObject> mListPoint = new List<GameObject>();

        // Use this for initialization
       protected void Start ()
        {
            var eventCenter = Facade.EventCenter;
            eventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.ShowGameResult, OnShowGameResult);
            eventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.InitGameResult, OnInitGameResult);
            eventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.ChangeLineScore, OnChangeLineScore);
            // 九个框 九个点
            for (var i = 0; i < 9; i++)
            {
                var go = transform.FindChild("Outer" + i).gameObject;
                if ( go == null ){ continue;}
                mListOuter.Add(go);

                go = transform.FindChild("Point" + i).gameObject;
                if ( go == null ){    continue;}
                mListPoint.Add(go);

                // 八条线 八个积分
                if (i < 8)
                {
                    go = transform.FindChild("Line" + i).gameObject;
                    if ( go == null ) { continue;}
                    mListLine.Add(go);

                    go = transform.FindChild("Score" + i + "/LabelScoreLine").gameObject;
                    mListScore.Add(go.GetComponent<UILabel>());
                } 
            }

        }

        private void OnInitGameResult(object obj)
        {
            foreach (var go in mListLine)
            {
                go.SetActive(false);
            }
            foreach (var go in mListOuter)
            {
                go.SetActive(false);
            }
            foreach (var go in mListPoint)
            {
                go.SetActive(false);
            }
        }

        private void OnChangeLineScore(object obj)
        {
            var gdata = App.GetGameData<Mx97GlobalData>();
            foreach (var uiLabel in mListScore)
            {
                var iBetScore = gdata.CurAnte * gdata.Ante;
                uiLabel.text = "￥" + YxUtiles.GetShowNumberForm(iBetScore);    
            }
        }

        private void OnShowGameResult(object obj)
        {
            //List<int> lines = App.GetGameData<Mx97GlobalData>().StartData.MLineList;         //后台划线的算法有bug，某些情况会出问题.

            FruitLine.getInstance().finalFruit();
            List<int> lines = FruitLine.getInstance().lineValue;

            if ( lines.Count <= 0 || 8 < lines.Count )
                return;

            // ----------------------------------
            if (lines[0] == 1)
            {
                mListLine[0].SetActive(true);
                //mListOuter[0].SetActive(true);
                //mListPoint[0].SetActive(true);
                //mListOuter[7].SetActive(true);
                //mListPoint[7].SetActive(true);
                //mListOuter[6].SetActive(true);
                //mListPoint[6].SetActive(true);
            }

            if (lines[1] == 1)
            {
                mListLine[1].SetActive(true);
                //mListOuter[1].SetActive(true);
                //mListPoint[1].SetActive(true);
                //mListOuter[8].SetActive(true);
                //mListPoint[8].SetActive(true);
                //mListOuter[5].SetActive(true);
                //mListPoint[5].SetActive(true);
            }

            if (lines[2] == 1)
            {
                mListLine[2].SetActive(true);
                //mListOuter[2].SetActive(true);
                //mListPoint[2].SetActive(true);
                //mListOuter[3].SetActive(true);
                //mListPoint[3].SetActive(true);
                //mListOuter[4].SetActive(true);
                //mListPoint[4].SetActive(true);
            }

            if (lines[3] == 1)
            {
                mListLine[3].SetActive(true);
                //mListOuter[2].SetActive(true);
                //mListPoint[2].SetActive(true);
                //mListOuter[8].SetActive(true);
                //mListPoint[8].SetActive(true);
                //mListOuter[6].SetActive(true);
                //mListPoint[6].SetActive(true);
            }

            if (lines[4] == 1)
            {
                mListLine[4].SetActive(true);
                //mListOuter[2].SetActive(true);
                //mListPoint[2].SetActive(true);
                //mListOuter[1].SetActive(true);
                //mListPoint[1].SetActive(true);
                //mListOuter[0].SetActive(true);
                //mListPoint[0].SetActive(true);
            }

            if (lines[5] == 1)
            {
                mListLine[5].SetActive(true);
                //mListOuter[3].SetActive(true);
                //mListPoint[3].SetActive(true);
                //mListOuter[8].SetActive(true);
                //mListPoint[8].SetActive(true);
                //mListOuter[7].SetActive(true);
                //mListPoint[7].SetActive(true);
            }

            if (lines[6] == 1)
            {
                mListLine[6].SetActive(true);
                //mListOuter[4].SetActive(true);
                //mListPoint[4].SetActive(true);
                //mListOuter[5].SetActive(true);
                //mListPoint[5].SetActive(true);
                //mListOuter[6].SetActive(true);
                //mListPoint[6].SetActive(true);
            }

            if (lines[7] == 1)
            {
                mListLine[7].SetActive(true);
                //mListOuter[4].SetActive(true);
                //mListPoint[4].SetActive(true);
                //mListOuter[8].SetActive(true);
                //mListPoint[8].SetActive(true);
                //mListOuter[0].SetActive(true);
                //mListPoint[0].SetActive(true);
            }

        }
    }
}
