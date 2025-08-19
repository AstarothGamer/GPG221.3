using System.Collections.Generic;
using Resource;

namespace Goap
{
    public static class ResourceReservationService
    {
        private static readonly Dictionary<Resource.Resource, object> owners = new();

        public static bool TryReserve(Resource.Resource res, object owner)
        {
            if (res == null) return false;
            if (owners.ContainsKey(res)) return false;
            owners[res] = owner ?? new object();
            return true;
        }

        public static void Release(Resource.Resource res, object owner)
        {
            if (res == null) return;
            if (owners.TryGetValue(res, out var o) && (owner == null || ReferenceEquals(o, owner)))
                owners.Remove(res);
        }

        public static bool IsReserved(Resource.Resource res) => res != null && owners.ContainsKey(res);
    }
}
