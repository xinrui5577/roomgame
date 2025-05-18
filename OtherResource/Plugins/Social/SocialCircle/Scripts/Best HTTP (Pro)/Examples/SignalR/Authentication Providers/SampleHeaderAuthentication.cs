/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SIGNALR

namespace BestHTTP.SignalR.Authentication
{
    /// <summary>
    /// Custom http-header based authenticator.
    /// <example>
    /// <code>
    ///     // Server side implementation of the Header-based authenticator
    ///     // Use it by adding the app.Use(typeof(HeaderBasedAuthenticationMiddleware)); line to the Startup class' Configuration function.
    ///     private class HeaderBasedAuthenticationMiddleware : OwinMiddleware
    ///     {
    ///         public HeaderBasedAuthenticationMiddleware(OwinMiddleware next)
    ///             : base(next)
    ///         {
    ///         }
    /// 
    ///         public override Task Invoke(IOwinContext context)
    ///         {
    ///             string username = context.Request.Headers.Get("username");
    ///             string roles = context.Request.Headers.Get("roles");
    /// 
    ///             if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(roles))
    ///             {
    ///                 var identity = new System.Security.Principal.GenericIdentity(username);
    /// 
    ///                 var principal = new System.Security.Principal.GenericPrincipal(identity, SplitString(roles));
    ///                 
    ///                 context.Request.User = principal;
    ///             }
    /// 
    ///             return Next.Invoke(context);
    ///         }
    /// 
    ///         private static string[] SplitString(string original)
    ///         {
    ///             if (String.IsNullOrEmpty(original))
    ///                 return new string[0];
    /// 
    ///             var split = from piece in original.Split(',') let trimmed = piece.Trim() where !String.IsNullOrEmpty(trimmed) select trimmed;
    /// 
    ///             return split.ToArray();
    ///         }
    ///     }
    /// </code>
    /// </example>
    /// </summary>
    class HeaderAuthenticator : IAuthenticationProvider
    {
        public string User { get; private set; }
        public string Roles { get; private set; }

        /// <summary>
        /// No pre-auth step required for this type of authentication
        /// </summary>
        public bool IsPreAuthRequired { get { return false; } }

#pragma warning disable 0067
        /// <summary>
        /// Not used event as IsPreAuthRequired is false
        /// </summary>
        public event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;

        /// <summary>
        /// Not used event as IsPreAuthRequired is false
        /// </summary>
        public event OnAuthenticationFailedDelegate OnAuthenticationFailed;

#pragma warning restore 0067

        /// <summary>
        /// Constructor to initialise the authenticator with username and roles.
        /// </summary>
        public HeaderAuthenticator(string user, string roles)
        {
            this.User = user;
            this.Roles = roles;
        }

        /// <summary>
        /// Not used as IsPreAuthRequired is false
        /// </summary>
        public void StartAuthentication()
        { }

        /// <summary>
        /// Prepares the request by adding two headers to it
        /// </summary>
        public void PrepareRequest(BestHTTP.HTTPRequest request, RequestTypes type)
        {
            request.SetHeader("username", this.User);
            request.SetHeader("roles", this.Roles);
        }
    }
}

#endif
