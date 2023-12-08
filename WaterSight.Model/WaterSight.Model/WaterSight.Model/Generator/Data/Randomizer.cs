using System;

namespace WaterSight.Model.Generator.Data;

public class Randomizer
{
    #region Constructor
    public Randomizer(int randomSeed)
    {
        // If user provides a non-zero seed, they will get the same values for each subsequent run of the tool
        // (provided all inputs are identical and the tool code has not been updated in the meantime).
        // Otherwise, each run of the tool will produce different results.
        
        //if (randomSeed > 0)
        //    Random = new Random(randomSeed);
        //else
        //    Random = new Random();


        Random = randomSeed > 0 
            ? new Random(randomSeed) 
            : new Random();
    }
    #endregion

    #region Public Methods
    public double RandomBetween(double min, double max, bool weightTowardMidpoint = false)
    {
        if (weightTowardMidpoint)
            // This approach creates a roughly linear/triangular distribution
            // where the averge value is the most likely value and the min/max are least likely.
            return (Random.NextDouble() - Random.NextDouble()) * ((max - min) / 2) + ((max + min) / 2);
        else
            return (max - min) * Random.NextDouble() + min;
    }
    #endregion

    #region Public Properties
    public Random Random { get; }
    #endregion
}
