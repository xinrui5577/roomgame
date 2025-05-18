using Assets.Scripts.Common.Models.CreateRoomRules;
using UnityEngine;

namespace Assets.Scripts.Common.UI
{
    public class NguiCheckBox : NguiCRComponent
    { 
        public UILabel Label;
        public UIToggle Toggle;
        public UISprite IconSprite;
        public UILabel NumLabel;

//        protected override void OnStart()
//        {
//            base.OnStart();
            //            Toggle.onChange.Add(new EventDelegate(ChangeValue));
//        }
//
//        private void ChangeValue()
//        {
//            var n = GetData<ItemData>().Name;
//        }

        protected override void OnFreshCRCView(ItemData itemData)
        {
            Label.text = itemData.Name;
            Toggle.value = itemData.State;
            Toggle.group = itemData.Group;
            if (itemData.Size > 0)
            {
                SetLabel(Label,itemData.Size);
                SetLabel(NumLabel, itemData.Size, UILabel.Overflow.ResizeFreely);
            }
            if (itemData.UseNum > 0)
            {
                if (NumLabel != null)
                {
                    NumLabel.text = string.Format("x{0}", itemData.UseNum);
                    NumLabel.transform.localScale = Vector3.one;
                }
                if (IconSprite != null)
                {
                    IconSprite.gameObject.SetActive(true);
                    IconSprite.spriteName = itemData.UseItem;
                    IconSprite.transform.localScale = Vector3.one; 
                }
            }
            else
            {
                if (NumLabel != null)
                {
                    NumLabel.transform.localScale = new Vector3(0, 1, 1);
                }
                if (IconSprite != null)
                {
                    IconSprite.gameObject.SetActive(false);
                    IconSprite.transform.localScale = new Vector3(0,1,1);
                }
            }
            UpdateWidget(itemData.Width, itemData.Height); 
        }

        private void SetLabel(UILabel label,int size,UILabel.Overflow overflowMethod = UILabel.Overflow.ClampContent)
        {
            if (label == null) { return;}
            label.overflowMethod = overflowMethod;
            label.alignment = NGUIText.Alignment.Left;
            label.fontSize = size;
        }

        public override bool IsValid()
        {
            return Toggle!=null&&Toggle.value;
        }

        public override void UpdateBoxCollider()
        {
            var box = Toggle.GetComponent<BoxCollider>();
            if (box == null) return;
            var bound = Bounds;
            var size = bound.size;
            var center = bound.extents;
            var local = Toggle.transform.localPosition;
            center.x = -local.x + size.x / 2 ;
            center.y = -local.y  - size.y / 2;
            box.center = center;
            box.size = size;
        }
    } 
}
