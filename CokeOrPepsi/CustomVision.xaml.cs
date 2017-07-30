using CokeOrPepsi.DataModels;
using CokeOrPepsi.Model;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CokeOrPepsi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void TakePhotoButton_OnClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "fff246c1abe0404e810d6f786908021f");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/62b96747-b2c6-41cf-84e1-c220a50c8f03/image?iterationId=9fc09c22-3d83-462a-b778-bd85f042c306";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = responseModel.Predictions.Max(m => m.Probability);

                    string pepsi, pepsiProbability, coke, cokeProbability;
                    if (responseModel.Predictions[0].Tag.ToString() == "Pepsi")
                    {
                        pepsi = responseModel.Predictions[0].Tag.ToString();
                        pepsiProbability = responseModel.Predictions[0].Probability.ToString();
                        coke = responseModel.Predictions[1].Tag.ToString();
                        cokeProbability = responseModel.Predictions[1].Probability.ToString();
                    }
                    else
                    {
                        pepsi = responseModel.Predictions[1].Tag.ToString();
                        pepsiProbability = responseModel.Predictions[1].Probability.ToString();
                        coke = responseModel.Predictions[0].Tag.ToString();
                        cokeProbability = responseModel.Predictions[0].Probability.ToString();
                    }
                    ProductLabel.Text = "your product is";

                    string productName = "";
                    if (max >= 0.5)
                    {
                        if (Double.Parse(cokeProbability) > Double.Parse(pepsiProbability))
                        {
                            ResultLabel.Text = "Coca-Cola";
                            ResultLabel.TextColor = Color.FromHex("#fe001a");
                            productName = "Coke";
                        }
                        else
                        {
                            ResultLabel.Text = "Pepsi";
                            ResultLabel.TextColor = Color.FromHex("#0085ca");
                            productName = "Pepsi";
                        }
                    }
                    else
                    {
                        ResultLabel.Text = "Indeterminable";
                        ResultLabel.TextColor = Color.Black;
                        productName = "Indeterminable";
                    }

                    await postCokeOrPepsiAsync(productName);
                }
                
                file.Dispose();
            }
        }

        async Task postCokeOrPepsiAsync(string productName)
        {

            CokeOrPepsiModel model = new CokeOrPepsiModel()
            {
                DateNZ = DateTime.Now,
                Product = productName
            };

            await AzureManager.AzureManagerInstance.PostCokeOrPepsi(model);
        }
    }
}