namespace theori.Resources
{
    // TODO(local): figure out if it might be important to cancel loading operations?
    // If so, guarantee that it's easy for code to clean up half-loaded state.
    public interface IAsyncLoadable
    {
        bool AsyncLoad();
        bool AsyncFinalize();
    }
}
