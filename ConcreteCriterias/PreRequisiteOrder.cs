using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using System.Net.Http;
    using Models;
    using Newtonsoft.Json;

    public class PreRequisiteOrder : Criteria
    {
        HttpClient client;
        public PreRequisiteOrder(double weight) : base(weight)
        {
            client = new HttpClient();
        }

        public override double getResult(ScheduleModel s)
        {
            HashSet<string> completedCourses = new HashSet<string>();

            // Sort quarters from earliest to latest
            List<Quarter> quarters = s.Quarters;
            quarters.Sort();

            int invalidCourses = 0;

            Dictionary<int, List<CourseNode>> allCourses = null;    // CourseNetwork API output
            List<int> courses = new List<int>();                    // CourseNetwork API input

            // Prepare input list of all courses for CourseNetwork API
            foreach (Quarter q in quarters)
            {
                foreach (Course c in q.Courses)
                {
                    courses.Add(Int32.Parse(c.Id));
                }
            }

            // Get CourseNodes for each course in the schedule
            Task.Run(async () =>
            {
                allCourses = await getCourseNetworks(courses);
            }).GetAwaiter().GetResult();

            if (allCourses == null) throw new Exception("Could not get CourseNetwork");

            foreach (Quarter q in quarters)
            {
                // Iterate over courses twice to concurrent classes from being
                // seen as completed prerequisites
                foreach (Course c in q.Courses)
                {
                    // First check if prereqs are met
                    if (!verifyPrereqs(c.Id, completedCourses, allCourses)) invalidCourses++;
                }

                // Then add to completed courses
                foreach (Course c in q.Courses) completedCourses.Add(c.Id);
            }

            return (invalidCourses > 0 ? 0 : 1) * weight;
        }

        // Verifies completion of a course's prereqs
        private bool verifyPrereqs(string cId, HashSet<string> complete, Dictionary<int, List<CourseNode>> cn)
        {
            List<CourseNode> prereqs = null;
            if (!cn.TryGetValue(Int32.Parse(cId), out prereqs)) return false;

            // Verify that each course's prereqs have been completed
            foreach (CourseNode course in prereqs)
            {
                if (course.courseID != cId)
                {
                    if (!complete.Contains(course.courseID))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<Dictionary<int, List<CourseNode>>> getCourseNetworks(List<int> courses)
        {
            HttpResponseMessage resp;
            string jsonInput = JsonConvert.SerializeObject(courses);
            try
            {
                resp = await client.GetAsync(
                   $"http://vaacoursenetwork.azurewebsites.net/v1/GetMultipleCourses?input={jsonInput}"
                   );
                return JsonConvert.DeserializeObject<Dictionary<int, List<CourseNode>>>
                    (await resp.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught During HTTP Request");
                Console.WriteLine("Message: {0}", e.Message);
            }
            return null;
        }

        public async Task<List<CourseNode>> getCourseNetwork(string id)
        {
            HttpResponseMessage resp;
            try
            {
                resp = await client.GetAsync(
                   $"http://vaacoursenetwork.azurewebsites.net/v1/CourseNetwork?course={id}"
                   );
                return JsonConvert.DeserializeObject<List<CourseNode>>
                    (await resp.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught During HTTP Request");
                Console.WriteLine("Message: {0}", e.Message);
            }
            return null;
        }
    }
}

