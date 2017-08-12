using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using FluentValidation;
using FluentValidation.Results;

namespace HA.MVVMClient.Infrastructure
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event EventHandler CloseView; 
        public event PropertyChangedEventHandler PropertyChanged;
        private ValidationResult result;
        private Dictionary<string, bool> propertiesDictionary;
        private bool isValid;

        public BaseViewModel()
        {
            propertiesDictionary = new Dictionary<string, bool>();
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            if (property.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = property.Body as MemberExpression;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
            }
        }

        protected void OnCloseView()
        {
            if (CloseView != null)
            {
                Navigator = null;
                CloseView(this, EventArgs.Empty);
            }
        }

        public INavigator Navigator
        {
            get;
            set;
        }

        public virtual ValidationResult Validator(string propertyName)
        {
            return null;
        }


        public bool IsValid
        {
            get 
            {
                return isValid; 
            }
            private set
            {
                if (isValid != value)
                {
                    isValid = value;
                    OnPropertyChanged(() => IsValid);
                }
            }
        }

        public string Error
        {
            get
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errors.Append(error.ErrorMessage);
                    errors.Append(Environment.NewLine);
                }
                return errors.ToString();
            }
        }

        public string this[string columnName]
        {
            get
            {
                result = Validator(columnName);
                if (result == null)
                    return string.Empty;
                if (propertiesDictionary.Any(c => c.Key == columnName))
                    propertiesDictionary[columnName] = result.IsValid;
                else
                    propertiesDictionary.Add(columnName, result.IsValid);
                IsValid = propertiesDictionary.Values.All(c => c == true);
                if (!result.IsValid)
                {
                    var columnResult = result.Errors.FirstOrDefault(c => String.Compare(c.PropertyName, columnName, true) == 0);
                    return columnResult != null ? columnResult.ErrorMessage : string.Empty;
                }
                return string.Empty;
            }
        }
    }
}
