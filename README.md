# AL Object Parser - .NET Core

This is a DLL helper intended to help Unit Test generation and synchronization for [ATDD.TestScriptor](https://github.com/fluxxus-nl/ATDD.TestScriptor) project.

## Features

* Read AL Codeunit: parse file content into an `ALObject` object.
* Write `ALObject` object to file.
* Test Features/Scenarios: 
    * Read: Parse metadata from file content into `TestFeature` object
    * Write: Update Codeunit methods using metadata
* Generate AL object files using C# contructs

## TODO
* Preserve Test method contents if TestScenario was updated.