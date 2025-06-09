using System;
using UnityEngine;
using BRGEngine.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace BRGEngine.SDK {

    /// <summary>
    /// Test system 2 for dynamic systems testing.
    /// </summary>
    // [CreateAssetMenu(fileName = "Test2Config", menuName = "BRGEngine/Systems/DynamicTest2")]
    public class TestSystem2 : BaseSystem {

        public string TestString = "";

        public override void Init() {
            Debug.Log($"Init: {TestString}");
        }

    }

}