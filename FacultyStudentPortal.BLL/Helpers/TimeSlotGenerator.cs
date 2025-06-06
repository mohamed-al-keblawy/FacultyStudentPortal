using FacultyStudentPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.BLL.Helpers
{
    public static class TimeSlotGenerator
    {
        public static List<TimeSlot> GenerateLectureSlots()
        {
            var days = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
            var slots = new List<TimeSlot>();

            foreach (var day in days)
            {
                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(9, 0, 0)
                });
                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(9, 15, 0),
                    EndTime = new TimeSpan(10, 15, 0)
                });
                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(10, 30, 0),
                    EndTime = new TimeSpan(11, 30, 0)
                });
                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(11, 45, 0),
                    EndTime = new TimeSpan(12, 45, 0)
                });

                // For 1.5-hour lectures
                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(14, 30, 0)
                });
                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(14, 45, 0),
                    EndTime = new TimeSpan(16, 15, 0)
                });
            }

            return slots;
        }

        public static List<TimeSlot> GenerateLabSlots()
        {
            var days = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
            var slots = new List<TimeSlot>();

            foreach (var day in days)
            {
                // 2-hour continuous blocks only
                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0)
                });

                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(10, 15, 0),
                    EndTime = new TimeSpan(12, 15, 0)
                });

                slots.Add(new TimeSlot
                {
                    DayOfWeek = day,
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0)
                });
            }

            return slots;
        }
    }

}
