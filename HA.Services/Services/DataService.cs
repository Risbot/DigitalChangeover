using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace HA.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataService : IDataService
    {

        #region Attendance

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void AddAttendance(Objects.Attendance attendance)
        {
            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Attendance> repository = new Repositor<Attendance>(unitOfWork);  
            try
            {  
                ObjectValidator.IsValid(attendance);
                Attendance a = new Attendance();
                a.AttendanceDateID = attendance.DateID;
                a.AttendanceWorkerID = attendance.WorkerID;
                a.AttendanceWorkerStateID = attendance.WorkerStateID;
                a.AttendanceDescription = String.IsNullOrWhiteSpace(attendance.Description) ? null : attendance.Description;
                a.AttendanceWorkerTourID = attendance.WorkerTourID;
                unitOfWork.BeginTransaction();
                repository.Add(a);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteAttendance(Objects.Attendance attendance)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Attendance> repository = new Repositor<Attendance>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(attendance);
                unitOfWork.BeginTransaction();
                var a = repository.Single(c => c.AttendanceDateID == attendance.DateID && c.AttendanceWorkerID == attendance.WorkerID);
                if (a.Date.DateIsClosed)
                    throw new DateException("Směna je uzamčená, nelze odebírat ani editovat záznamy");
                repository.Delete(a);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateAttendance(Objects.Attendance attendance)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Attendance> repository = new Repositor<Attendance>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(attendance);
                unitOfWork.BeginTransaction();
                var a = repository.Single(c => c.AttendanceDateID == attendance.DateID && c.AttendanceWorkerID == attendance.WorkerID);
                if (a.Date.DateIsClosed)
                   throw new DateException("Směna je uzamčená, nelze odebírat ani editovat záznamy");
                a.AttendanceWorkerStateID = attendance.WorkerStateID;
                a.AttendanceWorkerTourID = attendance.WorkerTourID;
                a.AttendanceDescription = String.IsNullOrWhiteSpace(attendance.Description) ? null : attendance.Description;
                repository.Update(a);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }
     
        public List<Objects.Attendance> FindAttendances(Int64 dateID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Attendance> repository = new Repositor<Attendance>(unitOfWork);
            try
            {    
                return repository.Find(k => k.AttendanceDateID == dateID).Select(c => new Objects.Attendance()
                {
                    DateID = c.AttendanceDateID,
                    WorkerID = c.AttendanceWorkerID,
                    FirstName = c.Worker.WorkerFirstName,
                    LastName = c.Worker.WorkerLastName,
                    SapNumber = c.Worker.WorkerSapNumber,
                    WorkerStateID = c.AttendanceWorkerStateID,
                    WorkerState = c.WorkerState.WorkerStateName,
                    WorkerTourID = c.AttendanceWorkerTourID,
                    WorkerTour = c.Tour.TourStartTime.ToString("hh\\:mm") + " - " + c.Tour.TourEndTime.ToString("hh\\:mm"),
                    Description = c.AttendanceDescription
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Date

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int64 AddDate(Objects.Date date)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Date> repository = new Repositor<Date>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(date);
                Date d = new Date();
                d.DateDate = date.DateContent;
                d.DateIsNight = date.IsNight;
                d.DateIsClosed = false;
                d.DateDetachmentID = date.DetachmentID;
                d.DateDescription = String.IsNullOrWhiteSpace(date.Description) ? null : date.Description;
                unitOfWork.BeginTransaction();
                var de = repository.Find(c => c.DateDate.Date >= date.DateContent.Date && c.DateDetachmentID == date.DetachmentID).ToList();
                if (de != null)
                {
                    if (de.Any(c => c.DateDate.Date > date.DateContent.Date) || de.Any(c => c.DateDate.Date == date.DateContent.Date && c.DateIsNight == true))
                        throw new FormatException("Není možné vytvářet směnu zpětně!");
                }
                repository.Add(d);
                unitOfWork.CommitTransaction();
                return d.DateID;
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteDate(Int64 dateID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Date> repository = new Repositor<Date>(unitOfWork);
            try
            {   
                unitOfWork.BeginTransaction();
                var d = repository.Single(c => c.DateID == dateID);
                if (d.DateIsClosed)
                    throw new DateException("Směna je uzamčená, nelze odebírat ani editovat záznamy");
                repository.Delete(d);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateDate(Objects.Date date)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Date> repository = new Repositor<Date>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(date);
                unitOfWork.BeginTransaction();
                var d = repository.Single(c => c.DateID == date.ID);
                d.DateIsClosed = date.IsClosed;
                d.DateDescription = String.IsNullOrWhiteSpace(date.Description) ? null : date.Description;
                repository.Update(d);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Int32> GetYears(Int32 detachmentID)
        {
            try
            {
                using (Entities context = new Entities())
                {                 
                    return context.Years(detachmentID).Select(c=> c.Value).OrderBy(c=> c).ToList<Int32>();
                }
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Int32> GetMonths(int year, Int32 detachmentID)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.Months(year, detachmentID).Select(c => c.Value).OrderBy(c => c).ToList<Int32>();
                }
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.Date> FindDates(int year, int month, Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Date> repository = new Repositor<Date>(unitOfWork);
            try
            {
                return repository.Find(c => c.DateDate.Year == year && c.DateDate.Month == month && c.DateDetachmentID == detachmentID).Select(c => new Objects.Date()
                {
                    ID = c.DateID,
                    DateContent = c.DateDate,
                    IsClosed = c.DateIsClosed,
                    IsNight = c.DateIsNight,
                    DetachmentID = c.DateDetachmentID,
                    Description = c.DateDescription
                }).OrderBy(c => c.DateContent).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Changeover

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int32 AddChangeover(Objects.Changeover changeover)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Changeover> repository = new Repositor<Changeover>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(changeover);
                Changeover ch = new Changeover();
                ch.ChangeoverDateID = changeover.DateID;
                ch.ChangeoverVehicleID = changeover.VehicleID;
                ch.ChangeoverWorkTypeID = changeover.WorkTypeID;
                ch.ChangeoverDescription = String.IsNullOrWhiteSpace(changeover.Description) ? null : changeover.Description;
                ch.ChangeoverDetachmentID = changeover.DetachmentID; 
                unitOfWork.BeginTransaction();
                repository.Add(ch);
                unitOfWork.CommitTransaction();
                return ch.ChangeoverID;           
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteChangeover(Int32 changeoverID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Changeover> repository = new Repositor<Changeover>(unitOfWork);
            try
            { 
                unitOfWork.BeginTransaction();
                var ch = repository.Single(c => c.ChangeoverID == changeoverID);
                repository.Delete(ch);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateChangeover(Objects.Changeover changeover)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Changeover> repository = new Repositor<Changeover>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(changeover);
                unitOfWork.BeginTransaction();
                var ch = repository.Single(c => c.ChangeoverID == changeover.ID);
                ch.ChangeoverDescription = String.IsNullOrWhiteSpace(changeover.Description) ? null : changeover.Description;
                repository.Update(ch);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Objects.Work TransferChangeover(Int64 dateID,Int32 changeoverID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Changeover> repository = new Repositor<Changeover>(unitOfWork);
            IRepository<Work> workRepository = new Repositor<Work>(unitOfWork);
            try
            {
                unitOfWork.BeginTransaction();
                var ch = repository.Single(c => c.ChangeoverID == changeoverID);
                Work w = new Work();
                w.WorkDateID = dateID;
                w.WorkVehicleID = ch.ChangeoverVehicleID;
                w.WorkWorkTypeID = ch.ChangeoverWorkTypeID;
                w.WorkFaultDescription = String.IsNullOrWhiteSpace(ch.ChangeoverDescription) ? null : ch.ChangeoverDescription;
                workRepository.Add(w);
                repository.Delete(ch);
                unitOfWork.CommitTransaction();
                w = workRepository.UoW.Orm.Set<Work>().
                    Include("Vehicle").
                    Include("Date").
                    Include("WorkType").Single(c => c.WorkID == w.WorkID);
                return new Objects.Work() 
                {
                    DateContent = w.Date.DateDate,
                    DateID = w.WorkDateID,
                    FaultDescription = w.WorkFaultDescription,
                    ID = w.WorkID,
                    IsNight = w.Date.DateIsNight,
                    VehicleID = w.WorkVehicleID,
                    VehicleNumber = w.Vehicle.VehicleNumber,
                    WorkTypeID = w.WorkWorkTypeID,
                    WorkTypeName = w.WorkType.WorkTypeName
                };     
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.Changeover> FindChangeovers(Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Changeover> repository = new Repositor<Changeover>(unitOfWork);
            try
            { 
                return repository.Find(c => c.ChangeoverDetachmentID == detachmentID).Select(c => new Objects.Changeover()
                {
                    ID = c.ChangeoverID,
                    DateID = c.ChangeoverDateID,
                    DateContent = c.Date.DateDate,
                    IsNight = c.Date.DateIsNight,
                    VehicleID = c.ChangeoverVehicleID,
                    VehicleNumber = c.Vehicle.VehicleNumber,
                    WorkTypeID = c.ChangeoverWorkTypeID,
                    WorkTypeName = c.WorkType.WorkTypeName,
                    Description = c.ChangeoverDescription,
                    DetachmentID = c.ChangeoverDetachmentID
                }).ToList(); 
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Detachment

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public Int32 AddDetachment(Objects.Detachment detachment)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Detachment> repository = new Repositor<Detachment>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(detachment);
                Detachment d = new Detachment();
                d.DetachmentName = String.IsNullOrWhiteSpace(detachment.Name) ? null : detachment.Name;
                d.DetachmentDescription = String.IsNullOrWhiteSpace(detachment.Description) ? null : detachment.Description;
                unitOfWork.BeginTransaction();
                repository.Add(d);
                unitOfWork.CommitTransaction();
                return d.DetachmentID;
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public void DeleteDetachment(Int32 detachmentID)
        {


            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Detachment> repository = new Repositor<Detachment>(unitOfWork);
            try
            {
                unitOfWork.BeginTransaction();
                var d = repository.Single(c => c.DetachmentID == detachmentID);
                repository.Delete(d);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public void UpdateDetachment(Objects.Detachment detachment)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Detachment> repository = new Repositor<Detachment>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(detachment);
                unitOfWork.BeginTransaction();
                var d = repository.Single(c => c.DetachmentID == detachment.ID);
                d.DetachmentDescription = String.IsNullOrWhiteSpace(detachment.Description) ? null : detachment.Description;
                repository.Update(d);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public Objects.Detachment FindDetachment(Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Detachment> repository = new Repositor<Detachment>(unitOfWork);
            try
            {
                var d = repository.Single(c => c.DetachmentID == detachmentID);
                Objects.Detachment detachment = new Objects.Detachment();
                detachment.ID = d.DetachmentID;
                detachment.Name = d.DetachmentName;
                detachment.Description = d.DetachmentDescription;
                return detachment;
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.Detachment> GetDetachments()
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Detachment> repository = new Repositor<Detachment>(unitOfWork);
            try
            {
                return repository.GetAll().Select(c => new Objects.Detachment()
                {
                    ID = c.DetachmentID,
                    Name = c.DetachmentName,
                    Description = c.DetachmentDescription
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Tour

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int32 AddTour(Objects.Tour tour)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Tour> repository = new Repositor<Tour>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(tour);
                Tour t = new Tour();
                t.TourStartTime = tour.StartTime;
                t.TourEndTime = tour.EndTime;
                t.TourDetachmentID = tour.DetachmentID;
                t.TourDescription = String.IsNullOrWhiteSpace(tour.Description) ? null : tour.Description;
                unitOfWork.BeginTransaction();
                repository.Add(t);
                unitOfWork.CommitTransaction();  
                return t.TourID;    
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteTour(Int32 tourID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Tour> repository = new Repositor<Tour>(unitOfWork);
            try
            { 
                unitOfWork.BeginTransaction();
                var t = repository.Single(c => c.TourID == tourID);
                repository.Delete(t);
                unitOfWork.CommitTransaction();  
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateTour(Objects.Tour tour)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Tour> repository = new Repositor<Tour>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(tour);
                unitOfWork.BeginTransaction();
                var t = repository.Single(c => c.TourID == tour.ID);
                t.TourDescription = String.IsNullOrWhiteSpace(tour.Description) ? null : tour.Description;
                repository.Update(t);
                unitOfWork.CommitTransaction();  
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.Tour> FindTours(Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Tour> repository = new Repositor<Tour>(unitOfWork);
            try
            {
                return repository.Find(c => c.TourDetachmentID == detachmentID).Select(c => new Objects.Tour()
                {
                    ID = c.TourID,
                    EndTime = c.TourEndTime,
                    StartTime = c.TourStartTime,
                    DetachmentID = c.TourDetachmentID,
                    Description = c.TourDescription
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Vehicle

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int32 AddVehicle(Objects.Vehicle vehicle)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Vehicle> repository = new Repositor<Vehicle>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(vehicle);
                Vehicle v = new Vehicle();
                v.VehicleNumber = String.IsNullOrWhiteSpace(vehicle.Number) ? null : vehicle.Number;
                v.VehicleDescription = String.IsNullOrWhiteSpace(vehicle.Description) ? null : vehicle.Description;
                v.VehicleDetachmentID = vehicle.DetachmentID;
                unitOfWork.BeginTransaction();
                repository.Add(v);
                unitOfWork.CommitTransaction();
                return v.VehicleID;    
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteVehicle(Int32 vehicleID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Vehicle> repository = new Repositor<Vehicle>(unitOfWork);
            try
            {
                unitOfWork.BeginTransaction();
                var v = repository.Single(c => c.VehicleID == vehicleID);
                repository.Delete(v);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateVehicle(Objects.Vehicle vehicle)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Vehicle> repository = new Repositor<Vehicle>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(vehicle);         
                unitOfWork.BeginTransaction();
                var v = repository.Single(c => c.VehicleID == vehicle.ID);
                v.VehicleDescription = String.IsNullOrWhiteSpace(vehicle.Description) ? null : vehicle.Description;
                repository.Update(v);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.Vehicle> FindVehicles(Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Vehicle> repository = new Repositor<Vehicle>(unitOfWork);
            try
            {
                return repository.Find(c => c.VehicleDetachmentID == detachmentID).Select(c => new Objects.Vehicle()
                {
                    ID = c.VehicleID,
                    Number = c.VehicleNumber,
                    DetachmentID = c.VehicleDetachmentID,
                    Description = c.VehicleDescription
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Work

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int64 AddWork(Objects.Work work)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Work> repository = new Repositor<Work>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(work);
                Work w = new Work();
                w.WorkDateID = work.DateID;
                w.WorkVehicleID = work.VehicleID;
                w.WorkWorkTypeID = work.WorkTypeID;
                w.WorkFaultDescription = String.IsNullOrWhiteSpace(work.FaultDescription) ? null : work.FaultDescription;
                w.WorkCauseDescription = String.IsNullOrWhiteSpace(work.CauseDescription) ? null : work.CauseDescription;
                unitOfWork.BeginTransaction();
                repository.Add(w);
                unitOfWork.CommitTransaction();
                return w.WorkID;     
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteWork(Int64 workID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Work> repository = new Repositor<Work>(unitOfWork);
            try
            { 
                unitOfWork.BeginTransaction();
                var w = repository.Single(c => c.WorkID == workID);
                if (w.Date.DateIsClosed)
                    throw new DateException("Směna je uzamčená, nelze odebírat ani editovat záznamy");
                repository.Delete(w);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateWork(Objects.Work work)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Work> repository = new Repositor<Work>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(work);
                unitOfWork.BeginTransaction();
                var w = repository.Single(c => c.WorkID == work.ID);
                if (w.Date.DateIsClosed)
                    throw new DateException("Směna je uzamčená, nelze odebírat ani editovat záznamy");
                w.WorkVehicleID = work.VehicleID;
                w.WorkWorkTypeID = work.WorkTypeID;
                w.WorkFaultDescription = String.IsNullOrWhiteSpace(work.FaultDescription) ? null : work.FaultDescription;
                w.WorkCauseDescription = String.IsNullOrWhiteSpace(work.CauseDescription) ? null : work.CauseDescription;
                repository.Update(w);
                unitOfWork.CommitTransaction();     
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.Work> FindWorks(Int64 dateID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Work> repository = new Repositor<Work>(unitOfWork);
            try
            {
                return repository.Find(c => c.WorkDateID == dateID).Select(c => new Objects.Work()
                {
                    ID = c.WorkID,
                    DateID = c.WorkDateID,
                    VehicleID = c.WorkVehicleID,
                    VehicleNumber = c.Vehicle.VehicleNumber,
                    WorkTypeID = c.WorkWorkTypeID,
                    WorkTypeName = c.WorkType.WorkTypeName,
                    FaultDescription = c.WorkFaultDescription,
                    CauseDescription = c.WorkCauseDescription 
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<string> GetTopFaultWorks(Int32 count, Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Work> repository = new Repositor<Work>(unitOfWork);
            try
            {
                return repository.Find(c=>c.Date.DateDetachmentID == detachmentID).
                    GroupBy(c => c.WorkFaultDescription).OrderByDescending(
                    c=>c.Count()).Select(c=>c.Key).Take(count).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<string> GetTopCauseWorks(Int32 count, Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Work> repository = new Repositor<Work>(unitOfWork);
            try
            {
                return repository.Find(c => c.Date.DateDetachmentID == detachmentID).
                    GroupBy(c => c.WorkCauseDescription).OrderByDescending(
                    c => c.Count()).Select(c => c.Key).Take(count).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Worker

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int32 AddWorker(Objects.WorkerDetail worker)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Worker> repository = new Repositor<Worker>(unitOfWork);
            IRepository<Tour> repositoryTour = new Repositor<Tour>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(worker);
                Worker w = new Worker();
                w.WorkerFirstName = String.IsNullOrWhiteSpace(worker.FirstName) ? null : worker.FirstName;
                w.WorkerLastName = String.IsNullOrWhiteSpace(worker.LastName) ? null : worker.LastName;
                w.WorkerServiceNumber = String.IsNullOrWhiteSpace(worker.ServiceNumber) ? null : worker.ServiceNumber;
                w.WorkerSapNumber = String.IsNullOrWhiteSpace(worker.SapNumber) ? null : worker.SapNumber;
                w.WorkerServiceEmail = String.IsNullOrWhiteSpace(worker.ServiceEmail) ? null : worker.ServiceEmail;
                w.WorkerPersonalEmail = String.IsNullOrWhiteSpace(worker.PersonalEmail) ? null : worker.PersonalEmail;
                w.WorkerServicePhone = String.IsNullOrWhiteSpace(worker.ServicePhone) ? null : worker.ServicePhone;
                w.WorkerPersonalPhone = String.IsNullOrWhiteSpace(worker.PersonalPhone) ? null : worker.PersonalPhone;
                w.WorkerDescription = String.IsNullOrWhiteSpace(worker.Description) ? null : worker.Description;
                w.WorkerPhoto = worker.Photo;
                w.WorkerDetachmentID = worker.DetachmentID;
                foreach (var item in worker.Tours)
                {
                    var t = repositoryTour.Single(c => c.TourID == item.ID);
                    w.Tours.Add(t);
                }
                unitOfWork.BeginTransaction();
                repository.Add(w);
                unitOfWork.CommitTransaction();
                return w.WorkerID;     
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteWorker(int workerID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Worker> repository = new Repositor<Worker>(unitOfWork);
            try
            {     
                unitOfWork.BeginTransaction();
                var w = repository.Single(c => c.WorkerID == workerID);
                repository.Delete(w);
                unitOfWork.CommitTransaction(); 
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateWorker(Objects.WorkerDetail worker)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Worker> repository = new Repositor<Worker>(unitOfWork);
            IRepository<Tour> repositoryTour = new Repositor<Tour>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(worker);
                unitOfWork.BeginTransaction();
                var w = repository.Single(c => c.WorkerID == worker.ID);
                w.WorkerServiceEmail = String.IsNullOrWhiteSpace(worker.ServiceEmail) ? null : worker.ServiceEmail;
                w.WorkerPersonalEmail = String.IsNullOrWhiteSpace(worker.PersonalEmail) ? null : worker.PersonalEmail;
                w.WorkerServicePhone = String.IsNullOrWhiteSpace(worker.ServicePhone) ? null : worker.ServicePhone;
                w.WorkerPersonalPhone = String.IsNullOrWhiteSpace(worker.PersonalPhone) ? null : worker.PersonalPhone;
                w.WorkerDescription = String.IsNullOrWhiteSpace(worker.Description) ? null : worker.Description;
                w.WorkerPhoto = worker.Photo;
                var oldIds = w.Tours.Select(c => c.TourID);
                var addTours = worker.Tours.Where(c => !oldIds.Contains(c.ID)).ToList();
                var newIds = worker.Tours.Select(c => c.ID);
                var removeTours = w.Tours.Where(c => !newIds.Contains(c.TourID)).ToList();
                foreach (var item in removeTours)
                {
                    var a = repositoryTour.Single(c => c.TourID == item.TourID);
                    w.Tours.Remove(a);
                }
                foreach (var item in addTours)
                {
                    var a = repositoryTour.Single(c => c.TourID == item.ID);    
                    w.Tours.Add(a);
                }    
                repository.Update(w);
                unitOfWork.CommitTransaction();             
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public Objects.WorkerDetail FindWorker(Int32 workerID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Worker> repository = new Repositor<Worker>(unitOfWork);
            try
            { 
                var w = repository.Single(c => c.WorkerID == workerID);
                Objects.WorkerDetail worker = new Objects.WorkerDetail();
                worker.ID = w.WorkerID;
                worker.FirstName = w.WorkerFirstName;
                worker.LastName = w.WorkerLastName;
                worker.ServiceNumber = w.WorkerServiceNumber;
                worker.SapNumber = w.WorkerSapNumber;
                worker.ServiceEmail = w.WorkerServiceEmail;
                worker.PersonalEmail = w.WorkerPersonalEmail;
                worker.ServicePhone = w.WorkerServicePhone;
                worker.PersonalPhone = w.WorkerPersonalPhone;
                worker.Description = w.WorkerDescription;
                worker.Photo = w.WorkerPhoto;
                worker.DetachmentID = w.WorkerDetachmentID;
                worker.Tours = w.Tours.Select(c => new Objects.Tour()
                {
                    ID = c.TourID,
                    Description = c.TourDescription,
                    EndTime = c.TourEndTime,
                    StartTime = c.TourStartTime
                });
                return worker;
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.Worker> FindWorkers(Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<Worker> repository = new Repositor<Worker>(unitOfWork);
            try
            {
                return repository.Find(c => c.WorkerDetachmentID == detachmentID).Select(c => new Objects.Worker()
                {
                    ID = c.WorkerID,
                    FirstName = c.WorkerFirstName,
                    LastName = c.WorkerLastName,
                    SapNumber = c.WorkerSapNumber,
                    ServicePhone = c.WorkerServicePhone,
                    DetachmentID = c.WorkerDetachmentID,
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Worker state

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int32 AddWorkerState(Objects.WorkerState workerState)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkerState> repository = new Repositor<WorkerState>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(workerState);
                WorkerState ws = new WorkerState();
                ws.WorkerStateName = String.IsNullOrWhiteSpace(workerState.Name) ? null : workerState.Name;
                ws.WorkerStateDescription = String.IsNullOrWhiteSpace(workerState.Description) ? null : workerState.Description;
                ws.WorkerStateDetachmentID = workerState.DetachmentID;
                unitOfWork.BeginTransaction();
                repository.Add(ws);
                unitOfWork.CommitTransaction();
                return ws.WorkerStateID;     
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteWorkerState(Int32 workerStateID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkerState> repository = new Repositor<WorkerState>(unitOfWork);
            try
            {        
                unitOfWork.BeginTransaction();
                var ws = repository.Single(c=> c.WorkerStateID == workerStateID);
                repository.Delete(ws);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateWorkerState(Objects.WorkerState workerState)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkerState> repository = new Repositor<WorkerState>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(workerState);             
                unitOfWork.BeginTransaction();
                var ws = repository.Single(c => c.WorkerStateID == workerState.ID);
                ws.WorkerStateDescription = String.IsNullOrWhiteSpace(workerState.Description) ? null : workerState.Description;
                repository.Update(ws);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.WorkerState> FindWorkerStates(Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkerState> repository = new Repositor<WorkerState>(unitOfWork);
            try
            {
                return repository.Find(c => c.WorkerStateDetachmentID == detachmentID).Select(c => new Objects.WorkerState()
                {
                    ID = c.WorkerStateID,
                    Name = c.WorkerStateName,
                    DetachmentID = c.WorkerStateDetachmentID,
                    Description = c.WorkerStateDescription
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion

        #region Work type

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public Int32 AddWorkType(Objects.WorkType workType)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkType> repository = new Repositor<WorkType>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(workType);
                WorkType wt = new WorkType();
                wt.WorkTypeName = String.IsNullOrWhiteSpace(workType.Name) ? null : workType.Name;
                wt.WorkTypeDescription = String.IsNullOrWhiteSpace(workType.Description) ? null : workType.Description;
                wt.WorkTypeDetachmentID = workType.DetachmentID;
                unitOfWork.BeginTransaction();
                repository.Add(wt);
                unitOfWork.CommitTransaction();
                return wt.WorkTypeID;            
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void DeleteWorkType(Int32 workTypeID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkType> repository = new Repositor<WorkType>(unitOfWork);
            try
            { 
                unitOfWork.BeginTransaction();
                var wt = repository.Single(c => c.WorkTypeID == workTypeID);
                repository.Delete(wt);
                unitOfWork.CommitTransaction();
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Write")]
        public void UpdateWorkType(Objects.WorkType workType)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkType> repository = new Repositor<WorkType>(unitOfWork);
            try
            {
                ObjectValidator.IsValid(workType);
                unitOfWork.BeginTransaction();
                var wt = repository.Single(c => c.WorkTypeID == workType.ID);
                wt.WorkTypeDescription = String.IsNullOrWhiteSpace(workType.Description) ? null : workType.Description;
                repository.Update(wt);
                unitOfWork.CommitTransaction();      
            }
            catch (Exception e)
            {
                unitOfWork.RollbackTransaction();
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        public List<Objects.WorkType> FindWorkTypes(Int32 detachmentID)
        {

            IUnitOfWork unitOfWork = SessionFactory.GetUnitOfWork;
            IRepository<WorkType> repository = new Repositor<WorkType>(unitOfWork);
            try
            {
                return repository.Find(c => c.WorkTypeDetachmentID == detachmentID).Select(c => new Objects.WorkType()
                {
                    ID = c.WorkTypeID,
                    Name = c.WorkTypeName,
                    DetachmentID = c.WorkTypeDetachmentID,
                    Description = c.WorkTypeDescription
                }).ToList();
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            }
        }

        #endregion
    }
}
