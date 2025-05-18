/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BestHTTP.Logger
{
    /// <summary>
    /// Available logging levels.
    /// </summary>
    public enum Loglevels
    {
        /// <summary>
        /// All message will be logged.
        /// </summary>
        All,

        /// <summary>
        /// Only Informations and above will be logged.
        /// </summary>
        Information,

        /// <summary>
        /// Only Warnings and above will be logged.
        /// </summary>
        Warning,

        /// <summary>
        /// Only Errors and above will be logged.
        /// </summary>
        Error,

        /// <summary>
        /// Only Exceptions will be logged.
        /// </summary>
        Exception,

        /// <summary>
        /// No logging will be occur.
        /// </summary>
        None
    }

    public interface ILogger
    {
        /// <summary>
        /// The minimum severity to log
        /// </summary>
        Loglevels Level { get; set; }
        string FormatVerbose { get; set; }
        string FormatInfo { get; set; }
        string FormatWarn { get; set; }
        string FormatErr { get; set; }
        string FormatEx { get; set; }

        void Verbose(string division, string verb);
        void Information(string division, string info);
        void Warning(string division, string warn);
        void Error(string division, string err);
        void Exception(string division, string msg, Exception ex);
    }
}
