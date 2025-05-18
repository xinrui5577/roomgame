using Assets.Scripts.Game.lswc.Data;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lswc.Item
{
    public class LSAnimalItemControl:LSItemControlBase
    {
        private LSRotate _rotate;

        private int _nowPosition=0;
        private void Start()
        {
            _rotate = GetComponent<LSRotate>();
            if (_rotate==null)
            {
                _rotate = gameObject.AddComponent<LSRotate>();
            }
        }       


        public void ResetLayout()
        {
            transform.localEulerAngles = Vector3.zero;
            InitOther();
        }

        protected override void InitOther()
        {
            base.InitOther();
            for (var i = 0; i < Items.Count; i++)
            {
                var item = (LSAnimalItem) Items[i];
                var type = App.GetGameData<LswcGameData>().Animals[i];
                if(!item.IsRightAnimal(type))
                {
                    ChangeItem(i,GetLastIndex(type));
                }
                Items[i].name = type.ToString();
                Items[i].transform.SetSiblingIndex(i);
            }
        }

        private int GetLastIndex(LSAnimalType type)
        {
            var index = Items.FindLastIndex(delegate(LSItemBase obj)
                {
                    var item = (LSAnimalItem) obj;
                    return item.IsRightAnimal(type);
                });
            if(index<0||index>=Items.Count)
            {
                index = -1;
                YxDebug.LogError("Animal is not exist");
            }
            return index;
        }

        public float GetTotalAngle(float angle)
        {
            angle += 360 * LSConstant.Num_AnimalRotateNumber;
            return angle;
        }

        public override void SetRadius()
        {
            _raidus = LSConstant.Num_AnimalRadius;
        }

        public void Rotate(float angle,float time)
        {
            angle = GetTotalAngle(angle);
            //Debug.LogError("动物的旋转角度是"+angle);
            _rotate.StartRotate(time,LSConstant.RotationTime,-angle,0);
        }

        public void QuickRotate(float angle)
        {
            transform.localEulerAngles=new Vector3(0,-angle,0);
        }

        public LSAnimalItem GetAnimalItem(int animalChangIndex)
        {
            return (LSAnimalItem)Items[animalChangIndex];
        }

        public void PlayAnimation(int times)
        {
            
            foreach (var lsItemBase in Items)
            {
                var item = (LSAnimalItem) lsItemBase;
                StartCoroutine(item.PlayAnimalAnimation(times));
            }
        }
    }
}
