using GARCA.Utils.IOC;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para partialTransactions.xaml
    /// </summary>
    public partial class PartialHome : Page
    {
        #region Variables

        private readonly MainWindow parentForm;

        #endregion

        #region Constructor

        public PartialHome(MainWindow parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
        }

        #endregion

        #region Events


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCharts();
        }

        #endregion

        #region Functions

        private async Task LoadChartForecast()
        {
            //Header

            Border border = new()
            {

                BorderThickness = new Thickness(0.5),

                BorderBrush = new SolidColorBrush(Colors.Black),

                Margin = new Thickness(10),

                CornerRadius = new CornerRadius(5)
            };

            TextBlock textBlock = new()
            {

                Text = "Prevision de cobros / pagos",

                Margin = new Thickness(5),

                FontSize = 14

            };

            textBlock.Effect = new DropShadowEffect
            {

                Color = Colors.Black,

                Opacity = 0.5

            };

            border.Child = textBlock;

            chForecast.Header = border;

            //Legend

            chForecast.Legend = new ChartLegend
            {
                IconHeight = 10,
                IconWidth = 10,
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                DockPosition = ChartDock.Right,
                IconVisibility = Visibility.Visible,
                CornerRadius = new CornerRadius(5),
                ItemMargin = new Thickness(5),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                CheckBoxVisibility = Visibility.Visible
            };

            //Axis

            DateTimeAxis primaryAxis = new();
            primaryAxis.Header = "Fecha";
            primaryAxis.PlotOffsetStart = 20;
            primaryAxis.PlotOffsetEnd = 20;
            primaryAxis.IntervalType = DateTimeIntervalType.Days;
            primaryAxis.Interval = 2;
            primaryAxis.LabelFormat = "dd/MM";
            chForecast.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new();
            secondaryAxis.Header = "Importe (€)";
            chForecast.SecondaryAxis = secondaryAxis;

            //ToolTip

            DataTemplate tooltip = new();

            FrameworkElementFactory stackpanel = new(typeof(StackPanel));
            stackpanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory textblock = new(typeof(TextBlock));
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Item.Account"));
            textblock.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock);

            FrameworkElementFactory textblock1 = new(typeof(TextBlock));
            textblock1.SetValue(TextBlock.TextProperty, " : ");
            textblock1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock1.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock1.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock1.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock1);

            FrameworkElementFactory textblock2 = new(typeof(TextBlock));
            textblock2.SetBinding(TextBlock.TextProperty,
                new Binding("Item.Amount")
                {
                    StringFormat = "C",
                    ConverterCulture = new System.Globalization.CultureInfo("es-ES")
                });

            textblock2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock2.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock2.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock2.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock2);
            tooltip.VisualTree = stackpanel;

            //Series

            chForecast.Series.Clear();

            foreach (var accounts in (await DependencyConfigView.AccountsServiceView.GetAllOpenedAync())?
                .Where(x => DependencyConfigView.AccountsTypesServiceView.AccountExpensives(x.AccountsTypesid)))
            {

                LineSeries series = new()
                {
                    ItemsSource = (await Task.Run(() => DependencyConfigView.ForecastsChartServiceView.GetMonthForecast())).Where(x => x.Accountid == accounts.Id).OrderByDescending(x => x.Date),
                    Label = accounts.Description,
                    XBindingPath = "Date",
                    YBindingPath = "Amount",
                    ShowTooltip = true,
                    TooltipTemplate = tooltip,
                    EnableAnimation = true,
                    AnimationDuration = new TimeSpan(0, 0, 3),
                    AdornmentsInfo = new ChartAdornmentInfo
                    {
                        ShowMarker = true,
                        SymbolStroke = new SolidColorBrush(Colors.Blue),
                        SymbolInterior = new SolidColorBrush(Colors.DarkBlue),
                        SymbolHeight = 10,
                        SymbolWidth = 10,
                        Symbol = ChartSymbol.Ellipse
                    }
                };


                ChartTooltip.SetShowDuration(series, 5000);
                chForecast.Series.Add(series);
            }
        }

        private async Task LoadChartExpenses()
        {
            //Header

            Border border = new()
            {

                BorderThickness = new Thickness(0.5),

                BorderBrush = new SolidColorBrush(Colors.Black),

                Margin = new Thickness(10),

                CornerRadius = new CornerRadius(5)

            };

            TextBlock textBlock = new()
            {

                Text = "Clasificación Gastos",

                Margin = new Thickness(5),

                FontSize = 14

            };

            textBlock.Effect = new DropShadowEffect
            {

                Color = Colors.Black,

                Opacity = 0.5

            };

            border.Child = textBlock;

            chExpenses.Header = border;

            //Axis

            CategoryAxis primaryAxis = new();
            primaryAxis.Header = "Categoría";
            chExpenses.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new();
            secondaryAxis.Header = "Importe (€)";
            chExpenses.SecondaryAxis = secondaryAxis;

            //ToolTip

            DataTemplate tooltip = new();

            FrameworkElementFactory stackpanel = new(typeof(StackPanel));
            stackpanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory textblock = new(typeof(TextBlock));
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Item.Category"));
            textblock.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock);

            FrameworkElementFactory textblock1 = new(typeof(TextBlock));
            textblock1.SetValue(TextBlock.TextProperty, " : ");
            textblock1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock1.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock1.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock1.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock1);

            FrameworkElementFactory textblock2 = new(typeof(TextBlock));
            textblock2.SetBinding(TextBlock.TextProperty,
                new Binding("Item.NegAmount")
                {
                    StringFormat = "C",
                    ConverterCulture = new System.Globalization.CultureInfo("es-ES")
                });

            textblock2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock2.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock2.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock2.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock2);
            tooltip.VisualTree = stackpanel;

            //Series

            var lExpensesCharts = await DependencyConfigView.IvBalancebyCategoryServiceView.GetExpensesbyYearMonthAsync(DateTime.Now.Month, DateTime.Now.Year);
            chExpenses.Series.Clear();

            ColumnSeries series = new()
            {
                ItemsSource = lExpensesCharts?.OrderByDescending(x => x.NegAmount).Take(10),
                XBindingPath = "Category",
                YBindingPath = "NegAmount",
                ShowTooltip = true,
                TooltipTemplate = tooltip,
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(0, 0, 3)
            };

            ChartTooltip.SetShowDuration(series, 5000);
            chExpenses.Series.Add(series);

            //Grid

            gvExpenses.ItemsSource = lExpensesCharts?.OrderByDescending(x => x.NegAmount);
        }

        public async Task LoadCharts()
        {
            await LoadChartForecast();
            await LoadChartExpenses();
        }

        #endregion

    }
}

