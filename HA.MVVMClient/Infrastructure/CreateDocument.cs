using HA.MVVMClient.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace HA.MVVMClient.Infrastructure
{
    public class CreateDocument
    {
        private static Size pagesize;
        private static Size measuresize;
        private static double avalableheight;
        private static double measuredheight;
        private static FixedDocument document;


        #region Date

        private static void CreateDate(Grid pagegrid, Date date)
        {
            pagegrid.RowDefinitions.Add(new RowDefinition());
            TextBlock datebloc = new TextBlock();
            datebloc.Text = "Směna : " + date.DateContent.ToString("dd.MMMM.yyyy") + (date.IsNight ? " Noční" : " Denní");
            datebloc.Margin = new Thickness(0, 15, 0, 15);
            datebloc.HorizontalAlignment = HorizontalAlignment.Center;
            datebloc.FontWeight = FontWeights.Bold;
            datebloc.FontSize = 15;
            datebloc.SetValue(Grid.RowProperty, 0);
            pagegrid.Children.Add(datebloc);

            pagegrid.Measure(measuresize);
            measuredheight = pagegrid.DesiredSize.Height;
            avalableheight = pagesize.Height - pagegrid.DesiredSize.Height;
        }

        #endregion

        #region Attendance

        private static Grid CreateAttendanceTable(Grid pagegrid)
        {
            pagegrid.RowDefinitions.Add(new RowDefinition());
            pagegrid.RowDefinitions.Add(new RowDefinition());

            TextBlock attendancesbloc = new TextBlock();
            attendancesbloc.Text = "Docházka";
            attendancesbloc.Margin = new Thickness(0, 15, 0, 5);
            attendancesbloc.FontWeight = FontWeights.Bold;
            attendancesbloc.FontSize = 15;
            attendancesbloc.SetValue(Grid.RowProperty, 1);
            pagegrid.Children.Add(attendancesbloc);

            Grid grid = new Grid();
            grid.Width = pagesize.Width;
            ColumnDefinition firstname = new ColumnDefinition();
            ColumnDefinition lastname = new ColumnDefinition();
            ColumnDefinition tour = new ColumnDefinition();
            ColumnDefinition state = new ColumnDefinition();
            ColumnDefinition description = new ColumnDefinition();
            firstname.Width = GridLength.Auto;
            lastname.Width = GridLength.Auto;
            tour.Width = GridLength.Auto;
            state.Width = GridLength.Auto;
            description.Width = new GridLength(1, GridUnitType.Star);

            grid.ColumnDefinitions.Add(firstname);
            grid.ColumnDefinitions.Add(lastname);
            grid.ColumnDefinitions.Add(tour);
            grid.ColumnDefinitions.Add(state);
            grid.ColumnDefinitions.Add(description);

            grid.SetValue(Grid.RowProperty, 2);
            pagegrid.Children.Add(grid);
            CreateAttendanceHeader(grid);
            pagegrid.Measure(measuresize);
            avalableheight -= (pagegrid.DesiredSize.Height - measuredheight);
            return grid;
        }

        private static void CreateAttendanceHeader(Grid grid)
        {
            RowDefinition row = new RowDefinition();
            TextBlock firstnamebloc = new TextBlock();
            firstnamebloc.Text = "Jméno";
            firstnamebloc.FontWeight = FontWeights.Bold;
            TextBlock lastnamebloc = new TextBlock();
            lastnamebloc.Text = "Příjmení";
            lastnamebloc.FontWeight = FontWeights.Bold;
            TextBlock tourbloc = new TextBlock();
            tourbloc.Text = "Turnus";
            tourbloc.FontWeight = FontWeights.Bold;
            TextBlock statebloc = new TextBlock();
            statebloc.Text = "Stav";
            statebloc.FontWeight = FontWeights.Bold;
            TextBlock descriptionbloc = new TextBlock();
            descriptionbloc.Text = "Dodatečná informace";
            descriptionbloc.FontWeight = FontWeights.Bold;

            firstnamebloc.Margin = new Thickness(0, 5, 0, 0);
            lastnamebloc.Margin = new Thickness(5, 5, 0, 0);
            tourbloc.Margin = new Thickness(5, 5, 0, 0);
            statebloc.Margin = new Thickness(5, 5, 0, 0);
            descriptionbloc.Margin = new Thickness(5, 5, 0, 0);

            firstnamebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            firstnamebloc.SetValue(Grid.ColumnProperty, 0);
            lastnamebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            lastnamebloc.SetValue(Grid.ColumnProperty, 1);
            tourbloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            tourbloc.SetValue(Grid.ColumnProperty, 2);
            statebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            statebloc.SetValue(Grid.ColumnProperty, 3);
            descriptionbloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            descriptionbloc.SetValue(Grid.ColumnProperty, 4);

            grid.Children.Add(firstnamebloc);
            grid.Children.Add(lastnamebloc);
            grid.Children.Add(tourbloc);
            grid.Children.Add(statebloc);
            grid.Children.Add(descriptionbloc);
            grid.RowDefinitions.Add(row);
        }

        private static void AddAttendanceRow(Grid grid, Attendance attendance)
        {
            RowDefinition row = new RowDefinition();
            TextBlock firstnamebloc = new TextBlock();
            firstnamebloc.Text = attendance.FirstName;
            TextBlock lastnamebloc = new TextBlock();
            lastnamebloc.Text = attendance.LastName;
            TextBlock tourbloc = new TextBlock();
            tourbloc.Text = attendance.WorkerTour;
            TextBlock statebloc = new TextBlock();
            statebloc.Text = attendance.WorkerState;
            TextBlock descriptionbloc = new TextBlock();
            descriptionbloc.TextWrapping = TextWrapping.Wrap;
            descriptionbloc.Text = attendance.Description;

            firstnamebloc.Margin = new Thickness(0, 5, 0, 0);
            lastnamebloc.Margin = new Thickness(5, 5, 0, 0);
            tourbloc.Margin = new Thickness(5, 5, 0, 0);
            statebloc.Margin = new Thickness(5, 5, 0, 0);
            descriptionbloc.Margin = new Thickness(5, 5, 0, 0);

            firstnamebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            firstnamebloc.SetValue(Grid.ColumnProperty, 0);
            lastnamebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            lastnamebloc.SetValue(Grid.ColumnProperty, 1);
            tourbloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            tourbloc.SetValue(Grid.ColumnProperty, 2);
            statebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            statebloc.SetValue(Grid.ColumnProperty, 3);
            descriptionbloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            descriptionbloc.SetValue(Grid.ColumnProperty, 4);

            grid.Children.Add(firstnamebloc);
            grid.Children.Add(lastnamebloc);
            grid.Children.Add(tourbloc);
            grid.Children.Add(statebloc);
            grid.Children.Add(descriptionbloc);
            grid.RowDefinitions.Add(row);
        }

        private static void RemoveAttendanceRow(Grid grid)
        {
            grid.RowDefinitions.Remove(grid.RowDefinitions.Last());
            grid.Children.RemoveRange(grid.Children.Count - 5, 5);
        }

        #endregion

        #region Work

        private static Grid CreateWorkTable(Grid pagegrid)
        {
            pagegrid.RowDefinitions.Add(new RowDefinition());
            pagegrid.RowDefinitions.Add(new RowDefinition());
            TextBlock worksbloc = new TextBlock();
            worksbloc.Text = "Práce";
            worksbloc.Margin = new Thickness(0, 15, 0, 5);
            worksbloc.FontWeight = FontWeights.Bold;
            worksbloc.FontSize = 15;
            worksbloc.SetValue(Grid.RowProperty, pagegrid.RowDefinitions.Count == 3 ? 1 : 3);
            pagegrid.Children.Add(worksbloc);
            Grid grid = new Grid();
            grid.Width = pagesize.Width;
            ColumnDefinition vehicle = new ColumnDefinition();
            ColumnDefinition worktype = new ColumnDefinition();
            ColumnDefinition workfault = new ColumnDefinition();
            ColumnDefinition workcause = new ColumnDefinition();
            vehicle.Width = GridLength.Auto;
            worktype.Width = GridLength.Auto;
            workfault.Width = new GridLength(1, GridUnitType.Star);
            workcause.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(vehicle);
            grid.ColumnDefinitions.Add(worktype);
            grid.ColumnDefinitions.Add(workfault);
            grid.ColumnDefinitions.Add(workcause);
            grid.SetValue(Grid.RowProperty, pagegrid.RowDefinitions.Count == 3 ? 2 : 4);
            pagegrid.Children.Add(grid);
            CreateWorkHeader(grid);
            pagegrid.Measure(measuresize);
            avalableheight -= (pagegrid.DesiredSize.Height - measuredheight);
            return grid;
        }

        private static void CreateWorkHeader(Grid grid)
        {
            RowDefinition row = new RowDefinition();
            TextBlock vehiclebloc = new TextBlock();
            vehiclebloc.Text = "Vozidlo";
            vehiclebloc.FontWeight = FontWeights.Bold;
            TextBlock worktypebloc = new TextBlock();
            worktypebloc.Text = "Typ opravy";
            worktypebloc.FontWeight = FontWeights.Bold;
            TextBlock workfaultbloc = new TextBlock();
            workfaultbloc.Text = "Popis závady";
            workfaultbloc.FontWeight = FontWeights.Bold;
            TextBlock workcausebloc = new TextBlock();
            workcausebloc.Text = "Způsob opravy";
            workcausebloc.FontWeight = FontWeights.Bold;

            vehiclebloc.Margin = new Thickness(0, 10, 0, 0);
            worktypebloc.Margin = new Thickness(10, 10, 0, 0);
            workfaultbloc.Margin = new Thickness(10, 10, 0, 0);
            workcausebloc.Margin = new Thickness(10, 10, 0, 0);

            vehiclebloc.SetValue(Grid.RowProperty, 0);
            vehiclebloc.SetValue(Grid.ColumnProperty, 0);
            worktypebloc.SetValue(Grid.RowProperty, 0);
            worktypebloc.SetValue(Grid.ColumnProperty, 1);
            workfaultbloc.SetValue(Grid.RowProperty, 0);
            workfaultbloc.SetValue(Grid.ColumnProperty, 2);
            workcausebloc.SetValue(Grid.RowProperty, 0);
            workcausebloc.SetValue(Grid.ColumnProperty, 3);
            grid.Children.Add(vehiclebloc);
            grid.Children.Add(worktypebloc);
            grid.Children.Add(workfaultbloc);
            grid.Children.Add(workcausebloc);
            grid.RowDefinitions.Add(row);
        }

        private static void AddWorkRow(Grid grid, Work work)
        {
            RowDefinition row = new RowDefinition();
            TextBlock vehiclebloc = new TextBlock();
            vehiclebloc.Text = work.VehicleNumber;
            TextBlock worktypebloc = new TextBlock();
            worktypebloc.Text = work.WorkTypeName;
            TextBlock workfaultbloc = new TextBlock();
            workfaultbloc.TextWrapping = TextWrapping.Wrap;
            workfaultbloc.Text = work.FaultDescription;
            TextBlock workcausebloc = new TextBlock();
            workcausebloc.TextWrapping = TextWrapping.Wrap;
            workcausebloc.Text = work.CauseDescription;

            vehiclebloc.Margin = new Thickness(0, 5, 0, 0);
            worktypebloc.Margin = new Thickness(5, 5, 0, 0);
            workfaultbloc.Margin = new Thickness(5, 5, 0, 0);
            workcausebloc.Margin = new Thickness(5, 5, 0, 0);

            vehiclebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            vehiclebloc.SetValue(Grid.ColumnProperty, 0);
            worktypebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            worktypebloc.SetValue(Grid.ColumnProperty, 1);
            workfaultbloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            workfaultbloc.SetValue(Grid.ColumnProperty, 2);
            workcausebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            workcausebloc.SetValue(Grid.ColumnProperty, 3);
            grid.Children.Add(vehiclebloc);
            grid.Children.Add(worktypebloc);
            grid.Children.Add(workfaultbloc);
            grid.Children.Add(workcausebloc);
            grid.RowDefinitions.Add(row);
        }

        private static void RemoveWorkRow(Grid grid)
        {
            grid.RowDefinitions.Remove(grid.RowDefinitions.Last());
            grid.Children.RemoveRange(grid.Children.Count - 4, 4);
        }

        #endregion


        #region Changeover

        private static Grid CreateChangeoverTable(Grid pagegrid)
        {
            pagegrid.RowDefinitions.Add(new RowDefinition());
            pagegrid.RowDefinitions.Add(new RowDefinition());
            TextBlock worksbloc = new TextBlock();
            worksbloc.Text = "Předávka";
            worksbloc.Margin = new Thickness(0, 15, 0, 5);
            worksbloc.FontWeight = FontWeights.Bold;
            worksbloc.FontSize = 15;
            worksbloc.SetValue(Grid.RowProperty, pagegrid.RowDefinitions.Count == 3 ? 1 : (pagegrid.RowDefinitions.Count == 5 ? 3 : 5));
            pagegrid.Children.Add(worksbloc);
            Grid grid = new Grid();
            grid.Width = pagesize.Width;
            ColumnDefinition date = new ColumnDefinition();
            ColumnDefinition vehicle = new ColumnDefinition();
            ColumnDefinition worktype = new ColumnDefinition();
            ColumnDefinition description = new ColumnDefinition();
            date.Width = GridLength.Auto;
            vehicle.Width = GridLength.Auto;
            worktype.Width = GridLength.Auto;
            description.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(date);
            grid.ColumnDefinitions.Add(vehicle);
            grid.ColumnDefinitions.Add(worktype);
            grid.ColumnDefinitions.Add(description);
            grid.SetValue(Grid.RowProperty, pagegrid.RowDefinitions.Count == 3 ? 2 : (pagegrid.RowDefinitions.Count == 5 ? 4 : 6));
            pagegrid.Children.Add(grid);
            CreateChangeoverHeader(grid);
            pagegrid.Measure(measuresize);
            avalableheight -= (pagegrid.DesiredSize.Height - measuredheight);
            return grid;
        }

        private static void CreateChangeoverHeader(Grid grid)
        {
            RowDefinition row = new RowDefinition();
            TextBlock datebloc = new TextBlock();
            datebloc.Text = "Datum založení";
            datebloc.FontWeight = FontWeights.Bold;
            TextBlock vehiclebloc = new TextBlock();
            vehiclebloc.Text = "Vozidlo";
            vehiclebloc.FontWeight = FontWeights.Bold;
            TextBlock worktypebloc = new TextBlock();
            worktypebloc.Text = "Typ opravy";
            worktypebloc.FontWeight = FontWeights.Bold;
            TextBlock descriptionbloc = new TextBlock();
            descriptionbloc.Text = "Popis závady";
            descriptionbloc.FontWeight = FontWeights.Bold;
            datebloc.Margin = new Thickness(0, 10, 0, 0);
            vehiclebloc.Margin = new Thickness(10, 10, 0, 0);
            worktypebloc.Margin = new Thickness(10, 10, 0, 0);  
            descriptionbloc.Margin = new Thickness(10, 10, 0, 0);
            datebloc.SetValue(Grid.RowProperty, 0);
            datebloc.SetValue(Grid.ColumnProperty, 0);
            vehiclebloc.SetValue(Grid.RowProperty, 0);
            vehiclebloc.SetValue(Grid.ColumnProperty, 1);
            worktypebloc.SetValue(Grid.RowProperty, 0);
            worktypebloc.SetValue(Grid.ColumnProperty, 2);
            descriptionbloc.SetValue(Grid.RowProperty, 0);
            descriptionbloc.SetValue(Grid.ColumnProperty, 3);
            grid.Children.Add(datebloc);
            grid.Children.Add(vehiclebloc);
            grid.Children.Add(worktypebloc);
            grid.Children.Add(descriptionbloc);
            grid.RowDefinitions.Add(row);
        }

        private static void AddChangeoverRow(Grid grid, Changeover changeover)
        {
            RowDefinition row = new RowDefinition();
            TextBlock datebloc = new TextBlock();
            datebloc.Text = changeover.DateContent.ToString("dd.MM.yyyy") + (changeover.IsNight ? " Noční" : " Denní");
            TextBlock vehiclebloc = new TextBlock();
            vehiclebloc.Text = changeover.VehicleNumber;
            TextBlock worktypebloc = new TextBlock();
            worktypebloc.Text = changeover.WorkTypeName;
            TextBlock descriptionbloc = new TextBlock();
            descriptionbloc.TextWrapping = TextWrapping.Wrap;
            descriptionbloc.Text = changeover.Description;

            datebloc.Margin = new Thickness(0, 10, 0, 0);
            vehiclebloc.Margin = new Thickness(10, 10, 0, 0);
            worktypebloc.Margin = new Thickness(10, 10, 0, 0);
            descriptionbloc.Margin = new Thickness(10, 10, 0, 0);

            datebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            datebloc.SetValue(Grid.ColumnProperty, 0);
            vehiclebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            vehiclebloc.SetValue(Grid.ColumnProperty, 1);
            worktypebloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            worktypebloc.SetValue(Grid.ColumnProperty, 2);
            descriptionbloc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count);
            descriptionbloc.SetValue(Grid.ColumnProperty, 3);
            grid.Children.Add(datebloc);
            grid.Children.Add(vehiclebloc);
            grid.Children.Add(worktypebloc);
            grid.Children.Add(descriptionbloc);   
            grid.RowDefinitions.Add(row);
        }

        private static void RemoveChangeoverRow(Grid grid)
        {
            grid.RowDefinitions.Remove(grid.RowDefinitions.Last());
            grid.Children.RemoveRange(grid.Children.Count - 4, 4);
        }

        #endregion


        #region Global

        private static void RemoveTable(Grid pagegrid)
        {
            pagegrid.Children.RemoveRange(pagegrid.Children.Count - 2, 2);
        }

        private static Grid CreatePage()
        {
            FixedPage page = new FixedPage();
            PageContent content = new PageContent();
            Grid pagegrid = new Grid();
            avalableheight = 0;
            measuredheight = 0;
            page.Height = document.DocumentPaginator.PageSize.Height;
            page.Width = document.DocumentPaginator.PageSize.Width;
            page.Margin = new Thickness(30);
            measuresize = new Size(page.Width - page.Margin.Left - page.Margin.Right, 1000000);
            pagesize = new Size(page.Width - page.Margin.Left - page.Margin.Right, page.Height - page.Margin.Top - page.Margin.Bottom);
            page.Children.Add(pagegrid);
            ((IAddChild)content).AddChild(page);
            document.Pages.Add(content);
            return pagegrid;
        }

        #endregion

        private static void InsertChangeovers(Grid grid, List<Changeover> changeovers)
        {
            while (changeovers.Count != 0)
            {
                AddChangeoverRow(grid, changeovers[0]);
                grid.Measure(measuresize);
                if (0 > avalableheight - grid.DesiredSize.Height)
                {
                    RemoveChangeoverRow(grid);
                    grid.Measure(measuresize);
                    break;
                }
                changeovers.RemoveAt(0);
            }
        }

        private static void InsertWorks(Grid grid, List<Work> works)
        {
            while (works.Count != 0)
            {
                AddWorkRow(grid, works[0]);
                grid.Measure(measuresize);
                if (0 > avalableheight - grid.DesiredSize.Height)
                {
                    RemoveWorkRow(grid);
                    grid.Measure(measuresize);
                    break;
                }
                works.RemoveAt(0);
            }
        }

        private static void InsertAttendances(Grid grid, List<Attendance> attendances)
        {
            while (attendances.Count != 0)
            {
                AddAttendanceRow(grid, attendances[0]);
                grid.Measure(measuresize);
                if (0 > avalableheight - grid.DesiredSize.Height)
                {
                    RemoveAttendanceRow(grid);
                    grid.Measure(measuresize);
                    break;
                }
                attendances.RemoveAt(0);
            }
        }


        public static FixedDocument Create(Date date, List<Work> works, List<Attendance> attendances, List<Changeover> changeovers)
        {     
            Grid pagegrid = null;
            Grid grid = null;
            document = new FixedDocument();

            if (attendances.Count != 0)
            {

                while (attendances.Count != 0)
                {
                    pagegrid = CreatePage();
                    CreateDate(pagegrid, date);
                    grid = CreateAttendanceTable(pagegrid);
                    InsertAttendances(grid, attendances);
                }
                grid.Measure(measuresize);
                pagegrid.Measure(measuresize);
                avalableheight -= grid.DesiredSize.Height;
                measuredheight = pagegrid.DesiredSize.Height;
            }
            else
            {
                if (works.Count != 0 || changeovers.Count != 0)
                {
                    pagegrid = CreatePage();
                    CreateDate(pagegrid, date);
                }
            }

            while (works.Count != 0)
            {
                grid = CreateWorkTable(pagegrid);
                InsertWorks(grid, works);
                if (works.Count != 0)
                {
                    pagegrid = CreatePage();
                    CreateDate(pagegrid, date);
                }
            }


            while (changeovers.Count != 0)
            {
                grid = CreateChangeoverTable(pagegrid);
                InsertChangeovers(grid, changeovers);
                if (changeovers.Count != 0)
                {
                    pagegrid = CreatePage();
                    CreateDate(pagegrid, date);
                }
            }

            return document;
        }
    }
}
