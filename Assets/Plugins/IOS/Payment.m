//
//  Payment.m
//  Unity-iPhone
//
//  Created by AimTi on 16/3/10.
//
//

#import "Payment.h"
#import "IAPManager.h"

@implementation Payment

static IAPManager *iapM = nil;

//unity调用计费传入计费ID为钱数
void appPayment(const char* id){
    
    if (iapM == nil) {
        iapM = [[IAPManager alloc] init];
    }
    
    NSString *str = [NSString stringWithCString:id encoding:NSUTF8StringEncoding];
    
    [iapM appPayment:str];
}

//关闭交易
void closeTransaction(int index,int indexArray){
    //int intIndex = [[NSString stringWithUTF8String:index] intValue];
    NSLog(@"调用关闭交易");
    [iapM provingCloseTransaction:index ArrayIndex:indexArray];
}

//初始化payment
void InitPayment(const char* gameObject,const char* method){
    NSLog(@"进入 == InitPayment");
    NSLog(@"%s",gameObject);
    NSLog(@"%s",method);
    
    if (iapM == nil) {
        iapM = [[IAPManager alloc] init];
    }
    
    NSString* ReceiveGmaeObject = [[NSString alloc] initWithUTF8String:gameObject];
    NSString* ReceiveMethod = [[NSString alloc] initWithUTF8String:method];
    
    [iapM InitPayment:ReceiveGmaeObject second:ReceiveMethod];
}

@end
