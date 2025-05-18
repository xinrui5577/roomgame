using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.DragonSlayers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 注意:
    /// 1.该脚本不能和fish放在同一个脚本,因为Fish如果要响应OnTriggerEnter事件的话需要带collider,但带collider的话就会比子弹击中
    /// 
    /// </remarks>
    public class FishEx_DragonSlayer : MonoBehaviour {
        //public Fish[] SlayFish;//杀死的鱼,
        public Player Owner;//所有者
        public DragonSlayer Creator;//创建者

        void OnTriggerEnter(Collider other)
        {
            Fish f = other.GetComponent<Fish>();
            if (f != null)
            {
                Creator.On_FishExDragonSlayerTouchFish(this,f, Owner);
            }
        }

        public void Clear()
        {
            Fish f = transform.parent.GetComponent<Fish>();
        
            f.Clear();
            //Destroy(transform.parent.gameObject);
        }

    }
}
