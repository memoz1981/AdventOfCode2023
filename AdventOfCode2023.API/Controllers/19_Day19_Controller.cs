using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers;
[Route("day19")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class _19_Day19_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1()
    {
        var lines = System.IO.File.ReadAllLines("data19.txt").ToArray();
        int index = 0;

        var workFlows = new List<WorkFlow>();
        var parts = new List<Part>();

        for (; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
                break;

            workFlows.Add(new WorkFlow(lines[index]));
        }
        index++;

        for (; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
                break;

            parts.Add(new Part(lines[index]));

        }

        var workFlowDictionary = workFlows.ToDictionary(m => m.Name, m => m);
        var workFlowIn = workFlowDictionary["in"];

        long result = 0;

        for (int x = 1; x <= 4000; x++)
            for (int m = 1; m <= 4000; m++)
                for (int a = 1; a <= 4000; a++)
                    for (int s = 1; s <= 4000; s++)
                    {
                        var part = new Part(x, m, a, s);

                        workFlowIn.Apply(workFlowDictionary, part);

                        if (part.Accepted)
                            result++;
                    }

        //foreach (var part in parts)
        //{
        //    workFlowIn.Apply(workFlowDictionary, part); 
        //}



        //var sum = workFlows.Sum(w => w.GetTotal()); 


        return Ok(result);
    }

    [HttpGet("exercise2_2")]
    public IActionResult Exercise2New()
    {
        var lines = System.IO.File.ReadAllLines("data19.txt").ToArray();
        var workFlows = new List<WorkFlow>();
        var parts = new List<Part>();

        for (int index = 0; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
                break;

            workFlows.Add(new WorkFlow(lines[index]));
        }
        var workFlowDictionary = workFlows.ToDictionary(m => m.Name, m => m);
        var workFlowIn = workFlowDictionary["in"];

        var accepted = new HashSet<PartRange>();
        var rejected = new HashSet<PartRange>();

        var partRange = new PartRange((1, 4000), (1, 4000), (1, 4000), (1, 4000));
        workFlowIn.ApplyWithCascade(accepted, rejected, workFlowDictionary, partRange);

        var result = accepted.Sum(a => a.GetCombinationCount());

        return Ok(result);

    }

    [HttpGet("exercise2")]
    [Obsolete]
    public IActionResult Exercise2()
    {
        var lines = System.IO.File.ReadAllLines("data19.txt").ToArray();
        int index = 0;

        var workFlows = new List<WorkFlow>();
        var parts = new List<Part>();

        for (; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
                break;

            workFlows.Add(new WorkFlow(lines[index]));
        }

        var workFlowDictionary = workFlows.ToDictionary(m => m.Name, m => m);
        var workFlowIn = workFlowDictionary["in"];

        var distinctRules = workFlows
            .SelectMany(m => m.Rules)
            .Where(m => m.Property != null).Select(m => new { Value = m.Value, Property = m.Property })
            .Distinct().ToList();

        var distinctXs = distinctRules.Where(m => m.Property == 'x' && m.Value <= 4000).OrderBy(m => m.Value).ToList();
        var distinctMs = distinctRules.Where(m => m.Property == 'm' && m.Value <= 4000).OrderBy(m => m.Value).ToList();
        var distinctAs = distinctRules.Where(m => m.Property == 'a' && m.Value <= 4000).OrderBy(m => m.Value).ToList();
        var distinctSs = distinctRules.Where(m => m.Property == 's' && m.Value <= 4000).OrderBy(m => m.Value).ToList();

        var xRanges = ReturnRanges(distinctXs.Select(m => m.Value).ToList());
        var mRanges = ReturnRanges(distinctMs.Select(m => m.Value).ToList());
        var aRanges = ReturnRanges(distinctAs.Select(m => m.Value).ToList());
        var sRanges = ReturnRanges(distinctSs.Select(m => m.Value).ToList());

        var sumX = xRanges.Select(m => m.end - m.start + 1).Sum(m => m);
        var sumM = mRanges.Select(m => m.end - m.start + 1).Sum(m => m);
        var sumA = aRanges.Select(m => m.end - m.start + 1).Sum(m => m);
        var sumS = sRanges.Select(m => m.end - m.start + 1).Sum(m => m);

        var incorrectXs = xRanges.Where(x => x.end < x.start).ToList();
        var incorrectMs = mRanges.Where(x => x.end < x.start).ToList();
        var incorrectAs = aRanges.Where(x => x.end < x.start).ToList();
        var incorrectSs = sRanges.Where(x => x.end < x.start).ToList();

        long sumAccepted = 0;
        long sumRejected = 0;
        int count = 1;

        long xSum = 0;
        long mSum = 0;
        long aSum = 0;
        long sSum = 0;

        decimal sumNew = 0;

        var aRangesNew = new List<(int start, int end)>() { (1,2000), (2001, 4000) };
        var sRangesNew = new List<(int start, int end)>() { (1, 1000), (1001, 4000) };
        var xRangesNew = new List<(int start, int end)>() { (1, 1), (2,4000) };
        var mRangesNew = new List<(int start, int end)>() { (1, 4000)};


        foreach (var a in aRanges)
        {
            foreach (var s in sRanges)
            {
                foreach (var x in xRanges)
                {
                    foreach (var m in mRanges)
                    {
                        if (TryReturnCountForInclusiveStartEnd(workFlowDictionary, x.start, x.end, m.start, m.end, a.start,
                            a.end, s.start, s.end, workFlows, out var result, ref count,
                            ref xSum, ref mSum, ref aSum, ref sSum))
                        {
                            sumAccepted += result;
                        }
                        else
                            sumRejected += result;
                    }
                }
            }
        }
         

        return Ok(sumAccepted);
    }

    private List<(int start, int end)> ReturnRanges(List<int> distinctNumbers)
    {
        var ranges = new List<(int start, int end)>();

        if (distinctNumbers.First() != 1)
        {
            ranges.Add((1, distinctNumbers[0] - 1));
            ranges.Add((distinctNumbers[0], distinctNumbers[0]));
        }

        for (int i = 1; i < distinctNumbers.Count; i++)
        {
            if(distinctNumbers[i-1] != distinctNumbers[i] - 1)
                ranges.Add((distinctNumbers[i-1] + 1, distinctNumbers[i] - 1));
            
            ranges.Add((distinctNumbers[i], distinctNumbers[i]));
        }
        
        if (distinctNumbers.Last() < 4000)
            ranges.Add((distinctNumbers.Last() + 1, 4000));

        return ranges; 
    }

    private bool TryReturnCountForInclusiveStartEnd(Dictionary<string, WorkFlow> workFlowDictionary, int xStart, int xEnd, int mStart, int mEnd, int aStart, int aEnd, int sStart, int sEnd,
        List<WorkFlow> workFlows, out long result, ref int count, ref long xSum, ref long mSum, 
        ref long aSum, ref long sSum)
    {
        count++; 
        var workFlowIn = workFlowDictionary["in"];

        long countX = xEnd - xStart + 1; 
        long countM = mEnd - mStart + 1;
        long countA = aEnd - aStart + 1;
        long countS = sEnd - sStart + 1;

        result = countX * countM * countA * countS;
        
        var partFirst = new Part(xStart, mStart, aStart, sStart);

        workFlowIn.Apply(workFlowDictionary, partFirst);

        return partFirst.Accepted; 
    }

    private long FindSum(int start, int end)
    {
        return (start - 1) * (end - start + 1) + ((end - start + 1) * (end - start + 2)) / 2; 
    }
}

