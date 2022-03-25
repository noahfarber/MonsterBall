
namespace BZFramework.Math
{
    public interface IBZMathOutcomeKey
    {
        string Value { get; }
    }

    public interface IBZMathOutcome
    {
        IBZMathOutcomeKey Key { get; }
        int TotalAward { get; }

    }

    public interface IBZMathWeight
    {
        IBZMathOutcomeKey Key { get; }
        int Weight { get; }
    }

    public interface IBZMathRandom
    {
        void Reseed(int aSeed);  //  Where supported, reinitializes the RNG with the provided seed
        int Rand(int aMinVal, int aMaxVal);  //  Return a random selection range aMinVal <= num <= aMaxVal
    }

    public interface IBZMathWeightTable
    {
        IBZMathWeight SelectRandom();  //  Pick one of the defined keys in the table
    }

    static class BZMathExtensions
    {
        public static int Next(this IBZMathRandom rng, int anUpperLimit)
        {
            return rng.Rand(0, anUpperLimit - 1);  //  Acts like standard Next function with one parameter, range 0 <= num < anUpperLimit
        }
    }

}