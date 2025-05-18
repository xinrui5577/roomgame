using System;
using System.Collections;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.toubao
{

    /// <summary>
    /// 庄家相关处理 2016年4月18日16:29:13 创建人：王乔
    /// </summary>
    public class BankerCtrl : MonoBehaviour
    {
        public UILabel Time;

        IEnumerator Timer()
        {
            while (true)
            {
                Time.text = Time.text = DateTime.Now.ToString("MM.dd hh:mm:ss");
                yield return new WaitForSeconds(1f);  
            }
        }

        /// <summary>
        /// 单例对象
        /// </summary>
        public static BankerCtrl Instance;

        public static BankerCtrl GetInstance()
        {
            if (Instance == null)
            {
                Instance = new BankerCtrl();
            }

            return Instance;
        }

        public void Awake()
        {
            Instance = this;
            StartCoroutine(Timer());
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(int min)
        {
            MinApplyBanker = min;
        }
        /// <summary>
        /// 上庄限制金额
        /// </summary>
        [HideInInspector]
        public int MinApplyBanker;
        
    }

    /// <summary>
    /// 庄家状态
    /// </summary>
    public enum BankerType
    {
        StayedBanker, //正在坐庄
        CanBecomeBanker, //可以申请上庄
        CanNotBeBanker, //无法申请上庄
    }
}