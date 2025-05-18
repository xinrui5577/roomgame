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

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class ContentInfo
        : Asn1Encodable
    {
        private readonly DerObjectIdentifier	contentType;
        private readonly Asn1Encodable			content;

        public static ContentInfo GetInstance(object obj)
        {
            if (obj == null)
                return null;
            ContentInfo existing = obj as ContentInfo;
            if (existing != null)
                return existing;
            return new ContentInfo(Asn1Sequence.GetInstance(obj));
        }

        private ContentInfo(
            Asn1Sequence seq)
        {
            contentType = (DerObjectIdentifier) seq[0];

            if (seq.Count > 1)
            {
                content = ((Asn1TaggedObject) seq[1]).GetObject();
            }
        }

        public ContentInfo(
            DerObjectIdentifier	contentType,
            Asn1Encodable		content)
        {
            this.contentType = contentType;
            this.content = content;
        }

        public DerObjectIdentifier ContentType
        {
            get { return contentType; }
        }

        public Asn1Encodable Content
        {
            get { return content; }
        }

        /**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * ContentInfo ::= Sequence {
         *          contentType ContentType,
         *          content
         *          [0] EXPLICIT ANY DEFINED BY contentType OPTIONAL }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(contentType);

            if (content != null)
            {
                v.Add(new BerTaggedObject(0, content));
            }

            return new BerSequence(v);
        }
    }
}

#endif
