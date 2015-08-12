namespace Gu.Settings.Core
{
    using System;

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
    public class TrackingAttribute : Attribute
    {
        public readonly TrackAs TrackAs;

        public TrackingAttribute(TrackAs trackAs)
        {
            TrackAs = trackAs;
        }
    }
}
