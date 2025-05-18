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
    /// Base class for specialized parsers
    /// </summary>
    public class KeyValuePairList
    {
        public List<HeaderValue> Values { get; protected set; }

        public bool TryGet(string value, out HeaderValue @param)
        {
            @param = null;
            for (int i = 0; i < Values.Count; ++i)
                if (string.CompareOrdinal(Values[i].Key, value) == 0)
                {
                    @param = Values[i];
                    return true;
                }
            return false;
        }
    }
}
