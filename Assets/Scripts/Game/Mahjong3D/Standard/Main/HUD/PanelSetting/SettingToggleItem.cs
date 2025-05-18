using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum MahjongUIToggleType
    {
        LanguageVoice = 1,
        MahjongTableColor,
        MahjongCardColor,
        Shake,
    }

    public class SettingToggleItem : MonoBehaviour
    {
        public bool Active;
        public int Index;
        public Image Checkmark;
        public MahjongUIToggleType Type;

        private PanelSetting mPanel;

        private void Awake()
        {
            mPanel = GetComponentInParent<PanelSetting>();
            mPanel.BtnSwitchAction += Switch;
            if (Type == MahjongUIToggleType.LanguageVoice && Index == 2)
            {
                gameObject.SetActive(GameCenter.DataCenter.Config.Localism);
            }
        }

        public void OnEnable()
        {
            int value = 0;
            switch (Type)
            {
                case MahjongUIToggleType.LanguageVoice:
                    value = MahjongUtility.LanguageVoice; break;
                case MahjongUIToggleType.MahjongTableColor:
                    value = MahjongUtility.MahjongTableColor; break;
                case MahjongUIToggleType.MahjongCardColor:
                    value = MahjongUtility.MahjongCardColor; break;
                case MahjongUIToggleType.Shake:
                    value = MahjongUtility.ShakeCtrl; break;
            }
            Switch(Type, value);
        }

        public void Switch(MahjongUIToggleType type, int value)
        {
            if (type != Type) return;
            Active = Index == value;
            Checkmark.ExCompSetActive(Active);
            if (Active)
            {
                switch (Type)
                {
                    case MahjongUIToggleType.MahjongTableColor:
                        {
                            MahjongUtility.MahjongTableColor = Index;
                            GameCenter.Scene.TableManager.SwitchTableSkin();
                        }
                        break;
                    case MahjongUIToggleType.MahjongCardColor:
                        {
                            MahjongUtility.MahjongCardColor = Index;
                            GameCenter.Assets.SwitchMahjongSkin();                            
                        }
                        break;
                    case MahjongUIToggleType.LanguageVoice:
                        {
                            MahjongUtility.LanguageVoice = Index;
                        }
                        break;
                    case MahjongUIToggleType.Shake:
                        {
                            MahjongUtility.ShakeCtrl = Index;
                        }
                        break;
                }
            }
        }

        public void OnChangeBtnClick()
        {
            if (!Active)
            {
                mPanel.SwitchAction(Type, Index);
            }
        }
    }
}