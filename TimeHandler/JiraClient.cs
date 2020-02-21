using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;

namespace TimeHandler
{
    class JiraClient
    {
        private static class Statuses
        {
            public const string NotStarted = "Not Started";
            public const string InProgress = "In Progress";
            public const string CodeReview = "Code Review";
            public const string NotExecuted = "Not Executed";
            public const string Pass = "Pass";
            public const string Fail = "Fail";
        }
        private const string Host = "revspringinc.atlassian.net";
        private readonly string Password = ConfigurationManager.AppSettings["JiraAuthToken"];
        private const string Path = "rest/api/2/search";
        private const string Scheme = "https";
        private readonly string UserName = ConfigurationManager.AppSettings["JiraUserName"];
        private const string AssigneeJQL = "assignee = currentUser()";
        private const string OrderByJQL = "ORDER BY updated DESC";
        

        public async Task<List<JiraStory>> GetIssues(DateTime from, DateTime to)
        {
            var rawQuery = GenerateBasicSearchJQL(
                Statuses.NotStarted, 
                Statuses.InProgress, 
                Statuses.CodeReview, 
                from, 
                to
            );
            var query = Uri.EscapeDataString(rawQuery);
            var fullQuery = $"jql={query}&fields=summary,status,id,key,assignee,issuetype&expand=changelog";
            var raw = await Request<RawIssues>(Path, fullQuery);
            var testIssues = await GetTestStories(raw.issues, from, to);
            var parents = raw.ToJira();
            foreach (var issue in testIssues)
            {
                if (issue.ParentKey != null)
                {
                    parents.Find(story => story.Key == issue.ParentKey)?.SubTasks.Add(issue);
                }
            }
            return raw.ToJira();
        }

        

        public async Task<List<JiraStory>> GetTestStories(List<RawIssue> issues, DateTime from, DateTime to)
        {
            var jql = GenerateTestSearchJQL(issues, from, to);
            var query = Uri.EscapeDataString(jql);
            var fullQuery = $"jql={query}&fields=summary,status,id,key,assignee,issuetype,parent&expand=changelog";
            var raw = await Request<RawIssues>(Path, fullQuery);
            return raw.ToJira();
        }

        private async Task<T> Request<T>(string path, string query)
        {
            var ub = new UriBuilder
            {
                Host = Host,
                Path = path,
                Port = 443,
                Scheme = Scheme,
                Query = query,
            };
            var request = new HttpRequestMessage()
            {
                RequestUri = ub.Uri,
            };
            var un = Base64ify($"{UserName}:{Password}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", un);
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                var s = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(s);
            }
        }

        private static string GenerateTestSearchJQL(List<RawIssue> issues, DateTime from, DateTime to)
        {
            var keys = $"(\"{string.Join("\",\"", issues.Select(i => i.key))}\")";
            var statuses = GenerateBothStatusesWithDates(
                Statuses.NotExecuted,
                Statuses.Fail,
                Statuses.Pass,
                from,
                to
            );
            return $"(parent in {keys} AND {statuses}) OR proctor = currentUser() {OrderByJQL}";
        }

        private static string GenerateBasicSearchJQL(
            string fromStatus, 
            string middleStatus, 
            string toStatus, 
            DateTime fromDate, 
            DateTime toDate)
        {
            var statuses = GenerateBothStatusesWithDates(
                fromStatus, 
                middleStatus, 
                toStatus, 
                fromDate, 
                toDate
            );
            return $"{AssigneeJQL} AND {statuses} {OrderByJQL}";
        }

        private static string GenerateBothStatusesWithDates(
            string fromStatus, 
            string middleStatus, 
            string toStatus, 
            DateTime fromDate, 
            DateTime toDate)
        {
            var dates = GenerateBetweenJQL(fromDate, toDate);
            var status1 = GenerateStatusJQL(fromStatus, middleStatus);
            var status2 = GenerateStatusJQL(middleStatus, toStatus);
            return $"({status1} during {dates} OR {status2} during {dates})";
        }

        private static string GenerateStatusJQL(string fromStatus, string toStatus)
        {
            return $"status changed from '{fromStatus}' to '{toStatus}'";
        }

        private static string GenerateBetweenJQL(DateTime fromDate, DateTime toDate)
        {
            return $"('{fromDate:yyyy-MM-dd}', '{toDate:yyyy-MM-dd}')";
        }
        private static string Base64ify(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
    }

    class RawIssues
    {
        public List<RawIssue> issues { get; set; }
        public List<JiraStory> ToJira()
        {
            return (this.issues ?? new List<RawIssue>()).Select(issue => issue.ToJira()).ToList();
        }
    }
    class RawIssue
    {
        public string id { get; set; }
        public string key  { get; set; }
        public RawFields fields  { get; set; }
        public RawChangLog changelog { get; set; }
        public JiraStory ToJira()
        {
            return new JiraStory
            {
                Id = this.id,
                Key = this.key,
                ParentKey = this.fields.parent?.key,
                Summary = this.fields.summary,
                Status = new StoryStatus
                {
                    Id = this.fields.status.id,
                    Description = this.fields.status.description,
                    Name = this.fields.status.name,
                },
                SubTasks = this.fields.subtasks?.Select(subtask => subtask.ToJira()).ToList() ?? new List<JiraStory>(),
                Transitions = this.changelog?
                                .histories?
                                .Where(raw => raw.IsStatus())
                                .Select(raw => raw.ToJira())
                                .ToList() ?? new List<Transition>(),
            };
        }
    }
    class RawChangLog
    {
        public List<RawHistory> histories { get; set; }
    }
    class RawHistory
    {
        public DateTime created { get; set; }
        public List<RawChangelogItem> items { get; set; }
        public bool IsStatus()
        {
            return this.items.Any(item => item.IsStatus());
        }
        public Transition ToJira()
        {
            var first = this.items.First(item => item.IsStatus());
            return new Transition
            {
                From = first.fromString,
                To = first.toString,
                When = this.created
            };
        }
    }
    class RawChangelogItem
    {
        public string field { get; set; }
        public string fieldtype { get; set; }
        public string from { get; set; }
        public string fromString { get; set; }
        public string to { get; set; }
        public string toString { get; set; }
        public bool IsStatus()
        {
            return this.field == "status";
        }
    }
    class RawFields
    {
        public RawAssignee assignee { get; set; }
        public string summary { get; set; }
        public RawStatus status { get; set; }
        public List<RawIssue> subtasks { get; set; }
        public RawIssueType issuetype { get; set; }
        public RawIssue parent { get; set; }
    }
    class RawAssignee
    {
        public string name  { get; set; }
        public string accountId  { get; set; }
    }
    class RawStatus
    {
        public string name { get; set; }
        public string id  { get; set; }
        public string description { get; set; }
    }

    class RawIssueType
    {
        public string name { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public bool subtask { get; set; }
    }
}
