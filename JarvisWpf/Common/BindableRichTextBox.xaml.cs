using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JarvisWpf.Common
{
    /// <summary>
    /// Interaction logic for BindableRichTextBox.xaml
    /// </summary>
    public partial class BindableRichTextBox : UserControl, INotifyPropertyChanged
    {
        public BindableRichTextBox()
        {
            InitializeComponent();
        }

        public void ReadRtfFromTxtBox()
        {

            var content = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
            if (content.CanSave(DataFormats.Rtf))
            {
                using (var stream = new MemoryStream())
                {
                    content.Save(stream, DataFormats.Rtf);
                    if (stream != null)
                    {
                        Rtf = System.Text.Encoding.Default.GetString(stream.ToArray());
                    }
                }
            }
        }

        bool setOnce;

        public void LoadRtfToTxtBox()
        {
            var content = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
            if (Rtf == null)
            {
                RTB.Document = new FlowDocument();
                return;
            }

            byte[] by = System.Text.Encoding.Default.GetBytes(Rtf);
            MemoryStream ms = new MemoryStream();
            ms.Write(by, 0, by.Length);
            ms.Position = 0;
            try
            {
                if (content.CanLoad(DataFormats.Rtf))
                {
                    content.Load(ms, DataFormats.Rtf);
                }
            }
            catch(Exception)
            {
                //Swallow this
            }
            ms.Close();
            ms.Dispose();
        }

        private void RTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReadRtfFromTxtBox();
        }




        public string Rtf
        {
            get
            {
                return (string)GetValue(RtfProperty);
            }
            set
            {
                SetValue(RtfProperty, value);
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Rtf"));
            }
        }

        // Using a DependencyProperty as the backing store for Rtf.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RtfProperty =
            DependencyProperty.Register("Rtf", typeof(string), typeof(BindableRichTextBox), new PropertyMetadata(SomethingChanged));

        private static void SomethingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableRichTextBox btx = ((BindableRichTextBox)d);
            btx.LoadRtfToTxtBox();
        }

        private void RTB_LostFocus(object sender, RoutedEventArgs e)
        {
            ReadRtfFromTxtBox();
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
