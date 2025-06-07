using FacultyStudentPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FacultyStudentPortal.BLL.Services
{
    public class SectionScheduler
    {
        private readonly List<Course> _courses;
        private readonly List<User> _faculty;
        private readonly List<Room> _rooms;
        private readonly List<TimeSlot> _lectureSlots;
        private readonly List<TimeSlot> _labSlots;
        private readonly List<Section> _scheduledSections = new();

        public SectionScheduler(
            List<Course> courses,
            List<User> faculty,
            List<Room> rooms,
            List<TimeSlot> lectureSlots,
            List<TimeSlot> labSlots)
        {
            _courses = courses;
            _faculty = faculty;
            _rooms = rooms;
            _lectureSlots = lectureSlots;
            _labSlots = labSlots;
        }

        public List<Section> GenerateSchedule()
        {
            foreach (var course in _courses.OrderByDescending(c => c.EligibleStudents))
            {
                AllocateSections(course, isLab: false);

                if (course.LabHour > 0)
                    AllocateSections(course, isLab: true);
            }

            return _scheduledSections;
        }

        private void AllocateSections(Course course, bool isLab)
        {
            if (!isLab && course.CreditHours == 3)
            {
                Allocate3HourLectures(course);
                return;
            }

            string sectionType = isLab ? "Lab" : "Lecture";
            int hours = isLab ? course.LabHour : course.CreditHours;

            var eligibleRooms = _rooms
                .Where(r => r.RoomType.Equals(sectionType, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.Capacity)
                .ToList();

            var slots = isLab ? _labSlots : _lectureSlots;

            int studentsRemaining = course.EligibleStudents;

            foreach (var room in eligibleRooms)
            {
                int studentsPerSection = room.Capacity;
                int neededSections = (int)Math.Ceiling((double)studentsRemaining / studentsPerSection);

                for (int i = 0; i < neededSections; i++)
                {
                    foreach (var slot in slots)
                    {
                        if (RoomAndFacultyAvailable(room.RoomId, _faculty.First().UserId, slot) &&
                            (!isLab || !ConflictsWithCourseLectures(course.CourseId, slot)))
                        {
                            var faculty = GetAvailableFaculty(slot);
                            if (faculty == null) continue;

                            _scheduledSections.Add(new Section
                            {
                                CourseId = course.CourseId,
                                FacultyId = faculty.UserId,
                                RoomId = room.RoomId,
                                DayOfWeek = slot.DayOfWeek,
                                StartTime = slot.StartTime,
                                EndTime = slot.EndTime,
                                SectionType = sectionType
                            });

                            studentsRemaining -= room.Capacity;
                            break;
                        }
                    }

                    if (studentsRemaining <= 0)
                        break;
                }

                if (studentsRemaining <= 0)
                    break;
            }
        }

        private User? GetAvailableFaculty(TimeSlot slot)
        {
            foreach (var faculty in _faculty)
            {
                bool isBusy = _scheduledSections.Any(s =>
                    s.FacultyId == faculty.UserId &&
                    s.DayOfWeek == slot.DayOfWeek &&
                    Overlaps(s.StartTime, s.EndTime, slot.StartTime, slot.EndTime));

                if (!isBusy)
                    return faculty;
            }

            return null; // No available faculty found
        }


        private void Allocate3HourLectures(Course course)
        {
            var rooms = _rooms
                .Where(r => r.RoomType == "Lecture")
                .OrderByDescending(r => r.Capacity)
                .ToList();

            int studentsRemaining = course.EligibleStudents;
            int totalSections = (int)Math.Ceiling((double)studentsRemaining / rooms[0].Capacity);
            int schemeASlots = totalSections / 2;
            int schemeBSlots = totalSections - schemeASlots;

            var aLength = new TimeSpan(1, 30, 0);
            var bLength = new TimeSpan(1, 0, 0);

            foreach (var room in rooms)
            {
                while (schemeASlots > 0 || schemeBSlots > 0)
                {
                    var faculty = _faculty.FirstOrDefault(f =>
                        !_scheduledSections.Any(s => s.FacultyId == f.UserId));

                    if (faculty == null) break;

                    if (schemeASlots > 0)
                    {
                        var pairs = GetNonConsecutiveTimeSlotPairs(_lectureSlots, aLength);
                        foreach (var pair in pairs)
                        {
                            if (RoomAndFacultyAvailable(room.RoomId, faculty.UserId, pair[0]) &&
                                RoomAndFacultyAvailable(room.RoomId, faculty.UserId, pair[1]))
                            {
                                _scheduledSections.Add(CreateLectureSection(course, room, faculty, pair[0]));
                                _scheduledSections.Add(CreateLectureSection(course, room, faculty, pair[1]));
                                schemeASlots--;
                                studentsRemaining -= room.Capacity;
                                break;
                            }
                        }
                    }
                    else if (schemeBSlots > 0)
                    {
                        var triplets = GetNonConsecutiveTimeSlotTriplets(_lectureSlots, bLength);
                        foreach (var triplet in triplets)
                        {
                            if (triplet.All(slot => RoomAndFacultyAvailable(room.RoomId, faculty.UserId, slot)))
                            {
                                foreach (var slot in triplet)
                                {
                                    _scheduledSections.Add(CreateLectureSection(course, room, faculty, slot));
                                }
                                schemeBSlots--;
                                studentsRemaining -= room.Capacity;
                                break;
                            }
                        }
                    }

                    if (studentsRemaining <= 0) return;
                }
            }
        }

        private bool RoomAndFacultyAvailable(int roomId, int facultyId, TimeSlot slot)
        {
            return !_scheduledSections.Any(s =>
                s.RoomId == roomId &&
                s.DayOfWeek == slot.DayOfWeek &&
                Overlaps(s.StartTime, s.EndTime, slot.StartTime, slot.EndTime)) &&
                !_scheduledSections.Any(s =>
                s.FacultyId == facultyId &&
                s.DayOfWeek == slot.DayOfWeek &&
                Overlaps(s.StartTime, s.EndTime, slot.StartTime, slot.EndTime));
        }

        private bool ConflictsWithCourseLectures(int courseId, TimeSlot slot)
        {
            return _scheduledSections.Any(s =>
                s.CourseId == courseId &&
                s.SectionType == "Lecture" &&
                s.DayOfWeek == slot.DayOfWeek &&
                Overlaps(s.StartTime, s.EndTime, slot.StartTime, slot.EndTime));
        }

        private bool Overlaps(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && start2 < end1;
        }

        private Section CreateLectureSection(Course course, Room room, User faculty, TimeSlot slot)
        {
            return new Section
            {
                CourseId = course.CourseId,
                FacultyId = faculty.UserId,
                RoomId = room.RoomId,
                DayOfWeek = slot.DayOfWeek,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                SectionType = "Lecture"
            };
        }

        private List<TimeSlot[]> GetNonConsecutiveTimeSlotPairs(List<TimeSlot> slots, TimeSpan duration)
        {
            var groupedByDay = slots
                .Where(s => s.EndTime - s.StartTime == duration)
                .GroupBy(s => s.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dayOrder = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
            var result = new List<TimeSlot[]>();

            for (int i = 0; i < dayOrder.Length - 1; i++)
            {
                for (int j = i + 1; j < dayOrder.Length; j++)
                {
                    if (groupedByDay.ContainsKey(dayOrder[i]) && groupedByDay.ContainsKey(dayOrder[j]))
                    {
                        var s1 = groupedByDay[dayOrder[i]].FirstOrDefault();
                        var s2 = groupedByDay[dayOrder[j]].FirstOrDefault();
                        if (s1 != null && s2 != null)
                            result.Add(new[] { s1, s2 });
                    }
                }
            }

            return result;
        }

        private List<TimeSlot[]> GetNonConsecutiveTimeSlotTriplets(List<TimeSlot> slots, TimeSpan duration)
        {
            var result = new List<TimeSlot[]>();
            var groupedByDay = slots
                .Where(s => s.EndTime - s.StartTime == duration)
                .GroupBy(s => s.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dayOrder = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };

            for (int i = 0; i < dayOrder.Length - 2; i++)
            {
                for (int j = i + 1; j < dayOrder.Length - 1; j++)
                {
                    for (int k = j + 1; k < dayOrder.Length; k++)
                    {
                        var d1 = dayOrder[i];
                        var d2 = dayOrder[j];
                        var d3 = dayOrder[k];

                        if (groupedByDay.ContainsKey(d1) && groupedByDay.ContainsKey(d2) && groupedByDay.ContainsKey(d3))
                        {
                            var s1 = groupedByDay[d1].FirstOrDefault();
                            var s2 = groupedByDay[d2].FirstOrDefault();
                            var s3 = groupedByDay[d3].FirstOrDefault();

                            if (s1 != null && s2 != null && s3 != null)
                                result.Add(new[] { s1, s2, s3 });
                        }
                    }
                }
            }

            return result;
        }
    }
}
