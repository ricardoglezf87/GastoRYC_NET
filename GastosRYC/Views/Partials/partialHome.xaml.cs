using BOLib.Services;
using BOLib.Models;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para partialTransactions.xaml
    /// </summary>
    public partial class PartialHome : Page
    {
        #region Variables

        private readonly SimpleInjector.Container servicesContainer;
        private readonly MainWindow parentForm;

        #endregion

        #region Constructor

        public PartialHome(SimpleInjector.Container _servicesContainer, MainWindow _parentForm)
        {
            InitializeComponent();
            servicesContainer = _servicesContainer;
            parentForm = _parentForm;
        }

        #endregion

        #region Events


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadCharts();
        }

        #endregion

        #region Functions

        private void loadChartForecast()
        {
            //Header

            Border border = new Border()
            {

                BorderThickness = new Thickness(0.5),

                BorderBrush = new System.Windows.Media.SolidColorBrush(Colors.Black),

                Margin = new Thickness(10),

                CornerRadius = new CornerRadius(5)
            };

            TextBlock textBlock = new TextBlock()
            {

                Text = "Prevision de cobros / pagos",

                Margin = new Thickness(5),

                FontSize = 14

            };

            textBlock.Effect = new DropShadowEffect()
            {

                Color = Colors.Black,

                Opacity = 0.5

            };

            border.Child = textBlock;

            chForecast.Header = border;

            //Legend

            chForecast.Legend = new ChartLegend()
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

            DateTimeAxis primaryAxis = new DateTimeAxis();
            primaryAxis.Header = "Fecha";
            //primaryAxis.Minimum = DateTime.Today.AddDays(-1);
            //primaryAxis.Maximum= DateTime.Today.AddMonths(1).AddDays(1);
            primaryAxis.PlotOffsetStart = 20;
            primaryAxis.PlotOffsetEnd = 20;
            primaryAxis.IntervalType = DateTimeIntervalType.Days;
            primaryAxis.Interval = 2;
            primaryAxis.LabelFormat = "dd/MM";
            chForecast.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Header = "Importe (€)";
            chForecast.SecondaryAxis = secondaryAxis;

            //ToolTip

            DataTemplate tooltip = new DataTemplate();

            FrameworkElementFactory stackpanel = new FrameworkElementFactory(typeof(StackPanel));
            stackpanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory textblock = new FrameworkElementFactory(typeof(TextBlock));
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Item.account"));
            textblock.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock);

            FrameworkElementFactory textblock1 = new FrameworkElementFactory(typeof(TextBlock));
            textblock1.SetValue(TextBlock.TextProperty, " : ");
            textblock1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock1.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock1.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock1.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock1);

            FrameworkElementFactory textblock2 = new FrameworkElementFactory(typeof(TextBlock));
            textblock2.SetBinding(TextBlock.TextProperty,
                new Binding("Item.amount")
                {
                    StringFormat = "C",
                    ConverterCulture = new System.Globalization.CultureInfo("es-ES")
                });

            textblock2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock2.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock2.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock2.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock2);
            tooltip.VisualTree = stackpanel;

            //Series

            chForecast.Series.Clear();

            foreach (Accounts accounts in servicesContainer.GetInstance<AccountsService>().getAllOpened()?
                .Where(x => servicesContainer.GetInstance<AccountsTypesService>().accountExpensives(x.accountsTypesid)))
            {

                LineSeries series = new LineSeries()
                {
                    ItemsSource = servicesContainer.GetInstance<ForecastsChartService>().getMonthForecast()
                        .Where(x => x.accountid == accounts.id).OrderByDescending(x => x.date),
                    Label = accounts.description,
                    XBindingPath = "date",
                    YBindingPath = "amount",
                    ShowTooltip = true,
                    TooltipTemplate = tooltip,
                    EnableAnimation = true,
                    AnimationDuration = new TimeSpan(0, 0, 3),
                    AdornmentsInfo = new ChartAdornmentInfo()
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

        private void loadChartExpenses()
        {
            //Header

            Border border = new Border()
            {

                BorderThickness = new Thickness(0.5),

                BorderBrush = new System.Windows.Media.SolidColorBrush(Colors.Black),

                Margin = new Thickness(10),

                CornerRadius = new CornerRadius(5)

            };

            TextBlock textBlock = new TextBlock()
            {

                Text = "Clasificación Gastos",

                Margin = new Thickness(5),

                FontSize = 14

            };

            textBlock.Effect = new DropShadowEffect()
            {

                Color = Colors.Black,

                Opacity = 0.5

            };

            border.Child = textBlock;

            chExpenses.Header = border;

            //Axis

            CategoryAxis primaryAxis = new CategoryAxis();
            primaryAxis.Header = "Categoría";
            chExpenses.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Header = "Importe (€)";
            chExpenses.SecondaryAxis = secondaryAxis;

            //ToolTip

            DataTemplate tooltip = new DataTemplate();

            FrameworkElementFactory stackpanel = new FrameworkElementFactory(typeof(StackPanel));
            stackpanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory textblock = new FrameworkElementFactory(typeof(TextBlock));
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Item.category"));
            textblock.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock);

            FrameworkElementFactory textblock1 = new FrameworkElementFactory(typeof(TextBlock));
            textblock1.SetValue(TextBlock.TextProperty, " : ");
            textblock1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock1.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock1.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock1.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock1);

            FrameworkElementFactory textblock2 = new FrameworkElementFactory(typeof(TextBlock));
            textblock2.SetBinding(TextBlock.TextProperty,
                new Binding("Item.neg_amount")
                {
                    StringFormat = "C",
                    ConverterCulture = new System.Globalization.CultureInfo("es-ES")
                });

            textblock2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock2.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock2.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock2.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock2);
            tooltip.VisualTree = stackpanel;

            //Series

            List<VBalancebyCategory>? lExpensesCharts = servicesContainer.GetInstance<VBalancebyCategoryService>().getExpensesbyYearMonth(DateTime.Now.Month, DateTime.Now.Year);
            chExpenses.Series.Clear();

            ColumnSeries series = new ColumnSeries()
            {
                ItemsSource = lExpensesCharts?.OrderByDescending(x => x.neg_amount).Take(10),
                XBindingPath = "category",
                YBindingPath = "neg_amount",
                ShowTooltip = true,
                TooltipTemplate = tooltip,
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(0, 0, 3)
            };

            ChartTooltip.SetShowDuration(series, 5000);
            chExpenses.Series.Add(series);

            //Grid

            gvExpenses.ItemsSource = lExpensesCharts?.OrderByDescending(x => x.neg_amount);
        }

        public void loadCharts()
        {
            loadChartForecast();
            loadChartExpenses();
        }

        #endregion

    }
}

