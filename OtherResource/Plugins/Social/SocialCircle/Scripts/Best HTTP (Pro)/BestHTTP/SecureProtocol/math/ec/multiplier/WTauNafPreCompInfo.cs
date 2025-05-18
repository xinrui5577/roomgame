/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    /**
     * Class holding precomputation data for the WTNAF (Window
     * <code>&#964;</code>-adic Non-Adjacent Form) algorithm.
     */
    public class WTauNafPreCompInfo
        : PreCompInfo
    {
        /**
         * Array holding the precomputed <code>AbstractF2mPoint</code>s used for the
         * WTNAF multiplication in <code>
         * {@link org.bouncycastle.math.ec.multiplier.WTauNafMultiplier.multiply()
         * WTauNafMultiplier.multiply()}</code>.
         */
        protected AbstractF2mPoint[] m_preComp;

        public virtual AbstractF2mPoint[] PreComp
        {
            get { return m_preComp; }
            set { this.m_preComp = value; }
        }
    }
}

#endif
