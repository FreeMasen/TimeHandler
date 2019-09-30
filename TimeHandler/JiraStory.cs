using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeHandler
{
    public class JiraStory
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string ParentKey { get; set; }
        public string Summary { get; set; }
        public StoryStatus Status { get; set; }
        public List<JiraStory> SubTasks { get; set; }
        public List<Transition> Transitions { get; set; }
        public bool ShouldIncludeIn(DateTime date)
        {
            return WasInProgressDuring(date) || SentBackForTestingDuring(date);
        }
        public bool WasInProgressDuring(DateTime date)
        {
            var agg = this.Transitions.Aggregate(new TransitionAggregate(), (acc, t) =>
            {
                if (t.To == "In Progress")
                {
                    acc.start = t.When.Date;
                }
                if (t.To == "Code Review")
                {
                    acc.end = t.When.Date.AddDays(1).AddSeconds(-1.0);
                }
                return acc;
            });
            
            if (date < agg.start)
            {
                return false;
            }
            if (date > agg.end)
            {
                return false;
            }
            return true;
        }
        public bool SentBackForTestingDuring(DateTime date)
        {
            return this.SubTasks.Any(subtask =>
            {
                var agg = subtask.Transitions.Aggregate(new TransitionAggregate(), (acc, t) =>
                {
                    if (t.To == "Fail")
                    {
                        acc.start = t.When.Date;
                    }
                    if (t.To == "Pass")
                    {
                        acc.end = t.When.Date.AddDays(1).AddSeconds(-1.0);
                    }
                    return acc;
                });
                if (date < agg.start)
                {
                    return false;
                }
                if (date > agg.end)
                {
                    return false;
                }
                return true;
            });
        }
    }

    public class TransitionAggregate
    {
        public DateTime? start = null;
        public DateTime? end = null;
    }

    public class StoryStatus
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class IssueType
    {
        public string Id { get; set; }
        public string Name  { get; set; }
        public string Description { get; set; }
        public bool SubTask  { get; set; }

    }

    public class Transition
    {
        public DateTime When { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
