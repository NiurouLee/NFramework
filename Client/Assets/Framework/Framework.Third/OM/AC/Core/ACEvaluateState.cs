namespace OM.AC
{
    /// <summary>
    /// Evaluate state of the clip
    /// </summary>
    public enum ACEvaluateState
    {
        None = 0, // Not Started || Waiting
        Running = 1, // Running
        Finished = 2, // Finished after completing the clip duration
        Failed = 3 // Failed 
    }
}