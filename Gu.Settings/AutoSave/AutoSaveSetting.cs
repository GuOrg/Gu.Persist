namespace Gu.Settings
{
    using System;
    using System.IO;

    internal sealed class AutoSaveSetting
    {
        private AutoSaveSetting(AutoSaveMode mode, TimeSpan time, FileInfo file)
        {
            Ensure.NotNull(file, nameof(file));
            Mode = mode;
            Time = time;
            File = file;
        }

        public AutoSaveMode Mode { get; private set; }

        public TimeSpan Time { get; private set; }

        public FileInfo File { get; private set; }

        /// <summary>
        /// Saves automatically every x seconds
        /// </summary>
        /// <param name="saveEvery"></param>
        /// <param name="file">Information about file to save and backup</param> 
        /// <returns></returns>
        public static AutoSaveSetting OnSchedule(TimeSpan saveEvery, FileInfo file)
        {
            return new AutoSaveSetting(AutoSaveMode.OnSchedule, saveEvery, file);
        }

        /// <summary>
        /// Saves on propertychange
        /// </summary>
        /// <param name="file">Information about file to save and backup</param>
        /// <returns></returns>
        public static AutoSaveSetting OnChanged(FileInfo file)
        {
            return new AutoSaveSetting(AutoSaveMode.OnChanged, TimeSpan.Zero, file);
        }

        /// <summary>
        /// Saves on propertychange but waits buffertime after last change before saving.
        /// </summary>
        /// <param name="bufferTime"></param>
        /// <param name="maxTime"></param>
        /// <param name="file">Information about file to save and backup</param> 
        /// <returns></returns>
        public static AutoSaveSetting Deferred(TimeSpan bufferTime, TimeSpan maxTime, FileInfo file)
        {
            return new AutoSaveSetting(AutoSaveMode.Deferred, bufferTime, file);
        }
    }
}