# Introduction 
Sample UWP application targeting Windows 10 fall creators update, focused on LOB. 

This business line application shows the advantages of Fluent Design, the Windows Template Studio and the possibilities that Microsoft's services can offer.

The goal of this demo is to encourage the developers to play with the code and check for themselves how they can program an application with a simple code.

[![Build status](https://rido.visualstudio.com/_apis/public/build/definitions/989ddbdd-c86a-4fa8-8d80-89eb785d8056/83/badge)](https://rido.visualstudio.com/_apis/public/build/definitions/989ddbdd-c86a-4fa8-8d80-89eb785d8056)

# Prerequisites
System requirements:
- __Windows 10 Insider Preview 16278__
- __SDK 10.0.16267.0__ -> [Download](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK)
- __Visual Studio 15.4 Preview__ -> [Download](https://www.visualstudio.com/en-us/news/releasenotes/vs2017-preview-relnotes)


# Getting Started

Install from
[http://aka.ms/eshopuwp](http://aka.ms/eshopuwp)


## Project structure
The solution is divided into 4 projects. Its visual tree looks like this:
- eShop.Cortana -> Service
- eShop.Domain -> Domain model
- eShop.Providers -> Manages and creates each model
- eShop.UWP -> Main project

# Intructions
Before first installation, click right on the appx file, then select properties, digital signatures, View Certificate and install it on LocalMachine/TrustedPeople

## Development
The following software has to be installed in the development machine to be able to clone the repository.
- [Git](https://git-scm.com/downloads) -> Installed globally

## Downloading the code
You have to download the main source code.

```
git clone https://plainconcepts.visualstudio.com/DefaultCollection/MSFT.eShopContainer.UWP/_git/eShop.UWP
```

# Features

## Windows Template Studio
We will work with this template. This is how you can set it up:

- __Windows Template Studio (Universal Windows)__ -> File / Visual C# / Windows Universal

Once the template is selected, set these features:
- __Project type__ -> Navigation Pane
- __Framework__ -> MVVM Light
- __Pages:__
    - Blank
    - Grid -> Telerik UI control for UWP
    - Chart -> Telerik UI control for UWP
- __Background Task__
- __User Interaction__ 
    - Toast Notifications
    - Live Tile

## Cortana
You have to run Cortana at least once, because when done it installs the voice command definitions. Once it has been run, you can close the application and start using Cortana's search. 

### Voice Commands
These are some supported voice commands (Cortana may take time to refresh its voice commands):

- "shop, show me __mug__ type products"
- "shop, give me __shirt__ type products"
- "shop, show me __cap__ type products"
- "shop, show me __sheet__ type products"

## Windows Ink
Select and edit multiple products using the Surface Pen. There are several signs with differents functions.
This application recognizes a hand gesture and translate it to text. These are the supported forms:

- __Circle - "o"__ : Select multiple items
- __Tick - "v"__: Active or inactive the status
- __Cross - "x"__ : Remove an item

![Windows Ink](/docs/WindowInk.gif)

## Windows Hello
There are two ways to log-in, either with user/password or using Windows Hello.
The first time the application is run, the user/password log in option will appear by default. You will be able to log-in through Windows Hello once you enter the user name and the button enabling to log-in is activated. 
Once you have logged-in using Windows Hello, your user will be saved and the next time you open the application Windows Hello authentication will appear as default. 
Depending on how your Windows Hello settings are (Settings/Accounts/Sign-in options), you will be able to authenticate by using:

- Pin
- Face recognition
- Fingerprint
- Et al.

## Fluent Design


### Acrylic material
[Acrylic material](https://docs.microsoft.com/es-es/windows/uwp/style/acrylic) is a type of Brush that creates a partially transparent texture.

![Acrylic material](/docs/AcrylicFluent.png)



### Connected animations
[Connected animations](https://docs.microsoft.com/es-es/windows/uwp/style/connected-animation) let you create a dynamic and compelling navigation experience by animating the transition of an element between two different views.

![Connected animations](/docs/ConnectedAnimation.gif)



### Reveal
[Reveal](https://docs.microsoft.com/es-es/windows/uwp/style/reveal) is a lighting effect that helps bring depth and focus to your app's interactive elements.

![Connected animations](/docs/RevealFluent.gif)

## ShyHeader
[ShyHeader](https://github.com/Microsoft/WindowsUIDevLabs/tree/master/SampleGallery/Samples/SDK%2014393/ShyHeader) Demonstrates how to use ExpressionAnimations Tookit with a ScrollViewer to create a shinking header tied to scroll position.

![ShyHeader](/docs/ShyHeaderToolkit.gif)