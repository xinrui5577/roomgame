using System.Collections;
using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.item;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.server
{
    public class ResultCtrl : MonoBehaviour
    {

        public ResultItem[] resultItems;

        public UIButton GoOnBt;

        public UISprite Tip;

        public GameObject SnowPerfab;

        public Transform SnowPanel;

        private string normalName = "text_012";

        public void Reset()
        {
            foreach (ResultItem resultItem in resultItems)
            {
                resultItem.Reset();
            }
            Tip.spriteName = normalName;
        }

        public void ShowResult(ISFSObject param)
        {
            ISFSArray users = param.GetSFSArray("result");
            int len = resultItems.Length;
            int flagSeat = param.GetInt("flagseat");
            int[] leaveOrder = param.GetIntArray("leaveOrder");
            for (int i = 0; i < len; i++)
            {
                int index = -1;
                for (int j = 0; j < leaveOrder.Length; j++)
                {
                    if (leaveOrder[j]==i)
                    {
                        index = j;
                        break;
                    }
                }
                resultItems[i].SetInfo(users.GetSFSObject(i), flagSeat,i,index);
            }
            int m_score = users.GetSFSObject(0).GetInt("win");
            //if (Mathf.Abs(m_score)==2)
            //{
            //    Tip.spriteName = "xiaoxue";
            //    SnowAni();
            //}
            //else if (Mathf.Abs(m_score) == 4)
            //{
            //    Tip.spriteName = "daxue";
            //    SnowAni();
            //}
        }

        public void ClickRy()
        {
            if (App.GetGameData<SanPianGameData>().RoundNum >= App.GetGameData<SanPianGameData>().MaxRound)
            {
                App.GetGameManager<SanPianGameManager>().GameOver.gameObject.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            Reset();
            App.GetGameManager<SanPianGameManager>().Reset();
            gameObject.SetActive(false);
        }

        void SnowAni()
        {
            StartCoroutine(CreateSnow());
        }

        private float SnowMax = 0.6f;
        private float SnowMin = 0.2f;
        IEnumerator CreateSnow()
        {
            for (int i = 0; i < 40; i++)
            {
                GameObject snowobj = (GameObject)Instantiate(SnowPerfab, Vector3.one, Quaternion.identity);
                snowobj.transform.SetParent(SnowPanel);
                float Sc=Random.Range(SnowMin,SnowMax);
                snowobj.transform.localScale = new Vector3(Sc, Sc,1);
                snowobj.transform.localPosition = new Vector3(Random.Range(-(float)Screen.width / 2, (float)Screen.width / 2), Screen.height * 4/3, 0);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
