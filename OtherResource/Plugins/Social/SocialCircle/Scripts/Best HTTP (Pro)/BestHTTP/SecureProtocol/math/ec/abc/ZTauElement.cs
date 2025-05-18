/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

namespace Org.BouncyCastle.Math.EC.Abc
{
	/**
	* Class representing an element of <code><b>Z</b>[&#964;]</code>. Let
	* <code>&#955;</code> be an element of <code><b>Z</b>[&#964;]</code>. Then
	* <code>&#955;</code> is given as <code>&#955; = u + v&#964;</code>. The
	* components <code>u</code> and <code>v</code> may be used directly, there
	* are no accessor methods.
	* Immutable class.
	*/
	internal class ZTauElement 
	{
		/**
		* The &quot;real&quot; part of <code>&#955;</code>.
		*/
		public readonly BigInteger u;

		/**
		* The &quot;<code>&#964;</code>-adic&quot; part of <code>&#955;</code>.
		*/
		public readonly BigInteger v;

		/**
		* Constructor for an element <code>&#955;</code> of
		* <code><b>Z</b>[&#964;]</code>.
		* @param u The &quot;real&quot; part of <code>&#955;</code>.
		* @param v The &quot;<code>&#964;</code>-adic&quot; part of
		* <code>&#955;</code>.
		*/
		public ZTauElement(BigInteger u, BigInteger v)
		{
			this.u = u;
			this.v = v;
		}
	}
}

#endif
