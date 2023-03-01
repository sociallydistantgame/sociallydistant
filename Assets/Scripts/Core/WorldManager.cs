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
        private DataEventDispatcher eventDispatcher;
        private DataCallbacks dataCallbacks;

        public World World => world;
        public DataCallbacks Callbacks => dataCallbacks;
        
        public WorldManager()
        {
            eventDispatcher = new DataEventDispatcher();
            instanceIdGenerator = new UniqueIntGenerator();
            dataCallbacks = new DataCallbacks(eventDispatcher);
            
            world = new World(instanceIdGenerator, eventDispatcher);
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

        public void WipeWorld()
        {
            this.world.Wipe();
        }
    }
}