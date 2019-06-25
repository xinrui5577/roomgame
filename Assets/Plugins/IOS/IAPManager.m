//
//  IAPManager.m
//  Unity-iPhone
//
//  Created by AimTi on 16/3/11.
//
//

#import "Payment.h"
#import "IAPManager.h"

@implementation IAPManager

static bool hasObserver = NO;

NSString* ReceiveGmaeObject;
NSString* ReceiveMethod;

-(void)InitPayment:(NSString *) gameObject second: (NSString *) method{
    NSLog(@"调用初始化IAP");
    ReceiveGmaeObject = gameObject;
    ReceiveMethod = method;
}

//调用支付
-(void)appPayment:(NSString *)id{
    if([SKPaymentQueue canMakePayments]){
        NSLog(@"调用成功，允许计费%@",id);
        
        if(!hasObserver){
            //设置支付监听对象
            [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
            tsArray = [NSMutableArray array];
            hasObserver = YES;
        }
        
        //调用计费
        [self buyRequest:id];
    }
    else{
        NSLog(@"不允许计费");
    }
}

//获取产品信息
-(void)getProductInfo:(NSString *)id{
    NSLog(@"获取产品信息");
    NSSet *set = [NSSet setWithArray:@[id]];
    SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers:set];
    request.delegate = self;
    [request start];
}
//产品信息回调
-(void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response{
    NSLog(@"产品信息回调");
    NSArray *myProduct = response.products;
    if(myProduct.count == 0){
        NSLog(@"无法获取产品信息，购买失败！");
        return;
    }
    SKPayment *payment =[SKPayment paymentWithProduct:myProduct[0]];
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}

//发送计费请求
-(void)buyRequest:(NSString *)id{
    SKPayment *payment = [SKPayment paymentWithProductIdentifier:id];
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}

//计费回调对象，用于验证服务器后关闭交易
NSMutableArray *tsArray = nil;

//接收计费回调
-(void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions{
    
    NSLog(@"-------调用回调---------");
//    //把交易信息添加到array里
//    [tsArray addObject:transactions];
    
    for (int i = 0;i < transactions.count;i ++) {//SKPaymentTransaction *transaction in transactions) {
        
        SKPaymentTransaction *transaction = [transactions objectAtIndex:i];
        //把交易信息添加到array里
        [tsArray addObject:transaction];
        //关闭交易的array索引  数据标识,id_transactionsID
        const char* closePoint =[[NSString stringWithFormat:@"%@,%lu_%d",@"IapFailed",tsArray.count - 1,i] UTF8String];
//        const char* closePoint =[[NSString stringWithFormat:@"%lu_%d",tsArray.count - 1,i] UTF8String];
//
        //nsstring的收据
        NSString *receipt = [[NSString alloc] initWithData:transaction.transactionReceipt encoding:NSUTF8StringEncoding];
        //成功传入unity的消息格式为 数据标识,id_transactionsID_receipt
        const char* message = [[NSString stringWithFormat:@"%@,%lu_%d_%@",@"IapSuccess",tsArray.count - 1,i,receipt] UTF8String];
//        const char* message = [[NSString stringWithFormat:@"%lu_%d_%@",tsArray.count - 1,i,receipt] UTF8String];
        
        switch (transaction.transactionState) {
                //交易完成
            case SKPaymentTransactionStatePurchased:
                
                //UnitySendMessage("IOS", "PaymentSuccess", message);
                UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], message);
                NSLog(@"id = %s计费完成，发送信息到unity == %s", closePoint,message);
                [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                break;
                //交易失败
            case SKPaymentTransactionStateFailed:
            	
                if (transaction.error.code != SKErrorPaymentCancelled) {
                    NSLog(@"计费失败 %@",transaction.error.description);
                    NSLog(@"计费失败closePoint %s",closePoint);

                }
                //UnitySendMessage("IOS", "PaymentFailed", closePoint);
                UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], closePoint);
                [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                break;
                //已经购买过该商品
            case SKPaymentTransactionStateRestored:
                NSLog(@"已经购买过该商品");
                
                //UnitySendMessage("IOS", "PaymentSuccess", message);
                UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], message);
                NSLog(@"计费完成，发送信息到unity == %s", message);
                [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                break;
            case SKPaymentTransactionStatePurchasing:
                NSLog(@"商品添加进列表");
                
                break;
        }
        
    }
}

//关闭交易的次数 到达tsArray的count时清除监听
int _closeCount = 0;

//服务器验证后关闭交易
-(void)provingCloseTransaction:(int)index ArrayIndex:(int)arrayIndex{
//    _closeCount ++;
//    if (_closeCount >= tsArray.count) {
//        [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
//        _closeCount = 0;
//    }
    NSLog(@"关闭ID == %d",index);
//    NSArray *array = (NSArray *)[tsArray objectAtIndex:index];
//    [[SKPaymentQueue defaultQueue] finishTransaction:[array objectAtIndex:arrayIndex]];
    
}

//获取验证信息
-(NSString *)transactionInfo:(SKPaymentTransaction *)transaction{
    return [self encode:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
}

//改编码网上找得
- (NSString *)encode:(const uint8_t *)input length:(NSInteger)length {
    static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    
    NSMutableData *data = [NSMutableData dataWithLength:((length + 2) / 3) * 4];
    uint8_t *output = (uint8_t *)data.mutableBytes;
    
    for (NSInteger i = 0; i < length; i += 3) {
        NSInteger value = 0;
        for (NSInteger j = i; j < (i + 3); j++) {
            value <<= 8;
            
            if (j < length) {
                value |= (0xFF & input[j]);
            }
        }
        
        NSInteger index = (i / 3) * 4;
        output[index + 0] =                    table[(value >> 18) & 0x3F];
        output[index + 1] =                    table[(value >> 12) & 0x3F];
        output[index + 2] = (i + 1) < length ? table[(value >> 6)  & 0x3F] : '=';
        output[index + 3] = (i + 2) < length ? table[(value >> 0)  & 0x3F] : '=';
    }
    
    return [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];
}



@end
