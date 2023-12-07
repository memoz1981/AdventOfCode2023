using System.Text;

namespace Common.Services
{
    public class StringLineReader
    {
        public List<string> ReadLines(string fileName)
        {
            var lines = new List<string>();
            using (var fileStream = File.OpenRead(fileName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                    lines.Add(line);
            }
            return lines; 
        }

        public char[][] ReadArray(string fileName)
        {
            var lines = new List<char[]>();
            using (var fileStream = File.OpenRead(fileName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                    lines.Add(line.ToArray());
            }
            return lines.ToArray();
        }
    }
}
