//
//  TencentAuth.m
//  Unity-iPhone
//
//  Created by 游侠 on 18/1/4.
//
//

#import <Foundation/Foundation.h>
#import <TencentOpenAPI/TencentOAuth.h>
#import "TencentAuth.h"


@implementation TencentAuth

//QQ授权登录主类
static TencentOAuth* TcApi = nil;
// 单例
static TencentAuth* tenAuth = nil;

static const char* ReceiveGmaeObject;
static const char* ReceiveMethod;

//初始化unity回调信息
void InitUnityTencent(const char* gameObject,const char* method){
//    NSLog(@"进入 == InitUnityTencent");
//    NSLog(@"%s",gameObject);
//    NSLog(@"%s",method);
//    ReceiveGmaeObject = gameObject;
//    ReceiveMethod = method;
//    tenAuth = [TencentAuth alloc];
}

//初始化腾讯API
void InitTencent(const char* qqAppId){
//    NSString* strQqAppId = [[NSString alloc] initWithUTF8String:qqAppId];
//    TcApi = [[TencentOAuth alloc] initWithAppId:strQqAppId andDelegate:tenAuth];
}

//开始授权
void StartTencentAuth(){
//    NSArray* permissions = [NSArray arrayWithObjects:
//                            kOPEN_PERMISSION_GET_USER_INFO,
//                            kOPEN_PERMISSION_GET_SIMPLE_USER_INFO,
//                            kOPEN_PERMISSION_ADD_SHARE,
//                            nil];
//    [TcApi authorize:permissions inSafari:NO];
}

/**
 * 登录成功后的回调
 */
- (void)tencentDidLogin{
    NSLog(@"登录成功！");
    NSString* receive = @"qqIosAuthSuccess,";
    receive = [receive stringByAppendingString:TcApi.openId];
    receive = [receive stringByAppendingString:@","];
    receive = [receive stringByAppendingString:TcApi.accessToken];
    
    UnitySendMessage(ReceiveGmaeObject,ReceiveMethod, [receive UTF8String]);
}

/**
 * 登录失败后的回调
 * \param cancelled 代表用户是否主动退出登录
 */
- (void)tencentDidNotLogin:(BOOL)cancelled{
    NSLog(@"取消登录！");
    NSString* receive = @"qqAuthCancel,";
    UnitySendMessage(ReceiveGmaeObject, ReceiveMethod, [receive UTF8String]);
}

/**
 * 登录时网络有问题的回调
 */
- (void)tencentDidNotNetWork{
    NSLog(@"登录失败！");
    NSString* receive = @"qqAuthFailed,";
    UnitySendMessage(ReceiveGmaeObject, ReceiveMethod, [receive UTF8String]);
}

///**
// * unionID获得
// */
//- (void)didGetUnionID{
//
//}
//
///**
// * 退出登录的回调
// */
//- (void)tencentDidLogout{
//
//}
//
///**
// * 因用户未授予相应权限而需要执行增量授权。在用户调用某个api接口时，如果服务器返回操作未被授权，则触发该回调协议接口，由第三方决定是否跳转到增量授权页面，让用户重新授权。
// * \param tencentOAuth 登录授权对象。
// * \param permissions 需增量授权的权限列表。
// * \return 是否仍然回调返回原始的api请求结果。
// * \note 不实现该协议接口则默认为不开启增量授权流程。若需要增量授权请调用\ref TencentOAuth#incrAuthWithPermissions: \n注意：增量授权时用户可能会修改登录的帐号
// */
//- (BOOL)tencentNeedPerformIncrAuth:(TencentOAuth *)tencentOAuth withPermissions:(NSArray *)permissions{
//    return true;
//}
//
///**
// * [该逻辑未实现]因token失效而需要执行重新登录授权。在用户调用某个api接口时，如果服务器返回token失效，则触发该回调协议接口，由第三方决定是否跳转到登录授权页面，让用户重新授权。
// * \param tencentOAuth 登录授权对象。
// * \return 是否仍然回调返回原始的api请求结果。
// * \note 不实现该协议接口则默认为不开启重新登录授权流程。若需要重新登录授权请调用\ref TencentOAuth#reauthorizeWithPermissions: \n注意：重新登录授权时用户可能会修改登录的帐号
// */
//- (BOOL)tencentNeedPerformReAuth:(TencentOAuth *)tencentOAuth{
//
//}
//
///**
// * 用户通过增量授权流程重新授权登录，token及有效期限等信息已被更新。
// * \param tencentOAuth token及有效期限等信息更新后的授权实例对象
// * \note 第三方应用需更新已保存的token及有效期限等信息。
// */
//- (void)tencentDidUpdate:(TencentOAuth *)tencentOAuth{
//
//}
//
///**
// * 用户增量授权过程中因取消或网络问题导致授权失败
// * \param reason 授权失败原因，具体失败原因参见sdkdef.h文件中\ref UpdateFailType
// */
//- (void)tencentFailedUpdate:(UpdateFailType)reason{
//
//}
//
///**
// * 获取用户个人信息回调
// * \param response API返回结果，具体定义参见sdkdef.h文件中\ref APIResponse
// * \remarks 正确返回示例: \snippet example/getUserInfoResponse.exp success
// *          错误返回示例: \snippet example/getUserInfoResponse.exp fail
// */
//- (void)getUserInfoResponse:(APIResponse*) response{
//
//}
//
//
///**
// * 社交API统一回调接口
// * \param response API返回结果，具体定义参见sdkdef.h文件中\ref APIResponse
// * \param message 响应的消息，目前支持‘SendStory’,‘AppInvitation’，‘AppChallenge’，‘AppGiftRequest’
// */
//- (void)responseDidReceived:(APIResponse*)response forMessage:(NSString *)message{
//
//}
//
///**
// * post请求的上传进度
// * \param tencentOAuth 返回回调的tencentOAuth对象
// * \param bytesWritten 本次回调上传的数据字节数
// * \param totalBytesWritten 总共已经上传的字节数
// * \param totalBytesExpectedToWrite 总共需要上传的字节数
// * \param userData 用户自定义数据
// */
//- (void)tencentOAuth:(TencentOAuth *)tencentOAuth didSendBodyData:(NSInteger)bytesWritten totalBytesWritten:(NSInteger)totalBytesWritten totalBytesExpectedToWrite:(NSInteger)totalBytesExpectedToWrite userData:(id)userData{
//
//}


///**
// * 通知第三方界面需要被关闭
// * \param tencentOAuth 返回回调的tencentOAuth对象
// * \param viewController 需要关闭的viewController
// */
//- (void)tencentOAuth:(TencentOAuth *)tencentOAuth doCloseViewController:(UIViewController *)viewController{
//
//}
//
//- (BOOL) tencentWebViewShouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation{
//
//}
//- (NSUInteger) tencentWebViewSupportedInterfaceOrientationsWithWebkit{
//
//}
//- (BOOL) tencentWebViewShouldAutorotateWithWebkit{
//
//}

@end
