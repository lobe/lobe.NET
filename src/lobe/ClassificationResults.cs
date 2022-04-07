using System.Collections.Generic;

namespace lobe;

public class ClassificationResults
{
    public override string ToString()
    {
        return $"Prediction: {Prediction}";
    }

    public ClassificationResults(Classification prediction, IEnumerable<Classification> classifications)
    {
        Prediction = prediction;
        Classifications = classifications;
    }

    public Classification Prediction { get; }
    public IEnumerable<Classification> Classifications { get; }
}