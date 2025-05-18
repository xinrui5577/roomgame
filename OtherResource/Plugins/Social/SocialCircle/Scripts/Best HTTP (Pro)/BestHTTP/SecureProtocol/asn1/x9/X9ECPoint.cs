/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Asn1.X9
{
    /**
     * class for describing an ECPoint as a Der object.
     */
    public class X9ECPoint
        : Asn1Encodable
    {
        private readonly ECPoint p;

        public X9ECPoint(
            ECPoint p)
        {
            this.p = p.Normalize();
        }

        public X9ECPoint(
            ECCurve			c,
            Asn1OctetString	s)
        {
            this.p = c.DecodePoint(s.GetOctets());
        }

        public ECPoint Point
        {
            get { return p; }
        }

        /**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         *  ECPoint ::= OCTET STRING
         * </pre>
         * <p>
         * Octet string produced using ECPoint.GetEncoded().</p>
         */
        public override Asn1Object ToAsn1Object()
        {
            return new DerOctetString(p.GetEncoded());
        }
    }
}

#endif
