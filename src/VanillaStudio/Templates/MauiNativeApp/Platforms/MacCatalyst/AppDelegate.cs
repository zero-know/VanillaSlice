using Foundation;
using {{ProjectName}}.MauiNativeApp;
namespace {{ProjectName}}.HybridApp
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
