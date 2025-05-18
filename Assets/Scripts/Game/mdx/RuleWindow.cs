using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mdx
{
    public class RuleWindow : MonoBehaviour
    {
        public string Format;

        public UILabel FristLine;

        // Use this for initialization
        protected void Start ()
        {
            var gdata = App.GetGameData<MdxGameData>();
            FristLine.text = string.Format(Format, YxUtiles.ReduceNumber(gdata.MinApplyBanker), YxUtiles.ReduceNumber(gdata.MaxApplyBanker));
        }
    }
}
