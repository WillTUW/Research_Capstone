﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;

    public class CoreClassesLastYear : Criteria
    {
        public CoreClassesLastYear(double weight) : base(weight)
        {
        }

        public override double getResult(ScheduleModel s)
        {
            long lastYear = -1;
            foreach (Quarter q in s.Quarters) {
                if (q.Year > lastYear) {
                    lastYear = q.Year;
                }
            }
            double sum = 0;
            foreach (Quarter q in s.Quarters) {
                if (q.Year == lastYear) {
                    foreach (Course c in q.Courses) {
                        if (c.DepartmentID == s.PreferenceSet.DepartmentID) {
                            sum++;
                        }
                    }
                }
            }
            return ((double)sum / (double)s.PreferenceSet.CoreCountLastYear) * weight;
        }
    }
}
