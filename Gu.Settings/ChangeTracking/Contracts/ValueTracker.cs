namespace Gu.Settings
{
    public abstract class ValueTracker : ChangeTracker, IValueTracker
    {
        protected ValueTracker(object value)
        {
            Ensure.NotNull(value, "value");
            Value = value;
        }

        public object Value { get; private set; }
    }
}