using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    /// <summary>
    /// 输入框适配器
    /// </summary>
    [RequireComponent(typeof(UIInput))]
    public class NguiInputAdapter : YxBaseInputLabelAdapter
    {
        private UIInput _input;

        protected UIInput Input
        {
            get { return _input == null ? _input = GetComponent<UIInput>() : _input; }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        public override void SetValue(string content)
        {
            var input = Input;
            if (input == null){return;}
            input.value = content;
        }

        public override int GetCharacterLimit()
        {
            return Input == null ? 0 : Input.characterLimit;
        }

        public override void Submit()
        {
            var input = Input;
            if (input == null) { return; }
            input.Submit();
            input.RemoveFocus();
        }

        public override void RemoveFocus()
        {
            var input = Input;
            if (input == null) { return; }
            input.RemoveFocus();
        }

        public override string Value
        {
            get { return Input == null ? "" : Input.value; }
        }
    }
}
