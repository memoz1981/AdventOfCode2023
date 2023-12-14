using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day13")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class _13_Day13_Mirrors_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data13.txt").ToArray();

            var areas = ReadAllAreas(lines);
            int result = 0; 

            foreach (var area in areas)
            {
                if (TryFindRowReflection(lines, area.Item1, area.Item2, out var sumR, out var _, out var _, out var _))
                    result += sumR * 100;
                else if (TryFindColumnReflection(lines, area.Item1, area.Item2, out var sumC, out var _, out var _, out var _))
                    result += sumC;
                else
                    throw new ArgumentException();
            }

            return Ok(result);
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            var lineReader = new StringLineReader();
            var lines = lineReader.ReadLines("data13.txt").ToArray();

            var areas = ReadAllAreas(lines);
            int result = 0;

            List<(int start, int end)> notFound = new(); 
            foreach (var area in areas)
            {
                List<(int start, int end, bool smudgeFound, int sum)> list = new();

                var row = TryFindRowReflection(lines, area.Item1, area.Item2, out var sumR, out var startR, out var endR,
                    out var smudgeFoundR);

                var col = TryFindColumnReflection(lines, area.Item1, area.Item2, out var sumC, out var startC, out var endC, out var smudgeFoundC);

                if (row && smudgeFoundR)
                {
                    result += sumR * 100;
                }
                else if (col && smudgeFoundC)
                {
                    result += sumC;
                }
                else
                {
                    notFound.Add(area); 
                }
            }

            return Ok(result);
        }

        private bool IsSmadged(string line1, string line2)
        {
            int dif = 0; 
            for (int i = 0; i < line1.Length; i++)
            {
                if (line1[i] != line2[i])
                    dif++;
            }

            return dif == 1; 
        }

        private bool TryFindColumnReflection(string[] lines, int startIndex, int endIndex, out int sum, out int start, 
            out int end, out bool smudgeFound)
        {
            var transposed = Transpose(lines, startIndex, endIndex);

            sum = 0;
            start = -1;
            end = -1;
            smudgeFound = false; 

            if (TryFindRowReflection(transposed, 0, transposed.Length-1, out var sumR, out var startR, out var endR, out var found))
            {
                sum = sumR;
                start = startR;
                end = endR;
                smudgeFound = found; 
                return true;
            }
            
            return false; 
        }

        private string[] Transpose(string[] lines, int startIndex, int endIndex)
        {
            var list = new List<string>();
            for (int j = 0; j < lines[startIndex].Length; j++)
            {
                var array = new List<char>();
                for (int i = startIndex; i <= endIndex; i++)
                {
                    array.Add(lines[i][j]);
            
                }
                list.Add(new string(array.ToArray()));
            }
            return list.ToArray(); 
        }

        private bool TryFindRowReflection(string[] lines, int startIndex, int endIndex, 
            out int sum, out int start, out int end, out bool smudgeFound)
        {
            sum = 0;
            var startIndices = new List<int>();
            start = -1;
            end = -1;
            smudgeFound = false;

            if (IsSmadged(lines[startIndex], lines[startIndex + 1]))
            {
                sum = 1;
                start = startIndex;
                end = startIndex + 1;
                smudgeFound = true; 
                return true;
            }

            if (IsSmadged(lines[endIndex - 1], lines[endIndex]))
            {
                sum = endIndex - startIndex;
                start = endIndex - 1;
                end = endIndex;
                smudgeFound = true;
                return true;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                if (lines[i] == lines[i + 1] || IsSmadged(lines[i], lines[i+1]))
                    startIndices.Add(i);
            }

            (int start, int end) smudged = (-1, -1);
            (int start, int end) nonSmudged = (-1, -1);

            foreach (var i in startIndices)
            {
                var potential = FindStartEnd(lines, i);

                if (potential.start != startIndex && potential.end != endIndex)
                {
                    continue; 
                }
                else
                {
                    nonSmudged = potential; 
                }
            }

            foreach (var i in startIndices)
            {
                var potential = FindStartEnd(lines, i, true);

                if (potential.start != startIndex && potential.end != endIndex)
                {
                    continue;
                }
                else
                {
                    smudged = potential; 
                }
            }

            if (smudged.start == -1)
            {
                if (nonSmudged.start != -1)
                {
                    sum = (nonSmudged.end - nonSmudged.start + 1) / 2 + nonSmudged.start - startIndex;
                    start = nonSmudged.start;
                    end = nonSmudged.end;
                    smudgeFound = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                sum = (smudged.end - smudged.start + 1) / 2 + smudged.start - startIndex;
                start = smudged.start;
                end = smudged.end;
                smudgeFound = true;
                return true;
            }
        }

        private (int start, int end) FindStartEnd(string[] lines, int start, bool smudgeEnabled = false)
        {
            int st = start;
            int end = start + 1;
            int smudgeCount = 0; 
            while (st >= 0 && end < lines.Length)
            {
                if (st == 0 || end == lines.Length - 1)
                {
                    if (lines[st] == lines[end])
                    {
                        break;
                    }
                    else if (!smudgeEnabled)
                    {
                        st = st + 1;
                        end = end - 1;
                        break;
                    }
                    else if (IsSmadged(lines[st], lines[end]))
                    {
                        smudgeCount++;
                        break;
                    }
                    else 
                    {
                        st = st + 1;
                        end = end - 1;
                        break;
                    }
                    
                }
                if (string.IsNullOrWhiteSpace(lines[end]) || string.IsNullOrWhiteSpace(lines[st]))
                {
                    if (smudgeEnabled)
                    {
                        if (smudgeCount == 1)
                            return (st + 1, end - 1);
                        else
                            return (-1, -1);
                    }
                    else
                    {
                        return (st + 1, end - 1);
                    }
                }
                else if (lines[st] == lines[end])
                {
                    st--;
                    end++;
                }
                else if (smudgeEnabled && IsSmadged(lines[st], lines[end]))
                {
                    if (smudgeCount == 1)
                        return (st, end);
                    else
                    {
                        smudgeCount++;
                        st--;
                        end++;
                    }
                }
                else
                {
                    st = st + 1;
                    end = end - 1;
                    break;
                } 
            }

            if (smudgeEnabled)
            {
                if (smudgeCount == 1)
                    return (st, end);
                else
                    return (-1, -1); 
            }

            return (st, end); 
        }

        private bool IsRowReflectionValid(string[] lines, int startIndex, int endIndex)
        {
            while (startIndex <= endIndex)
            {
                if (lines[startIndex].Trim() == lines[endIndex].Trim())
                {
                    startIndex++;
                    endIndex--;
                }
                else
                    return false;
            }
            return true; 
        }

        private List<(int,int)> ReadAllAreas(string[] lines)
        {
            List<(int,int)> areas = new();
            var startIndex = 0;
            var endIndex = 0;

            foreach (var line in lines)
            {
                if (line == "" || line[0] == ' ')
                {
                    if (endIndex != startIndex)
                    {
                        
                        areas.Add((startIndex, endIndex-1));
                        startIndex = endIndex + 1;
                    }
                }
                endIndex++;
            }

            if (endIndex != startIndex)
            {
                
                areas.Add((startIndex, endIndex-1));
            }

            return areas;
        }
    }
}
