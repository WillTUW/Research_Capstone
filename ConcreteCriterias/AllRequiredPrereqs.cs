using System;
using System.Collections.Generic;
using System.Text;


namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;

    public class AllPrereqs : Criteria
    {

        public AllPrereqs(double weight) : base(weight)
        {

        }

        // Determines if all major prerequisite courses are included in the
        // given schedule model.
        public override double getResult(ScheduleModel s)
        {
            List<string> coursesScheduled = new List<string>();
            foreach (Quarter q in s.Quarters)
            {
                foreach (Course c in q.Courses)
                {
                    coursesScheduled.Add(c.Id);
                }
            }

            int prereqsNotMet = 0;

            foreach (string courseID in s.PreferenceSet.RequiredCourses)
            {
                if (!coursesScheduled.Contains(courseID)) prereqsNotMet++;
            }

            return (prereqsNotMet > 0 ? 0 : 1) * weight;
        }
    }
}