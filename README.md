# SharpBattle.net

## Introduction

SharpBattle.net aims to be a complete emulator for Blizzard games that run on Battle.net V1. These include the following games:

 + StarCraft
 + StarCraft - Broodwar
 + Diablo I
 + Diablo II
 + Diablo II - Lord Of Destruction
 + WarCraft 2 - Battle.net Edition
 + WarCraft 3
 + WarCraft 3 - Frozen Throne

It follows in the same vein as [PvPGN](http://pvpgn.berlios.de/) but written in C#. It runs on Windows as well as on Mono, so that
includes Linux and MacOS.

This project is completely open source, and community involvement is highly encouraged.

If you wish to contribute ideas or code, please fork this repository and make a pull request.

## Requirements

 + Platform: Windows, Linux, or MacOS
 + Compiler: Visual Studio, Local Windows Compiler, or Mono
 + SQL Server: MSSQL, MySQL, PostgreSQL or SQLite

## Installation

There are two ways you can start with this. If you wish to contribute to the projects, first create a fork of this project, and
clone it from there, otherwise if you just want to get started, clone this repository straight away.

A SQL server is not required to run SharpBattle.net as it can run on a local file using SQLite (Which is the default out of the
box). If you wish to run SharpBattle.net agains a SQL server, the supported ones are listed on top. Search Google for how to set
them up on you're desired platform.

Then from there it depends on what platform you are. On Windows, simply open the solution provided in the Source folder if you
want to run and debug it with Visual Studio and compile it from there. If you do not have Visual Studio installed, simply
running the Build.bat file will build the solution using the local Windows C# compiler. After compiling has finished, look
inside the Binaries folder, and all required binaries are there to start the server.

NOTE: Add Linux and MacOS area for Mono.

## Notes

Reporting issues should be done with the GitHub issue tracker. Fixes should be a forker repo and a pull request should be
sent for it.

Version 0.0.1