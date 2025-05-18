using System.Collections.Generic;

namespace Assets.Scripts.Common.Interface
{
    public interface ITrendReciveData
    {
        /// <summary>
        /// 设置结算位置
        /// </summary>
        /// <returns></returns>
        ITrendReciveData SetResultArea();
        /// <summary>
        /// 获得结算位置
        /// </summary>
        /// <returns></returns>
        List<string> GetResultArea();
        /// <summary>
        /// 获得结算牌类型
        /// </summary>
        /// <returns></returns>
        int GetResultType();
        /// <summary>
        /// 获得统计的次数数组
        /// </summary>
        /// <returns></returns>
//        List<int> GetTotalNum();
    }
}

