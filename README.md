# Gu.Persist
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/347rs0n3van46k50/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-persist/branch/master)

A small framework for reading and saving data.

- XmlRepository is a baseclass for managing xml files.
- BinaryRepository is a baseclass for managing binary files.
- JsonRepository is a baseclass for managing json files.

## Save transaction.
Happy path

1. Lock `file` if exists.
2. Lock `file.delete` if it exists.
3. Create and lock `file.tmp` if it exists.
4. Save to `file.tmp`
5. Rename `file` to `file.backup if `creating backups.
6. Rename `file.tmp`-> `file`

On error everything is reset back to initial state.

## Features

- Atomic save transactions. Avoids corrupted data on application crash etc.
- T Clone<T>(T item); deep clone by serializing and then deserializing an instance.
- bool IsDirty<T>(T item, IEqualityComparer<T> comparer); check if an instance is dirty after last save.
- Repository manages a singleton reference for each file.
- EqualityComparers that checks structural equality by serializing and comparing bytes. If performance is an issue overloads with IEqualityComparer<T> are exposed.
- Repository bootstraps itself with settings file in directory.
- Creates backups on save. Backuprules configurable via setting.
    - Extension
    - Directory
    - Number of backups
    - Max age backups.

## Repository

Helper class for reading and saving files. It is meant tyo be cached as it is expensive to instantiate.


Sample:

    [Test]
    public void XmlSample()
    {
        var repository = new XmlRepository(); // Uses %AppData%/Settings. 
        var setting = repository.ReadOrCreate(() => new DummySerializable()); // Uses typeof(T).Name as filename
        setting.Value ++;
        Assert.IsTrue(repository.IsDirty(setting));
        repository.Save(setting);
        Assert.IsFalse(repository.IsDirty(setting));
    }
