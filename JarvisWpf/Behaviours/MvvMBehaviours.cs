using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace JarvisWpf.Behaviours
{
    static public class MvvmBehaviours
    {
        public static string GetLoadedMethodName(DependencyObject obj)
        {
            return (string)obj.GetValue(LoadedMethodNameProperty);
        }

        public static void SetLoadedMethodName(DependencyObject obj, string value)
        {
            obj.SetValue(LoadedMethodNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for LoadedMethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadedMethodNameProperty =
            DependencyProperty.RegisterAttached("LoadedMethodName", typeof(string), typeof(MvvmBehaviours), new PropertyMetadata(null, LoadedMethodNameChanged));

        private static void LoadedMethodNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement F = d as FrameworkElement;
            if (F == null) return;
            F.Loaded += (sender, newE) =>
            {
                var dataContext = F.DataContext;
                if (dataContext == null) return;
                var loadedMethodInstance = dataContext.GetType().GetMethod(e.NewValue.ToString());
                if (loadedMethodInstance == null) return;
                loadedMethodInstance.Invoke(dataContext, null);
            };
        }




        //DataContext Changed Event Wire Up



        public static string GetDataContextChanged(DependencyObject obj)
        {
            return (string)obj.GetValue(DataContextChangedProperty);
        }

        public static void SetDataContextChanged(DependencyObject obj, string value)
        {
            obj.SetValue(DataContextChangedProperty, value);
        }

        // Using a DependencyProperty as the backing store for DataContextChanged.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataContextChangedProperty =
            DependencyProperty.RegisterAttached("DataContextChanged", typeof(string), typeof(MvvmBehaviours), new PropertyMetadata(new PropertyChangedCallback(DataContextChanged)));

        private static void DataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }



    }
}
