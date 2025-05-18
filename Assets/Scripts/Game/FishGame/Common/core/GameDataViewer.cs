using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class GameDataViewer : MonoBehaviour {
        public tk2dTextMesh Text_Info;
        public tk2dSprite Spr_BG;

        [System.NonSerialized]
        public bool IsOnlyViewNumber = false;//是否只显示数字

        private int[] mUpdateDatas;
        private int mRemainTime;
        private BackStageSetting mBss;
        private string[] ViewFormattedStrs =
            {
                "全部总盈利:  {0:d}币\r\n本次实际利润:  {1:d}币\r\n上分:  {2:d}币\r\n下分:  {3:d}币\r\n投币:  {4:d}币\r\n退币:  {5:d}币\r\n彩票:  {6:d}张\r\n剩余运行时间:  {7:d}分钟",
                "Totally Profits:  {0:d} Coin\r\nThis Time Net Profits:  {1:d} Coin\r\nAdding Scores Number:  {2:d} Coin\r\nReducing Score Number:  {3:d} Coin\r\nInserting Coin Quantity:  {4:d} Coin\r\nReturn Coin Quantity:  {5:d} Coin\r\nTicket:  {6:d} Ticket\r\nRemaining Run Time:  {7:d} Minute",
                "             {0:d}\r\n             {1:d}\r\n             {2:d}\r\n             {3:d}\r\n             {4:d}\r\n             {5:d}\r\n             {6:d}\r\n剩余运行时间:  {7:d}",
                "             {0:d}\r\n             {1:d}\r\n             {2:d}\r\n             {3:d}\r\n             {4:d}\r\n             {5:d}\r\n             {6:d}\r\nRemaining Run Time:  {7:d}"
            };
        private int mLaguageType;//0为中文，1为英文

        // Use this for initialization
        void Start () {
            mBss = GameMain.Singleton.BSSetting;

            if (mBss.LaguageUsing.Val == Scripts.Game.FishGame.Common.core.Language.Cn)
            {
                mLaguageType = 0;
            }
            else
                mLaguageType = 1;

            mUpdateDatas = new int[]
                {
                    mBss.His_GainTotal.Val
                    , mBss.His_GainCurrent.Val
                    , mBss.His_CoinUp.Val
                    , mBss.His_CoinDown.Val
                    , mBss.His_CoinInsert.Val
                    , mBss.His_CoinOut.Val
                    , mBss.His_TicketOut.Val
                    ,mBss.GetRemainRuntime()
                };
            UpdateView();
        }

        void UpdateView()
        {
            Text_Info.text
                = string.Format(IsOnlyViewNumber?ViewFormattedStrs[mLaguageType+2]:ViewFormattedStrs[mLaguageType]
                                , mUpdateDatas[0]
                                , mUpdateDatas[1]
                                , mUpdateDatas[2]
                                , mUpdateDatas[3]
                                , mUpdateDatas[4]
                                , mUpdateDatas[5]
                                , mUpdateDatas[6]
                                , mUpdateDatas[7]);
            Text_Info.Commit();
        }
        void Update()
        {
            if (mUpdateDatas[0] != mBss.His_GainTotal.Val)
            {
                mUpdateDatas[0] = mBss.His_GainTotal.Val;
                UpdateView();
            }

            if (mUpdateDatas[1] != mBss.His_GainCurrent.Val)
            {
                mUpdateDatas[1] = mBss.His_GainCurrent.Val;
                UpdateView();
            }
            if (mUpdateDatas[2] != mBss.His_CoinUp.Val)
            {
                mUpdateDatas[2] = mBss.His_CoinUp.Val;
                UpdateView();
            }
            if (mUpdateDatas[3] != mBss.His_CoinDown.Val)
            {
                mUpdateDatas[3] = mBss.His_CoinDown.Val;
                UpdateView();
            }
            if (mUpdateDatas[4] != mBss.His_CoinInsert.Val)
            {
                mUpdateDatas[4] = mBss.His_CoinInsert.Val;
                UpdateView();
            }
            if (mUpdateDatas[5] != mBss.His_CoinOut.Val)
            {
                mUpdateDatas[5] = mBss.His_CoinOut.Val;
                UpdateView();
            }
            if (mUpdateDatas[6] != mBss.His_TicketOut.Val)
            {
                mUpdateDatas[6] = mBss.His_TicketOut.Val;
                UpdateView();
            }
            if (mUpdateDatas[7] != mBss.GetRemainRuntime())
            {
                mUpdateDatas[7] = mBss.GetRemainRuntime();
                UpdateView();
            } 
        }
    }
}
