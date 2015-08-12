namespace Gu.Settings.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;

    public class ChangeTrackerSettings
    {
        private ObservableCollection<ChangeTrackerSpecialType> _specialTypes = new ObservableCollection<ChangeTrackerSpecialType>();

        public static readonly ChangeTrackerSettings Default = CreateDefault();

        public ObservableCollection<ChangeTrackerSpecialType> SpecialTypes
        {
            get { return _specialTypes; }
            set { _specialTypes = value; }
        }

        public void AddSpecialType<T>(TrackAs trackas)
        {
            AddSpecialType(typeof(T), trackas);
        }

        public void AddSpecialType(Type type, TrackAs trackas)
        {
            SpecialTypes.Add(new ChangeTrackerSpecialType(type, trackas));
        }

        private static ChangeTrackerSettings CreateDefault()
        {
            var settings = new ChangeTrackerSettings();
            settings.AddSpecialType<FileInfo>(TrackAs.Explicit);
            settings.AddSpecialType<DirectoryInfo>(TrackAs.Explicit);
            settings.AddSpecialType<Type>(TrackAs.Immutable);
            settings.AddSpecialType<CultureInfo>(TrackAs.Immutable);
            settings.AddSpecialType<DateTime>(TrackAs.Immutable);
            settings.AddSpecialType<DateTime?>(TrackAs.Immutable);
            settings.AddSpecialType<DateTimeOffset>(TrackAs.Immutable);
            settings.AddSpecialType<DateTimeOffset?>(TrackAs.Immutable);
            settings.AddSpecialType<TimeSpan>(TrackAs.Immutable);
            settings.AddSpecialType<TimeSpan?>(TrackAs.Immutable);
            settings.AddSpecialType<string>(TrackAs.Immutable);
            settings.AddSpecialType<double?>(TrackAs.Immutable);
            settings.AddSpecialType<float?>(TrackAs.Immutable);
            settings.AddSpecialType<decimal?>(TrackAs.Immutable);
            settings.AddSpecialType<int?>(TrackAs.Immutable);
            settings.AddSpecialType<uint?>(TrackAs.Immutable);
            settings.AddSpecialType<long?>(TrackAs.Immutable);
            settings.AddSpecialType<ulong?>(TrackAs.Immutable);
            settings.AddSpecialType<short?>(TrackAs.Immutable);
            settings.AddSpecialType<ushort?>(TrackAs.Immutable);
            settings.AddSpecialType<sbyte?>(TrackAs.Immutable);
            settings.AddSpecialType<byte?>(TrackAs.Immutable);
            return settings;
        }
    }
}
