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
using System.IO;

namespace Org.BouncyCastle.Asn1
{
    public class Asn1OutputStream
        : DerOutputStream
    {
        public Asn1OutputStream(Stream os) : base(os)
        {
        }

		[Obsolete("Use version taking an Asn1Encodable arg instead")]
        public override void WriteObject(
            object    obj)
        {
            if (obj == null)
            {
                WriteNull();
            }
            else if (obj is Asn1Object)
            {
                ((Asn1Object)obj).Encode(this);
            }
            else if (obj is Asn1Encodable)
            {
                ((Asn1Encodable)obj).ToAsn1Object().Encode(this);
            }
            else
            {
                throw new IOException("object not Asn1Encodable");
            }
        }
    }
}

#endif
