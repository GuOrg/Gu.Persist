# Gu.Settings
A small framework for managing settings.

- XmlRepository is a baseclass for managing xml files. Keeps a singleton reference of the files.
- BinaryRepository is a baseclass for managing binary files. Keeps a singleton reference of the files.
- AutoSaver is a baseclass for saving files on changes.

Features:
- T Clone<T>(T item); deep clone by serializing and then deserializing an instance.
- bool IsDirty<T>(T item, IEqualityComparer<T> comparer); check if an instance is dirty after last save.
