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
using BestHTTP;
using BestHTTP.Forms;
using BestHTTP.Extensions;

namespace BestHTTP.Forms
{
    /// <summary>
    /// A HTTP Form implementation to send textual and binary values.
    /// </summary>
    public sealed class HTTPMultiPartForm : HTTPFormBase
    {
        #region Private Fields

        /// <summary>
        /// A random boundary generated in the constructor.
        /// </summary>
        private string Boundary;

        /// <summary>
        /// 
        /// </summary>
        private byte[] CachedData;

        #endregion

        public HTTPMultiPartForm()
        {
            this.Boundary = this.GetHashCode().ToString("X");
        }

        #region IHTTPForm Implementation

        public override void PrepareRequest(HTTPRequest request)
        {
            // Set up Content-Type header for the request
            request.SetHeader("Content-Type", "multipart/form-data; boundary=\"" + Boundary + "\"");
        }

        public override byte[] GetData()
        {
            if (CachedData != null)
                return CachedData;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                for (int i = 0; i < Fields.Count; ++i)
                {
                    HTTPFieldData field = Fields[i];

                    // Set the boundary
                    ms.WriteLine("--" + Boundary);

                    // Set up Content-Disposition header to our form with the name
                    ms.WriteLine("Content-Disposition: form-data; name=\"" + field.Name + "\"" + (!string.IsNullOrEmpty(field.FileName) ? "; filename=\"" + field.FileName + "\"" : string.Empty));

                    // Set up Content-Type head for the form.
                    if (!string.IsNullOrEmpty(field.MimeType))
                        ms.WriteLine("Content-Type: " + field.MimeType);

                    ms.WriteLine("Content-Length: " + field.Payload.Length.ToString());
                    ms.WriteLine();

                    // Write the actual data to the MemoryStream
                    ms.Write(field.Payload, 0, field.Payload.Length);

                    ms.Write(HTTPRequest.EOL, 0, HTTPRequest.EOL.Length);
                }

                // Write out the trailing boundary
                ms.WriteLine("--" + Boundary + "--");

                IsChanged = false;

                // Set the RawData of our request
                return CachedData = ms.ToArray();
            }
        }

        #endregion
    };
}
