using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using UnityEngine;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class ChooseCgPnl : MonoBehaviour
    {
        public HdCdsZone CgGroup;
        public DVoidNoParam CancelCall;

        //void Start()
        //{
        //    //if (null != GameAdpaterManager.Singleton)
        //    //{                
        //    //    GetComponent<RectTransform>().anchoredPosition3D = GameAdpaterManager.Singleton.GetConfig.ChooseCpPnl_Pos;
        //    //}
        //}

        public virtual void OnCancelClick()
        {
            Reset();
            CancelCall();
            CancelCall = null;
            EventDispatch.Dispatch((int)UIEventId.RedisplayButtons);
        }

        public virtual void SetChooseCg(List<int[]> cgList, DVoidInt call, int outPutCard=UtilDef.NullMj)
        {
            gameObject.SetActive(true);
            CgGroup.Clear();

            for (int i = 0;i < cgList.Count;i++)
            {
                var valueList = new List<int>(cgList[i]);
                if (valueList.Count < 4 && outPutCard != UtilDef.NullMj)
                    valueList.Add(outPutCard);

                valueList.Sort((a, b) =>
                {
                    if (a > b)
                        return 1;
                    if ((a < b))
                        return -1;
                    return 0;         
                });

                UiCardGroup group = D2MahjongMng.Instance.GetGroup(valueList.ToArray(), EnD2MjType.Me, true);
                CgGroup.AddUiCdGroup(group);
                int i1 = i;
                group.SetClickCallFunc(() =>
                {
                    UtilFunc.OutPutArray(cgList[i1], "选着的牌");
                    call(i1);
                    gameObject.SetActive(false);
                });
            }

            CgGroup.Sort(1);
        }

        public virtual void SetChooseCg(List<int[]> cgList, DVoidInt call, List<int> hzlzg, DVoidInt gCall, int outPutCard = UtilDef.NullMj)
        {
        }

        public void SetChooseTing()
        {
            gameObject.SetActive(true);

            CgGroup.Clear();
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            CgGroup.Clear();
        }
    }
}
