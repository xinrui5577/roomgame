using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class CoinStack : MonoBehaviour,IPoolObj {
        public tk2dSpriteAnimator Ani_PileTopCoin;
        public tk2dTextMesh Text_CoinNum;
        public tk2dSlicedSprite SliSpr_CoinNumBG;//数字背景
        //public AnimationCurve CoinNumToPileNumCurve;
        public Renderer Rd_PileCoin;
        public Transform TsPileCoin;
        public float TimeDisappear = 1F;
        public bool IsRoleToHead = false;
        public float PileUpOneCoinElapse = 0.02F;//上一个币的间隔时间 s
        public float PileUpViewTime = 0.5F;//到顶后显示时间
 
        public delegate void Event_Generic(CoinStack p);
        public Event_Generic EvtDisappear;

        public Event_Generic EvtRoleToHead;


        private int mCoinNum = 50;//金币数目
        private int mNumToTop = 10;//到顶数目

        private const float OneCoinHeight = 5.9733333333333504F;
        GameObject mPrefab;
        public GameObject Prefab
        {
            get { return mPrefab; }
            set { mPrefab = value; }

        }

        public void Awake()
        {
            Rd_PileCoin.sharedMaterial = Instantiate(Rd_PileCoin.sharedMaterial) as Material;
            //YxDebug.Log("Awake");
        }

        public void OnDestroy()
        {
            if(Rd_PileCoin != null)
                Destroy(Rd_PileCoin.sharedMaterial);
        }
        /// <summary>
        /// 重用
        /// </summary>
        public void On_Reuse(GameObject prefab)
        {
            gameObject.SetActive(true);
            Text_CoinNum.color = Color.white;
            Text_CoinNum.Commit();
            Rd_PileCoin.sharedMaterial.color = Color.white;//todo :检查泄露

            SliSpr_CoinNumBG.color = Color.white;
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void On_Recycle()
        {
            gameObject.SetActive(false);
            EvtDisappear = null;
            EvtRoleToHead = null;
        }


        public void PileUp(int coinNum,int stackHeight,bool redOrdGreenBG)
        {
            mCoinNum = coinNum;
            mNumToTop = stackHeight;//Mathf.RoundToInt(CoinNumToPileNumCurve.Evaluate((float)coinNum / 1000F));
            if (!redOrdGreenBG)
                SliSpr_CoinNumBG.spriteId = SliSpr_CoinNumBG.GetSpriteIdByName("CoinStackDigitBG_Green");
            else
                SliSpr_CoinNumBG.spriteId = SliSpr_CoinNumBG.GetSpriteIdByName("CoinStackDigitBG_Red");
            int digit = 1;
            int[] resets = new int[] { 10000, 1000, 100, 10 };
            for (int i = 0; i != resets.Length; ++i )
            {
                if (coinNum / resets[i] >= 1)
                {
                    digit = 5 - i;
                    break;
                }
            }
            //YxDebug.Log("digit " + digit);
            //Spr_CoinNumBG.transform.localScale = new Vector3(0.5F * digit, 1F, 1F);
            SliSpr_CoinNumBG.dimensions = new Vector2(10F + (10f * digit), 20F);
            //YxDebug.Log("numtoTop = " + mNumToTop + "     (float)coinNum / 1000F = " + ((float)coinNum / 1000F));
            StartCoroutine(_Coro_Pileup());

        }

        private bool mIsFadingOut = false;
        public void FadeOut()
        {
            StartCoroutine(_Coro_FadeOut());
        }

        IEnumerator _Coro_FadeOut()
        {
            if (mIsFadingOut)
                yield break;

            mIsFadingOut = true;
            //渐隐币堆和数字
            float curTime = 0F;
            Color c = Color.white;
            while (curTime < TimeDisappear)
            {
                c.a = 1F - curTime / TimeDisappear;
                Text_CoinNum.color = c;
                Text_CoinNum.Commit();
                //Spr_PileCoin.color = c;
                Rd_PileCoin.sharedMaterial.color = c;//todo :检查泄露
                SliSpr_CoinNumBG.color = c;
                curTime += Time.deltaTime;
                yield return 0;
            }

            if (EvtDisappear != null)
                EvtDisappear(this);
            mIsFadingOut = false;
        }

        IEnumerator _Coro_Pileup()
        {
            IsRoleToHead = false;
            //Ani_PileTopCoin.Play(Ani_PileTopCoin.clipId);
            Ani_PileTopCoin.Play();
            Ani_PileTopCoin.GetComponent<Renderer>().enabled = true;
            Text_CoinNum.GetComponent<Renderer>().enabled = false;
            SliSpr_CoinNumBG.GetComponent<Renderer>().enabled = false;
            int currentNum = 0;
            while (true)
            {
                //上升
                TsPileCoin.localScale = new Vector3(1F, 0.0333333333333333F * currentNum, 1F);//贴图有30个币. 所以单币缩放高度是: 1/30 = 0.03333

                Ani_PileTopCoin.transform.localPosition = new Vector3(0F, OneCoinHeight * currentNum + OneCoinHeight * 0.02F, 0F);
                //Ani_PileTopCoin.Play(Ani_PileTopCoin.clipId);
                if (currentNum >= mNumToTop)
                {
                    Ani_PileTopCoin.GetComponent<Renderer>().enabled = false;
                    Text_CoinNum.transform.localPosition = new Vector3(0F, OneCoinHeight * currentNum+OneCoinHeight*0.25F  , -0.05F);
                    Text_CoinNum.GetComponent<Renderer>().enabled = true;
                    Text_CoinNum.text = mCoinNum.ToString();
                    Text_CoinNum.Commit();

                    SliSpr_CoinNumBG.GetComponent<Renderer>().enabled = true;
                    yield return new WaitForSeconds(PileUpViewTime);
                    IsRoleToHead = true;
                    if (EvtRoleToHead != null)
                    {
                        EvtRoleToHead(this);
                    }
                    //Destroy(gameObject);
                    yield break;
                }
                ++currentNum;

                yield return new WaitForSeconds(PileUpOneCoinElapse);
            }
        }

        //private int upNum = 1;
        //void OnGUI()
        //{
        //    string numStr = GUILayout.TextArea(upNum.ToString());
        //    upNum = int.Parse(numStr);
        //    if (GUILayout.Button("roleUP"))
        //    {
        //        PileUp(upNum);
        //    }
        //}
    }
}
