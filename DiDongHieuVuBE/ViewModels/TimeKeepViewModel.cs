using ManageEmployee.Entities;
using System;

namespace ManageEmployee.ViewModels
{
    public class TimeKeep : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TargetId { get; set; }
        public int TypeOfWork { get; set; }
        public int Type { get; set; }
        public int TimeKeepSymbolId { get; set; }
        public DateTime DateTimeKeep { get; set; }
        public int IsOverTime { get; set; } = 1;  // 1 - BT; 2-TC; 3-P; 4-KP

    }

    public class TimeKeepViewModel : PagingationRequestModel
    {
        public int? DepartmentId { get; set; }
        public DateTime DateTimeKeep { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TargetId { get; set; }
        public bool CheckCurrentUser { get; set; } = false;
    }
    public class TimeKeepMapping
    {
        public class GetList : BaseEntity
        {
            public string FullName { get; set; }
            public bool isDelete { get; set; }
            public string Code { get; set; }
            public string TargetName { get; set; }
            public int TargetId { get; set; }
            public int TypeOfWork { get; set; }
            public int UserId { get; set; }
            public string DepartmentName { get; set; }
            public int Id { get; set; }
            public int Type { get; set; }
            public int TimeKeepSymbolId { get; set; }
            public int TimeKeepId { get; set; }
            public string Timekeep { get; set; }
            public DateTime? DateTimeKeep { get; set; }
            public string TargetCode { get; set; }
        }

        public class Report
        {
            public string FullName { get; set; }
            public string Code { get; set; }
            public int UserId { get; set; }
            public string DepartmentName { get; set; }
            public List<TimeKeepHistoryMapping.GetForReport> Histories { get; set; }
            public int SymbolId { get; set; }

            public int TotalWorkingDay
            {
                get
                {
                    if (Histories == null || Histories.Count <= 0)
                        return 0;
                    return Histories.GroupBy(x => x.DateTimeKeep.Day)
                        .Count();
                }
            }
            public int TotalPaidLeave
            {
                get
                {
                    if (Histories == null || Histories.Count <= 0)
                        return 0;
                    return Histories.Where(x => x.IsOverTime == 3).GroupBy(x => x.DateTimeKeep.Day)
                        .Count();
                }
            }
            public int TotalUnPaidLeave
            {
                get
                {
                    if (Histories == null || Histories.Count <= 0)
                        return 0;
                    return Histories.Where(x => x.IsOverTime == 4).GroupBy(x => x.DateTimeKeep.Day)
                        .Count();
                }
            }
            public double TotalPaid
            {
                get
                {
                    return TotalPaidLeave + TotalWorkingDay;
                }
            }

            public double TotalWorkingHours
            {
                get
                {
                    if (Histories == null || Histories.Count <= 0)
                        return 0;
                    return Histories
                        .Where(x => x.IsOverTime == 1)
                        .Sum(x => x.TimeKeepSymbolTimeTotal);
                }
            }

            public double TotalOverTimeWorkingHours
            {
                get
                {
                    if (Histories == null || Histories.Count <= 0)
                        return 0;
                    return Histories
                        .Where(x => x.IsOverTime == 2) // 1 - BT; 2-TC; 3-P; 4-KP
                        .Sum(x => x.TimeKeepSymbolTimeTotal);
                }
            }
        }



        public class GetListReport : BaseEntity
        {
            public string FullName { get; set; }
            public string Code { get; set; }
            public string DepartmentName { get; set; }
            public int UserId { get; set; }
            public List<TimeKeep> TypeKeepList { get; set; }
        }
        public class GetSelectList
        {
            public string FullName { get; set; }
            public string Code { get; set; }
            public int Id { get; set; }
            public string UserId { get; set; }
            public string Type { get; set; }
            public string Timekeep { get; set; }
            public DateTime DateTimeKeep { get; set; }

        }

    }


    public class TimeKeepHistoryMapping
    {
        public class GetForReport
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            //public int TimekeepId { get; set; }
            public DateTime? TimeIn { get; set; }
            public DateTime? TimeOut { get; set; }
            public int CheckInMethod { get; set; } // 0 -> Manual, 1 -> Automatic
            public int TimeKeepSymbolId { get; set; }
            public string TimeKeepSymbolCode { get; set; }
            public double TimeKeepSymbolTimeTotal { get; set; }
            public string TimeKeepSymbolName { get; set; }
            public DateTime DateTimeKeep { get; set; }
            public int IsOverTime { get; set; }  // 1 - BT; 2-TC; 3-P; 4-KP
            public int TargetId { get; set; }
            public string TargetCode { get; set; }
            public string TargetName { get; set; }
        }
        public class GetHistory
        {
            public int Id { get; set; }
            public int TimekeepId { get; set; }
            public DateTime TimeIn { get; set; }
            public DateTime TimeOut { get; set; }
            public int Type { get; set; }
            public int TimeKeepSymbolId { get; set; }
            public string TargetName { get; set; }
            public DateTime DateTimeKeep { get; set; }
            public string FullName { get; set; }
            public string Code { get; set; }
            public int TypeOfWork { get; set; }
        }
    }


}