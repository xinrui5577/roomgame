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

namespace BestHTTP.Forms
{
    /// <summary>
    /// This class represents a HTTP Form's field.
    /// </summary>
    public class HTTPFieldData
    {
        /// <summary>
        /// The form's field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Filename of the field. Optional.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Mime-type of the field. Optional
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Encoding of the data. Optional
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// The field's textual data.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The field's binary data.
        /// </summary>
        public byte[] Binary { get; set; }

        /// <summary>
        /// Will return with the binary data, or if it's not present the textual data will be decoded to binary.
        /// </summary>
        public byte[] Payload
        {
            get
            {
                if (Binary != null)
                    return Binary;

                if (Encoding == null)
                    Encoding = Encoding.UTF8;

                return Binary = Encoding.GetBytes(Text);
            }
        }
    }
}
