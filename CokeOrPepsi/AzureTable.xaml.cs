using CokeOrPepsi.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CokeOrPepsi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class AzureTable : ContentPage
    {
        public AzureTable()
        {
            InitializeComponent();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            loading.IsRunning = true;
            List<CokeOrPepsiModel> CokeOrPepsiInformation = await AzureManager.AzureManagerInstance.GetCokeOrPepsi();

            CokeOrPepsiList.ItemsSource = CokeOrPepsiInformation;

            loading.IsRunning = false;
        }
    }
}