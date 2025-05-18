/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
namespace Org.BouncyCastle.Asn1
{
    /**
     * class for breaking up an Oid into it's component tokens, ala
     * java.util.StringTokenizer. We need this class as some of the
     * lightweight Java environment don't support classes like
     * StringTokenizer.
     */
    public class OidTokenizer
    {
        private string  oid;
        private int     index;

		public OidTokenizer(
            string oid)
        {
            this.oid = oid;
        }

		public bool HasMoreTokens
        {
			get { return index != -1; }
        }

		public string NextToken()
        {
            if (index == -1)
            {
                return null;
            }

            int end = oid.IndexOf('.', index);
            if (end == -1)
            {
                string lastToken = oid.Substring(index);
                index = -1;
                return lastToken;
            }

            string nextToken = oid.Substring(index, end - index);
			index = end + 1;
            return nextToken;
        }
    }
}

#endif
