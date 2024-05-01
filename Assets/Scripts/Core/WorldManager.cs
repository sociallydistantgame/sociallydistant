using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.DataManagement;
using Core.Serialization;
using Core.Serialization.Binary;
using Core.Systems;
using Core.WorldData;
using Core.WorldData.Data;
using GamePlatform;
using OS.Network;
using UnityEngine;
using System.Threading.Tasks;

namespace Core
{
    public class WorldManager : 
        IWorldManager, 
        IDisposable
    {
        private static readonly Singleton<WorldManager> singleton = new();
        
        private World world;
        private UniqueIntGenerator instanceIdGenerator;
        private DataEventDispatcher eventDispatcher;
        private DataCallbacks dataCallbacks;

        public World World => world;
        public DataCallbacks Callbacks => dataCallbacks;

        /// <inheritdoc />
        IWorldDataCallbacks IWorldManager.Callbacks => this.dataCallbacks;
        
        /// <inheritdoc />
        IWorld IWorldManager.World => this.world;
        
        public WorldManager(GameManager gameManager)
        {
            singleton.SetInstance(this);
            
            eventDispatcher = new DataEventDispatcher(gameManager);
            instanceIdGenerator = new UniqueIntGenerator();
            dataCallbacks = new DataCallbacks(eventDispatcher);
            
            world = new World(instanceIdGenerator, eventDispatcher);
        }

        public void Dispose()
        {
            singleton.SetInstance(null);
        }
        
        public ObjectId GetNextObjectId()
        {
            return instanceIdGenerator.GetNextValue();
        }

        public void SaveWorld(IDataWriter saveDestination)
        {
            var serializer = new WorldSerializer(saveDestination);
            SaveWorldInternal(world, serializer);
        }

        private void SaveWorldInternal(IWorld worldToSave, WorldSerializer worldSerializer)
        {
            worldToSave.Serialize(worldSerializer);
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

        public string GetNextIspRange()
        {
            string[] takenRanges = world.InternetProviders.Select(x => x.CidrNetwork)
                .Distinct().ToArray();

            var stringBuilder = new StringBuilder(20);

            byte octet1 = 0;
            byte octet2 = 0;

            do
            {
                stringBuilder.Length = 0;

                octet1 = (byte) UnityEngine.Random.Range(20, 252);
                octet2 = (byte) UnityEngine.Random.Range(12, 253);

                stringBuilder.Append(octet1);
                stringBuilder.Append('.');
                stringBuilder.Append(octet2);
                stringBuilder.Append(".0.0/16");
            } while (octet1 == 127 || octet1 == 192 || takenRanges.Contains(stringBuilder.ToString()));

            return stringBuilder.ToString();
        }

        /// <inheritdoc />
        public uint GetNextPublicAddress(ObjectId ispId)
        {
            WorldInternetServiceProviderData internetServiceProvider = world.InternetProviders.First(x => x.InstanceId == ispId);

            if (!NetUtility.TryParseCidrNotation(internetServiceProvider.CidrNetwork, out Subnet subnet))
                throw new InvalidOperationException("ISP has an invalid subnet string. Corrupted world?");

            uint subnetMask = subnet.mask;
            uint maxAddress = ~subnetMask - 1;

            uint playerAddress = 0;
            if (world.PlayerData.Value.PublicNetworkAddress != 0 && world.PlayerData.Value.PlayerInternetProvider == ispId)
                playerAddress = world.PlayerData.Value.PublicNetworkAddress;
            
            uint[] takenAddresses = world.LocalAreaNetworks.Where(x => x.ServiceProviderId == ispId)
                .Select(x => x.PublicNetworkAddress)
                .Distinct()
                .ToArray();

            uint result = 0;

            do
            {
                result = (uint) UnityEngine.Random.Range(2u, maxAddress);
            } while (takenAddresses.Contains(result) || result == playerAddress);

            return result;
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

        public async Task RestoreWorld(IWorld worldToRestore)
        {
            if (worldToRestore == this.world)
            {
                Debug.LogWarning("Attempting to restore the current world as a snapshot. Don't.");
                return;
            }

            this.WipeWorld();
            
            // first we serialize the snapshot into RAM
            using var memory = new MemoryStream();

            await Task.Run(() =>
            {
                using var binaryWriter = new BinaryWriter(memory, Encoding.UTF8, true);
                using var dataWriter = new BinaryDataWriter(binaryWriter);

                var serializer = new WorldSerializer(dataWriter);
                SaveWorldInternal(worldToRestore, serializer);
            });

            memory.Seek(0, SeekOrigin.Begin);
            
            // Now we can load it from RAM as a copy!
            await Task.Run(() =>
            {
                using var binaryReader = new BinaryReader(memory, Encoding.UTF8, true);
                using var dataReader = new BinaryDataReader(binaryReader);

                var serializer = new WorldSerializer(dataReader);

                this.world.Serialize(serializer);
            });
        }
        
        public static WorldManager Instance => singleton.MustGetInstance();
    }
}