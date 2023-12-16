using UnityEngine;

namespace Vecerdi.Logging.Unity {
    public class UnityContext : IContext {
        public Object Context { get; }
        public UnityContext(Object context) => this.Context = context;
        public static implicit operator UnityContext(Object context) => new(context);
    }

    public static class UnityContextExtensions {
        public static IContext ToContext(this Object context) => new UnityContext(context);
        public static IContext Ctx(this Object context) => new UnityContext(context);
    }
}