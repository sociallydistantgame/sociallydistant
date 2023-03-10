#nullable enable

using Core.Serialization;

namespace Core.WorldData
{
    public class WorldRevisionComparer : IRevisionComparer<WorldRevision>
    {
        private WorldRevision currentRevision;

        /// <inheritdoc />
        public WorldRevision Earliest => WorldRevision.Begin;

        /// <inheritdoc />
        public WorldRevision Latest => WorldRevision.Latest - 1;

        /// <inheritdoc />
        public WorldRevision Current => currentRevision;

        public WorldRevisionComparer()
        {
            this.currentRevision = this.Latest;
        }

        public WorldRevisionComparer(WorldRevision currentRevision)
        {
            this.currentRevision = currentRevision;
        }

        /// <inheritdoc />
        public bool IsCurrent(WorldRevision revision)
        {
            return revision == Current;
        }

        /// <inheritdoc />
        public bool IsCurrentOrNewer(WorldRevision revision)
        {
            return Current >= revision;
        }

        /// <inheritdoc />
        public bool IsCurrentOrOlder(WorldRevision revision)
        {
            return Current <= revision;
        }

        /// <inheritdoc />
        public bool IsNewer(WorldRevision revision)
        {
            return Current > revision;
        }

        /// <inheritdoc />
        public bool IsOlder(WorldRevision revision)
        {
            return Current < revision;
        }
    }
}