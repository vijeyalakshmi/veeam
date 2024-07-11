# Folder Synchronization

This C# program synchronizes files and directories from a source folder to a replica folder at specified intervals. It logs file operations and errors to both the console and a specified log file.

## Features

#### File Synchronization: 
Copies or updates files from the source to the replica folder.

#### Directory Synchronization: 
Ensures directory structure in the replica folder matches the source folder.

#### Logging: 
Records synchronization activities and errors to a log file and displays them on the console.

#### Periodic Synchronization: 
Runs synchronization at regular intervals defined by the user.

## Prerequisites
1. .NET Core or .NET Framework installed on your machine.
2. Basic familiarity with command-line usage.

## Usage
### Command line arguments

```bash
dotnet run -- <sourcePath> <replicaPath> <syncInterval> <logFilePath>
```
1. sourcePath: Path to the source folder to synchronize from.
2. replicaPath: Path to the replica folder to synchronize to.
3. syncInterval: Interval in seconds between synchronization runs.
4. logFilePath: Path to the log file where synchronization operations will be logged.

### Example
```bash
dotnet run -- C:\SourceFolder D:\ReplicaFolder 60 C:\Logs\sync-log.txt
```
This example synchronizes C:\SourceFolder with D:\ReplicaFolder every 60 seconds, logging operations to C:\Logs\sync-log.txt.


## Implementation Details

### File Synchronization
Copies or updates files from sourcePath to replicaPath based on file content comparison using SHA1 hash.
###  Directory Synchronization
Creates and deletes directories in replicaPath to match those in sourcePath.
### Logging
Logs messages including synchronization start, file copy/update, file deletion, directory creation, directory deletion and errors.

## Author
Vijeyalakshmi Rajaguru ~ shwaguru@yahoo.com ~ +49 152 31 407986
