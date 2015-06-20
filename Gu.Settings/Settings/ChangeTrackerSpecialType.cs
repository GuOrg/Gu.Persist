namespace Gu.Settings
{
    using System;

    [Tracking(TrackAs.Explicit)]
    public class ChangeTrackerSpecialType
    {
        public ChangeTrackerSpecialType(Type type, TrackAs trackAs)
        {
            Ensure.NotNull(type, "type");
            TypeName = type.FullName;
            TrackAs = trackAs;
        }

        public string TypeName { get; set; }
        public TrackAs TrackAs { get; set; }
    }
}