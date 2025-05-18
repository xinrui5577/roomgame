using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class CoinStackManager : MonoBehaviour
    {
        [System.Serializable]
        public class ScoreRelateCoin
        {
            public int GunScore;//玩家炮押分
            public int CoinStackHeight;//币堆高度(单位:个)
        }

        [System.Serializable]
        public class CoinStackHeightData
        {
            public int FishOdd;                          //鱼的赔率
            public ScoreRelateCoin[] Relate;    //押分币堆关联
        }

        /// <summary>
        /// 待显示币堆数据
        /// </summary>
        public struct CoinStackDataToView
        {
            public int coinNum;
            public int fishOdd;
            public int gunScore;
        }

        enum State
        {
            Moveing,//所有币堆移动
            HeadViewing,//最前币堆显示等待
            HeadFadeout//最前币堆渐隐
        }
        public CoinStackHeightData[] CoinStackHeightDatas; 
    
        public CoinStack Prefab_coinStack;
        public CoinStackImm Prefab_coinStackImm;
        public bool IsHeightStyle = false;//币堆风格,是高还是低
        public int NumCoinStackLimit = 3;//同时最多币堆显示数
        private Queue<CoinStack> mCoinStacks;

        public float TimeMoveUse = 0.5F;//移动金币堆使用时间
        public float OneStackWidth = 31.8336F;//0.0844444444F;
        private const float OneStackSpace = 0.0000384F;

        private CoinStackImm mCoinStackImm;

        private float mSpeedMuti = 1F;

        private bool mCurCoinStackBGColor = true;//当前玩家金币堆数字的背景颜色true红色,false绿色
        private float mCoinStackCurrentDepth;
        private Queue<CoinStackDataToView> mCoinStackDataToView;

        private State mState;
        void Awake()
        {
            mCoinStacks = new Queue<CoinStack>();
            mCoinStackDataToView = new Queue<CoinStackDataToView>();
            mCenterToCenterBetweenStack = OneStackWidth + OneStackSpace;
            mState = State.HeadViewing;
        }

        // Use this for initialization
        //IEnumerator Start () {
        //    //队列处理
        //    while (true)
        //    {
        //        if (mCoinStackDataToView.Count > 0 && mCoinStacks.Count < NumCoinStackLimit)
        //        {
        //            CoinStackDataToView csdtv = mCoinStackDataToView.Dequeue();
        //            PushOneStack(csdtv.coinNum, csdtv.fishOdd, csdtv.gunScore);
        //        }

        //        if (mCoinStacks.Count == 0)
        //        {
        //            yield return 0;
        //            continue;
        //        }
        //        else
        //        {
        //            mSpeedMuti = 1F + 0.7F*(mCoinStacks.Count - 1);
        //        }
        //        CoinStack csHead = mCoinStacks.Peek();
        //        if (csHead.IsRoleToHead)
        //        {
 
        //            //csHead.TimeDisappear /= mSpeedMuti;
        //            csHead.FadeOut();
        //            yield return new WaitForSeconds(csHead.TimeDisappear);
        //            mCoinStacks.Dequeue();
        //            //Destroy(csHead.gameObject);
        //            Pool_GameObj.RecycleGO(null, csHead.gameObject);

 
        //            float stackCenterSpace = (OneStackWidth + OneStackSpace);
        //            float curTime = 0F;
        //            Transform TempCoinStackTs = null;
        //            //移动逻辑
        //            while (curTime <= TimeMoveUse / mSpeedMuti)
        //            {
        //                float needTime = TimeMoveUse / mSpeedMuti;
        //                int idx = 1;
                    
        //                foreach (CoinStack cs in mCoinStacks)
        //                {
        //                    TempCoinStackTs = cs.transform;
        //                    TempCoinStackTs.localPosition = new Vector3(-stackCenterSpace * idx + curTime / needTime * stackCenterSpace, 0F, TempCoinStackTs.localPosition.z);
        //                    ++idx;
        //                }
        //                if (curTime == needTime)
        //                {
        //                    break;
        //                }

        //                curTime += Time.deltaTime;
        //                if (curTime > needTime)
        //                    curTime = needTime;
        //                yield return 0;
        //            }

        //        }
        //        yield return 0;
        //    }
        
         
        //}

        private bool mState_IsMovingStacks = false;//是否正在移动币堆
        //private bool mState_
        private float mElapse_MovingStacks;//
        private float mCenterToCenterBetweenStack;//币堆间的距离
        void Update()
        {
            //如果显示中的币堆少于限制数,则加入新币堆.
            if (mCoinStackDataToView.Count > 0 && mCoinStacks.Count < NumCoinStackLimit)
            {
                CoinStackDataToView csdtv = mCoinStackDataToView.Dequeue();
                PushOneStack(csdtv.coinNum, csdtv.fishOdd, csdtv.gunScore);
            }

            if (mState_IsMovingStacks)
            {
                if (mCoinStacks.Count == 0)
                    goto TAG_BREAK_MOVINGSTACK;

                if (mElapse_MovingStacks < TimeMoveUse)
                {
                    int idx = 1;
                    Transform tsCoinStack;
                    foreach (CoinStack cs in mCoinStacks)
                    {
                        tsCoinStack = cs.transform;
                        tsCoinStack.localPosition = new Vector3(-mCenterToCenterBetweenStack * idx + mElapse_MovingStacks / TimeMoveUse * mCenterToCenterBetweenStack, 0F, tsCoinStack.localPosition.z);
                        ++idx;
                    }
                    //if (mElapse_MovingStacks == TimeMoveUse)
                    //{
                    //    break;
                    //}

                    //if (mElapse_MovingStacks > TimeMoveUse)
                    //    mElapse_MovingStacks = TimeMoveUse;


                    mElapse_MovingStacks += Time.deltaTime;
                }
                else//移动到目标位置
                {
                    int idx = 1;
                    Transform tsCoinStack;
                    foreach (CoinStack cs in mCoinStacks)
                    {
                        tsCoinStack = cs.transform;
                        tsCoinStack.localPosition = new Vector3(-mCenterToCenterBetweenStack * idx + mCenterToCenterBetweenStack, 0F, tsCoinStack.localPosition.z);
                        ++idx;
                    }
                    CoinStack coinStackHead = mCoinStacks.Peek();
                    if (coinStackHead.IsRoleToHead)//如果最前面的还没有pileup到顶部,则不进行消失
                    {
                        coinStackHead.FadeOut();
                        mElapse_MovingStacks = 0F;
                        mState_IsMovingStacks = false;
                    }
                }
                TAG_BREAK_MOVINGSTACK: ;
            }
        

        }

        void Handle_CoinStackRollToHead(CoinStack cStack)
        {
            if (mCoinStacks.Count != 0 
                && cStack.GetInstanceID() == mCoinStacks.Peek().GetInstanceID())
                cStack.FadeOut();
        }

        void Handle_CoinStackDisappear(CoinStack cStack)
        {
            mCoinStacks.Dequeue();
            Pool_GameObj.RecycleGO(null, cStack.gameObject);
            if (mCoinStacks.Count != 0)
            {
                mState_IsMovingStacks = true;
                mElapse_MovingStacks = 0F;
            }
        }
        /// <summary>
        /// 获得币堆高度
        /// </summary>
        /// <param name="fishOdd"></param>
        /// <param name="gunScore"></param>
        /// <returns></returns>
        public int GetCoinStackHeight(int fishOdd, int gunScore)
        {
            for (int i = CoinStackHeightDatas.Length - 1; i >= 0; --i)
            {
                if (fishOdd >= CoinStackHeightDatas[i].FishOdd)
                {
                    for (int j = CoinStackHeightDatas[i].Relate.Length - 1; j >= 0; --j)
                    {
                        if (gunScore >= CoinStackHeightDatas[i].Relate[j].GunScore)
                        {
                            int outVal = CoinStackHeightDatas[i].Relate[j].CoinStackHeight / (IsHeightStyle?1:2);
                            return outVal <= 0 ? 1: outVal;
                        }
                    }

                }
            }

            return 1;
        }
        /// <summary>
        /// 请求显示一个币堆(不一定即时显示)
        /// </summary>
        /// <param name="coinNum"></param>
        /// <param name="fishOdd"></param>
        /// <param name="gunScore"></param>
        /// <param name="playerIdx"></param>
        public void RequestViewOneStack(int coinNum, int fishOdd, int gunScore)
        {
            //return;
            CoinStackDataToView d;
            d.coinNum = coinNum;
            d.fishOdd = fishOdd;
            d.gunScore = gunScore;
            mCoinStackDataToView.Enqueue(d);
        }

    
        private void PushOneStack(int coinNum,int fishOdd,int gunScore)
        { 
            //return;
            if (coinNum <= 0)
                return;


            //CoinStack coinStackNew = Instantiate(Prefab_coinStack) as CoinStack;
            CoinStack coinStackNew = Pool_GameObj.GetObj(Prefab_coinStack.gameObject).GetComponent<CoinStack>();
            Transform coinStackNewTs = coinStackNew.transform;
            mCoinStackCurrentDepth = mCoinStacks.Count == 0 ? 0F : mCoinStackCurrentDepth - 0.1F;
            coinStackNewTs.parent = transform;
            coinStackNewTs.localRotation = Quaternion.identity;
            coinStackNewTs.localPosition = new Vector3(-mCoinStacks.Count * (OneStackWidth + OneStackSpace), 0F, mCoinStackCurrentDepth);
            coinStackNew.PileUp(coinNum, GetCoinStackHeight(fishOdd, gunScore), mCurCoinStackBGColor);

            mCurCoinStackBGColor = !mCurCoinStackBGColor;

            coinStackNew.EvtDisappear += Handle_CoinStackDisappear;
            coinStackNew.EvtRoleToHead += Handle_CoinStackRollToHead;

            mCoinStacks.Enqueue(coinStackNew);

            //mState_IsMovingStacks = true;
        }

        public void OneStack_SetNum(int num)
        {
            if (num != 0)
            {
                if (mCoinStackImm == null)
                {
                    mCoinStackImm = Instantiate(Prefab_coinStackImm) as CoinStackImm;
                    mCoinStackImm.transform.parent = transform;
                    mCoinStackImm.transform.localRotation = Quaternion.identity;
                    mCoinStackImm.transform.localPosition = Vector3.zero;// new Vector3(-(OneStackWidth + OneStackSpace), 0F, 0F);
                }
                mCoinStackImm.SetNum(num);
            }
            else
            {
                if (mCoinStackImm != null)
                {
                    Destroy(mCoinStackImm.gameObject);
                    mCoinStackImm = null;
                }
            }
        }
        //private int upNum = 1;
        //void OnGUI()
        //{
        //    string numStr = GUILayout.TextArea(upNum.ToString());
        //    upNum = int.Parse(numStr);
        //    if (GUILayout.Button("PushOneStack"))
        //    {
        //        PushOneStack(upNum);
        //    }
        //}
    
    }
}
