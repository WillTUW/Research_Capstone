using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;
    class EnglishClassStart : Criteria
    {
        const int ENGLISH_DEPARTMENT = 28;
        public EnglishClassStart(double weight) : base(weight) { }

        public override double getResult(ScheduleModel s)
        {
            foreach (Quarter q in s.Quarters)
            {
                foreach (Course c in q.Courses)
                {

                    if (c.DepartmentID == ENGLISH_DEPARTMENT)
                    {
                        return (Int32.Parse(c.Description) == s.PreferenceSet.PreferredEnglishStart ?
                            1.0 : 0.0) * weight;
                    }

                }
            }
            return 0.0;
        }
    }
}