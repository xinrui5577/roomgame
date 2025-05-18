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

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ParametersWithIV
		: ICipherParameters
    {
		private readonly ICipherParameters	parameters;
		private readonly byte[]				iv;

		public ParametersWithIV(
            ICipherParameters	parameters,
            byte[]				iv)
			: this(parameters, iv, 0, iv.Length)
		{
		}

		public ParametersWithIV(
            ICipherParameters	parameters,
            byte[]				iv,
            int					ivOff,
            int					ivLen)
        {
            // NOTE: 'parameters' may be null to imply key re-use
			if (iv == null)
				throw new ArgumentNullException("iv");

			this.parameters = parameters;
			this.iv = new byte[ivLen];
            Array.Copy(iv, ivOff, this.iv, 0, ivLen);
        }

		public byte[] GetIV()
        {
			return (byte[]) iv.Clone();
        }

		public ICipherParameters Parameters
        {
            get { return parameters; }
        }
    }
}

#endif
