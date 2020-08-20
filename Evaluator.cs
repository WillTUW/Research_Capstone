﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

// This class is the definition for the object that does the evaluation of schedules. A user
// must instantiate this class, and then call the evaluate method by passing in a Schedule object.
// The evaluate function lets each criteria object look at the schedule and determine whether or not
// it fulfills its requirement.

namespace ScheduleEvaluator
{
    using System.Diagnostics;
    using System.IO;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;

    public class Evaluator
    {
        // These fields are hardcoded before program runtime. They define the preferences and the weights associated with
        // each preference. For now these two structures are going to stay as fields, but in the future they may migrate to passed
        // in paramaters.
        // public static readonly CritTyp[] criteriaTypes = { CritTyp.CoreCreditsAQuarter, CritTyp.MaxQuarters };
        // public static readonly double[] weights = { 1.0, 1.0 };

        // This field holds all of the criteria objects that are created by the Criteria factory.
        private Criteria[] criterias;
        private HttpClient client;
        // Constructor for the evaluator. Creates all of the criteria objects and stores in criterias. 
        // For now I dont believe that the criterias should be mutable. 
        public Evaluator() {
            CritTyp[] criteriaTypes = { CritTyp.CoreCreditsAQuarter, CritTyp.MaxQuarters, CritTyp.TimeOfDay, CritTyp.CoreClassesLastYear, CritTyp.PreRequisiteOrder, CritTyp.MathBreaks, CritTyp.MajorSpecificBreaks, CritTyp.EnglishStart };
            double[] weights = { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
            CriteriaFactory fact;
            try
            {
                fact = new CriteriaFactory(criteriaTypes, weights);
            }
            catch(ArgumentException ag) 
            {
                Console.WriteLine("Error in creating Criterias: {0}", ag);
                throw;
            }
            client = new HttpClient();
            criterias = fact.Criterias;
        }

        // This constructor takes in a JSON string and checks the object against the schema in Criteria-Weights-Schema
        // If the JSON is valid then all of the data is extracted from the JSON and is used to create the criteria objects
        // To be assessed against the schedules
        public Evaluator(string criteriaJsonString, bool test = false) {
            JsonSchema schema;
            if (test) 
            {
                schema = JsonSchema.Parse(File.ReadAllText("./../../../ScheduleEvaluator/Criteria-Weights-Schema.json"));
            } 
            else
            {
                // So this file can not be tested due to the fact that the test dll gets executed from a different
                // directory
                schema = JsonSchema.Parse(File.ReadAllText("./../../Criteria-Weights-Schema.json"));
            }
            
            JObject criteriaJObject = JObject.Parse(criteriaJsonString);
            if (!criteriaJObject.IsValid(schema)) {
                throw new System.ArgumentException("Invalid JSON Schema");
            }
            
            JToken crits = criteriaJObject["Criteria"];
            // This for loop gets the number of criteria. It is super inefficent and should be replaced
            // with a non iterative approach
            int count = 0;
            foreach (JToken crit in crits) 
                count++;

            CritTyp[] criteriaTypes = new CritTyp[count];
            double[] weights = new double[count];
            int i = 0;
            foreach (JToken crit in crits) {
                Debug.WriteLine(crit);
                criteriaTypes[i] = CriteriaFactory.FromString(crit["CriteriaName"].ToString());
                weights[i] = Convert.ToDouble(crit["Weight"].ToString());
                i++;
            }
            CriteriaFactory fact;
            try
            {
                fact = new CriteriaFactory(criteriaTypes, weights);
            }
            catch (ArgumentException ag)
            {
                Console.WriteLine("Error in creating Criterias: {0}", ag);
                throw;
            }
            criterias = fact.Criterias;
            client = new HttpClient();
        }

        // The bread and butter of the class. At first this method doesn't look like much, but
        // by iterating over all of the criterias and having them evaluate the scheudle on their own
        // criteria this function is able to assign a score to the schedule.
        public double evalaute(ScheduleModel s) {
            double result = 0;
            double totalWeight = 0;
            foreach (Criteria c in criterias)
            {
                result += c.getResult(s);
                totalWeight += c.weight;
            }
            return result / totalWeight;
        }

        // Returns an array of evaluation results for each criteria.
        public double[] getEvaluationVector(ScheduleModel s)
        {
            int numOfCriteria = criterias.Length;
            double[] results = new double[numOfCriteria];
            for (int i = 0; i < numOfCriteria; i++)
            {
                results[i] = criterias[i].getResult(s);
            }
            return results;
        }
    }
}
