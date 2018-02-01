# Steam-Discord-Bot
[![Build status](https://ci.appveyor.com/api/projects/status/h0sltbhpyelqc066?svg=true)](https://ci.appveyor.com/project/Headline22/steam-discord-bot)

This a Steam Discord bot which provides notifications on game updates along with some useful commands that interact with the Steam API. Here are some examples of the commands thus far. If you'd like to see more commands, open an issue and recommend some!

If you are somebody looking to have this bot on your discord server, message me (Headline#9572) on discord and I can throw the bot in the server for you. Otherwise, you should only *really* need this source code if you'd like to extend the bot yourself.

```
!appinfo - Fetches up application info from steam.
!chat - Uses a Markov model with to generate response text.
!chatremove - Removes the term from knowledgebase.
!chatknowledge - Sends a pastebin link containing its knowledgebase.
!coin - Flips a coin.
!btc - Fetches the latest Bitcoin market value.
!eth - Fetches the latest Ethereum market value.
!roll - rolls a dice of arbitrary size. (example: '!roll 33' rolls a 33-sided die)
!help - Prints and formats all commands.
!numplayers - Fetches current player count from steam.
!numservers - Fetches current server count from steam.
!ping - Test command.
!pony - Grabs link based off pony name.
!repeat - Repeats last message X amount of times.
!s - Subsitutes one word for another in last message.
!update - Updates and reloads the bot.
!version - Get the current bot version.
```

In order to use this bot you will need a discord bot token. [Here](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) is an exellent guide on how to do so.


## Installation
- Install latest version from the [releases page](https://github.com/Headline22/Steam-Discord-Bot/releases).
- Set up Settings.xml
- Install [Python 3.6](https://www.python.org/downloads/) (for updater.py)
- Install [psutil](https://github.com/giampaolo/psutil/blob/master/INSTALL.rst) (for updater.py)
- Run Steam-Discord-Bot executable

## Credits 
Much of the code was adapted from [VoiDeD's bot](https://github.com/VoiDeD/steam-irc-bot/), so many thanks for the guidance. Also I'd like to thank the guys on #opensteamworks in gamesurge irc along with our contributors for making this bot awesome.
