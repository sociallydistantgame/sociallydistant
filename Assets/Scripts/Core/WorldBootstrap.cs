using System;
using System.Collections;
using System.Collections.Generic;
using Core.Config;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace Core
{
    public class WorldBootstrap : MonoBehaviour
    {
        [SerializeField]
        private WorldManagerHolder worldHolder = null!;

        private void Awake()
        {
            Registry.Initialize();
            
            Debug.Log(@"It's time for the moment you've been waiting for. 

Doo...

 Do do do do do do do
Do do do do do do do do do do do do do do dooooo
Do do do do do do dooooo

Do telolet do do do do do do do do do doooooo
Do do do do do do do do doooo
Tsss
Do do do do do do do dooooo
Do doooo, do dooooo
Di do di di do di di do
Telolet ta ta ta ta
Telolet ta ta ta ta
Telolet ta ta ta taaaaaaaaaaaaaaa

Di dotily dotility doooooo
Di do di li

[breath]

Di do di li do di li do di li do di li do di li do de di da de di da de di doooooooooooooooo

PREPARING THE WORLD MANAGER!");
            
            DontDestroyOnLoad(this.gameObject);
            
            this.AssertAllFieldsAreSerialized(typeof(WorldBootstrap));

            this.worldHolder.Value = new WorldManager();
        }

        private void Update()
        {
            worldHolder.Value.UpdateWorldClock();
        }
    }
}