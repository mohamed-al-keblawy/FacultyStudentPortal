using FacultyStudentPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.BLL.Services
{
    public class SectionScheduler
    {
        private List<Course> courses;
        private List<User> facultyMembers;
        private List<Room> rooms;
        private List<TimeSlot> timeSlots;
        private readonly List<TimeSlot> _labSlots;


        private List<Section> scheduledSections = new();

        public SectionScheduler(List<Course> courses, List<User> faculty, List<Room> rooms, List<TimeSlot> slots, List<TimeSlot> labSlots)
        {
            this.courses = courses;
            this.facultyMembers = faculty;
            this.rooms = rooms;
            this.timeSlots = slots;
            this._labSlots = labSlots;
        }

        public List<Section> GenerateSchedule()
        {
            foreach (var course in courses.OrderByDescending(c => c.EligibleStudents))
            {
                AllocateSections(course, isLab: false);
                if (course.LabHour > 0)
                    AllocateSections(course, isLab: true);
            }

            return scheduledSections;
        }

        private void AllocateSections(Course course, bool isLab)
        {
            string sectionType = isLab ? "Lab" : "Lecture";
            int hours = isLab ? course.LabHour : course.CreditHours;
            var eligibleRooms = rooms
                .Where(r => r.RoomType.Equals(sectionType, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.Capacity)
                .ToList();

            int studentsRemaining = course.EligibleStudents;

            foreach (var room in eligibleRooms)
            {
                int studentsPerSection = room.Capacity;
                int neededSections = (int)Math.Ceiling((double)studentsRemaining / studentsPerSection);

                for (int i = 0; i < neededSections; i++)
                {
                    foreach (var slot in timeSlots)
                    {
                        if (!IsRoomBooked(room.RoomId, slot) && !IsFacultyBooked(slot))
                        {
                            var faculty = GetAvailableFaculty(slot);
                            if (faculty == null) continue;

                            scheduledSections.Add(new Section
                            {
                                CourseId = course.CourseId,
                                FacultyId = faculty.UserId,
                                RoomId = room.RoomId,
                                DayOfWeek = slot.DayOfWeek,
                                StartTime = slot.StartTime,
                                EndTime = slot.EndTime,
                            });

                            studentsRemaining -= room.Capacity;
                            break;
                        }
                    }

                    if (studentsRemaining <= 0) break;
                }

                if (studentsRemaining <= 0) break;
            }
        }

        private bool IsRoomBooked(int roomId, TimeSlot slot)
        {
            return scheduledSections.Any(s =>
                s.RoomId == roomId &&
                s.DayOfWeek == slot.DayOfWeek &&
                s.StartTime == slot.StartTime);
        }

        private bool IsFacultyBooked(TimeSlot slot)
        {
            return scheduledSections.Any(s =>
                s.DayOfWeek == slot.DayOfWeek &&
                s.StartTime == slot.StartTime);
        }

        private User? GetAvailableFaculty(TimeSlot slot)
        {
            foreach (var faculty in facultyMembers)
            {
                bool busy = scheduledSections.Any(s =>
                    s.FacultyId == faculty.UserId &&
                    s.DayOfWeek == slot.DayOfWeek &&
                    s.StartTime == slot.StartTime);
                if (!busy) return faculty;
            }
            return null;
        }
    }

}
