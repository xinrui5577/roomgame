using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Adapters
{
    public class UguiButtonAdapter : YxBaseButtonAdapter
    {
        public Text Label;
        public Sprite[] Sprites;
        private Button _button;
        protected Button Btn
        {
            get { return _button == null ? _button = GetComponent<Button>() : _button; }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Ugui; }
        }

        protected override void InitSoundListen()
        {
            var trigger = GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<EventTrigger>();
            }
            if (!string.IsNullOrEmpty(SoundPlayer.ClickSName))
            {
                AddMouseListener(EventTriggerType.PointerClick, baseData =>
                {
                    var pointData = baseData as PointerEventData;
                    if (pointData != null)
                    {
                        if (pointData.clickCount == 1)
                        {
                            SoundPlayer.OnYxClick();
                        }
                    }
                },trigger);
            }
            if (!string.IsNullOrEmpty(SoundPlayer.DoubleClickSName))
            {
                AddMouseListener(EventTriggerType.PointerClick, baseData =>
                {
                    var pointData = baseData as PointerEventData;
                    if (pointData != null)
                    {
                        if (pointData.clickCount == 2)
                        {
                            SoundPlayer.OnYxDoubleClick();
                        }
                    }
                }, trigger);
            }
            if (!string.IsNullOrEmpty(SoundPlayer.PressSName))
            {
                AddMouseListener(EventTriggerType.PointerDown, baseData =>
                {
                    SoundPlayer.OnYxPress();
                }, trigger);
            }
            if (!string.IsNullOrEmpty(SoundPlayer.ClickSName))
            {
                AddMouseListener(EventTriggerType.PointerUp, baseData =>
                {
                    SoundPlayer.OnYxRelease();
                }, trigger);
            }
        }

        protected void AddMouseListener(EventTriggerType type, UnityAction<BaseEventData> callBack, EventTrigger trigger)
        {
            var triggerEvent = new EventTrigger.TriggerEvent();
            triggerEvent.AddListener(callBack);
            var enter = new EventTrigger.Entry
            {
                eventID = type,
                callback = triggerEvent
            };
            trigger.triggers.Add(enter);
        }

        public override bool SetSkinName(string skinName)
        { 
            if (Btn == null) return false;
            var target = Btn.targetGraphic as Image;
            if (target != null)
            {
                var normal = GetImage(skinName);
                target.sprite = normal;
                var spriteState = Btn.spriteState;
                var press = GetImage(string.Format(PressSuffix, skinName));
                var hover = GetImage(string.Format(HoverSuffix, skinName));
                var disable = GetImage(string.Format(DisableSuffix, skinName));
                spriteState.pressedSprite = press == null ? normal : press;
                spriteState.highlightedSprite = hover == null ? normal : hover;
                spriteState.disabledSprite = disable == null ? normal : disable;
                Btn.spriteState = spriteState;
            }
           
            return true;
        }

        public Sprite GetImage(string skinName)
        {
            foreach (var sp in Sprites)
            {
                if (sp.name != skinName) { continue; }
                return sp;
            }
            return null;
        }

        public override void SetLabel(string content)
        {
            if (Label == null) return;
            if (string.IsNullOrEmpty(content))
            {
                Label.gameObject.SetActive(false);
                return;
            }
            Label.text = content;
            Label.gameObject.SetActive(true);
        }

        public override bool IsEnabled
        {
            get { return Btn.interactable; }
            set { Btn.interactable = value; }
        }

    }
}
