using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.mdx
{
    public class ResultListCtrl : MonoBehaviour
    {
        public GameObject ResultView;
        public GameObject ResultItem;
        public UIGrid Grid;
        public int MaxCount = 50;

        private readonly List<GameObject> _resultObjects = new List<GameObject>();
        private readonly List<ResultData> _resultDataList = new List<ResultData>();

        public void GetHistoryInfo(ISFSObject data)
        {
            if (!data.ContainsKey("recordArr")) return;
            ISFSArray array = data.GetSFSArray("recordArr");
            foreach (ISFSObject item in array)
            {
                var diceVals = item.GetIntArray("dice");
                bool bankerWin = item.GetInt("bankWin") > 0;
                AddResult(diceVals, bankerWin);
            }    
        }

        public void AddResult(int[] diceVals,bool bankerWin)
        {
            ResultData data = new ResultData
            {
                DiceVals = diceVals,
                BnakerWin = bankerWin,
                DiceType = GetDiceType(diceVals)
            };
            _resultDataList.Insert(0, data);
            if (_resultDataList.Count > MaxCount)
            {
                _resultDataList.Remove(_resultDataList[MaxCount]);
            }
            if (Grid.gameObject.activeInHierarchy)
            {
                RefreshView();
            }
        }

       

        /// <summary>
        /// 0大,1小,2豹子
        /// </summary>
        /// <param name="diceVals"></param>
        /// <returns></returns>
        int GetDiceType(int[] diceVals)
        {
            int len = diceVals.Length;
            int sum = 0;
            bool baozi = true;
            int lastVal = diceVals[0];
            for (int i = 0; i < len; i++)
            {
                int curVal = diceVals[i];
                sum += curVal;
               
                if (lastVal != curVal)
                {
                    baozi = false;
                }
            }
            return baozi ? 2 : sum > 10 ? 0 : 1;
        }


        public void RefreshView()
        {
            int count = _resultDataList.Count;
            for (int i = 0; i < count; i++)
            {
                var data = _resultDataList[i];
                var temp = GetItem(i);
                temp.GetComponent<SetResultList>().SetItem(data);
                temp.SetActive(true);
            }
           
            Grid.repositionNow = true;
            Grid.Reposition();
        }

        GameObject GetItem(int index)
        {
            if (_resultObjects.Count <= index)
            {
                var temp = Instantiate(ResultItem);
                temp.transform.parent = ResultItem.transform.parent;
                temp.transform.localScale = ResultItem.transform.localScale;
                _resultObjects.Add(temp);
                return temp;
            }
            return _resultObjects[index];
        }

        public void OnClickShowResultViewBtn()
        {
            ResultView.SetActive(true);
            RefreshView();
        }

        public void OnClickCloseBtn()
        {
            ResultView.SetActive(false);
        }

        internal void AddResult(ISFSObject response)
        {
            var diceVals = response.GetIntArray("dices");
            bool bwin = response.GetLong("bwin") > 0;
            AddResult(diceVals, bwin);
        }
    }

    public class ResultData
    {
        public int[] DiceVals;
        public bool BnakerWin;
        public int DiceType;        //类型 0:大 , 1:小 , 2:通杀
    }
}
