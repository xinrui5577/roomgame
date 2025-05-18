using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public static partial class CSharpExtension
    {
        /// <summary>
        /// 字符串格式化
        /// </summary>   
        /// <code language= "csharp"><![CDATA[  
        /// "xxx{0}{1}{2}xxx".ExFormat(0,1,2);
        /// ]]></code>
        public static string ExFormat(this string text, params object[] args)
        {            
            return string.IsNullOrEmpty(text) ? "" : string.Format(text, args);
        }

        /// <summary>
        /// 将文本按行切分。
        /// </summary>
        /// <param name="text">要切分的文本</param>       
        public static string[] ExSplitToLines(this string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var offsetPosition = 0;
            var rowText = string.Empty;
            var texts = new List<string>();
            while ((rowText = ReadLine(text, ref offsetPosition)) != null)
            {
                texts.Add(rowText);
            }
            return texts.ToArray();
        }

        private static string ReadLine(string text, ref int offsetPosition)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var length = text.Length;
            var offset = offsetPosition;
            while (offset < length)
            {
                char ch = text[offset];
                switch (ch)
                {
                    case '\r':
                    case '\n':
                        var str = text.Substring(offsetPosition, offset - offsetPosition);
                        offsetPosition = offset + 1;
                        if (((ch == '\r') && (offsetPosition < length)) && (text[offsetPosition] == '\n'))
                        {
                            offsetPosition++;
                        }
                        return str;
                    default: offset++; break;
                }
            }
            if (offset > offsetPosition)
            {
                var str = text.Substring(offsetPosition, offset - offsetPosition);
                offsetPosition = offset;
                return str;
            }
            return null;
        }
    }
}