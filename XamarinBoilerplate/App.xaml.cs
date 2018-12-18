using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FreshMvvm;
using XamarinBoilerplate.Services;
using Plugin.DeviceInfo;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XamarinBoilerplate
{
    public partial class App : Application
    {


        #region Constant Fields
        #endregion

        #region Fields & Properties

        public static int ScreenHeight;
        public static bool IsiPhoneX;

        #endregion

        #region Constructors

        public App()
        {
            SetupIOC();

            InitializeComponent();

            AddSafeArea();

            MainPage = new Pages.MainPage();
        }

        #endregion

        #region Destructors
        #endregion

        #region Delegates
        #endregion

        #region Events
        #endregion

        #region Enums
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods

        protected override void OnStart()
        {
            // Handle when your app starts

#if !DEBUG
            // TODO: Creare app per iOS e Android in AppCenter e copiare la key
            AppCenter.Start("ios={Your App Secret};android={Your App Secret}", typeof(Crashes));
#endif
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #endregion

        #region Private Methods

        private void SetupIOC()
        {
            FreshIOC.Container.Register<ICacheServices, CacheService>().AsSingleton();
            FreshIOC.Container.Register<IRestServices, RestServices>().AsMultiInstance();
            FreshIOC.Container.Register<IDataServices, MemorySingletonDataServices>().AsSingleton();
        }

        private void AddSafeArea()
        {
            IsiPhoneX = Device.RuntimePlatform == Device.iOS &&
                              CrossDeviceInfo.Current.Idiom == Plugin.DeviceInfo.Abstractions.Idiom.Phone &&
                              ScreenHeight.Equals(812);
        }

        #endregion

        #region Structs
        #endregion

        #region Inner Classes
        #endregion

    }
}
