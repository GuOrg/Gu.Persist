# Gu.Persist
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.NewtonsoftJson.svg)](https://www.nuget.org/packages/Gu.Persist.NewtonsoftJson/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.SystemXml.svg)](https://www.nuget.org/packages/Gu.Persist.SystemXml/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.RuntimeBinary.svg)](https://www.nuget.org/packages/Gu.Persist.RuntimeBinary/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.RuntimeXml.svg)](https://www.nuget.org/packages/Gu.Persist.RuntimeXml/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Persist.Git.svg)](https://www.nuget.org/packages/Gu.Persist.Git/)
<!---
[![Build status](https://ci.appveyor.com/api/projects/status/347rs0n3van46k50/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-persist/branch/master)
-->
[![Build Status](https://dev.azure.com/guorg/Gu.Persist/_apis/build/status/GuOrg.Gu.Persist?branchName=master)](https://dev.azure.com/guorg/Gu.Persist/_build/latest?definitionId=4&branchName=master)

A small framework for reading and saving data.

- XmlRepository is a baseclass for managing xml files.
- BinaryRepository is a baseclass for managing binary files.
- JsonRepository is a baseclass for managing json files.

# Table of contents.
  - [Features](#features)
  - [Repository](#repository)
    - [SingletonRepository](#singletonrepository)
    - [DataRepository](#datarepository)
    - [Interfaces](#interfaces)
      - [Members](#members)
    - [Migration.](#migration)
    - [Save transaction.](#save-transaction)
    - [Sample wrapper](#sample-wrapper)
    - [Sample using git for backups.](#sample-using-git-for-backups)

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

### Migration.

For managing versions of files on disk. Sample for json:

```cs
var read = repository.Read<DummySerializable>(new JsonMigration(Version1To2, Version2To3));

JObject Version1To2(JObject jObject)
{
    if (jObject["Version"].Value<int>() == 1)
    {
        jObject["Version"] = 2;
        jObject.RenameProperty("Typo", "Name");
        return jObject;
    }

    return jObject;
}

JObject Version2To3(JObject jObject)
{
    if (jObject["Version"].Value<int>() == 2)
    {
        jObject["Version"] = 3;
        jObject.Add("NewProperty", "default value");
        return jObject;
    }

    return jObject;
}
```

### Save transaction.

Locks all files that will be part of the transaction. Uses atomic writes.

1. Lock `file` if exists.
2. Lock `file.delete` if it exists.
3. Create and lock `file.tmp` if it exists.
4. Save to `file.tmp`
5. Rename `file` to `file.backup if `creating backups.
6. Rename `file.tmp`-> `file`

On error everything is reset back to initial state.

### Samples

### Sample for reading and saving settings using git for backups.

```C#
public class Settings
{
    private static readonly DirectoryInfo Directory = new DirectoryInfo("./Settings");

    // Initializes with  ./Settings/RepositorySettings.json is present
    // Creates a git repository for history.
    private readonly SingletonRepository repository = new SingletonRepository(
        CreateDefaultSettings,
        new GitBackuper(Directory.FullName));

    public MySetting ReadFoo()
    {
        // Reads the contents of ./Settings/MySetting.json
        // As we are using a SingletonRepository Read will always return the same instance.
        return this.repository.Read<MySetting>();
    }

    public void Save(MySetting setting)
    {
        // Saves to of ./Settings/MySetting.json
        // Commits changes to git repository.
        this.repository.Save(setting);
    }

    private static RepositorySettings CreateDefaultSettings()
    {
        return new RepositorySettings(
            directory: Directory.FullName,
            jsonSerializerSettings: new JsonSerializerSettings { Formatting = Formatting.Indented },
            isTrackingDirty: true,
            backupSettings: null,
            extension: ".json",
            tempExtension: ".saving");
    }
}
```

### For reading and saving data

```C#
public class Data
{
    // Uses %AppData%/ApplicationName.
    // Initializes with  %AppData%/ApplicationName/RepositorySettings.cfg
    private readonly DataRepository repository = new DataRepository();

    public MyData ReadFoo()
    {
        // Reads the contents of %AppData%/ApplicationName/MyData.cfg
        // As we wrap a DataRepository Read will always return a new instance.
        return this.repository.Read<MyData>();
    }

    public void Save(MyData data)
    {
        // Saves to of %AppData%/ApplicationName/MyData.cfg
        // Creates a backup %AppData%/ApplicationName/MyData.bak
        this.repository.Save(data);
    }
}
```

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
