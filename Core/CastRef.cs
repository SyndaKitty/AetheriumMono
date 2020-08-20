namespace AetheriumMono.Core
{
    public struct CastRef<T> where T : GameObject
    {
        public EntityRef<GameObject> EntityRef { get; }

        public CastRef(EntityRef<GameObject> entityRef)
        {
            EntityRef = entityRef;
        }

        public bool Get(out T entity)
        {
            entity = null;
            bool result = EntityRef.Get(out var go);
            if (result) entity = (T) go;
            return result;
        }

        public CastRef<S> Convert<S>() where S : GameObject => new CastRef<S>(EntityRef);
    }
}
