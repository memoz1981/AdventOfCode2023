using Common.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers
{
    [Route("day15")]
    [ApiController]
    public class _15_Day15_Controller : ControllerBase
    {
        [HttpGet("exercise1")]
        public IActionResult Exercise1()
        {
            string readText = System.IO.File.ReadAllText("data15.txt");

            var stringList = readText.Split(',').Where(m => m != "" && !m.Equals('\n')).ToList();

            int result = 0;

            foreach (var el in stringList)
            
            {
                result += GetHash(el); 
            }

            return Ok(result); 
        }

        private int GetHash(string str)
        {
            int result = 0;

            for (int i = 0; i < str.Length; i++)
            {
                result = (((int)(str[i]) + result) * 17) % 256; 
            }
            return result; 
        }

        [HttpGet("exercise2")]
        public IActionResult Exercise2()
        {
            string readText = System.IO.File.ReadAllText("data15.txt");

            var stringList = readText.Split(',').Where(m => m != "" && !m.Equals('\n')).ToList();

            int result = 0;

            var boxes = new Dictionary<int, Box>();

            for (int i = 0; i < 255; i++)
            {
                boxes.Add(i, new Box(i)); 
            }

            foreach (var step in stringList)
            {
                PerformSingleStep(boxes, step); 
            }

            var builder = new StringBuilder();
            int resultNew = 0; 
            for (int i = 0; i < 255; i++)
            {
                if (!boxes[i].IsEmpty())
                {
                    builder.AppendLine(boxes[i].ToString());
                    resultNew += boxes[i].ReturnValue(); 
                }
                 
            }

            return Ok(resultNew);
        }

        private void PerformSingleStep(Dictionary<int, Box> boxes, string step)
        {
            var trydash = step.Split('-').ToList();

            if (trydash.Count == 2)
            {
                var hash = GetHash(trydash.First());
                boxes[hash].PerformDashOperation(trydash.First());
                return; 
            }

            var tryEquals = step.Split('=').ToList();

            if (tryEquals.Count == 2)
            {
                var hash = GetHash(tryEquals.First());
                boxes[hash].PerformEqualsOperation(tryEquals.First(), int.Parse(tryEquals.Last()));
                return; 
            }
            
            throw new ArgumentException();         
        }

    }

    public class Box
    {
        public Box(int index)
        {
            Index = index;
            _lensesByLabel = new();
            Head = null;
        }

        public int ReturnValue()
        {
            int lensIndex = 1;
            int result = 0;
            var previous = Head;
            while (previous != null)
            {
                result += (Index +1) * lensIndex * previous.FocalLength;
                lensIndex++;
                previous = previous.Backward; 
            }
            return result; 
        }

        public bool IsEmpty() => !_lensesByLabel.Any(); 

        private Dictionary<string, Lens> _lensesByLabel; 

        private Lens Head { get; set; }

        public void PerformEqualsOperation(string label, int focalLength)
        {
            if (_lensesByLabel.TryGetValue(label, out var lens))
            {
                lens.FocalLength = focalLength;
                return; 
            }

            if (Head == null)
            {
                Head = new Lens(label, focalLength);
                _lensesByLabel.Add(label, Head);
                return; 
            }
            var previous = Head;
            while (previous.Backward != null)
                previous = previous.Backward;

            var lensNew = new Lens(label, focalLength, previous);
            previous.Backward = lensNew;
            lensNew.Forward = previous; 
            _lensesByLabel.Add(label, lensNew); 
        }

        public void PerformDashOperation(string label)
        {
            if (!_lensesByLabel.TryGetValue(label, out var lens))
            {
                return; 
            }

            if (lens == Head)
            {
                if (lens.Backward == null)
                {
                    Head = null;
                    _lensesByLabel = new();
                }
                else
                {
                    Head = lens.Backward;
                    lens.Backward.Forward = null; 
                    _lensesByLabel.Remove(label);
                }
                
                return; 
            }

            lens.Forward.Backward = lens.Backward;

            if (lens.Backward != null)
                lens.Backward.Forward = lens.Forward;
            _lensesByLabel.Remove(label);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{Index} {ReturnValue()}: ");

            var previous = Head;

            while (previous != null)
            {
                builder.Append(previous.ToString());
                previous = previous.Backward; 
            }

            return builder.ToString(); 
        }

        public int Index { get; set; }
        private class Lens
        {
            public Lens(string label, int focalLength, Lens parent = null)
            {
                FocalLength = focalLength;
                Label = label;
                Forward = parent;
                Backward = null; 
            }

            public string Label { get; set; }

            public int FocalLength { get; set; }

            public Lens Forward { get; set; }

            public Lens Backward { get; set; }

            public void UpdateFocalLength(int newFocalLength)
            {
                FocalLength = newFocalLength; 
            }

            public void UpdateParent(Lens parent)
            {
                Forward = parent;
            }

            public void UpdateChild(Lens child)
            {
                Backward = child;
            }

            public override string ToString()
            {
                return $"[{Label} {FocalLength}]";
            }
        }
    }
}
