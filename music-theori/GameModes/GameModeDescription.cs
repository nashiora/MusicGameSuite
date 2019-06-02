namespace theori.GameModes
{
    public abstract class GameModeDescription
    {
        #region Meta Info

        public readonly string Name;

        #endregion

        #region Standalone Mode

        /// <summary>
        /// Whether this game mode supports its own standalone startup,
        ///  as if the game mode were its own game instead.
        /// When running in standalone, this game mode will be the only
        ///  playable option and has more control over layers.
        /// </summary>
        public virtual bool SupportsStandaloneStartup => false;

        #endregion

        #region Shared Mode

        #endregion

        protected GameModeDescription(string name)
        {
            Name = name;
        }
    }
}
