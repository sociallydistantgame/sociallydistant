using System;
using Core.DataManagement;
using Core.Serialization;
using Core.Serialization.Binary;
using Core.Systems;
using Core.WorldData;
using Core.WorldData.Data;
using UnityEngine;

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

            // Set a default time scale if we loaded a save with a time scale of 0.
            if (World.GlobalWorldState.Value.TimeScale == 0)
            {
                GlobalWorldData worldData = world.GlobalWorldState.Value;
                worldData.TimeScale = 60; // 60 in-game seconds per one real second.
                World.GlobalWorldState.Value = worldData;
            }
        }

        public void WipeWorld()
        {
            this.world.Wipe();
        }

        internal void UpdateWorldClock()
        {
            float clockDelta = Time.deltaTime * World.GlobalWorldState.Value.TimeScale;
            if (clockDelta == 0)
                return;

            // Suffice to say...never ever ever, FUCKING EVER, register for a ModifyEvent on this
            // data type because you will get one every frame. Calling these data events is not free.
            GlobalWorldData worldData = world.GlobalWorldState.Value;
            DateTime now = worldData.Now;
            worldData.Now = now.AddSeconds(clockDelta);
            World.GlobalWorldState.Value = worldData;
        }
    }
}