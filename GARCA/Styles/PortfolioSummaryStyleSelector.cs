using Syncfusion.UI.Xaml.Grid;
using System.Windows;
using System.Windows.Controls;

namespace GARCA.Styles
{
    public class PortfolioSummaryStyleSelector : StyleSelector
    {
        public Style CenterStyle { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            var cell = container as GridTableSummaryCell;

            return cell.ColumnBase.GridColumn.MappingName is "costShares" or "marketValue"
                or "profit"
                ? CenterStyle
                : null;
        }
    }
}
