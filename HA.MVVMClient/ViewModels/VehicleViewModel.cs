using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.MVVMClient.DataService;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HA.MVVMClient.Infrastructure;
using Microsoft.Practices.Unity;
using HA.MVVMClient.ViewModelsValidators;
using FluentValidation.Results;
using FluentValidation;
using System.Windows;

namespace HA.MVVMClient.ViewModels
{
    public class VehicleViewModel : BaseViewModel
    {     
        #region Variables

        private Vehicle selectedVehicle;
        private Vehicle oldObject;
        private ObservableCollection<Vehicle> vehicles;
        private bool enabled;
        private string number, description;
        private DataServiceClient dataClient;
        private VehicleViewModelValidator validator;
        private bool busy;

        #endregion

        #region Constructors

        public VehicleViewModel(DataServiceClient dataClient, VehicleViewModelValidator validator, INavigator navigator)
        {
            Busy = true;
            this.dataClient = dataClient;
            this.validator = validator;
            Navigator = navigator;
            Enabled = true;
            InitCommands();
            this.dataClient.FindVehiclesCompleted += FindVehiclesCompleted;
            this.dataClient.FindVehiclesAsync(LoginInit.user.DetachmentID);
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

        void FindVehiclesCompleted(object sender, FindVehiclesCompletedEventArgs e)
        {
            if (e.Error == null)
                Items = e.Result;
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.FindVehiclesCompleted -= FindVehiclesCompleted;
            Busy = false;
        }

        void UpdateVehicleCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
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
            dataClient.UpdateVehicleCompleted -= UpdateVehicleCompleted;
            Busy = false;
        }

        void AddVehicleCompleted(object sender, AddVehicleCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                selectedVehicle.ID = e.Result;
                Items.Add(selectedVehicle);
                SelectedItem = selectedVehicle;
            }
            else
            {
                ErrorProvider.ShowError(e.Error, Navigator);
                SelectedItem = null;
            }
            dataClient.AddVehicleCompleted -= AddVehicleCompleted;
            Busy = false;
        }

        void DeleteVehicleCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Items.Remove(SelectedItem);
                NewCommand.Execute(null);
            }
            else
                ErrorProvider.ShowError(e.Error, Navigator);
            dataClient.DeleteVehicleCompleted -= DeleteVehicleCompleted;
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
                selectedVehicle = new Vehicle()
                {
                    Description = Description,
                    DetachmentID = LoginInit.user.DetachmentID,
                    Number = Key
                };
                dataClient.AddVehicleCompleted += AddVehicleCompleted;
                dataClient.AddVehicleAsync(selectedVehicle);
            }
            else
            {
                oldObject = SelectedItem;
                SelectedItem.Description = Description;
                dataClient.UpdateVehicleCompleted += UpdateVehicleCompleted;
                dataClient.UpdateVehicleAsync(SelectedItem);
            }
        }

        private bool OnSaveCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
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
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedItem != null;
        }

        private void OnDeleteExecute()
        {
            var n = Navigator.CreateChild();
            if (n.ShowMessageBox("Jste si jistí že chcete smazat vozidlo: " + SelectedItem.Number + "?\nSmažou se i veškeré záznamy spojené s tímto vozidlem!", "Varováni", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Busy = true;
                dataClient = ContainerProvider.GetInstance.Resolve<DataServiceClient>();
                dataClient.DeleteVehicleCompleted += DeleteVehicleCompleted;
                dataClient.DeleteVehicleAsync(SelectedItem.ID);
            }
        }

        private bool OnDeleteCanExecute()
        {
            if (!LoginInit.user.Roles.Any(c => c.Name == "Write"))
                return false;
            return SelectedItem != null;
        }

        #endregion

        #region Properties

        public string Title
        {
            get { return "Vozidla"; }
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

        public string Name
        {
            get { return "Vozidlo:"; }
        }

        public string List
        {
            get { return "Vozidla:"; }
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
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;                 
                    OnPropertyChanged(() => Key);
                }
            }
        }
      
        public Vehicle SelectedItem
        {
            get { return selectedVehicle; }
            set
            {
                selectedVehicle = value;
                if (selectedVehicle != null)
                {
                    Description = selectedVehicle.Description;
                    Key = selectedVehicle.Number;
                    Enabled = false;
                }
                OnPropertyChanged(() => SelectedItem);
            }
        }
        
        public ObservableCollection<Vehicle> Items
        {
            get { return vehicles; }
            set
            {
                if (vehicles != value)
                {
                    vehicles = value;
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
