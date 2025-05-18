using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;


namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class CardsCtrl02 : MonoBehaviour
    {

        public SideGroup02 LongGroup;

        public SideGroup02 HuGroup;


        public GameObject[] WinPictures;



        ///// <summary>
        ///// 比牌中,牌的动画Tween,统一时长,一起结束
        ///// </summary>
        //public UITweener[] MatchTweeners;

        public Texture[] WinTextures;

        public string[] WinSoundNames;

        public GameObject MatchEffect;

        public GameObject WinEffect;

        public ParticleSystem PS;

        private int _point = -1;

        /// <summary>
        /// 胜利音效
        /// </summary>
        [SerializeField]
        private string[] _soundNames;

        public void BeginGiveCards(ISFSObject responseData)
        {
            Init();

            ISFSArray cards = responseData.GetSFSArray("cards");
            var longArray = cards.GetIntArray(0);
            var huArray = cards.GetIntArray(1);

            LongGroup.SetSideGroup(longArray);
            HuGroup.SetSideGroup(huArray);

            var pointArray = responseData.GetIntArray("cardsV");    //获得龙虎结果的数组,0号位是龙数据,1号位是虎数据
            //获取比牌的结果
            int longPoint = pointArray[0];      
            int huPoint = pointArray[1];

            _point = longPoint - huPoint;
            AddResult(_point);

            SetCardCtrlActive(true);

            if (MatchEffect != null)
                MatchEffect.SetActive(true);

            BeginMatch();
        }




        private void Init()
        {
            MatchEffect.SetActive(false);
            WinEffect.SetActive(false);
            int len = WinPictures.Length;
            for (int i = 0; i < len; i++)
            {
                WinPictures[i].SetActive(false);
            }
        }

        private void SetWinEffect(int winIndex)
        {
            var render = PS.GetComponent<Renderer>();
            render.material.mainTexture = WinTextures[winIndex];
        }

        /// <summary>
        /// 显示桌面胜利背景
        /// </summary>
        /// <param name="p"></param>
        private void ShowWinPic(int p)
        {
            var go = WinPictures[p];
            go.SetActive(true);
            var tween = go.GetComponent<UITweener>();
            if (tween != null)
            {
                tween.ResetToBeginning();
                tween.PlayForward();
            }
        }

        int GetWinIndex(int result)
        {
            if (result > 0)      //龙胜
            {
                return 0;
            }
            if (result < 0)     //虎胜
            {
                return 1;
            }
            
            return 2;
        }


        //private int GetCardsPoint(int[] array)
        //{
        //    int sum = 0;
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        int val = GetCardVal(array[i]);
        //        sum += val;
        //    }
        //    //此处为单牌,只取第一张数
        //    return sum % 10;
        //}

        //int GetCardVal(int cardVal)
        //{
        //    int val = cardVal%16;
        //    switch (val)
        //    {
        //        case 14:        //A
        //            val = 1;
        //            break;
        //        case 15:        //2
        //            val = 2;
        //            break;
        //        default:
        //            if(val > 10)    //J,Q,K为0点
        //            {
        //                val = 0;
        //            }
        //            break;
        //    }
        //    return val;
        //}
    

        public void SetCardCtrlActive(bool active)
        {
            gameObject.SetActive(active);
        }

        /// <summary>
        /// 开始比牌
        /// </summary>
        public void BeginMatch()
        {
            LongGroup.BeginMatchCard();
            HuGroup.BeginMatchCard();
        }

        /// <summary>
        /// 结果加入战绩
        /// </summary>
        /// <param name="trend"></param>
        public void AddResult(int trend)
        {
            var cfg = App.GetGameData<BjlGameData>().TrendConfig;

            cfg.AddTrend(trend);
        }

        /// <summary>
        /// 显示胜利特效
        /// </summary>
        public void ShowWinSide()
        {
            StartCoroutine(Show());
        }

        public float WaitTime = .7f;

        IEnumerator Show()
        {
            yield return new WaitForSeconds(WaitTime);

            //刷新战绩数据
            var cfg = App.GetGameData<BjlGameData>().TrendConfig;
            var view = cfg.TrendView;
            if (view != null)
            {
                view.UpdateView();
            }

            int winIndex = GetWinIndex(_point);
            ShowWinPic(winIndex);
            SetWinEffect(winIndex);
            MatchEffect.SetActive(false);
            WinEffect.SetActive(true);
            Facade.Instance<MusicManager>().Play(_soundNames[winIndex]);
        }
    }
}