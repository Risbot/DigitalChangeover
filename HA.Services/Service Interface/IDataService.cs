using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HA.Services
{
    [ServiceContract]
    public interface IDataService
    {

        #region Attendance
    
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void AddAttendance(Objects.Attendance attendance);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteAttendance(Objects.Attendance attendance);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateAttendance(Objects.Attendance attendance);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Attendance> FindAttendances(Int64 dateID);

        #endregion

        #region Date

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int64 AddDate(Objects.Date date);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteDate(Int64 dateID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateDate(Objects.Date date);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Int32> GetYears(Int32 detachmentID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Int32> GetMonths(Int32 year, Int32 detachmentID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Date> FindDates(Int32 year, Int32 month, Int32 detachmentID);

        #endregion

        #region Changeover

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int32 AddChangeover(Objects.Changeover changeover);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteChangeover(Int32 changeoverID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateChangeover(Objects.Changeover changeover);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Objects.Work TransferChangeover(Int64 dateID, Int32 changeoverID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Changeover> FindChangeovers(Int32 detachmentID);

        #endregion

        #region Detachment

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int32 AddDetachment(Objects.Detachment detachment);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteDetachment(Int32 detachmentID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateDetachment(Objects.Detachment detachment);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Objects.Detachment FindDetachment(Int32 detachmentID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Detachment> GetDetachments();

        #endregion

        #region Tour

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int32 AddTour(Objects.Tour tour);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteTour(Int32 tourID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateTour(Objects.Tour tour);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Tour> FindTours(Int32 detachmentID);

        #endregion

        #region Vehicle

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int32 AddVehicle(Objects.Vehicle vehicle);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteVehicle(Int32 vehicleID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateVehicle(Objects.Vehicle vehicle);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Vehicle> FindVehicles(Int32 detachmentID);

        #endregion

        #region Work

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int64 AddWork(Objects.Work work);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteWork(Int64 workID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateWork(Objects.Work work);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Work> FindWorks(Int64 dateID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<string> GetTopFaultWorks(Int32 count, Int32 detachmentID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<string> GetTopCauseWorks(Int32 count, Int32 detachmentID);

        #endregion

        #region Worker

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int32 AddWorker(Objects.WorkerDetail worker);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteWorker(Int32 workerID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateWorker(Objects.WorkerDetail worker);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Objects.WorkerDetail FindWorker(Int32 workerID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.Worker> FindWorkers(Int32 detachmentID);

        #endregion

        #region Worker state

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int32 AddWorkerState(Objects.WorkerState workerState);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteWorkerState(Int32 workerStateID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateWorkerState(Objects.WorkerState workerState);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.WorkerState> FindWorkerStates(Int32 detachmentID);

        #endregion

        #region Work type

        [FaultContract(typeof(WcfException))]
        [OperationContract]
        Int32 AddWorkType(Objects.WorkType workType);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void DeleteWorkType(Int32 workTypeID);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        void UpdateWorkType(Objects.WorkType workType);
        [FaultContract(typeof(WcfException))]
        [OperationContract]
        List<Objects.WorkType> FindWorkTypes(Int32 detachmentID);

        #endregion
    }
}
