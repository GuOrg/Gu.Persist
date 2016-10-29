# Gu.Persist
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/347rs0n3van46k50/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-persist/branch/master)

A small framework for reading and saving data.

- XmlRepository is a baseclass for managing xml files.
- BinaryRepository is a baseclass for managing binary files.
- JsonRepository is a baseclass for managing json files.

## Save transaction.
1. Lock `file` if exists.
2. Lock `file.delete` if it exists.
3. Save file to `file.saving`.
4. Rename file to backup.
5. Rename `file`  to `file.tmp`

### When creating backups.


Features:
- T Clone<T>(T item); deep clone by serializing and then deserializing an instance.
- bool IsDirty<T>(T item, IEqualityComparer<T> comparer); check if an instance is dirty after last save.
- Repository manages a singleton reference for each file.
- EqualityComparers that checks structural equality by serializing and comparing bytes. If performance is an issue overloads with IEqualityComparer<T> are exposed.
- Saves to .tmp file, on success it is renamed to .cfg extensions are configurable via settings.
- Creates backups on save. Backurules configurable via setting.
    - Extension
    - Directory
    - Number of backups
    - Max age backups.


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
