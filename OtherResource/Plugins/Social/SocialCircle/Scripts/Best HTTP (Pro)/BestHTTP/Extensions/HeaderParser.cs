/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System.Collections.Generic;

namespace BestHTTP.Extensions
{
    /// <summary>
    /// Will parse a comma-separeted header value
    /// </summary>
    public sealed class HeaderParser : KeyValuePairList
    {
        public HeaderParser(string headerStr)
        {
            base.Values = Parse(headerStr);
        }

        private List<HeaderValue> Parse(string headerStr)
        {
            List<HeaderValue> result = new List<HeaderValue>();

            int pos = 0;

            try
            {
                while (pos < headerStr.Length)
                {
                    HeaderValue current = new HeaderValue();

                    current.Parse(headerStr, ref pos);

                    result.Add(current);
                }
            }
            catch(System.Exception ex)
            {
                HTTPManager.Logger.Exception("HeaderParser - Parse", headerStr, ex);
            }

            return result;
        }
    }
}
