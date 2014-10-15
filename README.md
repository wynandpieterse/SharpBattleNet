# SharpBattle.net

## Introduction

Current build status:

[![Build status](https://ci.appveyor.com/api/projects/status/u180fx2lfy7bbesr)](https://ci.appveyor.com/project/wpieterse/sharpbattlenet)

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

In it's current implementation it cannot do anything usefull. The client's can connect if they are registered with
the gateway tool inside the 'Tools' folder. However, they can connect, there is no connection logic in at the moment
to handle processing it further.

My aim is to get this project running on Linux (Ubuntu) and MacOS X also. If there is any one out there with a Mac, I
would appreciate it very much if you can test it as I do not own a Mac.

## Libraries

SharpBattleNet uses the following libraries from NuGet and different places.

 + Ninject Dependcy Injection Library
 + Ninject.Factory Extension Library

## Thanks

I just want to say thanks to the following people for the awesome libraries they have made available that really
helps with the development of these software.

 + ServerToolkit - BufferPool - [Tenor](https://github.com/tenor)
 + Gateway Installer - [HarpyWar](https://github.com/HarpyWar)

Version 0.0.17.0
