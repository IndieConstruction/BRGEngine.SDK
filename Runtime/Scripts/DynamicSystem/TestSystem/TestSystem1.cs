using System;
using UnityEngine;
using BRGEngine.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace BRGEngine.SDK {

    /// <summary>
    /// Test system 1 for dynamic systems testing.
    /// </summary>
    // [CreateAssetMenu(fileName = "Test1Config", menuName = "BRGEngine/Systems/DynamicTest1")]
    public class TestSystem1 : BaseSystem {

        public string TestString = "";

        public override void Init() {
            Debug.Log($"Init: {TestString}");
        }

    }

}