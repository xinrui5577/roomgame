using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 快捷工具管理
    /// </summary>
    public class ShortcutsComponent : BaseComponent, IContinueGameCycle, IGameEndCycle, IGameInfoICycle
    {
        private LinkedList<ShortcutsPart> mShortcutsPartLinked = new LinkedList<ShortcutsPart>();

        /// <summary>
        /// 查询麻将
        /// </summary>
        public MahjongQuery MahjongQuery { get; set; }

        /// <summary>
        /// 延迟
        /// </summary>
        public DelayTimer DelayTimer { get; set; }

        /// <summary>
        /// 托管
        /// </summary>
        public AiAgency AiAgency { get; set; }

        /// <summary>
        /// 游戏功能开关
        /// </summary>
        public SwitchCombination SwitchCombination { get; set; }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
            SwitchCombination = new SwitchCombination(System.Enum.GetValues(typeof(GameSwitchType)).Length, 0);
            MahjongQuery = RegisterParts<MahjongQuery>();
            DelayTimer = RegisterParts<DelayTimer>();
            AiAgency = RegisterParts<AiAgency>();   
        }

        public void OnGameInfoICycle()
        {
            LinkedListNode<ShortcutsPart> current = mShortcutsPartLinked.First;
            while (current != null)
            {
                current.Value.OnGameInfot();
                current = current.Next;
            }
        }

        public void OnGameEndCycle()
        {
            //关闭托管
            GameCenter.EventHandle.Dispatch((int)EventKeys.AiAgency, new AiAgencyArgs() { State = false });
            //关闭开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Close((int)GameSwitchType.PowerAiAgency);
        }

        public void OnContinueGameCycle()
        {
            SwitchCombination.CloseAll();
        }

        /// <summary>
        /// 游戏开关状态
        /// </summary>
        public bool CheckState(GameSwitchType type)
        {
            return SwitchCombination.IsOpen((int)type);
        }
        /// <summary>
        /// 打开某开关
        /// </summary>
        public void SwitchOpen(GameSwitchType type)
        {
            SwitchCombination.Open((int)type);
        }
        /// <summary>
        /// 关闭某开关
        /// </summary>
        public void SwitchClose(GameSwitchType type)
        {
            SwitchCombination.Close((int)type);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T RegisterParts<T>() where T : ShortcutsPart
        {
            LinkedListNode<ShortcutsPart> current = mShortcutsPartLinked.First;
            while (current != null)
            {
                if (current.Value.GetType() == typeof(T))
                {
                    return current as T;
                }
                current = current.Next;
            }
            var part = gameObject.AddComponent<T>();
            if (part != null)
            {
                mShortcutsPartLinked.AddLast(part);
                part.OnInitalization();
            }
            return part;
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <typeparam name="T"> 部件 </typeparam>    
        /// <returns></returns>
        public T Parts<T>() where T : ShortcutsPart
        {
            LinkedListNode<ShortcutsPart> current = mShortcutsPartLinked.First;
            while (current != null)
            {
                if (current.Value.GetType() == typeof(T))
                {
                    return current.Value as T;
                }
                current = current.Next;
            }
            return null;
        }
    }
}