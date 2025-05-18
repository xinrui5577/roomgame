/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Asn1.X9
{
    /**
     * Class for processing an ECFieldElement as a DER object.
     */
    public class X9FieldElement
        : Asn1Encodable
    {
        private ECFieldElement f;

        public X9FieldElement(
            ECFieldElement f)
        {
            this.f = f;
        }

        public X9FieldElement(
            BigInteger		p,
            Asn1OctetString	s)
            : this(new FpFieldElement(p, new BigInteger(1, s.GetOctets())))
        {
        }

        public X9FieldElement(
            int				m,
            int				k1,
            int				k2,
            int				k3,
            Asn1OctetString	s)
            : this(new F2mFieldElement(m, k1, k2, k3, new BigInteger(1, s.GetOctets())))
        {
        }

        public ECFieldElement Value
        {
            get { return f; }
        }

        /**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         *  FieldElement ::= OCTET STRING
         * </pre>
         * <p>
         * <ol>
         * <li> if <i>q</i> is an odd prime then the field element is
         * processed as an Integer and converted to an octet string
         * according to x 9.62 4.3.1.</li>
         * <li> if <i>q</i> is 2<sup>m</sup> then the bit string
         * contained in the field element is converted into an octet
         * string with the same ordering padded at the front if necessary.
         * </li>
         * </ol>
         * </p>
         */
        public override Asn1Object ToAsn1Object()
        {
            int byteCount = X9IntegerConverter.GetByteLength(f);
            byte[] paddedBigInteger = X9IntegerConverter.IntegerToBytes(f.ToBigInteger(), byteCount);

            return new DerOctetString(paddedBigInteger);
        }
    }
}

#endif
