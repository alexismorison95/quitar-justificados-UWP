using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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

        public MainPage()
        {
            this.InitializeComponent();   
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
    }
}
