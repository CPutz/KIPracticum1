This package is intended for use on the Windows 7 practical machines at the BBG.
It should also work with windows on your own laptop/computer but this is not guaranteed.
If you use osx or linux this will not work.

this package contains 3 folders:
* Python: contains a portable version of python 2.7, do not edit anything in there!
* tools: contains the game engine and the run-scripts. If you change the name or location of "your bot" you will have to edit play_one_game.cmd and/or play_one_game_live.cmd accordingly.
* your bot: contains a visual C# project initialized with the C# starter package. When handing in the assignment only hand in this folder.



How to get started:
* compile and build your project.
* start "cmd" and navigate to the b2ki/tools folder
* type "play_one_game.cmd" and press enter
* when the game is done a browser window will open to show the replay of that game. If you unpacked the b2ki folder onto a network drive(such as your home or Download folder on the practical machines) the visualiser will not work in IE. copy the link to chrome/firefox. Alternatively you can run the play_one_game_live.cmd script instead which doesn't require a browser but relies on java instead.



How to debug:
* method 1: printing to the cmd window:
Using Console.Error.WriteLine() you can print messages to stderr. These messages will be interspresed between the engine output. The play_one_game_live script has less output which may make the output more readable.
* method 2: attach a visual studio debugger:
Put the following code at the beginning of your Main() function:
	#if DEBUG
		System.Diagnostics.Debugger.Launch();
		while (!System.Diagnostics.Debugger.IsAttached)
		{ }
	#endif
This will open a dialog that lets you attack the debugger of VS to your bot.
By default the game engine will timeout very quickly and kill your bot if it delays to long. To prevent this add the following two options to the play_one_game(_live) script in the tools folder:
 --loadtime=1000000 ^
 --turntime=1000000 ^
(spaces at the beginning are not optional!)



How to upload to the server:
When uploading to the server, zip the contents of the "your bot" folder and upload that zip. The files should be at the root of the zip and the main file has to be called MyBot.cs! do not include the bin and obj folder.