using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace GastosRYC.Styles
{
    public class PortfolioSummaryStyleSelector : StyleSelector
    {
        public Style CenterStyle { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            var cell = container as GridTableSummaryCell;

            if (cell.ColumnBase.GridColumn.MappingName == "costShares" || cell.ColumnBase.GridColumn.MappingName == "marketValue" 
                || cell.ColumnBase.GridColumn.MappingName == "profit")
            {
                return CenterStyle;
            }
            else
            {
                return null;
            }
        }
    }
}
