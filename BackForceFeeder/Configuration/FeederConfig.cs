namespace BackForceFeeder.Configuration
{
    public class FeederConfig
    {
        public ApplicationDB Application;
        public HardwareDB Hardware;
        public ControlSetDB CurrentControlSet;
        public ControlSetsDB AllControlSets;

        /// <summary>
        /// Default values
        /// </summary>
        public FeederConfig()
        {
            Application = new ApplicationDB();
            Hardware = new HardwareDB();
            CurrentControlSet = new ControlSetDB();
            AllControlSets = new ControlSetsDB();
        }


    }
}
