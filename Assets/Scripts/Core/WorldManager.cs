using Core.DataManagement;
using Core.Serialization;
using Core.Serialization.Binary;
using Core.Systems;
using Core.WorldData;

namespace Core
{
    public class WorldManager
    {
        private World world;
        private UniqueIntGenerator instanceIdGenerator;

        public World World => world;

        public WorldManager()
        {
            instanceIdGenerator = new UniqueIntGenerator();
            world = new World(instanceIdGenerator);
        }

        public ObjectId GetNextObjectId()
        {
            return instanceIdGenerator.GetNextValue();
        }

        public void SaveWorld(IDataWriter saveDestination)
        {
            var serializer = new WorldSerializer(saveDestination);
            world.Serialize(serializer);
        }

        public void LoadWorld(IDataReader reader)
        {
            var serializer = new WorldSerializer(reader);
            world.Serialize(serializer);
        }
    }
}