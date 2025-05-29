<p align="center">
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/resurgence_banner.png" />
</p>

<h1 align="center">
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/red_pill.png" width="25" height="25" />
  WELCOME TO THE WONDERLAND!
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/blue_pill.png" width="25" height="25" />
</h1>

<table align="center">
  <tr>
    <td><img alt="GitHub branch status" src="https://img.shields.io/github/checks-status/Vibrantic/mxo-resurgence/master"></td>
    <td><img alt="GitHub Downloads (all assets, all releases)" src="https://img.shields.io/github/downloads/Vibrantic/mxo-resurgence/total"></td>
    <td><img alt="GitHub License" src="https://img.shields.io/github/license/Vibrantic/mxo-resurgence"></td>
  </tr>
  <tr>
    <td><img alt="GitHub forks" src="https://img.shields.io/github/forks/Vibrantic/mxo-resurgence?style=plastic&logo=trailforks&logoColor=white&color=green"></td>
    <td><img alt="GitHub Repo stars" src="https://img.shields.io/github/stars/Vibrantic/mxo-resurgence?style=plastic&logo=starship&logoColor=white&color=green"></td>
    <td><img alt="GitHub watchers" src="https://img.shields.io/github/watchers/Vibrantic/mxo-resurgence?style=plastic&logo=amazoncloudwatch&logoColor=white&color=green"></td>
  </tr>
  <tr>
    <td><img alt="Static Badge" src="https://img.shields.io/badge/server_version-alpha_0.1-green"></td>
    <td><img alt="Static Badge" src="https://img.shields.io/badge/launcher_version-v7.5669-green"></td>
    <td><img alt="Static Badge" src="https://img.shields.io/badge/client_version-v7.5668-green">
</td>
  </tr>
</table>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/about.png" width="40" height="40" />
  What is The Matrix Online Resurgence?
</h2>
<p>
  First of all – welcome!
  <br>
  <br>
  This is a community-driven continuation of The Matrix Online (MxO) emulator, originally based on the Hardline Dreams project.
  <br>
  <br>
  Sadly, the Hardline Dreams team has not updated their codebase in over four years.
  <br>
  <br>
  This project aims to pick up where they left off — modernizing the emulator with support for .NET 9.0, MySQL 9.3.0, <br>
  and other current technologies, all while preserving the unique charm of the original game.
  <br>
  <br>
  “The Matrix is more real than ever.”
</p>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/install.png" width="40" height="40"/>
  Installation
</h2>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/run_locally.png" width="40" height="40"/>
  Run Locally
</h2>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/features.png" width="40" height="40"/>
  Features
</h2>
<table>
  <tr>
    <td>✅ Updated to .NET 9.0 </td>
    <td>We're moving to .NET 9.0 to make the server faster, more secure, and easier to keep updated with modern systems.
      This helps ensure the project stays stable and future-proof.</td>
  </tr>
  <tr>
    <td>✅ Modern database support: MySQL 9.3.0</td>
    <td>We're using MySQL 9.3.0 to take advantage of a modern, reliable database system that can handle more data efficiently.
      This helps improve performance and ensures better stability as the project grows.</td>
  </tr>
  <tr>
    <td>✅ Open-source & community-focused</td>
    <td>Resurgence is open source and community-focused because The Matrix Online has always been about the players.
      By sharing the code and welcoming contributions, we give fans a chance to preserve, improve, and relive the game together.
      Everyone is invited to shape its future.</td>
  </tr>
  <tr>
    <td>✅ Foundation for future development & game restoration</td>
    <td>We want to provides a foundation for future development and game restoration, meaning it's built to grow.
      As more features are added and more fans get involved, we can work toward fully reviving The Matrix Online — step by step.</td>
  </tr>
  <tr>
    <td>🛠️ Modular structure for emulator and server tools</td>
    <td>The project uses a modular structure, which means different parts of the emulator and server tools are separated into clean, manageable pieces.
      This makes it easier to update, improve, or replace individual components without breaking everything else.</td> 
  </tr>
</table>

***

<h3>Features that where already implemented by Hardline Dreams Team. Big thx to them ❤️</h3>

