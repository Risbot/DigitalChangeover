using FluentValidation.Results;
using FluentValidation;
using HA.MVVMClient.DataService;
using HA.MVVMClient.Infrastructure;
using HA.MVVMClient.ViewModelsValidators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ServiceModel;
using Microsoft.Practices.Unity;

namespace HA.MVVMClient.ViewModels
{
    public class DetachmentViewModel : BaseViewModel
    {
        #region Variables

        private Detachment selectedDetachment;
        private ObservableCollection<Detachment> detachments;
        private bool enabled;
        private string name, description;
        private DataServiceClient dataClient;
        private Detachment oldObject;
        private DetachmentViewModelValidator validator;
        private bool busy;

        #endregion

        #region Constructors

        public DetachmentViewModel(DataServiceClient dataClient, DetachmentViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Enabled = true;
            InitCommands();
            this.dataClient.GetDetachmentsCompleted += GetDetachmentsCompleted;
            this.dataClient.GetDetachmentsAsync();
            PropertyChanged += (s, e) =>
            {
                (SaveCommand as Command).OnCanExecuteChanged();
                (NewCommand as Command).OnCanExecuteChanged();
                (DeleteCommand as Command).OnCanExecuteChanged();
            };
        }

        #endregion

        #region Functions

        public override ValidationResult Validator(string propertyName)
        {
            return validator.Validate(this, propertyName);
        }

        #endregion

        #region Events

        void GetDetachmentsCompleted(object sender, GetDetachmentsCompletedEventArgs e)
        {
           
            if (e.Error == null)
            {
                Items = e.Result;
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.GetDetachmentsCompleted -= GetDetachmentsCompleted;
            Busy = false;
        }

        void DeleteDetachmentCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                Items.Remove(SelectedItem);
                NewCommand.Execute(null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteDetachmentCompleted -= DeleteDetachmentCompleted;
            Busy = false;
        }

        void UpdateDetachmentCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                oldObject = null;
                (SaveCommand as Command).OnCanExecuteChanged();
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedItem = oldObject;
            }
            dataClient.UpdateDetachmentCompleted -= UpdateDetachmentCompleted;
            Busy = false;
        }

        void AddDetachmentCompleted(object sender, AddDetachmentCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                selectedDetachment.ID = e.Result;
                Items.Add(selectedDetachment);
                SelectedItem = selectedDetachment;
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedItem = null;
            }
            dataClient.AddDetachmentCompleted -= AddDetachmentCompleted;
            Busy = false;
        }

        #endregion

        #region Commands

        private void InitCommands()
        {
            SaveCommand = new Command(OnSaveExecute, OnSaveCanExecute);
            NewCommand = new Command(OnNewExecute, OnNewCanExecute);
            DeleteCommand = new Command(OnDeleteExecute, OnDeleteCanExecute);
        }

        public ICommand SaveCommand
        {
            get;
            private set;
        }

        public ICommand NewCommand
        {
            get;
            private set;
        }

        public ICommand DeleteCommand
        {
            get;
            private set;
        }

        private void OnSaveExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            if (SelectedItem == null)
            {
                selectedDetachment = new Detachment()
                {
                    Description = Description,
                    Name = Key
                };
                dataClient.AddDetachmentCompleted += AddDetachmentCompleted;
                dataClient.AddDetachmentAsync(selectedDetachment);
            }
            else
            {
                oldObject = SelectedItem;
                SelectedItem.Description = Description;
                dataClient.UpdateDetachmentCompleted += UpdateDetachmentCompleted;
                dataClient.UpdateDetachmentAsync(SelectedItem);
            }
        }

        private bool OnSaveCanExecute()
        {
            return (SelectedItem == null ? true :
            (String.IsNullOrWhiteSpace(SelectedItem.Description) ? null : SelectedItem.Description) !=
            (String.IsNullOrWhiteSpace(Description) ? null : Description)) && IsValid;
        }

        private void OnNewExecute()
        {
            Description = null;
            Key = null;
            SelectedItem = null;
            Enabled = true;
        }

        private bool OnNewCanExecute()
        {
            return SelectedItem != null;
        }

        private void OnDeleteExecute()
        {
            Busy = true;
            dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
            dataClient.DeleteDetachmentCompleted += DeleteDetachmentCompleted;
            dataClient.DeleteDetachmentAsync(SelectedItem.ID);
        }

        private bool OnDeleteCanExecute()
        {
            return SelectedItem != null && SelectedItem.ID != LoginInit.user.DetachmentID;
        }

        #endregion

        #region Properties

        public string Title
        {
            get { return "Oddělení"; }
        }

        public bool Busy
        {
            get
            {
                return busy;
            }
            set
            {
                if (busy != value)
                {
                    busy = value;
                    OnPropertyChanged(() => Busy);
                }
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged(() => Enabled);
                }
            }
        }      
       
        public string Key
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(() => Key);
                }
            }
        }

        public Detachment SelectedItem
        {
            get { return selectedDetachment; }
            set
            {
                selectedDetachment = value;
                if (selectedDetachment != null)
                {
                    Description = selectedDetachment.Description;
                    Key = selectedDetachment.Name;
                    Enabled = false;
                }
                OnPropertyChanged(() => SelectedItem); 
            }
        }
        
        public ObservableCollection<Detachment> Items
        {
            get { return detachments; }
            set
            {
                if (detachments != value)
                {
                    detachments = value;
                    OnPropertyChanged(() => Items); 
                }
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(() => Description); 
                }
            }
        }

        #endregion
    }
}
