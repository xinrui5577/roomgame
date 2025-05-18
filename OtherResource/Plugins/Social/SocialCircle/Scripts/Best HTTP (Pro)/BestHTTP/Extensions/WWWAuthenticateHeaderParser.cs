/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BestHTTP.Extensions
{
    /// <summary>
    /// Used for parsing WWW-Authenticate headers:
    /// Digest realm="my realm", nonce="4664b327a2963503ba58bbe13ad672c0", qop=auth, opaque="f7e38bdc1c66fce214f9019ffe43117c"
    /// </summary>
    public sealed class WWWAuthenticateHeaderParser : KeyValuePairList
    {
        public WWWAuthenticateHeaderParser(string headerValue)
        {
            Values = ParseQuotedHeader(headerValue);
        }

        private List<HeaderValue> ParseQuotedHeader(string str)
        {
            List<HeaderValue> result = new List<HeaderValue>();

            if (str != null)
            {

                int idx = 0;

                // Read Type (Basic|Digest)
                string type = str.Read(ref idx, (ch) => !char.IsWhiteSpace(ch) && !char.IsControl(ch)).TrimAndLower();
                result.Add(new HeaderValue(type));

                // process the rest of the text
                while (idx < str.Length)
                {
                    // Read key
                    string key = str.Read(ref idx, '=').TrimAndLower();
                    HeaderValue qp = new HeaderValue(key);

                    // Skip any white space
                    str.SkipWhiteSpace(ref idx);

                    qp.Value = str.ReadPossibleQuotedText(ref idx);

                    result.Add(qp);
                }
            }
            return result;
        }
    }
}
