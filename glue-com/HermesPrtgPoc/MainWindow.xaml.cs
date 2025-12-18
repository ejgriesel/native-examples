using HermesPrtgPoc.Dto;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Tick42;
using Tick42.Windows;

namespace HermesPrtgPoc
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client;
        private Glue42 _glue;
        public IGlueWindow GlueWindow { get; set; }
        internal void RegisterGlue(Glue42 glue)
        {
            _glue = glue;
            //UpdateUI(true);

            glue.GlueWindows?.RegisterStartupWindow(this, Title).ContinueWith(t =>
            {
                if (t.IsCompleted)
                {
                    GlueWindow = t.Result;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public MainWindow()
        {
            _client = new HttpClient();
            // Pretend to be a browser
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            // Explicitly ask for JSON
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            InitializeComponent();

            DataContext = this;
            Visibility = Visibility.Hidden;
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
                string responseBody = await _client.GetStringAsync(url);

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