public class PartRange
{
    public PartRange((long xStart, long xEnd) x, (long mStart, long mEnd) m, 
        (long aStart, long aEnd) a, (long sStart, long sEnd) s)
    {
        XStart = x.xStart; 
        XEnd = x.xEnd;

        MStart = m.mStart; 
        MEnd = m.mEnd;

        AStart = a.aStart; 
        AEnd = a.aEnd;

        SStart = s.sStart;
        SEnd = s.sEnd;
    }

    public (long start, long end) GetProperty(char c)
    {
        if (c == 'x')
            return (XStart, XEnd);
        else if (c == 'm')
            return (MStart, MEnd);
        else if (c == 'a')
            return (AStart, AEnd);
        else if (c == 's')
            return (SStart, SEnd);
        else
            throw new ArgumentException();
    }

    public PartRange Copy()
    {
        return new PartRange((XStart, XEnd), (MStart, MEnd), (AStart, AEnd), (SStart, SEnd)); 
    }

    public long GetCombinationCount()
    {
        return (XEnd - XStart + 1) * (MEnd - MStart + 1) * (AEnd - AStart + 1) * (SEnd - SStart + 1); 
    }

    public PartRange CopyWithProperty(long start, long end, char property)
    {
        var partRange = Copy(); 
        
        if (property == 'x')
        {
            partRange.XStart = start;
            partRange.XEnd = end;
        }

        else if (property == 'm')
        {
            partRange.MStart = start;
            partRange.MEnd = end;
        }

        else if (property == 'a')
        {
            partRange.AStart = start;
            partRange.AEnd = end;
        }

        else if (property == 's')
        {
            partRange.SStart = start;
            partRange.SEnd = end;
        }
        return partRange; 
    }

