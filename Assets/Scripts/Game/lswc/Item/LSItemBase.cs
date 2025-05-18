using System.Globalization;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Tools;
using UnityEngine;

namespace Assets.Scripts.Game.lswc.Item
{
    /// <summary>
    /// 动物模型类与颜色区域的基类
    /// </summary>
    public class LSItemBase : MonoBehaviour
    {
        protected float angle;

        public int index;

        public virtual void InitItem(int itemIndex,float itemRadius)
        {
            index = itemIndex;
            angle = GetAngle(index);
            float radian = LSTools.GetRadian(angle);
            float x = itemRadius*Mathf.Cos(radian);
            float z = itemRadius*Mathf.Sin(radian);
            transform.localPosition=new Vector3(x,0,z);
            transform.localEulerAngles=new Vector3(0,-angle,0);
        }

        public virtual int GetItemsNumber()
        {
            return LSConstant.Num_ItemDefaultNumber;
        }

        public float GetAngle(int index)
        {
            float angle=360/GetItemsNumber()*index;
            return angle;
        }

        public override string ToString()
        {
            return index.ToString(CultureInfo.InvariantCulture);
        }
    }
}
