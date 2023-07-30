using Syncfusion.UI.Xaml.Grid;
using System.Windows;
using System.Windows.Controls;

namespace GARCA.View.Styles
{
    public class PortfolioSummaryStyleSelector : StyleSelector
    {
        public Style CenterStyle { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            var cell = container as GridTableSummaryCell;

            return cell.ColumnBase.GridColumn.MappingName is "CostShares" or "MarketValue"
                or "profit"
                ? CenterStyle
                : null;
        }
    }
}