```
Partically working features:
  •	Signposts
  •	Hyperjump (works but crappy and not really high enough)
  •	Multi Player support (it "should" work but wasnt tested enough so you should see each other and hopefully you can chat)
  •	Mission System (still at the beginning)
  •	Ability loadout change (still buggy if you unload/load more than one ability)
  •	Ability Vendor at Mara C just for tests (but you can buy items)
  •	Inventory System (you can wear items but stackable doesnt work currently)
  •	Friendlist (could be buggy too - not tested)
  •	Teleporting through hardlines (this should work nearly perfect)
  •	Opening Doors (for this you need an extra file with the static objects -
    so currently not working here and also not finished properly)
  •	Mobs are working partically (they get spawned and auto-move a little bit but still some calculations are wrong -
    you cannot fight them and they are not attacking you) EDIT: you can partically fight them
    (attack them with Hacker Attacks for example but its still very buggy)
  •	Vendors are implemented (but selling items doesnt work and buy items doesnt decrease amount of money :))
    Also i parsed all possible vendors from logs but many vendors missing which need be filled later.
    For Fun and that you didnt see a vendor window the first vendor from the CSV is the "default vendor".
  •	WIP: Crew and Factions Functionality (you can create crews and join factions / create factions
    but some parts are missing like updating all "online" playerviews)

Server Features:
  •	Entity and View System: Every View has an internal entityId so that we can just spawn view on static
    and dynamic obbjects.
  •	Network System: we have now message Queues for RPC and Object related Messages. And it will be resend until ack.
  •	Encryption Library Changed (but not finally - we still use engimaLib): to move to .NET Framework 4.0 and
    to compile for 64 Bit i created an encryption interface. So we use a C# Library and a C# implementation.
    The Reason for this is that EngimaLib had to be recompiled for 64 Bit (as it is a C++ Library) but i am not sure
    if the base libs are able to compile 64 Bit.
  	As i dont know if this works well and how good the performance is i created a Encryption Interface and moved
    the Engima Implemenation there. So if it doesnt work we could easily change back.

Chat commands: There are some ingame commands we added that you can use in HardlineDreams.
  •	?org Change the players aligment / organisation (possible values are 0 - 3)
  •	?rep Set the reputation for an organisation.
  •	Example: for zion ?rep zion 120
  •	Example: for machine: ?rep machine 120
  •	Example: for mero: ?rep mero 120
  •	?spawngameobject Spawns a GameObject of Type GoId in your current position and rotation.
    This can only work if this type of Go has values for Position and rotation (all other possible attributes are not set).
    This works with reflections.
  •	?gotopos X Y Z
  •	?rsi : you can change rsi parts but be careful with it
  •	?spawndatanode : This spawn a datanode
  •	?moa Change the moa (only visible to yourself currently)
  •	?playanim Play an animation.
  •	?playmove Should play movement (again i am not sure if this works)
  •	?mob This should spawn a testmob but this doesnt work currently
There are some other commands implemented but they are not important.
```

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/roadmap.png" width="40" height="40"/>
  Roadmap
</h2>
<p>
  You can find a Trello Board here ----> <a href="https://trello.com/b/QQFnu5GF/mxo-resurgence](https://trello.com/invite/b/6838a0ddf1b3840deb30802b/ATTIa13aba9b64ec739f989be98d78062c96EDF7D697/mxo-resurgence">TRELLO ROADMAP</a>
</p>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/contributing.png" width="40" height="40"/>
  Contributing
</h2>
  See `contributing.md` for ways to get started.
  Please adhere to this project's `code of conduct`.
  Contributions are always welcome!

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/documentation.png" width="40" height="40"/>
  Documentation
</h2>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/faq.png" width="40" height="40"/>
  Frequently asked questions
</h2>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/aknowledgments.png" width="40" height="40"/>
  Aknowledgments
</h2>
<table align="center">
  <tr>
    <td>Rajkosto<br><a href="mxoemu.info">mxoemu.info</a></td>
    <td>Many base logic like the encryption, GoProps etc. are based from him.</td>
  </tr>
  <tr>
    <td>Morpheus<br><a href="mxoemu.com">mxoemu.com</a></td>
    <td>Many researching stuff and "cortana".</td>
  </tr>
   <tr>
    <td>The Whole MxO Community</td>
    <td>Just for Motivation and support and sure for packet researching too.</td>
  </tr>
</table>
 
<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/authors.png" width="40" height="40"/>
  Authors
</h2>
<p>
  - [@Vibrantic](https://www.github.com/vibrantic)
</p>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/license.png" width="40" height="40"/>
  License
</h2>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/support.png" width="40" height="40"/>
  Support
</h2>
<p>
  For support, email help@mxo-resurgence.com or join our <a href="https://discord.gg/FyRmp7Bb">Discord</a> channel.
</p>

<h2>
  <img src="https://github.com/Vibrantic/mxo-resurgence/blob/master/github_files/images/feedback.png" width="40" height="40"/>
  Feedback
</h2>
<p>
  If you have any feedback, please reach out to us at feedback@mxo-resurgence.com
</p>

