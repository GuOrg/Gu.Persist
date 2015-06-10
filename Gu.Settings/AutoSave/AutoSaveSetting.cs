namespace Gu.Settings
{
    using System;

    public sealed class AutoSaveSetting
    {
        private AutoSaveSetting(AutoSaveMode mode, TimeSpan time, FileInfos files)
        {
            Ensure.NotNull(files, "files");
            Ensure.NotNull(files.File, "files");
            Mode = mode;
            Time = time;
            Files = files;
        }

        public AutoSaveMode Mode { get; private set; }

        public TimeSpan Time { get; private set; }

        public FileInfos Files { get; private set; }

        /// <summary>
        /// Saves automatically every x seconds
        /// </summary>
        /// <param name="saveEvery"></param>
        /// <param name="fileInfos">Information about file to save and backup</param> 
        /// <returns></returns>
        public static AutoSaveSetting OnSchedule(TimeSpan saveEvery, FileInfos fileInfos)
        {
            return new AutoSaveSetting(AutoSaveMode.OnSchedule, saveEvery, fileInfos);
        }

        /// <summary>
        /// Saves on propertychange
        /// </summary>
        /// <param name="fileInfos">Information about file to save and backup</param>
        /// <returns></returns>
        public static AutoSaveSetting OnChanged(FileInfos fileInfos)
        {
            return new AutoSaveSetting(AutoSaveMode.OnChanged, TimeSpan.Zero, fileInfos);
        }

        /// <summary>
        /// Saves on propertychange but waits buffertime after last change before saving.
        /// </summary>
        /// <param name="bufferTime"></param>
        /// <param name="fileInfos">Information about file to save and backup</param> 
        /// <returns></returns>
        public static AutoSaveSetting Deferred(TimeSpan bufferTime, TimeSpan maxTime, FileInfos fileInfos)
        {
            return new AutoSaveSetting(AutoSaveMode.Deferred, bufferTime, fileInfos);
        }
    }
}