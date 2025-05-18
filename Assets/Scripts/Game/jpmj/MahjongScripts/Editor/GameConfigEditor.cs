using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEditor;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameConfig), true)]
    public class GameConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("想更改属性 自己组个jsong个数 具体name去看代码", MessageType.Info);
        }
    }
}
