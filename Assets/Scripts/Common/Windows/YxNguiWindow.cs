using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows
{
    public class YxNguiWindow : YxWindow
    {
        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }
    }
}
