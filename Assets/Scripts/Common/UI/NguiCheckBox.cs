using UnityEngine;

namespace Assets.Scripts.Common.UI
{
    public class NguiCheckBox : NguiCRComponent
    { 
        public UILabel Label;
        public UIToggle Toggle;
        public UISprite IconSprite;
        public UILabel NumLabel;

        protected override void OnAwake()
        {
            base.OnStart();
            InitStateTotal = 2;
        }

        protected override void OnFreshCRCView(ItemData itemData)
        {
            if (itemData == null) return;
            Label.text = itemData.Name;
            Toggle.value = itemData.State;
            Toggle.group = itemData.Group;
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

        public override bool IsValid()
        {
            return Toggle!=null&&Toggle.value;
        }

        public override Vector2 UpdateWidget(int width = 0, int height = 0)
        {
            var size = base.UpdateWidget(width, height);
            UpdateBox(size);
            return size;
        }

        private void UpdateBox(Vector2 size)
        {
            var box = Toggle.GetComponent<BoxCollider>();
            if (box == null) return;
            var bound = Bounds;
            box.size = size;
            var center = bound.extents;
            var local = transform.position;
            local = box.transform.InverseTransformPoint(local);
            center.x = local.x + size.x / 2;
            center.y = Mathf.Abs(local.y) - center.y;
            box.center = center;
        } 
    } 
}