    public long XStart { get; set; }

    public long XEnd { get; set; }

    public long MStart { get; set; }

    public long MEnd { get; set; }

    public long AStart { get; set; }

    public long AEnd { get; set; }

    public long SStart { get; set; }

    public long SEnd { get; set; }
}


public class Part
{
    public Part(string line)
    {
        line = line.Trim();

        var indexStart = 1;
        var indexEnd = line.IndexOf('}');

        var properties = line.Substring(indexStart, indexEnd - indexStart).Split(',').Select(m => m.Trim()).ToList();

        X = int.Parse(properties[0].Substring(2, properties[0].Length - 2));
        M = int.Parse(properties[1].Substring(2, properties[1].Length - 2));
        A = int.Parse(properties[2].Substring(2, properties[2].Length - 2));
        S = int.Parse(properties[3].Substring(2, properties[3].Length - 2));
    }

    public Part(int x, int m, int a, int s)
    {
        X = x;
        M = m;
        A = a;
        S = s;
    }

    public int GetTotal() => X + M + A + S; 

    public int GetProperty(char c)
    {
        if (c == 'x')
            return X;
        else if (c == 'm')
            return M;
        else if (c == 'a')
            return A;
        else if (c == 's')
            return S;
        else
            throw new ArgumentException(); 
    }

    public bool Accepted { get; set; }

    public bool Rejected { get; set; }

    public int X { get; }

    public int M { get; }

    public int A { get; }

    public int S { get; }
}

public class WorkFlow
{
    public WorkFlow(string line)
    {
        line = line.Trim();

        var indexScopeStart = line.IndexOf('{');
        Name = line.Substring(0, indexScopeStart);
        Rules = line.Substring(indexScopeStart + 1, line.Length - 1 - indexScopeStart).Split(',')
            .Select(r => new Rule(r, this)).ToList(); 
        
        Accepted = new(); 
        
    }

    public int GetTotal() => Accepted.Sum(m => m.GetTotal()); 

    public void Apply(Dictionary<string, WorkFlow> workflows, Part part)
    {
        foreach (var rule in Rules)
        {
            var result = rule.Apply(workflows, part);

            if (result)
                break; 
        }
    }

    public string Name { get; }
    public List<Rule> Rules { get; }

    public List<Part> Accepted { get; }

    public void ApplyWithCascade(HashSet<PartRange> accepted, HashSet<PartRange> rejected, 
        Dictionary<string, WorkFlow> workflows, PartRange partRange)
    {
        foreach (var rule in Rules)
        {
            var partRangeReturned = rule.ApplyWithCascade(accepted, rejected, workflows, partRange);

            if (partRangeReturned == null)
                break; 

            partRange = partRangeReturned; 
        }
    }

