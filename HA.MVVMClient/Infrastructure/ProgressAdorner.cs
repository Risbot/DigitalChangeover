using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HA.MVVMClient.Infrastructure
{
    public class ProgressAdorner : ContentControl
    {
        private AdornerLayer adornerLayer = null;
        private ProgressAdornerElement adorner = null;

        public static readonly DependencyProperty IsAdornerVisibleProperty =
         DependencyProperty.Register("IsAdornerVisible", typeof(bool), typeof(ProgressAdorner),
             new FrameworkPropertyMetadata(AdornerPropertyChanged));

        public static readonly DependencyProperty AdornerContentProperty =
            DependencyProperty.Register("AdornerContent", typeof(FrameworkElement), typeof(ProgressAdorner),
                new FrameworkPropertyMetadata(AdornerPropertyChanged));

        private static void AdornerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressAdorner c = (ProgressAdorner)d;
            c.ShowOrHideAdornerInternal();
        }

        public FrameworkElement AdornerContent
        {
            get
            {
                return (FrameworkElement)GetValue(AdornerContentProperty);
            }
            set
            {
                SetValue(AdornerContentProperty, value);
            }
        }

        public bool IsAdornerVisible
        {
            get
            {
                return (bool)GetValue(IsAdornerVisibleProperty);
            }
            set
            {
                SetValue(IsAdornerVisibleProperty, value);
            }
        }

       

        private void UpdateAdornerDataContext()
        {
            if (this.AdornerContent != null)
            {
                this.AdornerContent.DataContext = this.DataContext;
            }
        }

        private void ShowAdorner()
        {
            if (this.adorner != null)
                return;


            if (this.AdornerContent != null)
            {
                if (this.adornerLayer == null)
                {
                    this.adornerLayer = AdornerLayer.GetAdornerLayer(this);
                }

                if (this.adornerLayer != null)
                {
                    this.adorner = new ProgressAdornerElement(this.AdornerContent, this);
                    this.adornerLayer.Add(this.adorner);
                    UpdateAdornerDataContext();
                }
            }
        }

      
        private void HideAdorner()
        {
            if (this.adornerLayer == null || this.adorner == null)
                return;
            this.adornerLayer.Remove(this.adorner);
            this.adorner.DisconnectChild();
            this.adorner = null;
            this.adornerLayer = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ShowOrHideAdornerInternal();
        }

        private void ShowOrHideAdornerInternal()
        {
            if (IsAdornerVisible)
            {
                ShowAdorner();
            }
            else
            {
                HideAdorner();
            }
        }

        //protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);
        //}

      
    }
}
