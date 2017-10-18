using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace JarvisWpf.Converters
{
    public class SourceControlTypeToBrush: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush bgBrush = Brushes.White;
            switch (System.Convert.ToString(value))
            {
                case "Changeset":
                    bgBrush = null;
                    break;
                case "Merge Request":
                    bgBrush = Brushes.PaleGreen;
                    break;
            }
            return bgBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
