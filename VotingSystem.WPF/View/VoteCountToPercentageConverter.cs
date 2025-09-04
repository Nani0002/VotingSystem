using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VotingSystem.WPF.View
{
    public class VoteCountToPercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int voteCount = (int)values[0];
            int sumCount = (int)values[1];
            if (sumCount != 0)
            {
                double percantage = (double)voteCount / sumCount * 100;
                return $"{Math.Round(percantage, 0)}%";
            }
            return "0%";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
