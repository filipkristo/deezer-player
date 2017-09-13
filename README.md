![Deezer](http://cdn-files.deezer.com/img/press/new_logo_white.jpg "Deezer") 

[![Build status](https://ci.appveyor.com/api/projects/status/55rxjeaqwyrm618p?svg=true)](https://ci.appveyor.com/project/filipkristo/deezer-player)

## DeezerWrapper

DeezerWrapper is a C# application which uses Deezer's Native SDK to play a song once a user was authenticated.

### Features

 - User authentication
 - Playing a Deezer content (track, album or playlist).

### Build instructions

* Download the latest version of the [Deezer Native SDK][1]
* Unzip it and copy content to Debug Folder

### Usage

Latest Nuget: [v1.0.1](https://www.nuget.org/packages/DeezerPlayer/)

Via Nuget:
```
Package Manager> Install-Package DeezerPlayer
```

Via Source: 
```
git clone https://github.com/filipkristo/deezer-player.git
```

Open solution in VS and build it.


### Run this sample

```
> DeezerWrapper.exe dzmedia:///CONTENT/CODE
```


 [1]: http://developers.deezer.com/sdk/native
