using theori.Charting;
using theori.Charting.IO;

namespace theori.GameModes
{
    public abstract class GameMode
    {
        #region Meta Info

        public readonly string Name;

        #endregion

        #region Standalone Mode

        /// <summary>
        /// Whether this game mode supports running as a standalone experience,
        ///  as if it were the only game installed on the client.
        /// When running in standalone, this game mode will be the only
        ///  playable option and has more control over layers.
        /// </summary>
        public virtual bool SupportsStandaloneUsage => false;

        public virtual void InvokeStandalone(string[] args)
        {
            if (SupportsStandaloneUsage)
                throw new System.Exception($"Game Mode \"{ Name }\" is stated to supprt standalone usage, but no standalone initialization is provided.");
            else throw new System.Exception($"Game Mode \"{ Name }\" does not support standalone usage.");
        }

        #endregion

        #region Shared Mode

        /// <summary>
        /// Whether this game mode supports running in the shared-mode client,
        ///  where all shared games can be freely selected.
        /// </summary>
        public virtual bool SupportsSharedUsage => false;

        public virtual Layer CreateSharedGameLayer()
        {
            if (SupportsSharedUsage)
                throw new System.Exception($"Game Mode \"{ Name }\" is stated to supprt shared usage, but no shared game layer is provided.");
            else throw new System.Exception($"Game Mode \"{ Name }\" does not support shared usage.");
        }

        #endregion

        protected GameMode(string name)
        {
            Name = name;
        }

        public virtual ChartFactory CreateChartFactory() => new ChartFactory();
        public virtual ChartSerializer CreateSerializer(string chartsDir) => new ChartSerializer(chartsDir, this);

        public abstract ChartObjectSerializer GetSerializerByID(int id);
        public abstract ChartObjectSerializer GetSerializerFor(Entity obj);
    }
}
