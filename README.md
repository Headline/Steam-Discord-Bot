# Steam-Discord-Bot
[![Build status](https://ci.appveyor.com/api/projects/status/h0sltbhpyelqc066?svg=true)](https://ci.appveyor.com/project/Headline22/steam-discord-bot)

This a Steam Discord bot which provides notifications on game updates along with some useful commands that interact with the Steam API. There are many other commands which are included that are unrelated to steam, but are general purpose or meant to exist just for fun. Here are some examples of the commands so far. If you'd like to see more commands, open an issue and recommend some! If you dislike any commands or functionality, commands can always be disabled in settings.json!

```
!appinfo - Fetches up application info from steam.
!chat - Uses a Markov model with to generate response text.
!chat about - Uses a Markov model with to generate response text using starting words.
!chatremove - Removes the term from knowledgebase. [Owner only]
!chatknowledge - Sends a pastebin link containing its knowledgebase.
!coin - Flips a coin.
!credits - Outputs the credits for the bot, ordered by contributions
!btc - Fetches the latest Bitcoin market value.
!eth - Fetches the latest Ethereum market value.
!roll - rolls a dice of arbitrary size. (example: '!roll 33' rolls a 33-sided die)
!duck - Appends input text to a picture of psychonic.
!learn - Saves a new fact to the bot's memory.
!facts - Outputs number of facts known.
!facts list - Outputs number of facts known.
!fact - Outputs a random fact
!facts about - Outputs a fact about the input, if learned.
!forget - Forgets a fact learned.
!help - Prints and formats all commands.
!lmgtfy - Responds with a lmgtfy link
!numplayers - Fetches current player count from steam.
!numservers - Fetches current server count from steam.
!ping - Test command.
!pony - Grabs link based off pony name.
!repeat - Repeats last message X amount of times.
!search - Performs google search and returns result(s).
!setgame - Sets the bot's game. [Owner only]
!s - Subsitutes one word for another in last message.
!update - Updates and reloads the bot. [Owner only]
!version - Get the current bot version.
```

## Don't feel like hosting this?
If you are somebody looking to have this bot on your discord server, message me (Headline#9572) on discord and I can throw the bot in the server for you. Otherwise, you should only *really* need this source code if you'd like to extend the bot yourself.

## Installation
- Install latest version from the [releases page](https://github.com/Headline22/Steam-Discord-Bot/releases).
- Set up settings.json
- Install [Python 3.6](https://www.python.org/downloads/) (for updater.py)
- Install [psutil](https://github.com/giampaolo/psutil/blob/master/INSTALL.rst) (for updater.py)
- Run Steam-Discord-Bot executable

## Configuration
Here's a breakdown for configuration settings, along with why they're needed.
- **SteamUsername** & **SteamPassword**: The bot needs credentials to a steam account that it can stay logged into for things like querying the master server list.
- **DiscordBotToken**: Token used for Discord's bot API, [reactiflux](https://github.com/reactiflux) made a great guide for getting one [here](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token)
- **DiscordAdminId**: Admin ID used for commands which should not be used by the general public. It's a unique ID which identifies you among others. Find yours using this [tutorial](https://support.discordapp.com/hc/en-us/articles/206346498-Where-can-I-find-my-User-Server-Message-ID-) made by Discord themselves.
- **DiscordAdminContact**: A contact string to be used to inform users who owns the bot. This can be a Discord username, email, or link.
- **AppIDList**: The list of steam appids that the bot watches for updates. If an update is detected it will alert all servers with a matching `AnnouncePref`
- **AnnouncePrefs**: A list of channels the bot will search for to send announcement into. This list can include many channels which span across multiple servers, but the bot will only send the message once to the first match found.
- **DisabledCommands**: A list of disabled commands, should you want to do so. All you need to do is specify a command name.I you wanted to remove the help command, enter "help" as an entry in the list.
- **GuildTriggers**: Allows you to map a guild id to a command trigger. An example item would be "<guildid>:-" to map the guild id to the character `-`
- **GitHubAuthToken**: An authentication token for GitHub API if you'd like the bot to automatically update itself. You may leave this blank, but !update will not work and any updates will not automatically occur. 
- **JobInterval**: How often the bot will run scheduled jobs. The time given is in seconds.
- **AlliedModdersThreadJob**: Alerts new plugin uploads to the AlliedModders [plugin subforum](https://forums.alliedmods.net/forumdisplay.php?f=108). This is disabled by default, as I believe it is only useful for my usecase. If you'd like to get the notifications, you may set it to true. Send channel is determined by `AnnouncePrefs` 
- **SteamCheckJob**: Job which ensures a steam connection is currently being held, and attempts a reconnect on failure.
- **SelfUpdateListener**: Job which periodically checks current version with latest, and updates if possible. If `GitHubAuthToken` isn't filled out, this job will never run
- **GitHubUpdateUsername**, **GitHubUpdateRepository**, and **GitHubUpdateReleaseFilename**: These exist if you'd like to fork the repository yourself, setup AppVeyor, and handle automatic updating yourself. This is useful for developers who'd like to maintain their own fork.

## Credits 
Much of the code was adapted from [VoiDeD's bot](https://github.com/VoiDeD/steam-irc-bot/), so many thanks for the guidance. Also I'd like to thank the guys on #opensteamworks in gamesurge irc along with our contributors for making this bot awesome.
