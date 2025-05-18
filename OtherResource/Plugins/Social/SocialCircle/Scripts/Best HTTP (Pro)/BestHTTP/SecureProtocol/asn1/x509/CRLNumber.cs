/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The CRLNumber object.
     * <pre>
     * CRLNumber::= Integer(0..MAX)
     * </pre>
     */
    public class CrlNumber
        : DerInteger
    {
        public CrlNumber(
			BigInteger number)
			: base(number)
        {
        }

		public BigInteger Number
		{
			get { return PositiveValue; }
		}

		public override string ToString()
		{
			return "CRLNumber: " + Number;
		}
	}
}

#endif
