/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Q42.WinRT.SampleApp"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Q42.WinRT.SampleApp.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<ConvertersExampleViewModel>();
            SimpleIoc.Default.Register<DataExampleViewModel>();
            SimpleIoc.Default.Register<StorageExampleViewModel>();
            SimpleIoc.Default.Register<UtilExampleViewModel>();

        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ConvertersExampleViewModel ConvertersExample
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConvertersExampleViewModel>();
            }
        }

        public DataExampleViewModel DataExample
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DataExampleViewModel>();
            }
        }

        public StorageExampleViewModel StorageExample
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StorageExampleViewModel>();
            }
        }

        public UtilExampleViewModel UtilExample
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UtilExampleViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}