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

namespace Org.BouncyCastle.Crypto
{
	/// <remarks>Base interface for a public/private key block cipher.</remarks>
	public interface IAsymmetricBlockCipher
    {
		/// <summary>The name of the algorithm this cipher implements.</summary>
        string AlgorithmName { get; }

		/// <summary>Initialise the cipher.</summary>
		/// <param name="forEncryption">Initialise for encryption if true, for decryption if false.</param>
		/// <param name="parameters">The key or other data required by the cipher.</param>
        void Init(bool forEncryption, ICipherParameters parameters);

		/// <returns>The maximum size, in bytes, an input block may be.</returns>
        int GetInputBlockSize();

		/// <returns>The maximum size, in bytes, an output block will be.</returns>
		int GetOutputBlockSize();

		/// <summary>Process a block.</summary>
		/// <param name="inBuf">The input buffer.</param>
		/// <param name="inOff">The offset into <paramref>inBuf</paramref> that the input block begins.</param>
		/// <param name="inLen">The length of the input block.</param>
		/// <exception cref="InvalidCipherTextException">Input decrypts improperly.</exception>
		/// <exception cref="DataLengthException">Input is too large for the cipher.</exception>
        byte[] ProcessBlock(byte[] inBuf, int inOff, int inLen);
    }
}

#endif
