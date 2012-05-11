#Flashdevelop Slimtimer plugin

##Description
A simple plugin that tracks the hours you work on a project, and submits them to slimtimer.
The plugin has a simple little UI that shows your login status, the current project being tracked, and the current amount of time spent on the project. You can close this UI if you want and the plugin will keep running in the background.

Be warned this is a very early beta, so expect bugs   

##Usage
To use the plugin:
Sign up for a free slimtimer account
Drop the plugin files into your flashdevelop plugins folder
Edit the plugin settings as needed, including your slimtimer username and password

Once you're up and running you can go to the slimtimer site to edit tasks, and entries, and run reports etc.

##Options
###FileComments
You can choose to include the names of the files you work on for a given time entry

###IdleTimeout
After this time the timer will stop tracking you (if you go to lunch etc)

###MinimumTime
The minimum time to submit (So you don't submit a few seconds by switching to the wrong projects etc)

###IgnoredProjects
A list of projects to ignore time tracking for

###TrackedProject
A list of projects to track in slimtimer (if AskIgnoreProjects is being used)

###AskIgnoreProjects
If set to true the application will ask you if you want to add a previously unopened project to the track or ignore list.

###CleanupDuplicates
On launch the plugin will check for tasks with duplicate names. If it finds any it will consolidate their time entries, and delete the duplicates.
This helps with duplicate entries caused by flakey slimtimer api connections.
If you have a lot of duplicates before running this for the first time, it may take a short while.

###CleanupOverlaps
On launch the plugin will check all tasks for overlapping entries, and if it finds any it will delete the shorter ones.
This fixes broken entries caused by a bug in the last version.
This is turned off by default as it takes a long time to run, isn't needed if you've never used the plugin before, and uses a lot of data calls, however it runs in a background thread so you can still work whist it's going. Just be careful not to leave it on after it's cleaned your data

##Todos
Add start/stop controls to UI
Ability to map projects to different tasks
Show recent entries in UI

##Changelog
v0.2 - 28/7/09:
Fixed a bug where only seconds were being submitted
v0.3 - 8/8/09:
Removed need to restart on settings change
Added error handling for connection loss
v0.4 - 22/8/09:
Fixed seconds bug (again   )
Added track/ignore project functionality
v0.5 - 22/9/09:
Added auto cleanup options
v0.6 - 25/9/09:
Fixed localisation errors
v0.7 - 22/9/10:
Fixed errors with api server move (no longer needs 2 dlls)
v0.8 - 8/10/11:
Updated for FD4
v0.9 - 10/5/12:
Fixed adding overlapping time entries
Moved all server calls to threads to stop it locking up your FD
Fixed duplicate projects being created when you have more than 50 active projects
Added cleanup overlaps to fix bugs in previous versions