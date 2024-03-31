using CryptoWPFX.Model;
using CryptoWPFX.Model.API;
using SciChart.Charting.Model.DataSeries;
using System.Linq;
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

        JsonElement ConvertCurrencyToken;
        public MainWindow()
        {
            InitializeComponent();
        }
        static string InsertSeparator(string input, char separator)
        {
            if (input.Length <= 3)
                return input;

            decimal number = decimal.Parse(input);
            string result = number.ToString("N");

            //string result = "";
            //int count = 0;

            //for (int i = input.Length - 1; i >= 0; i--)
            //{
            //    result = input[i] + result;
            //    count++;

            //    if (count == 3 && i > 0)
            //    {
            //        result = separator + result;
            //        count = 0;
            //    }

            //}

            return result;
        }

        // метод для аббривеатуры цифр (1M, 1B, 1T)
        static string AbbreviateNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            long number;
            if (!long.TryParse(input, out number))
                throw new ArgumentException("Invalid input: not a valid number");

            double num = (double)number;

            if (number >= 1000000000000)
                return (num / 1000000000000).ToString("0.###") + " T";
            if (number >= 1000000000)
                return (num / 1000000000).ToString("0.###") + " B";
            if (number >= 1000000)
                return (num / 1000000).ToString("0.###") + " M";
            if (number >= 1000)
                return (num / 1000).ToString("0.###") + " K";

            return number.ToString();
        }

        // обработчик кнопки увеличения и уменьшения окна
        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            FullScreenState();
        }

        // обработчик закрытия окна
        private void ScreenClose_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // обработчик для перетаскивания окна мышью
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

        // загрузка списка криптовалют на главной странице
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем список топ N криптовалют
                topCurrencies = await coinGeckoAPI.GetTopNCurrenciesAsync(500, 1);

                // Привязываем список к DataGrid
                DataGrid.ItemsSource = topCurrencies;
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

        // обработчик анимации при наведении на кнопку открытия токена (криптовалюты в таблице)
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border borderNextDataGrid)
            {
                borderNextDataGrid.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
                borderNextDataGrid.VerticalAlignment = VerticalAlignment.Center;
                borderNextDataGrid.HorizontalAlignment = HorizontalAlignment.Center;
                borderNextDataGrid.Width = 70;
                borderNextDataGrid.Height = 45;
                if (borderNextDataGrid.Child is System.Windows.Controls.Label labelBorderNext)
                {
                    labelBorderNext.Content = "Открыть";
                    labelBorderNext.FontSize = 15;
                    labelBorderNext.VerticalAlignment = VerticalAlignment.Center;
                    labelBorderNext.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
        }

        // обработчик анимации при отведении с кнопки открытия токена (криптовалюты в таблице)
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border borderNextDataGrid)
            {
                borderNextDataGrid.BorderBrush = Brushes.Transparent;
                if (borderNextDataGrid.Child is System.Windows.Controls.Label labelBorderNext)
                {
                    labelBorderNext.Content = "▶";
                    labelBorderNext.FontSize = 25;
                }
            }
        }


        // прокрутка таблица на главной
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
            borderClickDataGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
            borderClickDataGridMainPoolCrypto.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7163ba"));
            borderConverterCoin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E00AC"));
        }

        // поиск криптовалют
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

        // обработчик нажатия на кнопку для открытия токена в таблице
        private void Click_OpenToken(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            OpenToken(border.GetValue(AutomationProperties.AutomationIdProperty).ToString());

        }

        // метод открытия информации о токене
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

            //try
            //{
                NameToken.Content = InfoToken[CoinGeckoApi.CoinField.Symbol.ToString().ToLower()].ToString().ToUpper();
                PrecentToken.Content = $"{Math.Round(Convert.ToDouble(InfoToken[CoinGeckoApi.CoinField.Price_Change_Percentage_24h.ToString().ToLower()].ToString().Replace(".", ",")), 2)}%";
                if (PrecentToken.Content.ToString()[0] == '-')
                {
                    PrecentToken.Foreground = Brushes.Red;
                }

                //построение графика
                var series = new XyDataSeries<DateTime, double>();
                series = await CoinGeckoApi.GetActualChartToken(TokenID, "usd", "1");
                mountainRenderSeries.DataSeries = series;
                ChartToken.AnimateZoomExtentsCommand.Execute(null);

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
                    PriceToken.Content = InsertSeparator(InfoToken[CoinGeckoApi.CoinField.Current_Price.ToString().ToLower()].ToString().Replace(".", ","), ' ');
                }

                void TopBurseSecurity(ref System.Windows.Controls.Label lab, string name)
                {
                    lab.FontWeight = FontWeights.Bold;
                    if (name.ToLower().Contains("bybit"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f7a600"));
                    else if (name.ToLower().Contains("binance"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f6cd2f"));
                    else if (name.ToLower().Contains("okx"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff700"));
                    else if (name.ToLower().Contains("mexc"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1972e2"));
                    else if (name.ToLower().Contains("bingx"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#435af6"));
                    else if (name.ToLower().Contains("kucoin"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#23af91"));
                    else if (name.ToLower().Contains("gate.io"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17e6a1"));
                    else if (name.ToLower().Contains("huobi"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008fdd"));
                    else if (name.ToLower().Contains("bitget"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1ea1b4"));
                    else if (name.ToLower().Contains("coinbase"))
                        lab.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0052ff"));
                    
                    else
                    {
                        lab.Foreground = Brushes.White;
                        lab.FontWeight = FontWeights.Normal;
                    }
                       
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
                    TopBurseSecurity(ref label1, tickerData.Name);
                    label1.Margin = new Thickness(5);
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
            //}
            //catch
            //{
            //    MessageBorder.Visibility = Visibility.Visible;
            //    MessageText.Text = "Слишком много запросов, попробуйте позже...";
            //    TokenView.Visibility = Visibility.Collapsed;
            //    MainView.Visibility = Visibility.Visible;
            //}

            
        }

        // анимация при наведении на кнопку для перехода на биржу
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

        // кнопка для перехода на биржу
        private void Icon_MouseDownGetBurse(object sender, MouseButtonEventArgs e)
        {
            var icon = sender as FontAwesome.WPF.FontAwesome;
            // Открываем ссылку в браузере
            System.Diagnostics.Process.Start(icon.GetValue(AutomationProperties.AutomationIdProperty).ToString());
        }

        // кнопка для просмотра курса токена в других валютах
        private async void Click_ConvertTokenPrice(object sender, MouseButtonEventArgs e)
        {
            if (BorderConvertTokenPrice.Visibility == Visibility.Hidden)
            {
                BorderConvertTokenPrice.Visibility = Visibility.Visible;
                ConvertCurrencyToken = await CoinGeckoApi.GetInfoTokenToIDFull(TokenActiveID);
                ConvertTokenPrice.Children.Clear();
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

        // обработчик изменения времени графика
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

        // кнопка для возврата на главную страницу со страницы токена
        private void ClickBackTheMainView(object sender, MouseButtonEventArgs e)
        {
            TokenView.Visibility = Visibility.Collapsed;
            MainView.Visibility = Visibility.Visible;
        }

        // обработчик кнопки закрытия окна об ошибке
        private void MessageClose(object sender, MouseButtonEventArgs e)
        {
            MessageBorder.Visibility = Visibility.Collapsed;
        }

        // поиск курса токена в других валютах (на странице с токеном)
        private void SearchCryptoConvert(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;
            ConvertTokenPrice.Children.Clear();
            foreach (JsonProperty property in ConvertCurrencyToken.EnumerateObject())
            {
                if (property.Name.ToUpper().Contains(text.Text.ToUpper()))
                {
                    ConvertTokenPrice.Children.Add(new System.Windows.Controls.Label
                    {
                        Content = $"{property.Value.GetDouble()} {property.Name.ToUpper()}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = Brushes.White
                    });
                }
            }
            if (ConvertTokenPrice.Children.Count <= 0)
            {
                ConvertTokenPrice.Children.Add(new System.Windows.Controls.Label
                {
                    Content = "Не найдено",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.White
                });
            }
        }

        // сортировка главной таблицы с монетами
        private void ClickSortMainDataGrid(object sender, MouseButtonEventArgs e)
        {
            var SortText = sender as TextBlock;
            var top = new List<CryptoCurrency>();

            if (SortText.Text.Split(" ")[0] == "Цена")
            {
                if (SortText.Text[SortText.Text.Length - 1] == '●')
                {
                    top = topCurrencies.OrderByDescending(c => c.Price).ToList();
                    SortText.Text = "Цена ▽";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '▽')
                {
                    top = topCurrencies.OrderBy(c => c.Price).ToList();
                    SortText.Text = "Цена △";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '△')
                {
                    top = topCurrencies;
                    SortText.Text = "Цена ●";
                }
                foreach (TextBlock item in GridNameDataGridColumn.Children)
                {
                    if (item.Text.Split()[0] != "Цена" && item.Text.Split()[0] != "Монета" && item.Text.Split()[0] != "Переход")
                    {
                        item.Text = item.Text.Replace(item.Text.ToCharArray()[item.Text.Length - 1], '●');
                    }
                }
            }
            else if (SortText.Text.Split(" ")[0] == "Название")
            {
                if (SortText.Text[SortText.Text.Length - 1] == '●')
                {
                    top = topCurrencies.OrderByDescending(c => c.Name).ToList();
                    SortText.Text = "Название ▽";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '▽')
                {
                    top = topCurrencies.OrderBy(c => c.Name).ToList();
                    SortText.Text = "Название △";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '△')
                {
                    top = topCurrencies;
                    SortText.Text = "Название ●";
                }
                foreach (TextBlock item in GridNameDataGridColumn.Children)
                {
                    if (item.Text.Split()[0] != "Название" && item.Text.Split()[0] != "Монета" && item.Text.Split()[0] != "Переход")
                    {
                        item.Text = item.Text.Replace(item.Text.ToCharArray()[item.Text.Length - 1], '●');
                    }
                }
            }
            else if (SortText.Text.Split(" ")[0] == "Символы")
            {
                if (SortText.Text[SortText.Text.Length - 1] == '●')
                {
                    top = topCurrencies.OrderByDescending(c => c.Symbol).ToList();
                    SortText.Text = "Символы ▽";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '▽')
                {
                    top = topCurrencies.OrderBy(c => c.Symbol).ToList();
                    SortText.Text = "Символы △";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '△')
                {
                    top = topCurrencies;
                    SortText.Text = "Символы ●";
                }
                foreach (TextBlock item in GridNameDataGridColumn.Children)
                {
                    if (item.Text.Split()[0] != "Символы" && item.Text.Split()[0] != "Монета" && item.Text.Split()[0] != "Переход")
                    {
                        item.Text = item.Text.Replace(item.Text.ToCharArray()[item.Text.Length - 1], '●');
                    }
                }
            }
            else if (SortText.Text.Split(" ")[0] == "Объем")
            {
                if (SortText.Text[SortText.Text.Length - 1] == '●')
                {
                    top = topCurrencies.OrderByDescending(c => c.Volume).ToList();
                    SortText.Text = "Объем торгов 24ч ▽";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '▽')
                {
                    top = topCurrencies.OrderBy(c => c.Volume).ToList();
                    SortText.Text = "Объем торгов 24ч △";
                }
                else if (SortText.Text[SortText.Text.Length - 1] == '△')
                {
                    top = topCurrencies;
                    SortText.Text = "Объем торгов 24ч ●";
                }
                foreach (TextBlock item in GridNameDataGridColumn.Children)
                {
                    if (item.Text.Split()[0] != "Объем" && item.Text.Split()[0] != "Монета" && item.Text.Split()[0] != "Переход")
                    {
                        item.Text = item.Text.Replace(item.Text.ToCharArray()[item.Text.Length - 1], '●');
                    }
                }
            }


            DataGrid.ItemsSource = top;
        }
    }
}