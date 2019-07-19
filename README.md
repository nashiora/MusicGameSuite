# music:theori

An extensible rhythm game client with many built-in game modes for home and arcade use.

Demo Video(s):

[![Early Demo](https://img.youtube.com/vi/vbyPEh_-LfU/2.jpg)](https://www.youtube.com/watch?v=vbyPEh_-LfU)
[![Prettier Gameplay](https://img.youtube.com/vi/Y5oSLV1jdSo/2.jpg)](https://www.youtube.com/watch?v=Y5oSLV1jdSo)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

Note that currently this project has only been developed and tested on a Windows machine with Visual Studio (2017/2019) installed. Any Windows setup should work in theory, assuming all packages are installed properly, and Linux/OS X hasn't been a consideration as of yet.

### Prerequisites

You'll need MSBuild set up (Visual Studio, Rider, etc.) with up to .NET Framework 4.6.1 SDK installed and the ability to get the NuGet packages installed.

SDL2 and freetype are required and included as 32-bit DLLs (with support for 64-bit soon) for Windows, they're expected to be installed in the future for other operating systems.

### Building and Running

Setup should be as simple as cloning the repository and opening the solution file, making sure all NuGet packages are installed and `nsc-Win32` is the start-up project.
Due to SharpFont, you can only run the x86 builds. Don't try to build x64 unless you plan to fix the issue.

You'll need at least one `.ksh` file with associated music file to see this in action.
The client has the ability to load .ksh files and convert them to .theori binary files and .theori-set metadata files but you won't just have these lying around.
If you'd like you convert your KSH files to .theori/.theori-set, but the file serializer isn't yet finished for all features.

The default menu will allow you to select your input method and configure bindings for it as well as give you a sub menu for selecting charts.
Holding L- or R-CTRL when selecting an option to open a chart file will enable full autoplay mode.

## Running the tests

This project currently doesn't have any tests.

## Contributing

The project tries to follow [GitFlow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) wherever possible, please familiarize yourself with it. Small changes tend to be fine directly on the develop branch but should be avoided if the changes could become larger easily. Avoiding direct contact with develop keeps things a bit cleaner.

## Authors

Local Atticus (@nashiora)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments
