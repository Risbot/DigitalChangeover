using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace HA.MVVMClient.Infrastructure
{
    public class ProgressAdornerElement : Adorner
    {
        private FrameworkElement child;

        public ProgressAdornerElement(FrameworkElement adornerChildElement, FrameworkElement adornedElement)
            : base(adornedElement)
        {
            this.child = adornerChildElement;
            base.AddLogicalChild(adornerChildElement);
            base.AddVisualChild(adornerChildElement);
        }

        public void DisconnectChild()
        {
            base.RemoveLogicalChild(child);
            base.RemoveVisualChild(child);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        protected override Int32 VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(Int32 index)
        {
            return this.child;
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(this.child);
                return (IEnumerator)list.GetEnumerator();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(0, 0, AdornedElement.ActualWidth, AdornedElement.ActualHeight));
            return finalSize;
        }

        public new FrameworkElement AdornedElement
        {
            get
            {
                return (FrameworkElement)base.AdornedElement;
            }
        }
    }
}
