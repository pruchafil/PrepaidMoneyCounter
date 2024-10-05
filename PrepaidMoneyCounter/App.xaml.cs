using Autofac;
using PrepaidMoneyCounter.Repository;
using PrepaidMoneyCounter.ViewModel;
using System;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PrepaidMoneyCounter
{
    sealed partial class App : Application
    {
        public static Autofac.IContainer Container { get; set; }

        public App()
        {
            this.InitializeComponent();
            Container = ConfigureServices();
            this.Suspending += OnSuspending;
        }


        private Autofac.IContainer ConfigureServices()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<RecordRepository>()
                           .As<IRecordRepository>()
                           .SingleInstance();

            containerBuilder.RegisterType<MainViewModel>().AsSelf();

            Autofac.IContainer container = containerBuilder.Build();
            return container;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Neopakovat inicializaci aplikace, pokud už má okno obsah,
            // jenom ověřit, jestli je toto okno aktivní
            if (rootFrame == null)
            {
                // Vytvořit objekt Frame, který bude fungovat jako kontext navigace, a spustit procházení první stránky
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Načíst stav z dříve pozastavené aplikace
                }

                // Umístit rámec do aktuálního objektu Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Pokud není navigační zásobník obnovený, přejít na první stránku
                    // a nakonfigurovat novou stránku předáním požadovaných informací ve formě
                    // parametru navigace
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Zkontrolovat, jestli je aktuální okno aktivní
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Uložit stav aplikace a zastavit jakoukoliv aktivitu na pozadí
            deferral.Complete();
        }
    }
}
