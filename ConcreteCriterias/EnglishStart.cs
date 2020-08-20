using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;

    public class EnglishStart : Criteria
    {
        const int ENGLISH_DEPARTMENT = 28;
        public EnglishStart(double weight) : base(weight)
        {
        }

        public override double getResult(ScheduleModel s)
        {
            // This algorithm works for all of the 'preferred english starts' in
            // the database right now. They are all apart of Department ID 28, my
            // guess is that we can evaluate if a course is an english course by
            // checking against Department ID. From my understanding this D-ID would
            // change based on college.
            var currentquarter = 0;
            foreach (Quarter q in s.Quarters)
            {
                currentquarter++;
                foreach (Course c in q.Courses) {
                    if (c.DepartmentID == ENGLISH_DEPARTMENT)
                    {
                        if (currentquarter > 1) return 0;
                    }
                }
            }
            // No english courses or school with dept ID that is not in the consts
            return 1;
        }
    }
}
