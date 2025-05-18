using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows.MatchWindows
{
    /// <summary>
    /// ±»»¸œÍ«È
    /// </summary>
    public class YxMatchInfoWindow : YxWindow
    {
        protected override void OnFreshView()
        {
            base.OnFreshView();

        }


        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }

    }
}
