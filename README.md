Scheduler
=========
A framework to create schedules for a sports league with an arbitrary set of scheduling rules and preferences
that go beyond a simple round robin scheduler.

By default the scheduler is set up to have the following rules:
- Each team should play roughly the same number of games in each time slot
- Each team should play each other team roughly an even number of times
- Repeat games should not occur before a team has played most other teams

It also supports some situational rules that a coordinator can set up:
- Team X should play in timeslot B on a specified week
- Team X and Team Y should play in consecutive time slots (helpful for parents on different hat teams)
- When Team X and Team Y play against each other, it should be in an earlier/later timeslot.

At present, the league set-up and rules can be modified by tweaking Program.cs, however there are plans to
extend this to be more config file driven.  To add new rules, simply create new IRule implementations that
meet your unique needs.
