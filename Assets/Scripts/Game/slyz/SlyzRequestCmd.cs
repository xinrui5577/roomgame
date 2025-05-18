using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.slyz.Network.Protocol
{
	public class SlyzRequestCmd :RequestCmd
	{
        // Request 下的ID
        public const int JackpotChange = 0x00;          // 彩池金额变化
		public const int StartGame = 0x01;              // 开始游戏
        public const int GetMessage = 0x02;             // 中彩通知
	}
}
