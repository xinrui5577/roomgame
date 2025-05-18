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
using System.IO;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto
{
	public class BufferedIesCipher
		: BufferedCipherBase
	{
		private readonly IesEngine engine;
		private bool forEncryption;
		private MemoryStream buffer = new MemoryStream();

		public BufferedIesCipher(
			IesEngine engine)
		{
			if (engine == null)
				throw new ArgumentNullException("engine");

			this.engine = engine;
		}

		public override string AlgorithmName
		{
			// TODO Create IESEngine.AlgorithmName
			get { return "IES"; }
		}

		public override void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			this.forEncryption = forEncryption;

			// TODO
			throw Platform.CreateNotImplementedException("IES");
		}

		public override int GetBlockSize()
		{
			return 0;
		}

		public override int GetOutputSize(
			int inputLen)
		{
			if (engine == null)
				throw new InvalidOperationException("cipher not initialised");

			int baseLen = inputLen + (int) buffer.Length;
			return forEncryption
				?	baseLen + 20
				:	baseLen - 20;
		}

		public override int GetUpdateOutputSize(
			int inputLen)
		{
			return 0;
		}

		public override byte[] ProcessByte(
			byte input)
		{
			buffer.WriteByte(input);
			return null;
		}

		public override byte[] ProcessBytes(
			byte[]	input,
			int		inOff,
			int		length)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (inOff < 0)
				throw new ArgumentException("inOff");
			if (length < 0)
				throw new ArgumentException("length");
			if (inOff + length > input.Length)
				throw new ArgumentException("invalid offset/length specified for input array");

			buffer.Write(input, inOff, length);
			return null;
		}

		public override byte[] DoFinal()
		{
			byte[] buf = buffer.ToArray();

			Reset();

			return engine.ProcessBlock(buf, 0, buf.Length);
		}

		public override byte[] DoFinal(
			byte[]	input,
			int		inOff,
			int		length)
		{
			ProcessBytes(input, inOff, length);
			return DoFinal();
		}

		public override void Reset()
		{
			buffer.SetLength(0);
		}
	}
}

#endif
