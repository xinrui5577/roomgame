using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine
{
    /// <summary>
    /// 没返回值，没参数的代理
    /// </summary>
    public delegate void DVoidNoParam();
    public delegate bool DBoolNoParam();
    public delegate bool DBoolTParam<T>(T value);

    public delegate void DVoidTransform(Transform transf);

    /// <summary>
    /// 没有返回值，int 参数的代理
    /// </summary>
    /// <param name="value"></param>
    public delegate void DVoidInt(int value);
    public delegate int DIntInt(int value);
    public delegate void DVoidTArray<T>(T[] value);

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
    public delegate string DRStrString(string param);
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

    /// <summary>
    /// IDictionary 参数
    /// </summary>
    /// <param name="dic"></param>
    public delegate void DvoidIDictionary(IDictionary dic);

    /// <summary>
    /// eventData 参数
    /// </summary>
    /// <param name="data"></param>
    public delegate void DVoidEventData(EventData data);
}
