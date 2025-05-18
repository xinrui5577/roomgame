//
//  TencentAuth.h
//  Unity-iPhone
//
//  Created by 游侠 on 18/1/4.
//
//

#import <Foundation/Foundation.h>

@interface TencentAuth : NSObject<TencentSessionDelegate>

void InitUnityTencent(const char* gameObject,const char* method);
void InitTencent(const char* qqAppId);
void StartTencentAuth();

@end
