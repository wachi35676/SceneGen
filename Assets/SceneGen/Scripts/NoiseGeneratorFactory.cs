using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneGen.Scripts
{
    public class NoiseGeneratorFactory
    {
        private static NoiseGeneratorFactory noiseSingleton;
        private Dictionary<bool, Type> noiseGeneratorTypes;

        private NoiseGeneratorFactory()
        {
            //Initializing the dictionary 
            noiseGeneratorTypes = new Dictionary<bool, Type>()
            {
                { true, typeof(PerlinNoise) },
                { true, typeof(CellularNoise) },
                { true, typeof(SimplexNoise) }
            };
        }

        public static NoiseGeneratorFactory Instance
        {
            get
            {
                //If the singleton doesn't exist create it, otherwise make a universal instance referenced by all other classess
                if (noiseSingleton == null)
                {
                    noiseSingleton = new NoiseGeneratorFactory();
                }

                return noiseSingleton;
            }
        }

        public INoiseGenerator GetNoiseGenerator(bool PerlinNoise)
        {
            if (noiseGeneratorTypes.TryGetValue(PerlinNoise, out Type noiseGeneratorType))
            {
                return (INoiseGenerator)Activator.CreateInstance(noiseGeneratorType);
            }
            
            return null;
        }
    }
}