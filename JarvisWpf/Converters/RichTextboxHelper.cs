using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace JarvisWpf.Converters
{

    public class RtfToFlowDocumentConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
          var document = new FlowDocument();
          var range = new TextRange(document.ContentStart, document.ContentEnd);
          var stream = GetStream(value); // implement this method to get memorystream from the value which is ur rtf data.
          range.Load(stream, DataFormats.Rtf);
          return document ;
        }

        private Stream GetStream(object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
