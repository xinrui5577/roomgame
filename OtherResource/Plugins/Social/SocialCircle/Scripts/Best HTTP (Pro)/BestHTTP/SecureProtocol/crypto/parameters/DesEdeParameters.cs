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
    public class DesEdeParameters
		: DesParameters
    {
        /*
        * DES-EDE Key length in bytes.
        */
		public const int DesEdeKeyLength = 24;

		private static byte[] FixKey(
			byte[]	key,
			int		keyOff,
			int		keyLen)
		{
			byte[] tmp = new byte[24];

			switch (keyLen)
			{
				case 16:
					Array.Copy(key, keyOff, tmp, 0, 16);
					Array.Copy(key, keyOff, tmp, 16, 8);
					break;
				case 24:
					Array.Copy(key, keyOff, tmp, 0, 24);
					break;
				default:
					throw new ArgumentException("Bad length for DESede key: " + keyLen, "keyLen");
			}

			if (IsWeakKey(tmp))
				throw new ArgumentException("attempt to create weak DESede key");

			return tmp;
		}

		public DesEdeParameters(
            byte[] key)
			: base(FixKey(key, 0, key.Length))
        {
        }

		public DesEdeParameters(
			byte[]	key,
			int		keyOff,
			int		keyLen)
			: base(FixKey(key, keyOff, keyLen))
		{
		}

		/**
         * return true if the passed in key is a DES-EDE weak key.
         *
         * @param key bytes making up the key
         * @param offset offset into the byte array the key starts at
         * @param length number of bytes making up the key
         */
        public static bool IsWeakKey(
            byte[]  key,
            int     offset,
            int     length)
        {
            for (int i = offset; i < length; i += DesKeyLength)
            {
                if (DesParameters.IsWeakKey(key, i))
                {
                    return true;
                }
            }

            return false;
        }

        /**
         * return true if the passed in key is a DES-EDE weak key.
         *
         * @param key bytes making up the key
         * @param offset offset into the byte array the key starts at
         */
        public static new bool IsWeakKey(
            byte[]	key,
            int		offset)
        {
            return IsWeakKey(key, offset, key.Length - offset);
        }

		public static new bool IsWeakKey(
			byte[] key)
		{
			return IsWeakKey(key, 0, key.Length);
		}
    }
}

#endif
