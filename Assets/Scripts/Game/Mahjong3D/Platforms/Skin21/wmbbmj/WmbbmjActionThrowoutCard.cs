using UnityEngine;
using UnityEngine.UI;
using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class WmbbmjActionThrowoutCard : ActionThrowoutCard
    {
        private Sprite mCaipiaoSprite;
        private PanelPlayersInfo mPanelPlayers;

        /// <summary>
        /// 财飘状态
        /// </summary>
        private int[] mCaipiaoChairs;

        public override void OnReset()
        {
            mCaipiaoChairs = null;
        }

        public void SetCaipiaoChairs(int[] chairs)
        {
            mCaipiaoChairs = chairs;
        }      

        public override void ThrowoutCardAction(ISFSObject data)
        {
            base.ThrowoutCardAction(data);
            if (mCaipiaoSprite == null || mPanelPlayers == null)
            {
                var go = GameUtils.GetAssets<GameObject>("Caipiao");
                mCaipiaoSprite = go.GetComponent<Image>().sprite;
                mPanelPlayers = GameCenter.Hud.GetPanel<PanelPlayersInfo>();
            }
            if (mCaipiaoChairs == null)
            {
                var count = GameCenter.DataCenter.MaxPlayerCount;
                mCaipiaoChairs = new int[count];
            }
            var flag = false;
            var opPlayerChair = data.GetInt(RequestKey.KeySeat).ExSeatS2C();
            if (data.ContainsKey("caipiao"))
            {
                flag = true;
                mCaipiaoChairs[opPlayerChair] = data.GetInt("caipiao");
            }
            else
            {
                mCaipiaoChairs[opPlayerChair] = 0;
            }
            mPanelPlayers[opPlayerChair].SetHeadOtherImage(flag, mCaipiaoSprite);
            SetCaipiaoState();
        }

        public bool CheckCaipiaoState()
        {
            if (mCaipiaoChairs == null) return false;

            var flag = false;
            for (int i = 0; i < mCaipiaoChairs.Length; i++)
            {
                if (i != 0)
                {
                    flag = mCaipiaoChairs[i] >= 1;
                    if (flag)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SetCaipiaoState()
        {
            if (mCaipiaoChairs == null) return false;

            var flag = CheckCaipiaoState();
            var state = flag ? HandcardStateTyps.Ting : HandcardStateTyps.Normal;
            Game.MahjongGroups.MahjongHandWall[0].SetHandCardState(state);
            return flag;
        }
    }
}
