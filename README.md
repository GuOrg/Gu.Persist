# Gu.Persist
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.NewtonsoftJson.svg)](https://www.nuget.org/packages/Gu.Persist.NewtonsoftJson/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.SystemXml.svg)](https://www.nuget.org/packages/Gu.Persist.SystemXml/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.RuntimeBinary.svg)](https://www.nuget.org/packages/Gu.Persist.RuntimeBinary/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.RuntimeXml.svg)](https://www.nuget.org/packages/Gu.Persist.RuntimeXml/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.Git.svg)](https://www.nuget.org/packages/Gu.Persist.Git/)
[![Build status](https://ci.appveyor.com/api/projects/status/347rs0n3van46k50/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-persist/branch/master)

A small framework for reading and saving data.

- XmlRepository is a baseclass for managing xml files.
- BinaryRepository is a baseclass for managing binary files.
- JsonRepository is a baseclass for managing json files.

## Features

- Transactional atomic saves. Avoids corrupted data on application crash etc.
- Repository bootstraps itself with settings file in directory.
- SingletonRepository manages a singleton reference for each file.
- Creates backups on save. Backuprules configurable via setting.
    - Extension
    - Directory
    - Number of backups
    - Max age backups.
- Use git for backups.
- T Clone<T>(T item); deep clone by serializing and then deserializing an instance.
- bool IsDirty<T>(T item, IEqualityComparer<T> comparer); check if an instance is dirty after last save.
- EqualityComparers that checks structural equality by serializing and comparing bytes. If performance is an issue overloads with IEqualityComparer<T> are exposed.

## Save transaction.
Happy path

1. Lock `file` if exists.
2. Lock `file.delete` if it exists.
3. Create and lock `file.tmp` if it exists.
4. Save to `file.tmp`
5. Rename `file` to `file.backup if `creating backups.
6. Rename `file.tmp`-> `file`

On error everything is reset back to initial state.

## Repository

Helper class for reading and saving files. It is meant to be cached as it is expensive to instantiate.
There are a number of interfaces with subsets of the functionality. Exposing the raw repository class is probably a bad idea as it has so many overload of everything.
When not passing in an explicit `RepositorySetting` in the constructor the constructor looks on disk if there is a settings file to read and use. If there is no file it creates an instance and saves it for use next time.
This makes it configurable without recompiling.

If a setting is passed in the constructor does not look on disk.

The different libraries contains repository implementations for 
- `NewtonSoft.Json`
- `System.XmlSerializer`
- `System.Runtime.Serialization.Formatters.Binary.BinaryFormatter`
- `System.Runtime.Serialization.DataContractSerializer`

### SingletonRepository

Manages singleton instances of things read or written to disk. This is useful for settings etc.

### DataRepository

Simple repository for reading & saving data.

### Interfaces
A number of interfaces exposing subsets of the functionality are provided.

- `IAsyncFileNameRepository` 
- `IAsyncFileInfoRepository`
- `ICloner`
- `IDirty`
- `IStreamRepository`
- `IGenericRepository`
- `IGenericAsyncRepository`
- A bunch of StreamRepositories for reading raw streams.

#### Members

- GetFileInfo, for getting the file the repository uses for reading & saving.
- Delete for deleting files & backups.
- Exists for checking if files exists.
- Read for reading and deserializing the contents of files.
- ReadAsync for reading and deserializing the contents of files.
- ReadOrCreate, read the file if it exists, create and instance and save it to disk before returning it if not.
- Save for saving files.
- SaveAsync for saving files.
- CanRename, check for collisions before renaming.
- Rename, rename files and backups.
- ClearTrackerCache, clear the `IDirtyTracker`
- RemoveFromDirtyTracker, remove an item from `IDirtyTracker`
- DeleteBackups, delete backups for a file.

### Sample wrapper

```C#
public class MyRepository
{
    private readonly SingletonRepository repository;

    public MyRepository()
    {
        // Uses %AppData%/ApplicationName.
        // Initializes with  %AppData%/ApplicationName/RepositorySettings.cfg
        this.repository = new SingletonRepository();
    }

    public MySetting ReadMySetting()
    {
        // Reads the contents of %AppData%/ApplicationName/MySetting.cfg
        return this.repository.Read<MySetting>();
    }

    public void Save(MySetting setting)
    {
        // Saves to of %AppData%/ApplicationName/MySetting.cfg
		// Creates a backup %AppData%/ApplicationName/MySetting.bak
		// As we created a SingletonRepository in the ctor setting must be the same instance always.
        this.repository.Save(setting);
    }
}
```

### Sample using git for backups.

```C#
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
        // Reads the contents of ./Settings/MySetting.cfg
        return this.repository.Read<MySetting>();
    }

    public void Save(MySetting setting)
    {
        // Saves to of ./Settings/MySetting.cfg
        // Commits changes to git repository.
        this.repository.Save(setting);
    }

    private static RepositorySettings CreateDefaultSettings()
    {
        return new RepositorySettings(Directory.FullName, true, null, ".json", ".saving");
    }
}
```
