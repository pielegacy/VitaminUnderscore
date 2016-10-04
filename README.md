# Vitamin _
A text-based vitamin development simulator written in C# using .NET core.

This solution will run on Windows, Mac and a handful of Linux distros and can be compiled
using the .NET Core ([download here](https://www.microsoft.com/net/core/platform)).

## How to compile and run
Assuming .NET Core is installed, running Vitamin _ is as easy as opening a command line
in the main directory of the src code and typing:

`dotnet run`

If you'd just like to build the .dll files use :

`dotnet build`

## Features
Currently the game includes: 
- Formulation Creation through combining Vitamins, Minerals and other substances into one.
- Loading and Saving using an accessible JSON system
- Importing of custom save files with new ingredients and such
- Testing of formulation on subject with possible severe consequences
- Built in Ingredient builder (`ingredientgenerator.html`) all in one file, no internet required
    - [Try it now!](jsfiddle.net/o12qpg8t/embedded/result/)
## Planned Features
- Mass production of Drugs
- Resource management system
- Better documentation
- Animal varieties as opposed to just humans
- Basic Manual