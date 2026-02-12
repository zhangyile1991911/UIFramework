namespace Framework.UIFramework
{
    public abstract class UIOpenParam
    {
        public T Get<T>() where T : UIOpenParam
        {
            return this as T;
        }
    }
}