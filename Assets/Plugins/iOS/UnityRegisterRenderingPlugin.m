#import "UnityAppController.h"
#import "IUnityInterface.h"

@interface CNamaSDK_UNITY_PLUGIN_Controller : UnityAppController
{
}
- (void)shouldAttachRenderDelegate;
@end
@implementation CNamaSDK_UNITY_PLUGIN_Controller
- (void)shouldAttachRenderDelegate {
    // unlike desktops where plugin dynamic library is automatically loaded and registered
    // we need to do that manually on iOS
    UnityRegisterRenderingPluginV5(&UnityPluginLoad, &UnityPluginUnload);
}
@end
IMPL_APP_CONTROLLER_SUBCLASS(CNamaSDK_UNITY_PLUGIN_Controller);
