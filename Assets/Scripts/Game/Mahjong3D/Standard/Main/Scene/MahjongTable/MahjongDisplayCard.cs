using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongDisplayCard : MahjongTablePart
    {
        public Transform DisplayBottom;
        /// <summary>
        /// 暗宝特效
        /// </summary>
        private EffectObject mAnbaoEffect;

        public MahjongContainer DisplayMahjong
        {
            get { return mDisplayMahjong; }
        }

        /// <summary>
        /// 翻开的牌
        /// </summary>
        private MahjongContainer mDisplayMahjong;

        public override void OnReset()
        {
            if (mAnbaoEffect != null)
            {
                GameCenter.Pools.Push(mAnbaoEffect);
                mAnbaoEffect = null;
            }
            DisplayBottom.gameObject.SetActive(false);
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(mDisplayMahjong);
            mDisplayMahjong = null;
        }

        /// <summary>
        /// 设置翻开的牌
        /// </summary>  
        public MahjongContainer SetMahjong(int card, int laizi)
        {
            if (card == 0) return null;
            DisplayBottom.gameObject.SetActive(true);
            var scene = GameCenter.Scene;
            if (mDisplayMahjong.ExIsNullOjbect())
            {
                GameCenter.DataCenter.LeaveMahjongCnt--;
                scene.MahjongGroups.PopMahFromCurrWall();
                mDisplayMahjong = scene.MahjongCtrl.PopMahjong(card);
            }
            var bottom = DisplayBottom.FindChild("bottom");
            mDisplayMahjong.transform.SetParent(bottom);
            mDisplayMahjong.transform.localPosition = new Vector3(0, 0.1f, 0);
            mDisplayMahjong.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            mDisplayMahjong.transform.localScale = Vector3.one;
            mDisplayMahjong.gameObject.SetActive(true);
            if (card == laizi)
            {
                mDisplayMahjong.Laizi = true;
            }
            return mDisplayMahjong;
        }

        /// <summary>
        /// 设置翻宝麻将
        /// </summary>        
        public MahjongContainer SetBaoMahjong(int bao, bool isShow = true)
        {
            DisplayBottom.gameObject.SetActive(true);
            var scene = GameCenter.Scene;
            if (mDisplayMahjong.ExIsNullOjbect())
            {
                mDisplayMahjong = scene.MahjongCtrl.PopMahjong();
            }
            mDisplayMahjong.Value = bao;
            mDisplayMahjong.Laizi = true;
            int quadrant = 1;
            if (!isShow)
            {
                if (mAnbaoEffect == null)
                {
                    var obj = GameCenter.Pools.Pop<EffectObject>(PoolObjectType.anbao);
                    if (obj != null)
                    {
                        obj.ExSetParent(DisplayBottom);
                        obj.gameObject.SetActive(true);
                        obj.transform.localPosition = GameCenter.DataCenter.Config.AnbaoPos;
                        obj.Execute();
                        if (!obj.AutoRecycle)
                        {
                            mAnbaoEffect = obj;
                        }
                    }
                }
                quadrant = -1;
            }
            else
            {
                if (mAnbaoEffect != null)
                {
                    mAnbaoEffect.gameObject.SetActive(false);
                }
            }
            var bottom = DisplayBottom.FindChild("bottom");
            mDisplayMahjong.transform.SetParent(bottom);
            mDisplayMahjong.transform.localPosition = new Vector3(0, 0.1f, 0);
            mDisplayMahjong.transform.localRotation = Quaternion.Euler(new Vector3(90 * quadrant, 0, 0));
            mDisplayMahjong.transform.localScale = Vector3.one;
            mDisplayMahjong.gameObject.SetActive(true);
            //mDisplayMahjong.IsSign = true;
            return mDisplayMahjong;
        }
    }
}