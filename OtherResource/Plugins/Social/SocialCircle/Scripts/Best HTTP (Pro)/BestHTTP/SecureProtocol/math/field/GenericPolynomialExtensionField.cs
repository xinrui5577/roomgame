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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.Field
{
    internal class GenericPolynomialExtensionField
        : IPolynomialExtensionField
    {
        protected readonly IFiniteField subfield;
        protected readonly IPolynomial minimalPolynomial;

        internal GenericPolynomialExtensionField(IFiniteField subfield, IPolynomial polynomial)
        {
            this.subfield = subfield;
            this.minimalPolynomial = polynomial;
        }

        public virtual BigInteger Characteristic
        {
            get { return subfield.Characteristic; }
        }

        public virtual int Dimension
        {
            get { return subfield.Dimension * minimalPolynomial.Degree; }
        }

        public virtual IFiniteField Subfield
        {
            get { return subfield; }
        }

        public virtual int Degree
        {
            get { return minimalPolynomial.Degree; }
        }

        public virtual IPolynomial MinimalPolynomial
        {
            get { return minimalPolynomial; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            GenericPolynomialExtensionField other = obj as GenericPolynomialExtensionField;
            if (null == other)
            {
                return false;
            }
            return subfield.Equals(other.subfield) && minimalPolynomial.Equals(other.minimalPolynomial);
        }

        public override int GetHashCode()
        {
            return subfield.GetHashCode() ^ Integers.RotateLeft(minimalPolynomial.GetHashCode(), 16);
        }
    }
}

#endif
