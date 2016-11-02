﻿namespace Gu.Persist.Demo
{
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Git;
    using Gu.Persist.RuntimeXml;

    public class MyRepository
    {
        private static readonly DirectoryInfo Directory = new DirectoryInfo("./Settings");
        private readonly SingletonRepository repository;

        public MyRepository()
        {
            // Initializes with  ./Settings/RepositorySettings.cfg is present
            // Creates a git repository for history.
            this.repository = new SingletonRepository(
                                  CreateDefaultSettings,
                                  new GitBackuper(Directory.FullName));
        }

        public MySetting ReadMySetting()
        {
            // Reads the contents of %AppData%/ApplicationName/MySetting.cfg
            return this.repository.Read<MySetting>();
        }

        public void Save(MySetting setting)
        {
            // Saves to of %AppData%/ApplicationName/MySetting.cfg
            // Commits changes to git repository.
            this.repository.Save(setting);
        }

        private static RepositorySettings CreateDefaultSettings()
        {
            return new RepositorySettings(Directory.FullName, true, null, ".json", ".saving");
        }
    }
}