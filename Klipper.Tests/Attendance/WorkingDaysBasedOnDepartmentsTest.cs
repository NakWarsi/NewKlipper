﻿using DomainModel;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;
using UseCases;

namespace Klipper.Tests
{
    public class WorkingDaysBasedOnDepartmentsTest
    {
        private IAccessEventsRepository accessEventsContainer;
        private IEmployeeRepository employeeContainer;
        private IDepartmentRepository departmentContainer;
        private IAttendanceRegularizationRepository regularizationData;
        private ILeavesRepository leaveData;

        [SetUp]
        public void setup()
        {
            accessEventsContainer = Substitute.For<IAccessEventsRepository>();
            employeeContainer = Substitute.For<IEmployeeRepository>();
            departmentContainer = Substitute.For<IDepartmentRepository>();
            regularizationData = Substitute.For<IAttendanceRegularizationRepository>();
            leaveData = Substitute.For<ILeavesRepository>();

            departmentContainer.GetDepartment(Departments.Software).Returns(
                new Department(Departments.Software));
            departmentContainer.GetDepartment(Departments.Design).Returns(
                new Department(Departments.Design));
            departmentContainer.GetDepartment(Departments.Service).Returns(
                new Department(Departments.Service));
            regularizationData.GetRegularizedRecords(666).Returns(new List<Regularization>());
        }

