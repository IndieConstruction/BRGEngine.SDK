using System;
using UnityEngine;
using BRGEngine.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace BRGEngine.SDK {

    /// <summary>
    /// This system choose dynamically a system from a list of systems to be added to the BRG systems list.
    /// 
    /// <see href="">Documentation</see>
    /// </summary>
    [CreateAssetMenu(fileName = "DynamicSystemConfig", menuName = "BRGEngine/Systems/Dynamic")]
    public class DynamicSystem : BaseSystem {

        [SerializeField] private List<BaseSystem> systems;

        public override void Init() {
             
        }

        public override void PreInit() {
            ISystem newSystem = systems[0];
            SubSystems.Add(newSystem as BaseSystem);
        }
    }

}