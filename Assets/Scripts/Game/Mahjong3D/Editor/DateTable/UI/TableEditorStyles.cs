using UnityEngine;
using UnityEditor;
using Assets.Scripts.Game.Mahjong3D.Standard;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public class TableEditorStyles
    {     
        public static GUIStyle PreferenceSelection { get; private set; }

        public static GUIStyle MidLabelBold { get; private set; }

        public static GUIStyle MidLabel { get; private set; }

        /// <summary>
        /// 可重新排序的列表背景样式。
        /// </summary>        
        public static GUIStyle RL_Background { get; private set; }
        /// <summary>
        /// 可重新排序的列表元素样式。
        /// </summary>  
        public static GUIStyle RL_Element { get; private set; }
        /// <summary>
        /// 可重新排序的列表页脚样式。
        /// </summary>  
        public static GUIStyle RL_Footer { get; private set; }
        /// <summary>
        /// 可重新排序的列表页脚按钮样式。
        /// </summary>  
        public static GUIStyle RL_FooterButton { get; private set; }

        public static GUIStyle Toolbar { get; private set; }
   
        public static GUIStyle ToolbarButton { get; private set; }

        public static GUIStyle GroupBox { get; private set; }

        /// <summary>
        /// 得到自定义表头样式。
        /// </summary>       
        public static GUIStyle CustomSaveTableHeader { get; private set; }

        /// <summary>
        /// 得到自定义子菜单标题样式。
        /// </summary>       
        public static GUIStyle CustomSaveSubMenuHeader { get; private set; }

        /// <summary>
        /// 统一选择纹理颜色与统一选择颜色。
        /// </summary>
        public static Texture2D UnitySelectionTexture = new Texture2D(1, 1);

        public static Texture2D ToolbarPlus { get { return EditorGUIUtility.FindTexture("Toolbar Plus"); } }
   
        public static Texture2D ToolbarPlusMore { get { return EditorGUIUtility.FindTexture("Toolbar Plus More"); } }
     
        public static Texture2D ToolbarMinus { get { return EditorGUIUtility.FindTexture("Toolbar Minus"); } }
      

        static TableEditorStyles()
        {
            InitializeTextures();
            InitializeGUIStyles();
        }

        public static void InitializeTextures()
        {
            UnitySelectionTexture.SetPixel(0, 0, TableColorPreset.UnitySelection);
            UnitySelectionTexture.Apply();
        }

        public static void InitializeGUIStyles()
        {
            PreferenceSelection = new GUIStyle("PreferencesSection");
            PreferenceSelection.onNormal.background = UnitySelectionTexture;

            CustomSaveTableHeader = new GUIStyle("RL Header");
            CustomSaveTableHeader.alignment = TextAnchor.MiddleCenter;

            MidLabel = new GUIStyle("label");
            MidLabel.alignment = TextAnchor.MiddleCenter;

            MidLabelBold = new GUIStyle("label");
            MidLabelBold.alignment = TextAnchor.MiddleCenter;
            MidLabelBold.fontStyle = FontStyle.Bold;


            CustomSaveSubMenuHeader = new GUIStyle("MeTransitionHead");

            RL_Background = new GUIStyle("RL Background");
            RL_Background.stretchHeight = false;

            GroupBox = new GUIStyle("GroupBox");

            RL_Element = new GUIStyle("RL Element");
            RL_Footer = new GUIStyle("RL Footer");
            RL_FooterButton = new GUIStyle("RL FooterButton");
            Toolbar = new GUIStyle("Toolbar");

            ToolbarButton = new GUIStyle("toolbarbutton");
            ToolbarButton.alignment = TextAnchor.UpperCenter;
            ToolbarButton.fontSize = 11;
        }
    }
}
