using System.Collections.Generic;

namespace RLReplayMan
{
    public class PathComparer : IEqualityComparer<BaseViewModel>
    {
        public int GetHashCode(BaseViewModel co)
        {
            if (co == null)
            {
                return 0;
            }
            return co.FullPath.GetHashCode();
        }

        public bool Equals(BaseViewModel x1, BaseViewModel x2)
        {
            if (object.ReferenceEquals(x1, x2))
            {
                return true;
            }
            if (object.ReferenceEquals(x1, null) ||
                object.ReferenceEquals(x2, null))
            {
                return false;
            }
            return x1.FullPath == x2.FullPath;
        }
    }
}
