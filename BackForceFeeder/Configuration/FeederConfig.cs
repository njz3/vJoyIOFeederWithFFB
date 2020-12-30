namespace BackForceFeeder.Configuration
{
    /// <summary>
    /// Runtime configuration of whole application
    /// </summary>
    public class FeederConfig
    {
        /// <summary>
        /// Application (one file to deserialize)
        /// </summary>
        public ApplicationDB Application;
        /// <summary>
        /// Hardware (one file to deserialize)
        /// </summary>
        public HardwareDB Hardware;
        /// <summary>
        /// All control sets (multiple files to deserialize)
        /// </summary>
        public ControlSetsDB AllControlSets;

        
        /// <summary>
        /// Default values
        /// </summary>
        public FeederConfig()
        {
            Application = new ApplicationDB();
            Hardware = new HardwareDB();
            
            AllControlSets = new ControlSetsDB();
        }
       

    }
}
