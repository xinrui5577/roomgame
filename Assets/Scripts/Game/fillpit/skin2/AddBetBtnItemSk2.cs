using YxFramwork.Common;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class AddBetBtnItemSk2 : AddBetBtnItem
    {

        public string NoramlSpriteName = "ID_236";

        public string SpecialSpriteName = "ID_238";


        public override void OnAddBetBtnShow()
        {
            base.OnAddBetBtnShow();
            var array = App.GetGameData<FillpitGameData>().AnteRate;
            int lastVal = array[array.Count - 1];
            UISprite sprite = GetComponent<UISprite>();
            string spName = AddBetValue > lastVal ? SpecialSpriteName : NoramlSpriteName;
            sprite.spriteName = spName;
            var btn = GetComponent<UIButton>();
            if (btn == null) return;
            btn.normalSprite = spName;
        }
    }
}