        [Test]
        public void GivenNonworkingDayShouldCalculateAccurateWorkingHours()
        {
            //SETUP
            AttendanceService attendanceService = new AttendanceService
                (accessEventsContainer, employeeContainer, departmentContainer, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            accessEventsContainer
                .GetAccessEventsForDateRange(666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithID(666)
                .WithDepartment(Departments.Software)
                .BuildEmployee();
            employeeContainer.GetEmployee(666).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            var expectedData = new PerDayAttendanceRecordDTO()
            {
                Date = DateTime.Parse("2019/02/02"),
                TimeIn = new Time(12, 10),
                TimeOut = new Time(16, 11),
                OverTime = new Time(4, 1),
                LateBy = new Time(0, 0),
                WorkingHours = new Time(4, 1),
                DayStatus = DayStatus.NonWorkingDay
            };

            //EXECUTE TEST CASES
            var dactualData = attendanceService
                .AttendanceReportForDateRange(666,DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            var dactualDataForADay = dactualData.ListOfAttendanceRecordDTO.First();

            //ASSERT
            Assert.AreEqual(expectedData.WorkingHours.Hour, dactualDataForADay.WorkingHours.Hour);
            Assert.AreEqual(expectedData.WorkingHours.Minute, dactualDataForADay.WorkingHours.Minute);
        }

        [Test]
        public void GivenNonworkingDayShouldCalculateAccurateOverTime()
        {
            //SETUP
            AttendanceService attendanceService = new AttendanceService
                (accessEventsContainer, employeeContainer, departmentContainer, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            accessEventsContainer
                .GetAccessEventsForDateRange(666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithID(666)
                .WithDepartment(Departments.Software)
                .BuildEmployee();
            employeeContainer.GetEmployee(666).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            var expectedData = new PerDayAttendanceRecordDTO()
            {
                Date = DateTime.Parse("2019/02/02"),
                TimeIn = new Time(12, 10),
                TimeOut = new Time(16, 11),
                OverTime = new Time(4, 1),
                LateBy = new Time(0, 0),
                WorkingHours = new Time(4, 1),
                DayStatus = DayStatus.NonWorkingDay
            };

            //EXECUTE TEST CASES
            var dactualData = attendanceService.
                AttendanceReportForDateRange
                (666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            var dactualDataForADay = dactualData.ListOfAttendanceRecordDTO.First();

            //ASSERT
            Assert.AreEqual(dactualDataForADay.OverTime.Hour, expectedData.OverTime.Hour);
            Assert.AreEqual(dactualDataForADay.OverTime.Minute, expectedData.OverTime.Minute);
        }

        [Test]
        public void GivenNonworkingDayShouldCalculateLateByAsZero()
        {
            //SETUP
            AttendanceService attendanceService = new AttendanceService
                (accessEventsContainer, employeeContainer, departmentContainer, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            accessEventsContainer
                .GetAccessEventsForDateRange(666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithID(666)
                .WithDepartment(Departments.Software)
                .BuildEmployee();
            employeeContainer.GetEmployee(666).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            //EXECUTE TEST CASES
            var dactualData = attendanceService
                .AttendanceReportForDateRange(666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            var dactualDataForADay = dactualData.ListOfAttendanceRecordDTO.First();

            //ASSERT
            Assert.AreEqual(dactualDataForADay.LateBy.Hour, 0);
            Assert.AreEqual(dactualDataForADay.LateBy.Minute, 0);
        }

        [Test]
        public void GivenWorkedDayIsNonWorkingDayForSoftwareDepartment()
        {
            //SETUP
            AttendanceService attendanceService = new AttendanceService
                (accessEventsContainer, employeeContainer, departmentContainer, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            accessEventsContainer
                .GetAccessEventsForDateRange(666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithID(666)
                .WithDepartment(Departments.Software)
                .BuildEmployee();
            employeeContainer.GetEmployee(666).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            //EXECUTE TEST CASES
            var dactualData = attendanceService
                .AttendanceReportForDateRange(666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            var dactualDataForADay = dactualData.ListOfAttendanceRecordDTO.FirstOrDefault();

            //ASSERT
            Assert.AreEqual(dactualDataForADay.DayStatus, DayStatus.NonWorkingDay);
        }

        [Test]
        public void GivenWorkedDayIsNonWorkingDayForDesignDepartment()
        {
            //SETUP
            AttendanceService attendanceService = new AttendanceService
                (accessEventsContainer, employeeContainer, departmentContainer, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            accessEventsContainer
                .GetAccessEventsForDateRange(666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithID(666)
                .WithDepartment(Departments.Design)
                .BuildEmployee();
            employeeContainer.GetEmployee(666).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            //EXECUTE TEST CASES
            var dactualData = attendanceService.AttendanceReportForDateRange
                (666, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            var dactualDataForADay = dactualData.ListOfAttendanceRecordDTO.FirstOrDefault();

            //ASSERT
            Assert.AreEqual(dactualDataForADay.DayStatus, DayStatus.NonWorkingDay);
        }


        [Test]
        public void GivenWorked1stSaturdayIsWorkingDayForServiceDepartment()
        {
            //SETUP
            AttendanceService attendanceService = new AttendanceService
                (accessEventsContainer, employeeContainer, departmentContainer, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            accessEventsContainer
                .GetAccessEventsForDateRange(77, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithID(666)
                .WithDepartment(Departments.Service)
                .BuildEmployee();
            employeeContainer.GetEmployee(77).Returns(dummyEmployee);
            regularizationData.GetRegularizedRecords(77).Returns(new List<Regularization>());

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            //EXECUTE TEST CASES
            var dactualData = attendanceService.AttendanceReportForDateRange
                (77, DateTime.Parse("2019/02/02"), DateTime.Parse("2019/02/02"));
            var dactualDataForADay = dactualData.ListOfAttendanceRecordDTO.FirstOrDefault();

            //ASSERT
            Assert.AreEqual(dactualDataForADay.DayStatus, DayStatus.WorkingDay);
        }


        [Test]
        public void GivenWorked2ndSaturdayIsNonWorkingDayForServiceDepartment()
        {
            //SETUP
            AttendanceService attendanceService = new AttendanceService
                (accessEventsContainer, employeeContainer, departmentContainer, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2019/02/09"), DateTime.Parse("2019/02/09"));
            accessEventsContainer
                .GetAccessEventsForDateRange(77, DateTime.Parse("2019/02/09"), DateTime.Parse("2019/02/09")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithID(666)
                .WithDepartment(Departments.Service)
                .BuildEmployee();
            employeeContainer.GetEmployee(77).Returns(dummyEmployee);
            regularizationData.GetRegularizedRecords(77).Returns(new List<Regularization>());

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            //EXECUTE TEST CASES
            var dactualData = attendanceService
                .AttendanceReportForDateRange(77, DateTime.Parse("2019/02/09"), DateTime.Parse("2019/02/09"));
            var dactualDataForADay = dactualData.ListOfAttendanceRecordDTO.FirstOrDefault();

            //ASSERT
            Assert.AreEqual(dactualDataForADay.DayStatus, DayStatus.NonWorkingDay);
        }
    }
}