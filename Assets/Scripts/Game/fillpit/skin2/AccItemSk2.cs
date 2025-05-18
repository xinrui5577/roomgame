using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class AccItemSk2 : AccItem
    {

        
        public UILabel PlayerId;

        public UISprite BgSprite;

        public string SelfSpriteName = "ID_402";

        public override void InitAccItem(ISFSObject data)
        {
            base.InitAccItem(data);
            if (PlayerId != null)
            {
                PlayerId.text = data.GetInt("id").ToString();
            }
        }

        protected override void IsMyself()
        {
            base.IsMyself();
            BgSprite.spriteName = SelfSpriteName;
        }

    }
}
