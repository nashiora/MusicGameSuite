# music:theori

An extensible rhythm game client with many built-in game modes for home and arcade use.

Demo Video(s):

[![Early Demo](https://img.youtube.com/vi/vbyPEh_-LfU/2.jpg)](https://www.youtube.com/watch?v=vbyPEh_-LfU)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

Note that currently this project has only been developed and tested on a Windows machine with Visual Studio (2017/2019) installed. Any Windows setup should work in theory, and Linux/OS X hasn't been a consideration as of yet.

### Prerequisites

You'll need MSBuild set up (Visual Studio, Rider, etc.) with up to .NET Framework 4.6.1 SDK installed and the ability to get the NuGet packages installed.

SDL2 and freetype are required and included as 32-bit DLLs (with support for 64-bit soon) for Windows, they're expected to be installed in the future for other operating systems.

### Building and Running

Setup should be as simple as cloning the repository and opening the solution file, making sure all NuGet packages are installed and `theori-Win32` is the start-up project.

You'll need at least one `.ksh` file with associated music file to see this in action.

## Running the tests

This project currently doesn't have any tests.

## Contributing

The project tries to follow [GitFlow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) wherever possible, please familiarize yourself with it. Small changes tend to be fine directly on the develop branch but should be avoided if the changes could become larger easily. Avoiding direct contact with develop keeps things a bit cleaner.

## Authors

Local Atticus (@nashiora)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments
