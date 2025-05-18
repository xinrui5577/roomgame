/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.X9
{
    public class X962Parameters
        : Asn1Encodable, IAsn1Choice
    {
        private readonly Asn1Object _params;

		public X962Parameters(
            X9ECParameters ecParameters)
        {
            this._params = ecParameters.ToAsn1Object();
        }

		public X962Parameters(
            DerObjectIdentifier namedCurve)
        {
            this._params = namedCurve;
        }

		public X962Parameters(
            Asn1Object obj)
        {
            this._params = obj;
        }

		public bool IsNamedCurve
        {
			get { return (_params is DerObjectIdentifier); }
        }

		public Asn1Object Parameters
        {
            get { return _params; }
        }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * Parameters ::= CHOICE {
         *    ecParameters ECParameters,
         *    namedCurve   CURVES.&amp;id({CurveNames}),
         *    implicitlyCA Null
         * }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            return _params;
        }
    }
}

#endif
