using System;

namespace RLReplayMan
{
    public static class Globals
    {
        public static string RL_REPLAY_FOLDER_PATH
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                    "\\Documents\\My Games\\Rocket League\\TAGame\\Demos";
            }
        }
        public const string RL_REPLAY_EXTENSION = ".replay";
    }
}
