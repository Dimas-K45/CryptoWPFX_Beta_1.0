using CryptoWPFX.Model;
using CryptoWPFX.Model.API;
using SciChart.Charting.Model.DataSeries;
using SciChart.Core.Extensions;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CryptoWPFX.Model.API.CoinGeckoApi;

namespace CryptoWPFX
{

    public partial class MainWindow : Window
    {
        string TokenActiveID = "";
        private CoinGeckoApi coinGeckoAPI = new CoinGeckoApi();
        private List<CryptoCurrency> topCurrencies = new List<CryptoCurrency>();
        public CryptoCurrency CryptoCurrency { get; private set; }
        ApplicationContext db = new ApplicationContext();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = CryptoCurrency;
            Loaded += Window_Loaded;
        }
        static string InsertSeparator(string input, char separator)
        {
            if (input.Length <= 3)
                return input;

            string result = "";
            int count = 0;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                result = input[i] + result;
                count++;

                if (count == 3 && i > 0)
                {
                    result = separator + result;
                    count = 0;
                }
            }

            return result;
        }
        static string AbbreviateNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            long number;
            if (!long.TryParse(input, out number))
                throw new ArgumentException("Invalid input: not a valid number");

            double num = (double)number;

            if (number >= 1000000000000)
                return (num / 1000000000000).ToString("0.#") + " T";
            if (number >= 1000000000)
                return (num / 1000000000).ToString("0.#") + " B";
            if (number >= 1000000)
                return (num / 1000000).ToString("0.#") + " M";
            if (number >= 1000)
                return (num / 1000).ToString("0.#") + " K";

            return number.ToString();
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            FullScreenState();
        }

        private void ScreenClose_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        private void ScreenStateAndDragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                FullScreenState();
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ScrennHide_Click(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем список топ N криптовалют
                topCurrencies = await coinGeckoAPI.GetTopNCurrenciesAsync(500, 1);

                // Привязываем к комбо-боксам
                cmbFromCurrency.ItemsSource = topCurrencies;
                cmbFromCurrency.DisplayMemberPath = "Symbol";
                cmbToCurrency.ItemsSource = topCurrencies;
                cmbToCurrency.DisplayMemberPath = "Symbol";

                // Привязываем список к DataGrid
                DataGrid.ItemsSource = topCurrencies;
                db.Database.EnsureCreated();
                foreach (var currency in topCurrencies)
                {
                    if (!db.CryptoCoin.Any(c => c.Id == currency.Id))
                    {
                        db.CryptoCoin.Add(currency);
                    }
                }
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                // Обработка возможных ошибок, например, вывод в консоль
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Функция для полного экрана
        public void FullScreenState()
        {
            if (WindowState == WindowState.Maximized)
            {
                BorderScreen.CornerRadius = new CornerRadius(30);
                WindowState = WindowState.Normal;
            }
            else
            {
                BorderScreen.CornerRadius = new CornerRadius(0);
                WindowState = WindowState.Maximized;
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border borderNextDataGrid)
            {
                borderNextDataGrid.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
                borderNextDataGrid.VerticalAlignment = VerticalAlignment.Center;
                borderNextDataGrid.HorizontalAlignment = HorizontalAlignment.Center;
                borderNextDataGrid.Width = 60;
                borderNextDataGrid.Height = 45;
                if (borderNextDataGrid.Child is System.Windows.Controls.Label labelBorderNext)
                {
                    labelBorderNext.Content = "Открыть";
                    labelBorderNext.FontSize = 12;
                    labelBorderNext.VerticalAlignment = VerticalAlignment.Center;
                    labelBorderNext.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border borderNextDataGrid)
            {
                borderNextDataGrid.BorderBrush = Brushes.Transparent;
                if (borderNextDataGrid.Child is System.Windows.Controls.Label labelBorderNext)
                {
                    labelBorderNext.Content = "▶";
                    labelBorderNext.FontSize = 15;
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                scroll.LineUp();
            }
            else
            {
                scroll.LineDown();
            }
            e.Handled = true;
        }

        private void borderClickDataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid.Visibility = Visibility.Visible;
            DataGridMainPoolCrypto.Visibility = Visibility.Collapsed;
            ConverterCoin.Visibility = Visibility.Collapsed;
            borderHeaderDataGrid.Visibility = Visibility.Visible;
            borderCoinInput.Visibility = Visibility.Visible;
            borderClickDataGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E00AC"));
            borderClickDataGridMainPoolCrypto.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
            borderConverterCoin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
        }

        private void borderClickDataGridMainPoolCrypto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid.Visibility = Visibility.Collapsed;
            DataGridMainPoolCrypto.Visibility = Visibility.Visible;
            ConverterCoin.Visibility = Visibility.Collapsed;
            borderHeaderDataGrid.Visibility = Visibility.Visible;
            borderCoinInput.Visibility = Visibility.Visible;
            borderClickDataGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
            borderClickDataGridMainPoolCrypto.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E00AC"));
            borderConverterCoin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
        }

        private void borderConverterCoin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid.Visibility = Visibility.Collapsed;
            DataGridMainPoolCrypto.Visibility = Visibility.Collapsed;
            ConverterCoin.Visibility = Visibility.Visible;
            borderHeaderDataGrid.Visibility = Visibility.Collapsed;
            borderCoinInput.Visibility = Visibility.Collapsed;
            borderClickDataGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
            borderClickDataGridMainPoolCrypto.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
            borderConverterCoin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E00AC"));
        }

        private void coinInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = coinInput.Text.ToLower();
            List<CryptoCurrency> filteredList = topCurrencies
        .Where(coin =>
            coin.Symbol.ToLower().Contains(searchText) ||
            coin.Name.ToLower().Contains(searchText) ||
            coin.Name.ToLower().StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
        .ToList();

            DataGrid.ItemsSource = filteredList;

        }

        private void Click_OpenToken(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            OpenToken(border.GetValue(AutomationProperties.AutomationIdProperty).ToString());

        }

        private async void OpenToken(string TokenID)
        {
            TokenActiveID = TokenID;
            MainView.Visibility = Visibility.Collapsed;
            TokenView.Visibility = Visibility.Visible;
            var InfoToken = await CoinGeckoApi.GetInfoTokenToID(TokenID, "usd");

            foreach (System.Windows.Controls.Label lbl in TimeSetChartPanel.Children)
            {
                if (lbl.Content.ToString() == "1 день")
                {
                    lbl.Foreground = Brushes.White;
                }
                else
                {
                    lbl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));
                }
            }

            //построение графика
            var series = new XyDataSeries<DateTime, double>();
            series = await CoinGeckoApi.GetActualChartToken(TokenID, "usd", "1");
            mountainRenderSeries.DataSeries = series;
            ChartToken.AnimateZoomExtentsCommand.Execute(null);

            try
            {
                NameToken.Content = InfoToken[CoinGeckoApi.CoinField.Symbol.ToString().ToLower()].ToString().ToUpper();
                PrecentToken.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_24h.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";
                if (PrecentToken.Content.ToString()[0] == '-')
                {
                    PrecentToken.Foreground = Brushes.Red;
                }
                // Создаем новый объект BitmapImage
                BitmapImage bitmap = new BitmapImage();

                // Устанавливаем URI изображения
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(InfoToken[CoinGeckoApi.CoinField.Image.ToString().ToLower()].ToString());
                bitmap.EndInit();

                LogoToken.Source = bitmap;
                MaxPriceToken.Content = InsertSeparator(InfoToken[CoinGeckoApi.CoinField.High_24h.ToString().ToLower()].ToString(), ' ');
                MinPriceToken.Content = InsertSeparator(InfoToken[CoinGeckoApi.CoinField.Low_24h.ToString().ToLower()].ToString(), ' ');
                VolumeToken.Content = AbbreviateNumber(InfoToken[CoinGeckoApi.CoinField.Market_Cap.ToString().ToLower()].ToString());

                Percent_1h.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_1h_In_Currency.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";
                Percent_24h.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_24h_In_Currency.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";
                Percent_7d.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_7d_In_Currency.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";
                Percent_14d.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_14d_In_Currency.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";
                Percent_30d.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_30d_In_Currency.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";
                Percent_1year.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_1y_In_Currency.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";

                foreach (var label in GridPercentAll.Children)
                {
                    if (label is System.Windows.Controls.Label lbl)
                    {
                        if (lbl.Content.ToString()[0] == '-')
                        {
                            lbl.Foreground = Brushes.Red;
                        }
                    }
                }
                string[] prices = InfoToken[CoinGeckoApi.CoinField.Current_Price.ToString().ToLower()].ToString().Split('.');
                if (prices[0].Length < 4)
                {
                    PriceToken.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Current_Price.ToString().ToLower()].ToString().Replace(".", ",")), 6)}";
                }
                else if (prices[0].Length < 2)
                {
                    PriceToken.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Current_Price.ToString().ToLower()].ToString().Replace(".", ",")), 8)}";
                }
                else
                {
                    PriceToken.Content = InsertSeparator($"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Current_Price.ToString().ToLower()].ToString().Replace(".", ",")), 2)}", ' ');
                }
                PanelBurse.Children.Clear();
                List<TickerData> tickerDatas = await GetActualBurse(TokenID);
                foreach (TickerData tickerData in tickerDatas)
                {
                    if (tickerData.TradeURL == null)
                    {
                        continue;
                    }
                    // Создание элементов
                    Grid grid = new Grid();

                    // Определение колонок
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    // Создание и добавление Label 1
                    System.Windows.Controls.Label label1 = new System.Windows.Controls.Label();
                    label1.Content = tickerData.Name;
                    label1.Margin = new Thickness(5);
                    label1.Foreground = Brushes.White;
                    grid.Children.Add(label1);

                    // Создание и добавление Label 2
                    System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
                    label2.Content = $"{tickerData.Base}/{tickerData.Target}";
                    label2.Margin = new Thickness(5);
                    label2.Foreground = Brushes.White;
                    label2.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(label2, 1);
                    grid.Children.Add(label2);

                    // Создание и добавление Label 3
                    System.Windows.Controls.Label label3 = new System.Windows.Controls.Label();
                    label3.Content = tickerData.LastPrice;
                    label3.Margin = new Thickness(5);
                    label3.Foreground = Brushes.White;
                    label3.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(label3, 2);
                    grid.Children.Add(label3);

                    // Создание и добавление FontAwesome
                    var icon = new FontAwesome.WPF.FontAwesome();
                    icon.Icon = FontAwesome.WPF.FontAwesomeIcon.ArrowRight;
                    icon.FontSize = 15;
                    icon.VerticalAlignment = VerticalAlignment.Center;
                    icon.Foreground = new SolidColorBrush(Color.FromRgb(212, 212, 212));
                    icon.SetValue(AutomationProperties.AutomationIdProperty, tickerData.TradeURL);
                    icon.MouseDown += Icon_MouseDownGetBurse;
                    icon.MouseEnter += Icon_MouseEnter;
                    Grid.SetColumn(icon, 3);
                    grid.Children.Add(icon);

                    PanelBurse.Children.Add(grid);
                }
            }
            catch
            {
                MessageBorder.Visibility = Visibility.Visible;
                MessageText.Text = "Слишком много запросов, попробуйте позже...";
                TokenView.Visibility = Visibility.Collapsed;
                MainView.Visibility = Visibility.Visible;
            }


        }

        private void Icon_MouseEnter(object sender, MouseEventArgs e)
        {
            var icon = sender as FontAwesome.WPF.FontAwesome;
            // Создаем анимацию смещения
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0; // начальное положение
            animation.To = 10; // конечное положение
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(400)); // продолжительность анимации
            animation.AutoReverse = true; // автоматически вернуться обратно

            // Создаем объект TranslateTransform для анимации смещения
            TranslateTransform translateTransform = new TranslateTransform();

            // Применяем анимацию к свойству X TranslateTransform
            translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            // Применяем TranslateTransform к RenderTransform элемента
            icon.RenderTransform = translateTransform;
        }

        private void Icon_MouseDownGetBurse(object sender, MouseButtonEventArgs e)
        {
            var icon = sender as FontAwesome.WPF.FontAwesome;
            // Открываем ссылку в браузере
            System.Diagnostics.Process.Start(icon.GetValue(AutomationProperties.AutomationIdProperty).ToString());
        }

        private async void Click_ConvertTokenPrice(object sender, MouseButtonEventArgs e)
        {
            if (BorderConvertTokenPrice.Visibility == Visibility.Hidden)
            {
                BorderConvertTokenPrice.Visibility = Visibility.Visible;
                JsonElement ConvertCurrencyToken = await CoinGeckoApi.GetInfoTokenToIDFull(TokenActiveID);

                foreach (JsonProperty property in ConvertCurrencyToken.EnumerateObject())
                {
                    ConvertTokenPrice.Children.Add(new System.Windows.Controls.Label
                    {
                        Content = $"{property.Value.GetDouble()} {property.Name.ToUpper()}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = Brushes.White
                    });
                }
            }
            else
            {
                BorderConvertTokenPrice.Visibility = Visibility.Hidden;
            }

        }

        private async void SelectionChangedTimeChart(object sender, SelectionChangedEventArgs e)
        {
            // Получаем ComboBox, который вызвал событие
            ComboBox comboBox = sender as ComboBox;

            // Получаем выбранный элемент
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            // Проверяем, был ли выбран элемент
            if (selectedItem != null)
            {
                string content = selectedItem.Content.ToString();
                string[] strings = content.Split(' ');

                var series = new XyDataSeries<DateTime, double>();
                series = await CoinGeckoApi.GetActualChartToken(TokenActiveID, "usd", strings[0]);
                mountainRenderSeries.DataSeries = series;
            }
        }

        private async void Click_TimeSetChart(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
            foreach (System.Windows.Controls.Label lbl in TimeSetChartPanel.Children)
            {
                if (label.Content.ToString() == lbl.Content.ToString())
                {
                    lbl.Foreground = Brushes.White;
                }
                else
                {
                    lbl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));
                }
            }
            string[] strings = label.Content.ToString().Split(' ');
            var series = new XyDataSeries<DateTime, double>();
            series = await CoinGeckoApi.GetActualChartToken(TokenActiveID, "usd", strings[0]);
            if (mountainRenderSeries != null && mountainRenderSeries.DataSeries != null)
            {
                mountainRenderSeries.DataSeries.Clear();
            }
            mountainRenderSeries.DataSeries = series;
            ChartToken.AnimateZoomExtentsCommand.Execute(null);
        }

        private void ClickBackTheMainView(object sender, MouseButtonEventArgs e)
        {
            TokenView.Visibility = Visibility.Collapsed;
            MainView.Visibility = Visibility.Visible;
        }

        private void MessageClose(object sender, MouseButtonEventArgs e)
        {
            MessageBorder.Visibility = Visibility.Collapsed;
        }

        private async void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            decimal amount;
            if (!decimal.TryParse(txtAmount.Text, out amount))
            {
                MessageBox.Show("Invalid amount.");
                return;
            }

            if (cmbFromCurrency.SelectedItem == null || cmbToCurrency.SelectedItem == null)
            {
                MessageBox.Show("Please select currencies.");
                return;
            }
            CryptoCurrency fromCurrency = (CryptoCurrency)cmbFromCurrency.SelectedItem;
            CryptoCurrency toCurrency = (CryptoCurrency)cmbToCurrency.SelectedItem;

            CoinGeckoApi coinGeckoApi = new CoinGeckoApi();
            decimal? fromCurrencyPrice = await coinGeckoApi.GetCurrencyPriceByIdAsync(fromCurrency.Id, "usd"); // Получить цену в USD
            decimal? toCurrencyPrice = await coinGeckoApi.GetCurrencyPriceByIdAsync(toCurrency.Id, "usd"); // Получить цену в USD

            if (fromCurrencyPrice.HasValue && toCurrencyPrice.HasValue)
            {
                // Преобразуем в OACurrency
                decimal result = (amount / fromCurrencyPrice.Value) * toCurrencyPrice.Value;

                lblResult.Text = $"Result: {result} {toCurrency.Symbol}";
            }
            else
            {
                MessageBox.Show("Failed to get exchange rate.");
            }
        }
    }
}