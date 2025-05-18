using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.ChessCommon
{
    /// <summary>
    /// 没返回值，没参数的代理
    /// </summary>
	public delegate void DVoidNoParam();

    public delegate void DVoidTransform(Transform transf);

    /// <summary>
    /// 没有返回值，int 参数的代理
    /// </summary>
    /// <param name="value"></param>
    public delegate void DVoidInt(int value);

    /// <summary>
    /// 没有返回值，int 参数的代理
    /// </summary>
    /// <param name="value"></param>
    public delegate void DVoidFloat(float value);

    /// <summary>
    /// 没有返回值，bool 参数的代理
    /// </summary>
    /// <param name="value"></param>
    public delegate void DVoidBool(bool value);

    /// <summary>
    /// 无返回值，1个字符串
    /// </summary>
    /// <param name="param"></param>
    public delegate void DVoidString(string param);
    /// <summary>
    /// 无返回值，两个字符串
    /// </summary>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    public delegate void DStrString(string param1, string param2);

    /// <summary>
    /// sfsobject 参数
    /// </summary>
    /// <param name="sfsObject"></param>
    public delegate void DVoidSfsObject(ISFSObject sfsObject);
}
