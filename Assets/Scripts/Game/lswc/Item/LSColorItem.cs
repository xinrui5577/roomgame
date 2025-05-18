using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Manager;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Item
{
    public class LSColorItem : LSItemBase
    {

        public  LSColorType CurColor;

        private MeshRenderer _curRender;
         
        protected MeshRenderer GetRender
        {
            get
            {
                if (_curRender == null)
                {
                    _curRender = transform.GetChild(0).GetComponent<MeshRenderer>();
                }
                return _curRender;
            }
        }

        public void SetColorType(LSColorType type)
        {
            CurColor = type;
            var curRender = GetRender;
            if (curRender == null) { return;}
            curRender.material =App.GetGameData<LswcGameData>().GetColorMaterial(CurColor);
        }

        public bool IsRightColor(LSColorType type)
        {
            return type == CurColor;
        }

        public override int GetItemsNumber()
        {
            return LSConstant.Num_ColorItemNumber;
        }

       
    }
}
