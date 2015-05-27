This repository contains source code for the automated source control tool developed by [DMC](http://www.dmcinfo.com) to improve their Siemens programming and project process.

The utility takes two Siemens Step7 projects (labeled "left" and "right") and does both lightweight and detailed project comparisons. It was built to ease the process of project tracking and auto-versioning.

This tool uses two outside resources of note:

1. **Siemens Command Interface.** This is a set of DLL class libraries released by Siemens for free with the purpose of allowing programmatic access to Step7 projects. Its help file is available on DVD 2 of the Simatic Manager installation set.

2. **DotNetSiemensPLCToolbox.** This is an open source library available under the GNU public license. DMC is a fan of, but is not affiliated with, its development. Its repository can be found here: http://siemensplctoolboxlib.codeplex.com


There are four main directories in this project:

1. **DMC\_Library** - this folder contains several shared source files (DMC logo, etc)

2. **DotNetSiemensPLCToolBoxLibrary** - This is the open source third party library used for accessing Siemens projects through the file system. DMC has made small modifications here and there but for the most part this has not been touched. It also contains many sample projects describing how to use the library.

3. **S7\_DMCToolbox** - this is a branch application from the diff-merge tool. It is a small app used to analyze and process Siemens projects. Feel free to peruse this project, however, it is not needed for the diff-merge tool. Install package: http://dmc.blob.core.windows.net/s7-toolbox-clickonce/download.htm

4. **S7\_GenerateSource** - This is the diff-merge tool. As you can see the project name was changed halfway through development. I (Jon) will change the project name and directory when I have more than the tiny amount of time I currently have to work on this! Install package: http://dmc.blob.core.windows.net/s7-generatesource-clickonce/download.htm