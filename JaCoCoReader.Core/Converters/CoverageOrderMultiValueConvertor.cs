using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using JaCoCoReader.Core.ViewModels.CodeCoverage;

namespace JaCoCoReader.Core.Converters
{
    public class CoverageOrderMultiValueConvertor : IMultiValueConverter
    {
        public static readonly IMultiValueConverter Instance = new CoverageOrderMultiValueConvertor();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2
                || !(values[0] is IEnumerable<IFolderNodeViewModel>)
                || !(values[1] is CodeCoverageOrder)
                || !(values[2] is bool)
                )
            {
                return null;
            }
            IEnumerable<IFolderNodeViewModel> viewModels = values[0] as IEnumerable<IFolderNodeViewModel>;

            CodeCoverageOrder coverageOrder = (CodeCoverageOrder)values[1];

            if ((bool) values[2])
            {
                switch (coverageOrder)
                {
                    case CodeCoverageOrder.Description:
                        return viewModels.OrderByDescending(n => n.Description);
                    case CodeCoverageOrder.Missed:
                        return viewModels.OrderByDescending(n => n.MissedLines);
                    case CodeCoverageOrder.MissedPercentage:
                        return viewModels.OrderByDescending(n => n.MissedLinesPercentage);
                    case CodeCoverageOrder.Covered:
                        return viewModels.OrderByDescending(n => n.CoveredLines);
                    case CodeCoverageOrder.CoveredPercentage:
                        return viewModels.OrderByDescending(n => n.CoveredLinesPercentage);
                    case CodeCoverageOrder.Total:
                        return viewModels.OrderByDescending(n => n.TotalLines);
                }
            }
            else
            {
                switch (coverageOrder)
                {
                    case CodeCoverageOrder.Description:
                        return viewModels.OrderBy(n => n.Description);
                    case CodeCoverageOrder.Missed:
                        return viewModels.OrderBy(n => n.MissedLines);
                    case CodeCoverageOrder.MissedPercentage:
                        return viewModels.OrderBy(n => n.MissedLinesPercentage);
                    case CodeCoverageOrder.Covered:
                        return viewModels.OrderBy(n => n.CoveredLines);
                    case CodeCoverageOrder.CoveredPercentage:
                        return viewModels.OrderBy(n => n.CoveredLinesPercentage);
                    case CodeCoverageOrder.Total:
                        return viewModels.OrderBy(n => n.TotalLines);
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}