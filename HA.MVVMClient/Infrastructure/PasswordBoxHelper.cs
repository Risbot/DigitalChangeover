using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HA.MVVMClient.Infrastructure
{
    public class PasswordBoxHelper
    {
        public static readonly DependencyProperty IsBindingProperty =
            DependencyProperty.RegisterAttached("IsBinding",
            typeof(bool), typeof(PasswordBoxHelper),
            new PropertyMetadata(false, OnIsBindingPropertyChanged));

        public static readonly DependencyProperty PasswordTextProperty =
            DependencyProperty.RegisterAttached("PasswordText", 
            typeof(string), typeof(PasswordBoxHelper),
            new PropertyMetadata(null, OnPasswordTextPropertyChanged));

        public static string GetPasswordText(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordTextProperty);
        }

        public static void SetPasswordText(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordTextProperty, value);
        }

        public static bool GetIsBinding(DependencyObject obj)
        {
            return (bool)obj.GetValue(PasswordTextProperty);
        }

        public static void SetIsBinding(DependencyObject obj, string value)
        {
            obj.SetValue(IsBindingProperty, value);
        }

        private static void OnIsBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
                (d as PasswordBox).PasswordChanged += PasswordBoxHelper_PasswordChanged;
        }


        private static void OnPasswordTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as PasswordBox).Password != (string)e.NewValue)
                (d as PasswordBox).Password = (string)e.NewValue;
        }

        private static void PasswordBoxHelper_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SetPasswordText((DependencyObject)sender, (sender as PasswordBox).Password);
        }
    }
}
