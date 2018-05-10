# Justine Core BOT
[![Build Status](https://travis-ci.org/petrspelos/justine-core.svg?branch=master)](https://travis-ci.org/petrspelos/justine-core)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/petrspelos/justine-core/blob/master/LICENSE)

This is **Justine BOT**.

A personal (and Open Source) bot for Discord written in C# using the [Discord .NET library](https://github.com/RogueException/Discord.Net). :v:

This project is my personal bot self-hosted on a [Raspberry Pi 3B](https://www.raspberrypi.org/products/raspberry-pi-3-model-b/) running [Windows 10 IoT](https://developer.microsoft.com/en-us/windows/iot). 

## Getting Started

The following instructions describe a step by step process of getting Justine on your local machine and ready for development.

If you're interested in hosting your own instance of Justine, check the notes in [deployment](#deployment).

### Prerequisites

* The recommended IDE is [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15).
* Using **Visual Studio Installer** make sure you installed `.NET Core cross-platform development`.
    * Visual Studio Installer should be automatically installed with VS 2017 Community.

### Installing

This is a step by step guide to get Miunie ready on your machine and ready for development.

**Getting the source**

If you would like to contribute:
1. [Fork the repository](https://help.github.com/articles/fork-a-repo/).
2. Navigate to your fork.
3. [Clone](https://help.github.com/articles/cloning-a-repository/) your fork to your local machine.

If you just want a copy of the code:
1. Just directly [Clone](https://help.github.com/articles/cloning-a-repository/) or [download as a ZIP](https://stackoverflow.com/a/6466993) this repository. raised_hands:

**Setting up the development environment**

* The root directory of the project contains `JustineCore.sln`, this is a Visual Studio solution file and you can open it with Visual Studio (see [prerequisites](#prerequisites)).

* After the solution is loaded, right-click the JustineCore project through the Solution Explorer in Visual Studio _(It has a little C# in a green box icon by default)_ and go to Properties. Under **Debug**, you will see an `Application arguments:` field. You can paste your [bot token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) there. Application arguments are already added to `.gitignore` so you don't have to worry about accidentally making it public.

* Once you save your changes from the previous step, you can compile and run the application.
    * In Visual Studio, a common way of doing this is with the `F5` or `Ctrl + F5` shortcut.

* Try it out
    * The bot you have assigned with your token should now be online. Try to mention him and say hello!

```

@MyBot Hello

```
If you get a response back, everything is ready for development.

If you would like to run the bot without Visual Studio, follow the notes in [Deployment](#deployment).

## Running the tests

To run Unit Tests in Visual Studio, you use the `Ctrl + R, A` shortcut or go to `Test > Run > All Tests`.

## Deployment

To publish a version of Justine and run it on a machine, you can use [this tutorial](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish?tabs=netcore2x) that walks you through the process.

* When running the application for the first time, a notice about an invalid token might appear. If this happens, close the application, check its directory and search for `config.json`. Edit this file and make sure to put your token in the right place. The next launch of the application should properly connect.

## Built With

* [.NET Core 2.0](https://docs.microsoft.com/en-us/dotnet/core/) - Platform used
* [Discord .NET](https://github.com/RogueException/Discord.Net) - Discord API wrapper library
* :heart: Passion and a healthy bit of self-interest :yum:

## Contributing

Hey, thank you so much for trying to contribute! It is super nice of you! :two_hearts:

This is just my personal playground, feel free to learn from it, but if you want to contribute, check out [Miunie The Community Bot](https://github.com/petrspelos/Community-Discord-BOT). :yellow_heart:

## Author

* **Petr Sedláček** - *Developer* - [PetrSpelos](https://github.com/petrspelos)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
