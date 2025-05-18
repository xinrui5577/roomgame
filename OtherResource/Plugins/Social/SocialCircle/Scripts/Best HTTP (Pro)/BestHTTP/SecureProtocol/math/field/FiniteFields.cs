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

namespace Org.BouncyCastle.Math.Field
{
    public abstract class FiniteFields
    {
        internal static readonly IFiniteField GF_2 = new PrimeField(BigInteger.ValueOf(2));
        internal static readonly IFiniteField GF_3 = new PrimeField(BigInteger.ValueOf(3));

        public static IPolynomialExtensionField GetBinaryExtensionField(int[] exponents)
        {
            if (exponents[0] != 0)
            {
                throw new ArgumentException("Irreducible polynomials in GF(2) must have constant term", "exponents");
            }
            for (int i = 1; i < exponents.Length; ++i)
            {
                if (exponents[i] <= exponents[i - 1])
                {
                    throw new ArgumentException("Polynomial exponents must be montonically increasing", "exponents");
                }
            }

            return new GenericPolynomialExtensionField(GF_2, new GF2Polynomial(exponents));
        }

    //    public static IPolynomialExtensionField GetTernaryExtensionField(Term[] terms)
    //    {
    //        return new GenericPolynomialExtensionField(GF_3, new GF3Polynomial(terms));
    //    }

        public static IFiniteField GetPrimeField(BigInteger characteristic)
        {
            int bitLength = characteristic.BitLength;
            if (characteristic.SignValue <= 0 || bitLength < 2)
            {
                throw new ArgumentException("Must be >= 2", "characteristic");
            }

            if (bitLength < 3)
            {
                switch (characteristic.IntValue)
                {
                case 2:
                    return GF_2;
                case 3:
                    return GF_3;
                }
            }

            return new PrimeField(characteristic);
        }
    }
}

#endif
