namespace theori.Scoring
{
    /// <summary>
    /// Provides a wide array of possible judgements with arbitrary names.
    /// Their numeric values are increasing as they get closer to a perfect hit,
    ///  and ScoreResult.None is the 0 value.
    ///  
    /// To be more applicable to a wider range of rhythm game scoring systems, there are 2
    ///  categories with 3 values each plus a Miss value.
    /// The "Bad" category holds Bad, Okay and Close results.
    /// The "Good" category holds Good, Great and Perfect results.
    /// There is nothing better than a perfect, and anything worse than the Bad category should be a miss.
    /// 
    /// If your game needs even more granular scoring, one solution is to simply check the
    ///  <see cref="Judgement.JudgementResult.TimeOffset"/> manually; if it's a more serious issue,
    ///  contact a designer or programmer about it.
    /// If your game needs less granular scoring, these can easily be configured to be ignored.
    /// </summary>
    public enum ScoreResult
    {
        None,

        Miss,

        Bad,
        Okay,
        Close,

        Good,
        Great,
        Perfect,
    }
}
