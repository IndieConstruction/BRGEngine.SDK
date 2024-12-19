using System.Collections.Generic;
using UnityEngine;

namespace BRGEngine.SDK {

    /// <summary>
    /// This scriptable object is base type for any system.
    /// Extra options to config system can be placed in class derived from this class.
    /// Add menu creation attributes [CreateAssetMenu(menuName = "BRGEngine/Systems/Test", fileName = "TestSystemConfig")].
    /// </summary>
    public abstract class BaseSystem : ScriptableObject, ISystem {


        public bool RequiresNewInstance {
            get { return _requiresNewInstance; }
            set { _requiresNewInstance = value; }
        }
        [SerializeField] private bool _requiresNewInstance = true;


        public bool AutoInitAtStartup {
            get { return _autoInitAtStartup; }
            set { _autoInitAtStartup = value; }
        }
        private bool _autoInitAtStartup = true;


        public ISystem ParentSystem {
            get { return _parentSystem; }
            private set { _parentSystem = value; }
        }
        private ISystem _parentSystem;


        public List<ISystem> SubSystems {
            get { return _subSystems.ConvertAll(s => (ISystem)s); }
            set { _subSystems = value.ConvertAll(s => (BaseSystem)s); }
        }

        [SerializeField]
        private List<BaseSystem> _subSystems = new List<BaseSystem>();


        public void InitInternal(ISystem parentSystem = null) {
            ParentSystem = parentSystem;
            Init();
        }

        /// <summary>
        /// Override with init functionalities.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Called when finish system lifecycle. Override if needed.
        /// </summary>
        public virtual void Finish() { }
    }

}