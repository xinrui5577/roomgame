/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_UNITY_FORM
using UnityEngine;

namespace BestHTTP.Forms
{
    /// <summary>
    /// For backward compatibility.
    /// </summary>
    public sealed class UnityForm : HTTPFormBase
    {
        public WWWForm Form { get; set; }

        public UnityForm()
        {
        }

        public UnityForm(WWWForm form)
        {
            Form = form;
        }

        public override void CopyFrom(HTTPFormBase fields)
        {
            this.Fields = fields.Fields;
            this.IsChanged = true;

            if (Form == null)
            {
                Form = new WWWForm();

                if (Fields != null)
                    for (int i = 0; i < Fields.Count; ++i)
                    {
                        var field = Fields[i];

                        if (string.IsNullOrEmpty(field.Text) && field.Binary != null)
                            Form.AddBinaryData(field.Name, field.Binary, field.FileName, field.MimeType);
                        else
                            Form.AddField(field.Name, field.Text, field.Encoding);
                    }
            }
        }

        public override void PrepareRequest(HTTPRequest request)
        {
            if (Form.headers.ContainsKey("Content-Type"))
                request.SetHeader("Content-Type", Form.headers["Content-Type"] as string);
            else
                request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
        }

        public override byte[] GetData()
        {
            return Form.data;
        }
    }
}
#endif
