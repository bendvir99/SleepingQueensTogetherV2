using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Plugin.Maui.Biometric;

namespace SleepingQueensTogether
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbolsOutlined");
                    fonts.AddFont("AlfaSlabOne-Regular.ttf", "AlfaSlabOne-Regular");
                    fonts.AddFont("Anton-Regular.ttf", "Anton-Regular");
                    fonts.AddFont("ArchivoBlack-Regular.ttf", "ArchivoBlack-Regular");
                    fonts.AddFont("BBHHegarty-Regular.ttf", "BBHHegarty-Regular");
                    fonts.AddFont("BebasNeue-Regular.ttf", "BebasNeue-Regular");
                    fonts.AddFont("Bungee-Regular.ttf", "Bungee-Regular");
                    fonts.AddFont("ChangaOne-Italic.ttf", "ChangaOne-Italic");
                    fonts.AddFont("ChangaOne-Regular.ttf", "ChangaOne-Regular");
                    fonts.AddFont("LilitaOne-Regular.ttf", "LilitaOne-Regular");
                    fonts.AddFont("Sekuya-Regular.ttf", "Sekuya-Regular");
                });
            CrossFingerprint.SetCurrentActivityResolver(() => Platform.CurrentActivity);
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

    }

}
