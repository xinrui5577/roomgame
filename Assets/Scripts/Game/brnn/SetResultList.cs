using YxFramwork.Framework;

namespace Assets.Scripts.Game.brnn
{
    public class SetResultList : YxView
    {
        public string WinSpriteName = "ying";
        public string LoseSpriteName = "shu";
        public UISprite[] Sprites;

        protected override void OnFreshView()
        {
            var r = GetData<bool[]>();
            if (r == null) { return;}
            var len = r.Length;
            for (var i = 0; i < len; i++)
            {
                Sprites[i].spriteName = r[i] ? WinSpriteName : LoseSpriteName;
            }
        }
    }
}
