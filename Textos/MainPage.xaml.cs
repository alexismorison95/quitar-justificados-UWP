using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
            }
        }

        private void quitarJustificado()
        {
            char[] separator = {' '};

            string[] palabras = textBoxOriginal.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            string oracion = "";

            foreach (string palabra in palabras)
            {
                oracion += $"{palabra} ";
            }

            textBoxFormateado.Text = oracion;
        }

        private void btnCopiar_Click(object sender, RoutedEventArgs e)
        {
            // copy 
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
            quitarJustificado();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxOriginal.SelectAll();
        }

        private void textBoxOriginal_TextChanged(object sender, TextChangedEventArgs e)
        {
            quitarJustificado();
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

            AcrylicBrush acr = new AcrylicBrush();
            acr.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            acr.TintColor = Color.FromArgb(1, 0, 0, 0);
            acr.TintOpacity = 0.6;
            acr.FallbackColor = Color.FromArgb(1, 0, 0, 0);
            acr.TintLuminosityOpacity = 0.9;

            this.Background = acr;

            localSettings.Values["tema"] = true;
        }

        private void setThemeLight()
        {
            this.RequestedTheme = ElementTheme.Light;

            AcrylicBrush acr = new AcrylicBrush();
            acr.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            acr.TintColor = Color.FromArgb(1, 255, 255, 255);
            acr.TintOpacity = 0;
            acr.FallbackColor = Color.FromArgb(1, 255, 255, 255);
            acr.TintLuminosityOpacity = 0.9;

            this.Background = acr;

            localSettings.Values["tema"] = false;
        }
    }
}
