using System;
using System.Collections.Generic;

namespace BRGEngine.SDK {

    public interface ISystem {
        void InitInternal(ISystem parentSystem);
        void Init();
        void Finish();

        bool RequiresNewInstance { get; }

        bool AutoInitAtStartup { get; }

        List<BaseSystem> SubSystems { get; }

        /// <summary>
        /// Called when all sistems are ready and any init is done. Override if needed.
        /// </summary>
        void OnInitializationComplete();
    }

}