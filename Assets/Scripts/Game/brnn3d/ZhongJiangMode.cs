using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class ZhongJiangMode : MonoBehaviour
    {
        public Transform[] zhongjiangEffs = new Transform[4];
 
        //中奖闪烁效果的显示
        public void ShowZhongJiangEffect(int areaID, bool[] isShow)
        {
            zhongjiangEffs[areaID].gameObject.SetActive(false);
            zhongjiangEffs[areaID].gameObject.SetActive(isShow[areaID + 1]);
        }
        //隐藏中阿静的闪烁效果
        public void HideZhongJiangEffect()
        {
            for (int i = 0; i < zhongjiangEffs.Length; i++)
            {
                zhongjiangEffs[i].gameObject.SetActive(false);
            }
        }
    }
}
