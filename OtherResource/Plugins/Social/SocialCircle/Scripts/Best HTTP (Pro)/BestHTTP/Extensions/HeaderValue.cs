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
    /// Used in string parsers. Its Value is optional.
    /// </summary>
    public sealed class HeaderValue
    {
        #region Public Properties

        public string Key { get; set; }
        public string Value { get; set; }
        public List<HeaderValue> Options { get; set; }

        public bool HasValue { get { return !string.IsNullOrEmpty(this.Value); } }

        #endregion

        #region Contstructors

        public HeaderValue()
        { }

        public HeaderValue(string key)
        {
            this.Key = key;
        }

        #endregion

        #region Public Helper Functions

        public void Parse(string headerStr, ref int pos)
        {
            ParseImplementation(headerStr, ref pos, true);
        }

        public bool TryGetOption(string key, out HeaderValue option)
        {
            option = null;

            if (Options == null || Options.Count == 0)
                return false;

            for (int i = 0; i < Options.Count; ++i)
                if (String.Equals(Options[i].Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    option = Options[i];
                    return true;
                }

            return false;
        }

        #endregion

        #region Private Helper Functions

        private void ParseImplementation(string headerStr, ref int pos, bool isOptionIsAnOption)
        {
            string key = headerStr.Read(ref pos, (ch) => ch != ';' && ch != '=' && ch != ',', true);
            this.Key = key;

            char? skippedChar = headerStr.Peek(pos - 1);
            bool isValue = skippedChar == '=';
            bool isOption = isOptionIsAnOption && skippedChar == ';';

            while (skippedChar != null && isValue || isOption)
            {

                if (isValue)
                {
                    string value = headerStr.ReadPossibleQuotedText(ref pos);
                    this.Value = value;
                }
                else if (isOption)
                {
                    HeaderValue option = new HeaderValue();
                    option.ParseImplementation(headerStr, ref pos, false);

                    if (this.Options == null)
                        this.Options = new List<HeaderValue>();

                    this.Options.Add(option);
                }

                if (!isOptionIsAnOption)
                    return;

                skippedChar = headerStr.Peek(pos - 1);
                isValue = skippedChar == '=';
                isOption = isOptionIsAnOption && skippedChar == ';';
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Value))
                return String.Concat(Key, '=', Value);
            else
                return Key;
        }

        #endregion
    }
}