    public class Rule
    {
        public Rule(string line, WorkFlow workFlow)
        {
            line = line.Trim();
            Parent = workFlow;

            if (!line.Contains(':'))
            {
                ActionCondition = m => true; 
                ActionStatement = line.Substring(0, line.Length - 1);
                Property = null;
                return; 
            }

            Property = line[0];
            var indexScope = line.IndexOf(':');

            Value = int.Parse(line.Substring(2, indexScope - 2));
            ValueLong = Value; 
            ActionStatement = line.Substring(indexScope + 1, line.Length - indexScope - 1);
            ActionChar = line[1]; 
            if (line[1] == '<')
            {
                ActionCondition = m => (m.GetProperty(Property.Value) < Value); 
            }
            else if (line[1] == '>')
            {
                ActionCondition = m => (m.GetProperty(Property.Value) > Value);
            }
            else
            {
                throw new ArgumentException(); 
            }

        }

       
        public WorkFlow Parent { get; }
        
        public char? Property { get; }

        public int Value { get; }

        public long ValueLong { get; }

        public Func<Part, bool> ActionCondition { get; }

        public char ActionChar { get; set; }

        public string ActionStatement { get; }

        public bool Apply(Dictionary<string, WorkFlow> workflows, Part part)
        {
            if (Property == null)
            {
                ApplyCondition(workflows, part, ActionStatement);
                return true; 
            }

            if (!ActionCondition(part))
                return false;

            ApplyCondition(workflows, part, ActionStatement);

            return true; 
        }

        public PartRange ApplyWithCascade(HashSet<PartRange> accepted, HashSet<PartRange> rejected,
            Dictionary<string, WorkFlow> workflows, PartRange partRange)
        {
            if (Property == null)
            {
                if (ActionStatement == "A")
                {
                    accepted.Add(partRange.Copy());
                }
                else if (ActionStatement == "R")
                {
                    rejected.Add(partRange.Copy());
                }
                else
                {
                    var workFlow = workflows[ActionStatement];
                    workFlow.ApplyWithCascade(accepted, rejected, workflows, partRange.Copy());
                }
                return null; 
            }
            //if there's a condition - need to check and split... 
            
            (PartRange actionAccepted, PartRange actionRejected) = Split(partRange);

            if (actionAccepted == null)
                return actionRejected; 
            
            if (ActionStatement == "A")
            {
                accepted.Add(actionAccepted);
            }
            else if (ActionStatement == "R")
            {
                rejected.Add(actionAccepted);
            }
            else
            {
                var workFlow = workflows[ActionStatement];
                workFlow.ApplyWithCascade(accepted, rejected, workflows, actionAccepted);
            }

            return actionRejected; 
        }

        private (PartRange actionAccepted, PartRange actionRejected) Split(PartRange partRange)
        {
            var value = partRange.GetProperty(Property.Value);

            long acceptedPropertyStart = 0;
            long acceptedPropertyEnd = 0;

            long rejectedPropertyStart = 0;
            long rejectedPropertyEnd = 0;

            if (ActionChar == '<')
            {
                if (ValueLong <= value.start)
                {
                    return (null, partRange.Copy());
                }
                else if (ValueLong > value.end)
                {
                    return (partRange.Copy(), null);
                }
                acceptedPropertyStart = value.start;
                acceptedPropertyEnd = ValueLong - 1;
                rejectedPropertyStart = ValueLong;
                rejectedPropertyEnd = value.end; 
            }
            else if (ActionChar == '>')
            {
                if (ValueLong < value.start)
                {
                    return (partRange.Copy(), null);
                }
                else if (ValueLong >= value.end)
                {
                    return (null, partRange.Copy());
                }
                
                acceptedPropertyStart = ValueLong + 1;
                acceptedPropertyEnd = value.end;
                rejectedPropertyStart = value.start;
                rejectedPropertyEnd = ValueLong;
            }

            var accepted = partRange.CopyWithProperty(acceptedPropertyStart, acceptedPropertyEnd, Property.Value);
            var rejected = partRange.CopyWithProperty(rejectedPropertyStart, rejectedPropertyEnd, Property.Value);

            return (accepted, rejected); 
        }

        private void ApplyCondition(Dictionary<string, WorkFlow> workflows, Part part, string action)
        {
            if (action == "A")
            {
                Parent.Accepted.Add(part);
                part.Accepted = true;
            }
            else if (action == "R")
            {
                part.Rejected = true; 
                return;
            }
            else
            {
                var workFlow = workflows[action];

                workFlow.Apply(workflows, part); 
            }
        }
    }
}