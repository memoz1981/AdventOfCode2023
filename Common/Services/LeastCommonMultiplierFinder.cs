namespace Common.Services
{
    public class LeastCommonMultiplierFinder
    {
        static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long LCM(long a, long b)
        {
            return (a * b) / GCD(a, b);
        }

        static long CalculateLCM(long[] numbers)
        {
            long lcm = 1;
            foreach (long number in numbers)
            {
                lcm = LCM(lcm, number);
            }
            return lcm;
        }

        public static long Calculate(params long[] numbers)
        {
              return CalculateLCM(numbers);
        }
    }
}
