# Haere Rā [![Build Status](https://travis-ci.org/HaereRa/HaereRa.svg?branch=master)](https://travis-ci.org/HaereRa/HaereRa)
## An organisational tool to track joiners, movers and leavers, alert key staff and automatically disable accounts where possible

---

I'm really bad at writing these `README.md` files, but I'll make a nice one when it's actually released.

Basically does the following:

* Tracks people as they join, move around and leave your organisation
* Scrubs through various external platforms (like AWS, LastPass, Google Apps, etc) and discovers possible matching accounts for each person
* Notifies appropriate people when someone joins, moves or leaves
* Can automatically deactivate accounts when someone leaves, if you like

Written in .NET Core. Consists of two parts: 

* GraphQL Server (API)
* Web UI (static files)

Might be a Xamarin app or something in the future, who knows.