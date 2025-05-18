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
     * Class holding precomputation data for fixed-point multiplications.
     */
    public class FixedPointPreCompInfo
        : PreCompInfo
    {
        /**
         * Array holding the precomputed <code>ECPoint</code>s used for a fixed
         * point multiplication.
         */
        protected ECPoint[] m_preComp = null;

        /**
         * The width used for the precomputation. If a larger width precomputation
         * is already available this may be larger than was requested, so calling
         * code should refer to the actual width.
         */
        protected int m_width = -1;

        public virtual ECPoint[] PreComp
        {
            get { return m_preComp; }
            set { this.m_preComp = value; }
        }

        public virtual int Width
        {
            get { return m_width; }
            set { this.m_width = value; }
        }
    }
}

#endif
