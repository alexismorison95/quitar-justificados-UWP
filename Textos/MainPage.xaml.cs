using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace Textos
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DataPackage dataPackage = new DataPackage();

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();

            CustomTitleBar();

            // Cargo la configuracion del tema
            object value = localSettings.Values["tema"];

            if (value == null)
            {
                localSettings.Values["tema"] = false;
                toggleTema.IsOn = false;
            }
            else
            {
                if (value.Equals(true))
                {
                    toggleTema.IsOn = true;
                }
                else
                {
                    setThemeLight();
                }
            }
        }

        private void CustomTitleBar()
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            Window.Current.SetTitleBar(AppTitleBar);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Height = sender.Height;
        }

        /// <summary>
        /// Funcion que quita el justificado de un texto.
        /// </summary>
        private string QuitarJustificado(string texto)
        {
            char[] separator = {' '};

            string[] palabras = texto.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            string oracion = "";

            foreach (string palabra in palabras)
            {
                oracion += $"{palabra} ";
            }

            return oracion;
        }

        private void btnCopiar_Click(object sender, RoutedEventArgs e)
        {
            // Copiar el texto sin justificado 
            dataPackage.RequestedOperation = DataPackageOperation.Copy;

            dataPackage.SetText(textBoxFormateado.Text);

            Clipboard.SetContent(dataPackage);
        }

        private void btnLimpiarOriginal_Click(object sender, RoutedEventArgs e)
        {
            textBoxOriginal.Text = "";
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            textBoxFormateado.Text = QuitarJustificado(textBoxOriginal.Text);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxOriginal.SelectAll();
        }

        private void textBoxOriginal_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxFormateado.Text = QuitarJustificado(textBoxOriginal.Text);
        }

        private void textBoxFormateado_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxFormateado.SelectAll();
        }

        private void toggleTema_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleTema.IsOn)
            {
                setThemeDark();
            }
            else
            {  
                setThemeLight();
            }
        }

        private void setThemeDark()
        {
            this.RequestedTheme = ElementTheme.Dark;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(30, 255, 255, 255);
            titleBar.ButtonPressedForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = Colors.DodgerBlue;

            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            this.Background = CrearAcrilicoDark();

            localSettings.Values["tema"] = true;
        }

        private void setThemeLight()
        {
            this.RequestedTheme = ElementTheme.Light;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(180, 255, 255, 255);
            titleBar.ButtonPressedForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = Colors.DodgerBlue;

            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            this.Background = CrearAcrilicoLight();

            localSettings.Values["tema"] = false;
        }

        private AcrylicBrush CrearAcrilicoLight()
        {
            AcrylicBrush acr = new AcrylicBrush();
            acr.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            acr.TintColor = Colors.White;
            acr.TintOpacity = 0.2;
            acr.FallbackColor = Color.FromArgb(255, 233, 233, 233);
            acr.TintLuminosityOpacity = 0.8;

            return acr;
        }

        private AcrylicBrush CrearAcrilicoDark()
        {
            AcrylicBrush acr = new AcrylicBrush();
            acr.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            acr.TintColor = Colors.Black;
            acr.TintOpacity = 0.8;
            acr.FallbackColor = Color.FromArgb(255, 45, 45, 45);

            return acr;
        }
    }
}
