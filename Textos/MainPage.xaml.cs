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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Ocr;
using System.Drawing;
using Windows.Graphics.Imaging;
using Color = Windows.UI.Color;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Popups;

namespace Textos
{
    public sealed partial class MainPage : Page
    {
        DataPackage dataPackage = new DataPackage();

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        Boolean withGuiones = true;


        #region Constructor

        public MainPage()
        {
            this.InitializeComponent();

            CustomTitleBar();

            // Cargo la configuracion del tema
            object value = localSettings.Values["tema"];

            if (value == null)
            {
                localSettings.Values["tema"] = false;
                setThemeLight();
            }
            else
            {
                if (value.Equals(true)) toggleTema.IsOn = true;
                else setThemeLight();
            }
        }

        #endregion


        #region Theme

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

        private void toggleTema_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleTema.IsOn) setThemeDark();
            else setThemeLight();
        }

        private void setThemeDark()
        {
            this.RequestedTheme = ElementTheme.Dark;

            CrearTitleBarDark();

            this.Background = CrearAcrilicoDark();

            localSettings.Values["tema"] = true;

            (Application.Current.Resources["FlyoutBackBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 45, 45, 45);
        }

        private void setThemeLight()
        {
            this.RequestedTheme = ElementTheme.Light;

            CrearTitleBarLight();

            this.Background = CrearAcrilicoLight();

            localSettings.Values["tema"] = false;

            (Application.Current.Resources["FlyoutBackBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 233, 233, 233);
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

        private void CrearTitleBarLight()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(180, 255, 255, 255);
            titleBar.ButtonPressedForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = Colors.DodgerBlue;

            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        private void CrearTitleBarDark()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(30, 255, 255, 255);
            titleBar.ButtonPressedForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = Colors.DodgerBlue;

            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        #endregion


        #region Tab 1

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


        private string QuitarSaltos(string texto)
        {
            string[] oraciones = texto.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None);

            string textoResultado = "";

            foreach (string oracion in oraciones)
            {
                if (oracion.Contains("-"))
                {
                    textoResultado += oracion;
                }
                else
                {
                    textoResultado += $"{oracion} ";
                }
            }

            return textoResultado;
        }


        private string QuitarGuiones(string texto)
        {
            return texto.Replace("-", "");
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxOriginal.SelectAll();
        }

        private void textBoxOriginal_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxFormateado.Text = QuitarJustificado(QuitarSaltos(textBoxOriginal.Text));

            if (withGuiones)
            {
                textBoxFormateado.Text = QuitarGuiones(textBoxFormateado.Text);
            }
        }

        private void textBoxFormateado_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxFormateado.SelectAll();
        }

        private void checkBoxGuiones_Checked(object sender, RoutedEventArgs e)
        {
            textBoxFormateado.Text = QuitarGuiones(textBoxFormateado.Text);
            withGuiones = true;
        }

        private void checkBoxGuiones_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxFormateado.Text = QuitarJustificado(QuitarSaltos(textBoxOriginal.Text));
            withGuiones = false;
        }

        #endregion


        #region Tab 2

        BitmapImage img = new BitmapImage();


        private void btnCopiarTxtImg_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.RequestedOperation = DataPackageOperation.Copy;

            dataPackage.SetText(textBoxFormateado1.Text);

            Clipboard.SetContent(dataPackage);
        }

        private void textBoxFormateado1_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxFormateado1.SelectAll();
        }

        private async void SetFileAndOcr(StorageFile file)
        {
            if (file != null)
            {
                try
                {
                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        img.SetSource(fileStream);
                        ImageExControl1.Source = img;

                        var decoder = await BitmapDecoder.CreateAsync(fileStream);

                        SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                        OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();

                        var ocrResult = await ocrEngine.RecognizeAsync(bitmap);

                        textBoxFormateado1.Text = ocrResult.Text;
                    }
                }
                catch (Exception exc)
                {
                    var messageDialog = new MessageDialog(exc.Message);
                    await messageDialog.ShowAsync();
                }
                
            }
        }

        private async void btnSeleccionarImg_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();

            SetFileAndOcr(file);
        }

        private async void btnPegarImg_Click(object sender, RoutedEventArgs e)
        {
            var dataPackageView = Clipboard.GetContent();

            try
            {
                var imageReceived = await dataPackageView.GetStorageItemsAsync();

                if (imageReceived != null)
                {
                    foreach (var stoyageItem in imageReceived)
                    {
                        var file = stoyageItem as StorageFile;

                        SetFileAndOcr(file);

                        return;
                    }
                }
            }
            catch (Exception)
            {
                var messageDialog = new MessageDialog("Formato de archivo no compatible.");
                await messageDialog.ShowAsync();
            }   
        }

        #endregion
    }
}
