using System.Windows;

namespace GARCA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF5cWWFCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXpedXVdRmReUEFyWkE=");
        }
    }
}
