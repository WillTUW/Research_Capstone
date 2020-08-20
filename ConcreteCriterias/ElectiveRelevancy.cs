using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;

    public class ElectiveRelevancy : Criteria
    {
       public ElectiveRelevancy(double weight) : base(weight)
        {
        }

             /* 
         * Returns the relevance of a course elective. Each in major course 
         * elective will be awarded the a "1" meaning it is 100% a match. 
         * Classes outside a major catalog will be awarded a value between 0-1.
         * 0 being a complete miss match, 1 being a match. Anything inbetween a 0-1, 
         * we will determine what is a fair value <.5 for a relevant class. 
         */
        public override double getResult(ScheduleModel s)
        {
            double score_threshold = 0.5;
            int num_Class = 0;
            double overall_Score = 0;
            foreach (Quarter q in s.Quarters)
            {
                foreach (Course c in q.Courses)
                {
                    int relevancy = Int32.Parse(c.relevancy);
                    // Class is a core
                    if (relevancy == 1)
                    {
                        continue;
                    }
                    // Class is not core, but is relevant
                    else if (relevancy >= score_threshold)
                    {
                        overall_Score += relevancy;
                        num_Class++;
                        continue;
                    }
                    // Class not relevant
                    else
                    {
                        num_Class++;
                    }
                }
            }

            // Getting a cumulative average. 
            overall_Score = overall_Score / num_Class;
            return overall_Score;
        }
    }
}
