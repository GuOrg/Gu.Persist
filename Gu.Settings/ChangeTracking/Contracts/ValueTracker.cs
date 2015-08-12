namespace Gu.Settings
{
    public abstract class ValueTracker : ChangeTracker, IValueTracker
    {
        protected ValueTracker(object value)
        {
            Ensure.NotNull(value, nameof(value));
            Value = value;
        }

        public object Value { get; private set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}