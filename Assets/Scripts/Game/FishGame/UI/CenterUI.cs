namespace Assets.Scripts.Game.FishGame.UI
{
    public class CenterUI : UIView
    {
        public override void SetBound(float x, float y, float w, float h)
        {
            var pos = transform.localPosition;
            pos.x = x + w / 2;
            var yof = y + h/2;
            pos.y = yof;
            transform.localPosition = pos;
            base.SetBound(x, y - yof, w, h);
        }
    }
}
