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
using System.Collections;

using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.X9
{
    /**
     * A general class that reads all X9.62 style EC curve tables.
     */
    public class ECNamedCurveTable
    {
        /**
         * return a X9ECParameters object representing the passed in named
         * curve. The routine returns null if the curve is not present.
         *
         * @param name the name of the curve requested
         * @return an X9ECParameters object or null if the curve is not available.
         */
        public static X9ECParameters GetByName(string name)
        {
            X9ECParameters ecP = X962NamedCurves.GetByName(name);

            if (ecP == null)
            {
                ecP = SecNamedCurves.GetByName(name);
            }

            if (ecP == null)
            {
                ecP = NistNamedCurves.GetByName(name);
            }

            if (ecP == null)
            {
                ecP = TeleTrusTNamedCurves.GetByName(name);
            }

            if (ecP == null)
            {
                ecP = AnssiNamedCurves.GetByName(name);
            }

            return ecP;
        }

        /**
         * return the object identifier signified by the passed in name. Null
         * if there is no object identifier associated with name.
         *
         * @return the object identifier associated with name, if present.
         */
        public static DerObjectIdentifier GetOid(string name)
        {
            DerObjectIdentifier oid = X962NamedCurves.GetOid(name);

            if (oid == null)
            {
                oid = SecNamedCurves.GetOid(name);
            }

            if (oid == null)
            {
                oid = NistNamedCurves.GetOid(name);
            }

            if (oid == null)
            {
                oid = TeleTrusTNamedCurves.GetOid(name);
            }

            if (oid == null)
            {
                oid = AnssiNamedCurves.GetOid(name);
            }

            return oid;
        }

        /**
         * return a X9ECParameters object representing the passed in named
         * curve.
         *
         * @param oid the object id of the curve requested
         * @return an X9ECParameters object or null if the curve is not available.
         */
        public static X9ECParameters GetByOid(DerObjectIdentifier oid)
        {
            X9ECParameters ecP = X962NamedCurves.GetByOid(oid);

            if (ecP == null)
            {
                ecP = SecNamedCurves.GetByOid(oid);
            }

            if (ecP == null)
            {
                ecP = TeleTrusTNamedCurves.GetByOid(oid);
            }

            // NOTE: All the NIST curves are currently from SEC, so no point in redundant OID lookup

            return ecP;
        }

        /**
         * return an enumeration of the names of the available curves.
         *
         * @return an enumeration of the names of the available curves.
         */
        public static IEnumerable Names
        {
            get
            {
                IList v = Platform.CreateArrayList();
                CollectionUtilities.AddRange(v, X962NamedCurves.Names);
                CollectionUtilities.AddRange(v, SecNamedCurves.Names);
                CollectionUtilities.AddRange(v, NistNamedCurves.Names);
                CollectionUtilities.AddRange(v, TeleTrusTNamedCurves.Names);
                CollectionUtilities.AddRange(v, AnssiNamedCurves.Names);
                return v;
            }
        }
    }
}

#endif
