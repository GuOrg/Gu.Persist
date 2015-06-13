# Gu.Settings
A small framework for managing settings.

- XmlRepository is a baseclass for managing xml files.
- BinaryRepository is a baseclass for managing binary files.
- JsonRepository is a baseclass for managing json files.
- AutoSaver is a baseclass for saving files on changes.

Features:
- T Clone<T>(T item); deep clone by serializing and then deserializing an instance.
- bool IsDirty<T>(T item, IEqualityComparer<T> comparer); check if an instance is dirty after last save.
- Repository manages a singleton reference for each file.
- EqualityComparers that checks structural equality by serializing and comparing bytes. If performance is an issue overloads with IEqualityComparer<T> are exposed.


Sample:

    [Test]
    public void XmlSample()
    {
        var repository = new XmlRepository();
        var setting = repository.ReadOrCreate(() => new DummySerializable());
        setting.Value ++;
        Assert.IsTrue(repository.IsDirty(setting));
        repository.Save(setting);
        Assert.IsFalse(repository.IsDirty(setting));
    }
