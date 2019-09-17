#### 0.3.0
* BREAKING: Added optional parameter Migration to read methods.
* FEATURE: Migration support.
* BUGFIX: ReadOrCreate when passing in FileInfo
* BUGFIX: Read when passing in full path.


#### 0.2.0.4
* Sign the binaries.

#### 0.2.0
* BREAKING CHANGE: Split repository in SingletonRepository & DataRepository.
* BREAKING CHANGE: Reorder arguments in save methods to be like System.IO.File.Save.
* BREAKING CHANGE: Overhaul of constructors and factory methods for repositories & settings.
* FEATURE: Transactions for save & rename.