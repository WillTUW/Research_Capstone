﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;

    public class MathBreaks : Criteria
    {
        const int MATH_DEPT = 54;
        public MathBreaks(double weight) : base(weight)
        {
        }

        public override double getResult(ScheduleModel s)
        {
            Quarter prevQuarter = null;
            int totalGap = 0;
            int totalMathCourses = 0;
            foreach (Quarter q in s.Quarters)
            {
                if (hasMathCourse(q))
                {
                    totalMathCourses++;
                    if (prevQuarter == null)
                    {
                        prevQuarter = q;
                        continue;
                    }

                    Quarter nextQuarter = q;


                    int prevQID = Int32.Parse(prevQuarter.Id);
                    int nextQID = Int32.Parse(nextQuarter.Id);
                    if (prevQID + 1 != nextQID)
                        totalGap += nextQID - prevQID;
                    prevQuarter = nextQuarter;

                }
            }
            return (1 - ((double)totalGap / (double)totalMathCourses)) * weight;
        }

        private Boolean hasMathCourse(Quarter q)
        {
            foreach (Course c in q.Courses)
            {
                if (c.DepartmentID == MATH_DEPT)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
