namespace Gu.Settings
{
    using System;

    public class AutoSaveSetting
    {
        private AutoSaveSetting(AutoSaveMode mode, TimeSpan time, string fileName)
        {
            Mode = mode;
            Time = time;
            FileName = fileName;
        }

        public AutoSaveMode Mode { get; private set; }

        public TimeSpan Time { get; private set; }

        public string FileName { get; set; }

        /// <summary>
        /// Saves automatically every x seconds
        /// </summary>
        /// <param name="saveEvery"></param>
        /// <param name="fileName">If left empty the filename is the name of the type</param>
        /// <returns></returns>
        public static AutoSaveSetting OnSchedule(TimeSpan saveEvery, string fileName = null)
        {
            return new AutoSaveSetting(AutoSaveMode.OnSchedule, saveEvery, fileName);
        }

        /// <summary>
        /// Saves on propertychange
        /// </summary>
        /// <param name="fileName">If left empty the filename is the name of the type</param>
        /// <returns></returns>
        public static AutoSaveSetting OnChanged(string fileName = null)
        {
            return new AutoSaveSetting(AutoSaveMode.OnChanged, TimeSpan.Zero, fileName);
        }

        /// <summary>
        /// Saves on propertychange but waits buffertime after last change before saving.
        /// </summary>
        /// <param name="bufferTime"></param>
        /// <param name="fileName">If left empty the filename is the name of the type</param>
        /// <returns></returns>
        public static AutoSaveSetting Deferred(TimeSpan bufferTime, string fileName = null)
        {
            return new AutoSaveSetting(AutoSaveMode.Deferred, bufferTime, fileName);
        }
    }
}