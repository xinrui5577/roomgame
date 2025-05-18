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
    * Class holding precomputation data for the WNAF (Window Non-Adjacent Form)
    * algorithm.
    */
    public class WNafPreCompInfo
        : PreCompInfo 
    {
        /**
         * Array holding the precomputed <code>ECPoint</code>s used for a Window
         * NAF multiplication.
         */
        protected ECPoint[] m_preComp = null;

        /**
         * Array holding the negations of the precomputed <code>ECPoint</code>s used
         * for a Window NAF multiplication.
         */
        protected ECPoint[] m_preCompNeg = null;

        /**
         * Holds an <code>ECPoint</code> representing Twice(this). Used for the
         * Window NAF multiplication to create or extend the precomputed values.
         */
        protected ECPoint m_twice = null;

        public virtual ECPoint[] PreComp
        {
            get { return m_preComp; }
            set { this.m_preComp = value; }
        }

        public virtual ECPoint[] PreCompNeg
        {
            get { return m_preCompNeg; }
            set { this.m_preCompNeg = value; }
        }

        public virtual ECPoint Twice
        {
            get { return m_twice; }
            set { this.m_twice = value; }
        }
    }
}

#endif
