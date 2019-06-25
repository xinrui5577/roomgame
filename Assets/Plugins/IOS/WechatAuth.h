//
//  WechatAuth.h
//  Unity-iPhone
//
//  Created by 游侠 on 16/11/7.
//
//

#import <Foundation/Foundation.h>
#import "WechatAuthSDK.h"
#import "WXApi.h"
#import "WXApiObject.h"

@interface WechatAuth : NSObject<WXApiDelegate>

+(instancetype)sharedManager;
void InitUnityInfo(const char* gameObject,const char* method);
void InitWechat(const char* appId);
void LoginWechat();
bool IsInstalledWechat();
bool IsChcekWechatApiLevel();
void WXPayment(const char* appid,const char* partnerid,const char* prepayid,const char* packageV,const char* nonce,const char* time,const char* sign);
void ShareContent(const char* json);
void ShareText(NSDictionary *info);
void ShareImage(NSDictionary *info);
void ShareMusic(NSDictionary *info);
void ShareVideo(NSDictionary *info);
void ShareWebsite(NSDictionary *info);

@end

