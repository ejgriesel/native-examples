using HermesPrtgDemo.Dto;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace HermesPrtgDemo
{
    public partial class MainWindow : Window
    {
        // Use a single HttpClient instance for the life of the app
        private readonly HttpClient client;

        public MainWindow()
        {
            client = new HttpClient();
            InitializeComponent();
        }

        private async void BtnFetch_Click(object sender, RoutedEventArgs e)
        {
            // 1. Input Validation
            string instrCode = txtInstrCode.Text.Trim();
            if (string.IsNullOrEmpty(instrCode))
            {
                MessageBox.Show("Please enter an Instruction Code.");
                return;
            }

            // UI Reset
            lblStatus.Text = "Loading...";
            gridResults.Visibility = Visibility.Hidden;
            btnFetch.IsEnabled = false;

            try
            {
                // 2. Build the URL
                string url = $"http://localhost:12001/api/prtg_marketData.MarketData?InstrCode={instrCode}&Exchange=JSE";

                // 3. Make the Request
                string responseBody = await client.GetStringAsync(url);

                // 4. Deserialize JSON (Case insensitive makes it more robust)
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                MarketResponse result = JsonSerializer.Deserialize<MarketResponse>(responseBody, options);

                if (result != null && result.data != null)
                {
                    PopulateUI(result.data);
                }
                else
                {
                    lblStatus.Text = "No data found in response.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                lblStatus.Text = $"Connection Error: {httpEx.Message}";
            }
            catch (JsonException jsonEx)
            {
                lblStatus.Text = $"Data Format Error: {jsonEx.Message}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnFetch.IsEnabled = true;
            }
        }

        private void PopulateUI(MarketData data)
        {
            // Update TextBlocks
            lblExchange.Text = data.Exchange;
            lblPeriod.Text = data.PeriodName;

            // Format numbers as Currency (C2) or general numbers (N2)
            lblClosing.Text = data.ClosingPrice.ToString("N2");
            lblLast.Text = data.LastPrice.ToString("N2");

            // Handle nested objects (Bid)
            if (data.BestBid != null)
            {
                lblBidVal.Text = data.BestBid.Value.ToString("N2");
                lblBidVol.Text = $"({data.BestBid.Volume})";
            }

            // Handle nested objects (Offer)
            if (data.BestOffer != null)
            {
                lblOfferVal.Text = data.BestOffer.Value.ToString("N2");
                lblOfferVol.Text = $"({data.BestOffer.Volume})";
            }

            // Show the results grid and clear status
            gridResults.Visibility = Visibility.Visible;
            lblStatus.Text = "";
        }
    }
}