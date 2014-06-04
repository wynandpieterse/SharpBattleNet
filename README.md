# SharpBattle.net

## Introduction

SharpBattle.net aims to be a complete emulator for Battle.net V1 and all the games that ran on it. These games
include the following:

 + StarCraft
 + StarCraft - Broodwar
 + Diablo I
 + Diablo II
 + Diablo II - Lord Of Destruction
 + WarCraft 2 - Battle.net Edition
 + WarCraft 3
 + WarCraft 3 - Frozen Throne

It follows in the same vein as [PvPGN](http://pvpgn.berlios.de/) but written in C#. This is intended to be a fun
and learning experience. What people do with this is their own resposibility. I do not endores piracy.

This project is completely open source, and community involvement is highly encouraged.

If you wish to contribute ideas or code, please fork this repository and make a pull request.

## Requirements

 + Platform: Windows, Linux, MacOS X
 + Compiler: Visual Studio 2013 or higher, Mono for Linux and MacOS X
 + SQL Server: MSSQL, MySQL, PostgreSQL or SQLite

## Building

### Windows

 1. Clone this repo to a local directory
 2. If you have Visual Studio, open the solution file located inside the Source folder. Compile and run.
 3. If you dont have Visual Studio, run the Build.bat file. Locate the binaries inside the Binaries folder and run.

### Linux and MacOS X

 1. Clone this repo to a local directory
 2. Enter the directory and run the Build.sh file. This will create all the executables inside the Binaries folder.
 3. Run each of the required server files by starting the batch file in the root directory.
 4. NOTE : Mono is required on the host system to run SharpBattleNet

### Running Notes

Note, some configuration may be required. The configuration files are all located inside the public shared folder
under SharpBattleNet. The default values will make it run, but fine-tuning may help the servers perform more optimaly.

### Other Notes

The solution files uses NuGet as a package manager, thus all of the required dependancies will automatically be 
downloaded if required.

## Running

### Windows

If you compiled the executables via Visual Studio, just tap on the executable you want to run, right click and say
'Select As Startup Project' and press F5 to debug with Visual Studio.

If you built it with the batch file, go to the run scripts directory and run the desired server. There should be
2 ways. The first is to run it in a command window, and the other is running it as a Windows service. Select the
desired mode and double click.

### Linux and MacOS X

Go to the Scripts/Run directory and start the desired shell script in the mode you wish to run it.

## Notes

 + Reporting issues should be done with the GitHub issue tracker.
 + Any fixes should come from a pull request.

Version 0.0.6

