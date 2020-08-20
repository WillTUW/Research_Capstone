using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;

    public class TimeOfDay : Criteria
    {
        public TimeOfDay(double weight) : base(weight)
        {

        }

        public override double getResult(ScheduleModel s)
        {
            string timePreference = s.PreferenceSet.TimePreference;

            // If time preference is set to "Any", this criterion passes
            if (string.Equals(timePreference, "Any")) return weight;

            int numOutsideTimePref = 0;
            int totalCourses = 0;
            foreach (Quarter q in s.Quarters)
            {
                foreach (Course c in q.Courses)
                {
                    totalCourses++;
                    // Compares time of each course in the ScheduleModel to the
                    // time preference of its PreferenceSet; increments count of
                    // how many courses don't align with time preference.
                    if (!string.Equals(c.TimeOfDay, timePreference))
                    {
                        numOutsideTimePref++;
                    }
                }
            }
            // May we want to change this binary return.
            return (1 - ((double)numOutsideTimePref / (double)totalCourses)) * weight;
        }
    }
}
