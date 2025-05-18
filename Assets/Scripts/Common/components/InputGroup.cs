using System;
using System.Collections.Generic;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.components
{
    /// <summary>
    /// 输入框组合
    /// </summary>
    [Serializable]
    public class InputGroup
    {
        public InputGroupData[] InputGroupDatas;

        public void Imbark(Dictionary<string,object> dict)
        {
            if (dict == null) return;
            foreach (var data in InputGroupDatas)
            {
                dict[data.Key] = data.InputLabel.Value;
            }
        }

    }
    [Serializable]
    public class InputGroupData
    {
        public string Key;
        public YxBaseInputLabelAdapter InputLabel;
    }
}
