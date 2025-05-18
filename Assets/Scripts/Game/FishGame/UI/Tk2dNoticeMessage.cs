using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Game.FishGame.UI
{
    public class Tk2dNoticeMessage : YxNoticeMessage
    {
        private tk2dSlicedSprite _panel;

        public override void OnAwake()
        {
            _panel = GetComponent<tk2dSlicedSprite>();
        }

        public override Vector4 GetShowBounds()
        {
            var bounds = new Vector4();
            var dims = _panel.dimensions;
            var pos = transform.localPosition;
            var temp = dims.x / 2;
            var tempC = pos.x;
            bounds.x -= temp;
            bounds.z = tempC + temp;
            /****************************/
            temp = dims.y / 2;
            tempC = pos.y;
            bounds.y = tempC + temp;
            bounds.w = tempC - temp;
            return bounds; 
        }
    }
}
