using System;
using System.Collections.Generic;

namespace BRGEngine.SDK {

    public class StrategyExecutor<T> {

        private List<T> collection;
        private Action<T> executeAction;

        public StrategyExecutor(List<string> behavioursTypeNames, Action<T> executeAction) {
            collection = new List<T>();
            this.executeAction = executeAction;
            foreach (string typeName in behavioursTypeNames) {
                string fullTypeName = GetFullTypeName(typeName);
                T behaviorInstance = CreateInstance(fullTypeName);
                if (behaviorInstance != null) {
                    collection.Add(behaviorInstance);
                }
            }
        }

        public void ExecuteMethods() {
            foreach (T behaviour in collection) {
                executeAction(behaviour);
            }
        }

        private T CreateInstance(string className) {
            if (string.IsNullOrEmpty(className)) return default(T);

            Type type = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                type = asm.GetType(className);
                if (type != null) break;
            }

            if (type == null || !typeof(T).IsAssignableFrom(type)) return default(T);

            return (T)Activator.CreateInstance(type);
        }

        private string GetFullTypeName(string className) {
            if (string.IsNullOrEmpty(className)) return className;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in asm.GetTypes()) {
                    if (type.Name == className && typeof(T).IsAssignableFrom(type)) {
                        return type.FullName;
                    }
                }
            }
            return className; // Restituisce il nome originale se non trova nulla
        }
    }
}
